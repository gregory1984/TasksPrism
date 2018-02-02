using System;
using System.Collections.Generic;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Threading;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Mvvm;
using Prism.Commands;
using Tasks_Prism.Events;
using Tasks_Prism.Events.TaskCreation;
using Tasks_Prism.Events.Payloads;
using Tasks_Prism.ViewModels.Base;
using Tasks_Prism.Helpers;
using Tasks_Model.DTO;
using Tasks_Model.Interfaces;

namespace Tasks_Prism.ViewModels.Tasks
{
    public class TaskCreationWindowViewModel : ViewModelBase
    {
        #region Privileges
        private Visibility processTaskButtonVisibility;
        public Visibility ProcessTaskButtonVisibility
        {
            get { return processTaskButtonVisibility; }
            set { SetProperty(ref processTaskButtonVisibility, value); }
        }
        #endregion

        #region Properties
        private string windowTitle = "";
        public string WindowTitle
        {
            get { return windowTitle; }
            set { SetProperty(ref windowTitle, value); }
        }

        private string header = "";
        public string Header
        {
            get { return header; }
            set { SetProperty(ref header, value); }
        }
        #endregion

        #region Delegates
        public delegate void TaskStatusNotSelectedDelegate();
        public event TaskStatusNotSelectedDelegate TaskStatusNotSelected;

        public delegate void TaskGenreNotSelectedDelegate();
        public event TaskGenreNotSelectedDelegate TaskGenreNotSelected;

        public delegate void TaskPriorityNotSelectedDelegate();
        public event TaskPriorityNotSelectedDelegate TaskPriorityNotSelected;

        public delegate void TaskParticipantsNotSelectedDelegate();
        public event TaskParticipantsNotSelectedDelegate TaskParticipantsNotSelected;

        public delegate void TopicOrContentEmptyDelegate();
        public event TopicOrContentEmptyDelegate TopicOrContentEmpty;

        public delegate void EndPeriodEarlierThanStartPeriodDelegate();
        public event EndPeriodEarlierThanStartPeriodDelegate EndPeriodEarlierThanStartPeriod;

        public delegate void ClosedOrCanceledWithNoEndPeriodDelegate();
        public event ClosedOrCanceledWithNoEndPeriodDelegate ClosedOrCanceledWithNoEndPeriod;

        public delegate void TaskModifiedDelegate(int taskId);
        public event TaskModifiedDelegate TaskModified;

        public delegate void TaskAddedDelegate();
        public event TaskAddedDelegate TaskAdded;

        public delegate void CloseTaskCreationWindowDelegate();
        public event CloseTaskCreationWindowDelegate CloseTaskCreationWindow;
        #endregion

        #region Event tokens
        private SubscriptionToken taskDatesCollectedToken;
        private SubscriptionToken taskContentCollectedToken;
        private SubscriptionToken taskPropertiesCollectedToken;
        private SubscriptionToken taskParticipantsCollectedToken;
        private SubscriptionToken taskCommentsCollectedToken;
        #endregion

        #region Payloads from child controls
        private TaskDatesCollectedPayload taskDates;
        private TaskContentCollectedPayload taskContent;
        private TaskPropertiesCollectedPayload taskProperties;
        private TaskParticipantsCollectedPayload taskParticipants;
        private TaskCommentsCollectedPayload taskComments;
        #endregion

        private int? taskId;

        private readonly ITaskService taskService;

