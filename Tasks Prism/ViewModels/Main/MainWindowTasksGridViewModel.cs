using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Mvvm;
using Prism.Commands;
using Tasks_Prism.Events;
using Tasks_Prism.Events.TaskCreation;
using Tasks_Prism.Events.Payloads;
using Tasks_Prism.Events.Pagination;
using Tasks_Prism.Events.TaskSearching;
using Tasks_Prism.Helpers;
using Tasks_Prism.ViewModels.Base;
using Tasks_Prism.ViewModels.Main.Helpers;
using Tasks_Prism.ViewModels.Pagination;
using Tasks_Prism.ViewModels.ProgressBar;
using Tasks_Prism.ViewModels.Administration.Helpers;
using Tasks_Model.DTO;
using Tasks_Model.Interfaces;
using Tasks_Model.Searching;

namespace Tasks_Prism.ViewModels.Main
{
    public class MainWindowTasksGridViewModel : ViewModelBase
    {
        #region Properties
        private IList<TaskPrimaryDataViewModel> tasks;
        public IList<TaskPrimaryDataViewModel> Tasks
        {
            get { return tasks; }
            set { SetProperty(ref tasks, value); }
        }

        private TaskPrimaryDataViewModel selectedTask;
        public TaskPrimaryDataViewModel SelectedTask
        {
            get { return selectedTask; }
            set
            {
                SetProperty(ref selectedTask, value);
                SelectedTask?.SetParticipants();
            }
        }

        public PaginationViewModel Pagination { get; set; }
        public ProgressBarViewModel ProgressBar { get; set; }
        #endregion

        #region Delegates
        public delegate bool RemoveSelectedTaskDelegate(int taskId);
        public RemoveSelectedTaskDelegate RemoveSelectedTask;
        #endregion

        #region Event tokens
        private SubscriptionToken jumpToPageToken;
        private SubscriptionToken showMyTasksToken;
        private SubscriptionToken showTasksWithMyActivityToken;
        private SubscriptionToken showTasksToken;
        private SubscriptionToken showTonersToken;
        private SubscriptionToken showUpdatesToken;
        private SubscriptionToken showInstallationsToken;
        private SubscriptionToken useTaskFiltersToken;
        private SubscriptionToken resetTaskFiltersToken;
        private SubscriptionToken taskAddedModifiedToken;
        private SubscriptionToken refreshTasksGridToken;
        private SubscriptionToken removeSelectedTaskToken;
        #endregion

        #region Searching
        private TaskSearchCriteria taskSearchCriteria;
        #endregion

        private readonly IPreferencesService preferencesService;
        private readonly ITaskService taskService;

