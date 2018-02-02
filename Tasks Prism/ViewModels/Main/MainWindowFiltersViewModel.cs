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
using Tasks_Prism.Events.Payloads;
using Tasks_Prism.Events.Pagination;
using Tasks_Prism.Events.TaskSearching;
using Tasks_Prism.Helpers;
using Tasks_Prism.ViewModels.Base;
using Tasks_Prism.ViewModels.Main.Helpers;
using Tasks_Prism.ViewModels.Pagination;
using Tasks_Prism.ViewModels.Administration.Helpers;
using Tasks_Model.DTO;
using Tasks_Model.Interfaces;
using Tasks_Model.Searching;

namespace Tasks_Prism.ViewModels.Main
{
    public class MainWindowFiltersViewModel : ViewModelBase
    {
        #region Properties
        private string topic = "";
        public string Topic
        {
            get { return topic; }
            set { SetProperty(ref topic, value); }
        }

        private string content = "";
        public string Content
        {
            get { return content; }
            set { SetProperty(ref content, value); }
        }

        private string comment = "";
        public string Comment
        {
            get { return comment; }
            set { SetProperty(ref comment, value); }
        }

        private int? id;
        public int? Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private IList<UserSimpleDTO> taskAuthors;
        public IList<UserSimpleDTO> TaskAuthors
        {
            get { return taskAuthors; }
            set { SetProperty(ref taskAuthors, value); }
        }

        private UserSimpleDTO selectedTaskAuthor;
        public UserSimpleDTO SelectedTaskAuthor
        {
            get { return selectedTaskAuthor; }
            set { SetProperty(ref selectedTaskAuthor, value); }
        }

        private IList<UserSimpleDTO> taskParticipants;
        public IList<UserSimpleDTO> TaskParticipants
        {
            get { return taskParticipants; }
            set { SetProperty(ref taskParticipants, value); }
        }

        private UserSimpleDTO selectedTaskParticipant;
        public UserSimpleDTO SelectedTaskParticipant
        {
            get { return selectedTaskParticipant; }
            set { SetProperty(ref selectedTaskParticipant, value); }
        }

        private IList<UserSimpleDTO> commentAuthors;
        public IList<UserSimpleDTO> CommentAuthors
        {
            get { return commentAuthors; }
            set { SetProperty(ref commentAuthors, value); }
        }

        private UserSimpleDTO selectedCommentAuthor;
        public UserSimpleDTO SelectedCommentAuthor
        {
            get { return selectedCommentAuthor; }
            set { SetProperty(ref selectedCommentAuthor, value); }
        }

        private DateTime? startDate;
        public DateTime? StartDate
        {
            get { return startDate; }
            set { SetProperty(ref startDate, value); }
        }

        private DateTime? endDate;
        public DateTime? EndDate
        {
            get { return endDate; }
            set { SetProperty(ref endDate, value); }
        }

        private TimeSpan? startHour;
        public TimeSpan? StartHour
        {
            get { return startHour; }
            set { SetProperty(ref startHour, value); }
        }

        private TimeSpan? endHour;
        public TimeSpan? EndHour
        {
            get { return endHour; }
            set { SetProperty(ref endHour, value); }
        }

        private IList<TaskPriorityDTO> taskPriorities;
        public IList<TaskPriorityDTO> TaskPriorities
        {
            get { return taskPriorities; }
            set { SetProperty(ref taskPriorities, value); }
        }

        private TaskPriorityDTO selectedTaskPriority;
        public TaskPriorityDTO SelectedTaskPriority
        {
            get { return selectedTaskPriority; }
            set { SetProperty(ref selectedTaskPriority, value); }
        }

        private IList<TaskStatusDTO> taskStatuses;
        public IList<TaskStatusDTO> TaskStatuses
        {
            get { return taskStatuses; }
            set { SetProperty(ref taskStatuses, value); }
        }