        public TaskCreationWindowViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, ITaskService taskService, IUserService userService)
            : base(eventAggregator, unityContainer, userService)
        {
            this.taskService = taskService;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                taskDatesCollectedToken = eventAggregator.GetEvent<TaskDatesCollectedEvent>()
                    .Subscribe(dates => { taskDates = dates; ProcessCollectedData(); });

                taskContentCollectedToken = eventAggregator.GetEvent<TaskContentCollectedEvent>()
                    .Subscribe(content => { taskContent = content; ProcessCollectedData(); });

                taskPropertiesCollectedToken = eventAggregator.GetEvent<TaskPropertiesCollectedEvent>()
                    .Subscribe(properties => { taskProperties = properties; ProcessCollectedData(); });

                taskParticipantsCollectedToken = eventAggregator.GetEvent<TaskParticipantsCollectedEvent>()
                    .Subscribe(participants => { taskParticipants = participants; ProcessCollectedData(); });

                taskCommentsCollectedToken = eventAggregator.GetEvent<TaskCommentsCollectedEvent>()
                    .Subscribe(comments => { taskComments = comments; ProcessCollectedData(); });

                eventAggregator.ExecuteSafety(() =>
                {
                    taskId = unityContainer.Resolve<ShowTaskCreationWindowPayload>(UnityNames.ActuallyModifiedTaskId).TaskId;
                    TaskCreationDTO taskData = null;

                    if (taskId.HasValue)
                    {
                        WindowTitle = $"Modyfikowanie zlecenia: {taskId.Value}";
                        taskData = taskService.GetSelectedTask(taskId.Value);
                        Header = $"Modyfikowanie zlecenia nr: '{taskData.Id.Value}' - Autor: '{taskData.Author}'";
                    }
                    else
                    {
                        WindowTitle = "Nowe zlecenie";
                        Header = $"Nowe zlecenie - Autor: {userService.AuthenticatedUser.Username}";
                        taskData = new TaskCreationDTO { Id = null };
                    }

                    unityContainer.RegisterInstance(UnityNames.ModifiedTaskData, taskData);

                    SetPrivileges();
                });
            }));
        }

        private DelegateCommand processTask;
        public DelegateCommand ProcessTask
        {
            get => processTask ?? (processTask = new DelegateCommand(() =>
            {
                eventAggregator.GetEvent<TaskDataRequestEvent>().Publish();
            }));
        }

        private DelegateCommand close;
        public DelegateCommand Close
        {
            get => close ?? (close = new DelegateCommand(() => CloseTaskCreationWindow?.Invoke()));
        }

        private void ProcessCollectedData()
        {
            eventAggregator.ExecuteSafety(() =>
            {
                if (DataCollected())
                {
                    var task = new TaskCreationDTO
                    {
                        Id = taskId,
                        Topic = taskContent.Topic,
                        Content = taskContent.Content,

                        StartPeriod = new DateTime(
                            taskDates.StartDate.Year,
                            taskDates.StartDate.Month,
                            taskDates.StartDate.Day,
                            taskDates.StartHour.Hours,
                            taskDates.StartHour.Minutes,
                            taskDates.StartHour.Seconds),

                        EndPeriod = taskDates.EndDate.HasValue && taskDates.EndHour.HasValue ? (DateTime?)new DateTime(
                            taskDates.EndDate.Value.Year,
                            taskDates.EndDate.Value.Month,
                            taskDates.EndDate.Value.Day,
                            taskDates.EndHour.Value.Hours,
                            taskDates.EndHour.Value.Minutes,
                            taskDates.EndHour.Value.Seconds) : null,

                        Author = userService.AuthenticatedUser.Username,
                        GenreId = taskProperties.TaskGenreId,
                        StatusId = taskProperties.TaskStatusId,
                        PriorityId = taskProperties.TaskPriorityId,
                        ParticipantsIds = taskParticipants.TaskParticipantsIds
                    };

                    task.Comments = new List<TaskCommentCreationDTO>();
                    foreach (var c in taskComments.Comments)
                    {
                        task.Comments.Add(new TaskCommentCreationDTO(c.Id, c.Content, c.Date, c.AuthorId.Value, c.Author, taskId));
                    }

                    var status = taskService.AddModifyTask(task);
                    switch (status)
                    {
                        case TaskCreationStatus.Added:
                            {
                                eventAggregator.GetEvent<TaskAddedModifiedEvent>().Publish();
                                TaskAdded?.Invoke();
                                break;
                            }
                        case TaskCreationStatus.Modified:
                            {
                                eventAggregator.GetEvent<TaskAddedModifiedEvent>().Publish();
                                TaskModified?.Invoke(task.Id.Value);
                                break;
                            }
                        case TaskCreationStatus.ElementEmpty: { TopicOrContentEmpty?.Invoke(); break; }
                        case TaskCreationStatus.GenreNotSelected: { TaskGenreNotSelected?.Invoke(); break; }
                        case TaskCreationStatus.StatusNotSelected: { TaskStatusNotSelected?.Invoke(); break; }
                        case TaskCreationStatus.PriorityNotSelected: { TaskPriorityNotSelected?.Invoke(); break; }
                        case TaskCreationStatus.PatricipantsNotSelected: { TaskParticipantsNotSelected?.Invoke(); break; }
                        case TaskCreationStatus.ClosedOrCanceledWithNoEndPeriod: { ClosedOrCanceledWithNoEndPeriod?.Invoke(); break; }
                        case TaskCreationStatus.EndPeriodEarlierThanStartPeriod: { EndPeriodEarlierThanStartPeriod?.Invoke(); break; }
                    }

                    ClearCollectedPayloads();
                }
            });
        }

        private bool DataCollected()
        {
            return taskDates != null && taskContent != null && taskProperties != null &&
                   taskParticipants != null && taskComments != null;
        }

        private void ClearCollectedPayloads()
        {
            taskDates = null;
            taskContent = null;
            taskProperties = null;
            taskParticipants = null;
            taskComments = null;
        }

        private void SetPrivileges()
        {
            IList<Permission> granted = userService.AuthenticatedUserPermissions.ConvertFromDTO();

            if (granted.Contains(Permission.TasksBrowsing))
            {
                ProcessTaskButtonVisibility = Visibility.Collapsed;
                if ((taskId.HasValue && granted.Contains(Permission.TasksModifying)) ||
                   (!taskId.HasValue && granted.Contains(Permission.TasksAdding)))
                {
                    ProcessTaskButtonVisibility = Visibility.Visible;
                }
            }
        }

        public override void UnsubscribePrismEvents()
        {
            base.UnsubscribePrismEvents();
            eventAggregator.GetEvent<TaskDatesCollectedEvent>().Unsubscribe(taskDatesCollectedToken);
            eventAggregator.GetEvent<TaskContentCollectedEvent>().Unsubscribe(taskContentCollectedToken);
            eventAggregator.GetEvent<TaskPropertiesCollectedEvent>().Unsubscribe(taskPropertiesCollectedToken);
            eventAggregator.GetEvent<TaskParticipantsCollectedEvent>().Unsubscribe(taskParticipantsCollectedToken);
            eventAggregator.GetEvent<TaskCommentsCollectedEvent>().Unsubscribe(taskCommentsCollectedToken);
        }
    }
}