        public MainWindowTasksGridViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IPreferencesService preferencesService, ITaskService taskService, IUserService userService)
            : base(eventAggregator, unityContainer, userService)
        {
            this.preferencesService = preferencesService;
            this.taskService = taskService;
            this.taskSearchCriteria = new TaskSearchCriteria();

            Pagination = new PaginationViewModel(eventAggregator, preferencesService);
            ProgressBar = new ProgressBarViewModel(eventAggregator, preferencesService);
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                jumpToPageToken = eventAggregator.GetEvent<JumpToPageEvent>().Subscribe(pageNo => SetTasks(pageNo));

                showMyTasksToken = eventAggregator.GetEvent<ShowMyTasksEvent>().Subscribe(() =>
                {
                    taskSearchCriteria.TaskAuthorUsername = userService.AuthenticatedUser.Username;
                    SetTasks(Pagination.PageNo = 1);
                });

                showTasksWithMyActivityToken = eventAggregator.GetEvent<ShowTasksWithMyActivityEvent>().Subscribe(() =>
                {
                    taskSearchCriteria.TaskParticipantId = userService.AuthenticatedUser.Id.Value;
                    SetTasks(Pagination.PageNo = 1);
                });

                showTasksToken = eventAggregator.GetEvent<ShowTasksEvent>().Subscribe(() =>
                {
                    taskSearchCriteria.ShowTasksOnly = true;
                    SetTasks(Pagination.PageNo = 1);
                });

                showTonersToken = eventAggregator.GetEvent<ShowTonersEvent>().Subscribe(() =>
                {
                    taskSearchCriteria.ShowTonersOnly = true;
                    SetTasks(Pagination.PageNo = 1);
                });

                showUpdatesToken = eventAggregator.GetEvent<ShowUpdatesEvent>().Subscribe(() =>
                {
                    taskSearchCriteria.ShowUpdatesOnly = true;
                    SetTasks(Pagination.PageNo = 1);
                });

                showInstallationsToken = eventAggregator.GetEvent<ShowInstallationsEvent>().Subscribe(() =>
                {
                    taskSearchCriteria.ShowInstallationsOnly = true;
                    SetTasks(Pagination.PageNo = 1);
                });

                useTaskFiltersToken = eventAggregator.GetEvent<UseTaskFiltersEvent>().Subscribe(filtersPayload =>
                {
                    taskSearchCriteria.TaskId = filtersPayload.TaskId;

                    taskSearchCriteria.Topic = filtersPayload.Topic;
                    taskSearchCriteria.Content = filtersPayload.Content;
                    taskSearchCriteria.Comment = filtersPayload.Comment;

                    taskSearchCriteria.StartDate = filtersPayload.StartDate;
                    taskSearchCriteria.StartHour = filtersPayload.StartHour;

                    taskSearchCriteria.EndDate = filtersPayload.EndDate;
                    taskSearchCriteria.EndHour = filtersPayload.EndHour;

                    taskSearchCriteria.TaskStatusId = filtersPayload.TaskStatusId;
                    taskSearchCriteria.TaskPriorityId = filtersPayload.TaskPriorityId;

                    taskSearchCriteria.TaskAuthorUsername = filtersPayload.TaskAuthorUsername;
                    taskSearchCriteria.TaskParticipantId = filtersPayload.TaskParticipatorId;
                    taskSearchCriteria.CommentAuthorId = filtersPayload.CommentAuthorId;

                    SetTasks(Pagination.PageNo = 1);
                });

                resetTaskFiltersToken = eventAggregator.GetEvent<ResetTaskFiltersEvent>().Subscribe(() =>
                {
                    taskSearchCriteria.Reset();
                    SetTasks(Pagination.PageNo = 1);
                });

                removeSelectedTaskToken = eventAggregator.GetEvent<RemoveSelectedTaskEvent>().Subscribe(taskId =>
                {
                    if ((bool)RemoveSelectedTask?.Invoke(taskId))
                    {
                        eventAggregator.ExecuteSafety(() => taskService.RemoveTask(taskId));
                        SetTasks(Pagination.PageNo);
                    }
                });

                taskAddedModifiedToken = eventAggregator.GetEvent<TaskAddedModifiedEvent>()
                    .Subscribe(() => SetTasks(Pagination.PageNo));

                refreshTasksGridToken = eventAggregator.GetEvent<RefreshTasksGridEvent>()
                    .Subscribe(() => SetTasks(Pagination.PageNo));

                SetTasks(Pagination.PageNo);
                ProgressBar.StartCountdown();
            }));
        }

        private void SetTasks(int pageNo)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    if (preferencesService.Preferences.DisplayOnlyTasksWithMyParticipation)
                        taskSearchCriteria.TaskParticipantId = userService.AuthenticatedUser.Id.Value;

                    taskSearchCriteria.ShowWithoutCanceledTasks = preferencesService.Preferences.HideCanceledTasks;

                    Pagination.PageCount = taskService.GetTasksPagesCount(Pagination.PageSize, taskSearchCriteria);
                    IList<TaskPrimaryDataDTO> tasks = taskService.GetTasks(Pagination.PageNo, Pagination.PageSize, taskSearchCriteria);

                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Tasks = new ObservableCollection<TaskPrimaryDataViewModel>();
                        foreach (var t in tasks)
                        {
                            Tasks.Add(new TaskPrimaryDataViewModel(t, taskService, eventAggregator, userService));
                        }
                    }));
                });
            });
        }

        public override void UnsubscribePrismEvents()
        {
            base.UnsubscribePrismEvents();
            eventAggregator.GetEvent<JumpToPageEvent>().Unsubscribe(jumpToPageToken);
            eventAggregator.GetEvent<ShowMyTasksEvent>().Unsubscribe(showMyTasksToken);
            eventAggregator.GetEvent<ShowTasksWithMyActivityEvent>().Unsubscribe(showTasksWithMyActivityToken);
            eventAggregator.GetEvent<ShowTasksEvent>().Unsubscribe(showTasksToken);
            eventAggregator.GetEvent<ShowTonersEvent>().Unsubscribe(showTonersToken);
            eventAggregator.GetEvent<ShowUpdatesEvent>().Unsubscribe(showUpdatesToken);
            eventAggregator.GetEvent<ShowInstallationsEvent>().Unsubscribe(showInstallationsToken);
            eventAggregator.GetEvent<UseTaskFiltersEvent>().Unsubscribe(useTaskFiltersToken);
            eventAggregator.GetEvent<ResetTaskFiltersEvent>().Unsubscribe(resetTaskFiltersToken);
            eventAggregator.GetEvent<RefreshTasksGridEvent>().Unsubscribe(refreshTasksGridToken);
            eventAggregator.GetEvent<RemoveSelectedTaskEvent>().Unsubscribe(removeSelectedTaskToken);
        }
    }
}