        private TaskStatusDTO selectedTaskStatus;
        public TaskStatusDTO SelectedTaskStatus
        {
            get { return selectedTaskStatus; }
            set { SetProperty(ref selectedTaskStatus, value); }
        }
        #endregion

        private readonly IAdministrationService administrationService;
        private readonly ITaskService taskService;

        public MainWindowFiltersViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IUserService userService, IAdministrationService administrationService, ITaskService taskService)
            : base(eventAggregator, unityContainer, userService)
        {
            this.administrationService = administrationService;
            this.taskService = taskService;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    SetUsers();
                    SetPriorities();
                    SetStatuses();
                });
            }));
        }

        private DelegateCommand find;
        public DelegateCommand Find
        {
            get => find ?? (find = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    var filtersPayload = new UseTaskFiltersPayload
                    {
                        TaskId = this.Id,
                        Topic = this.Topic,
                        Content = this.Content,
                        Comment = this.Comment,
                        StartDate = this.StartDate,
                        StartHour = this.StartHour,
                        EndDate = this.EndDate,
                        EndHour = this.EndHour,
                        TaskStatusId = this.SelectedTaskStatus.Id,
                        TaskPriorityId = this.SelectedTaskPriority.Id,
                        TaskAuthorUsername = this.SelectedTaskAuthor.Username,
                        CommentAuthorId = this.SelectedCommentAuthor.Id,
                        TaskParticipatorId = this.SelectedTaskParticipant.Id
                    };

                    eventAggregator.GetEvent<UseTaskFiltersEvent>().Publish(filtersPayload);
                });
            }));
        }

        private DelegateCommand resetFilters;
        public DelegateCommand ResetFilters
        {
            get => resetFilters ?? (resetFilters = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    ClearForm();
                    eventAggregator.GetEvent<ResetTaskFiltersEvent>().Publish();
                });
            }));
        }

        private void SetUsers()
        {
            TaskAuthors = new ObservableCollection<UserSimpleDTO>() { new UserSimpleDTO { Id = null, Name = " -- wszyscy --" } };
            SelectedTaskAuthor = TaskAuthors.First();

            foreach (var a in administrationService.GetUsersSimpleData())
                TaskAuthors.Add(a);

            TaskParticipants = new ObservableCollection<UserSimpleDTO>() { new UserSimpleDTO { Id = null, Name = " -- wszyscy --" } };
            SelectedTaskParticipant = TaskParticipants.First();

            foreach (var p in administrationService.GetUsersSimpleData())
                TaskParticipants.Add(p);

            CommentAuthors = new ObservableCollection<UserSimpleDTO>() { new UserSimpleDTO { Id = null, Name = " -- wszyscy --" } };
            SelectedCommentAuthor = CommentAuthors.First();

            foreach (var a in administrationService.GetUsersSimpleData())
                CommentAuthors.Add(a);
        }

        private void SetPriorities()
        {
            TaskPriorities = new ObservableCollection<TaskPriorityDTO>() { new TaskPriorityDTO { Id = null, Name = " -- wszystkie --" } };
            SelectedTaskPriority = TaskPriorities.First();

            foreach (var p in taskService.GetTaskPriorities())
                TaskPriorities.Add(p);
        }

        private void SetStatuses()
        {
            TaskStatuses = new ObservableCollection<TaskStatusDTO>() { new TaskStatusDTO { Id = null, Name = " -- wszystkie --" } };
            SelectedTaskStatus = TaskStatuses.First();

            foreach (var s in taskService.GetTaskStatuses())
                TaskStatuses.Add(s);
        }

        private void ClearForm()
        {
            Topic = Content = Comment = "";
            StartDate = EndDate = null;
            StartHour = EndHour = null;
            Id = null;

            SelectedTaskAuthor = TaskAuthors.First();
            SelectedTaskParticipant = TaskParticipants.First();
            SelectedCommentAuthor = CommentAuthors.First();

            SelectedTaskPriority = TaskPriorities.First();
            SelectedTaskStatus = TaskStatuses.First();
        }
    }
}
