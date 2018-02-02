using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using Tasks_Model.DTO;
using Tasks_Model.Interfaces;
using Tasks_Prism.Helpers;
using Tasks_Prism.Events;
using ModelConsts = Tasks_Model.Helpers.Constants;

namespace Tasks_Prism.ViewModels.Main.Helpers
{
    public class TaskPrimaryDataViewModel : BindableBase
    {
        #region Privileges
        private Visibility modifyTaskButtonVisibility;
        public Visibility ModifyTaskButtonVisibility
        {
            get { return modifyTaskButtonVisibility; }
            set { SetProperty(ref modifyTaskButtonVisibility, value); }
        }

        private Visibility removeTaskButtonVisibility;
        public Visibility RemoveTaskButtonVisibility
        {
            get { return removeTaskButtonVisibility; }
            set { SetProperty(ref removeTaskButtonVisibility, value); }
        }
        #endregion

        public int Id { get; set; }

        private string idBackgroundColor = "";
        public string IdBackgroundColor
        {
            get { return idBackgroundColor; }
            set { SetProperty(ref idBackgroundColor, value); }
        }

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

        private string author = "";
        public string Author
        {
            get { return author; }
            set { SetProperty(ref author, value); }
        }

        private DateTime startPeriod;
        public DateTime StartPeriod
        {
            get { return startPeriod; }
            set { SetProperty(ref startPeriod, value); }
        }

        private DateTime? endPeriod;
        public DateTime? EndPeriod
        {
            get { return endPeriod; }
            set { SetProperty(ref endPeriod, value); }
        }

        private string priority = "";
        public string Priority
        {
            get { return priority; }
            set { SetProperty(ref priority, value); }
        }

        private string status = "";
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        private string genre = "";
        public string Genre
        {
            get { return genre; }
            set { SetProperty(ref genre, value); }
        }

        private string participants = "";
        public string Participants
        {
            get { return participants; }
            set { SetProperty(ref participants, value); }
        }

        private string modifyTaskButtonLabel = "";
        public string ModifyTaskButtonLabel
        {
            get { return modifyTaskButtonLabel; }
            set { SetProperty(ref modifyTaskButtonLabel, value); }
        }

        public TaskCommentPrimaryDataViewModel LastComment { get; set; }

        private readonly ITaskService taskService;
        private readonly IEventAggregator eventAggregator;
        private readonly IUserService userService;

        public TaskPrimaryDataViewModel(TaskPrimaryDataDTO task, ITaskService taskService, IEventAggregator eventAggregator, IUserService userService)
        {
            this.taskService = taskService;
            this.eventAggregator = eventAggregator;
            this.userService = userService;

            Id = task.Id.Value;
            Topic = task.Topic;
            Author = task.Author;
            Content = task.Content;
            StartPeriod = task.StartPeriod;
            EndPeriod = task.EndPeriod;
            Status = task.Status;
            Priority = task.Priority;
            Genre = task.Genre;

            if (task.LastComment != null)
            {
                LastComment = new TaskCommentPrimaryDataViewModel
                {
                    Id = task.LastComment.Id.Value,
                    Content = task.LastComment.Content,
                    Date = task.LastComment.Date,
                    Author = task.LastComment.Author
                };
            }

            IdBackgroundColor = Status == ModelConsts.TaskStatus_Opened ? "#f5ff80" :
                               (Status == ModelConsts.TaskStatus_Canceled) ? "#ffcd7f" : "#7fff7f";

            SetPrivileges();
        }

        private DelegateCommand modifyTask;
        public DelegateCommand ModityTask
        {
            get => modifyTask ?? (modifyTask = new DelegateCommand(() =>
            {
                eventAggregator.GetEvent<ShowTaskCreationWindowEvent>()
                    .Publish(new Events.Payloads.ShowTaskCreationWindowPayload { TaskId = Id });
            }));
        }

        private DelegateCommand removeTask;
        public DelegateCommand RemoveTask
        {
            get => removeTask ?? (removeTask = new DelegateCommand(() =>
            {
                eventAggregator.GetEvent<RemoveSelectedTaskEvent>().Publish(Id);
            }));
        }

        public void SetParticipants()
        {
            Participants = taskService.GetSelectedTaskParticipants(Id);
        }

        private void SetPrivileges()
        {
            IList<Permission> granted = userService.AuthenticatedUserPermissions.ConvertFromDTO();

            if (granted.Contains(Permission.TasksBrowsing))
            {
                ModifyTaskButtonVisibility = Visibility.Visible;
                ModifyTaskButtonLabel = "PODGLĄD";
                RemoveTaskButtonVisibility = Visibility.Collapsed;

                if (granted.Contains(Permission.TasksModifying))
                {
                    ModifyTaskButtonLabel = "ZMIEŃ";
                }

                if (granted.Contains(Permission.TasksRemoving))
                {
                    RemoveTaskButtonVisibility = Visibility.Visible;
                }
            }
            else
            {
                ModifyTaskButtonVisibility = Visibility.Collapsed;
                RemoveTaskButtonVisibility = Visibility.Collapsed;
            }
        }
    }
}
