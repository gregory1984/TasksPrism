using System;
using System.Linq;
using System.Collections.Generic;
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

namespace Tasks_Prism.ViewModels.Tasks.Controls
{
    public class TaskPropertiesViewModel : ViewModelBase
    {
        #region Privileges
        private bool isEnabled;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set { SetProperty(ref isEnabled, value); }
        }
        #endregion

        #region Properties
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
            set
            { SetProperty(ref taskStatuses, value); }
        }

        private TaskStatusDTO selectedTaskStatus;
        public TaskStatusDTO SelectedTaskStatus
        {
            get { return selectedTaskStatus; }
            set
            {
                SetProperty(ref selectedTaskStatus, value);
                eventAggregator.GetEvent<TaskStatusSelectedEvent>().Publish(SelectedTaskStatus.Name);
            }
        }

        private IList<TaskGenreDTO> taskGenres;
        public IList<TaskGenreDTO> TaskGenres
        {
            get { return taskGenres; }
            set { SetProperty(ref taskGenres, value); }
        }

        private TaskGenreDTO selectedTaskGenre;
        public TaskGenreDTO SelectedTaskGenre
        {
            get { return selectedTaskGenre; }
            set { SetProperty(ref selectedTaskGenre, value); }
        }
        #endregion

        #region Event tokens
        private SubscriptionToken taskDataRequestToken;
        #endregion

        private readonly ITaskService taskService;

        public TaskPropertiesViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, ITaskService taskService, IUserService userService)
            : base(eventAggregator, unityContainer, userService)
        {
            this.taskService = taskService;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                taskDataRequestToken = eventAggregator.GetEvent<TaskDataRequestEvent>().Subscribe(() =>
                {
                    var properties = new TaskPropertiesCollectedPayload
                    {
                        TaskGenreId = SelectedTaskGenre.Id,
                        TaskStatusId = SelectedTaskStatus.Id,
                        TaskPriorityId = SelectedTaskPriority.Id
                    };

                    eventAggregator.GetEvent<TaskPropertiesCollectedEvent>().Publish(properties);
                });

                eventAggregator.ExecuteSafety(() =>
                {
                    SetProperties();

                    var taskData = unityContainer.Resolve<TaskCreationDTO>(UnityNames.ModifiedTaskData);

                    if (taskData.Id.HasValue)
                    {
                        SelectedTaskGenre = TaskGenres.SingleOrDefault(g => g.Id == taskData.GenreId);
                        SelectedTaskPriority = TaskPriorities.SingleOrDefault(p => p.Id == taskData.PriorityId);
                        SelectedTaskStatus = TaskStatuses.SingleOrDefault(s => s.Id == taskData.StatusId);
                    }
                    else
                    {
                        SelectedTaskGenre = TaskGenres.First();
                        SelectedTaskPriority = TaskPriorities.First();
                        SelectedTaskStatus = TaskStatuses.First();
                    }

                    SetPrivileges(taskData.Id);
                });
            }));
        }

        private void SetPrivileges(int? taskId)
        {
            IList<Permission> granted = userService.AuthenticatedUserPermissions.ConvertFromDTO();

            if (granted.Contains(Permission.TasksBrowsing))
            {
                IsEnabled = false;
                if (IsTaskAddingMode(taskId, granted) || IsTaskModifyingMode(taskId, granted))
                {
                    IsEnabled = true;
                }
            }
        }

        private bool IsTaskAddingMode(int? taskId, IList<Permission> granted)
            => !taskId.HasValue && granted.Contains(Permission.TasksAdding);

        private bool IsTaskModifyingMode(int? taskId, IList<Permission> granted)
            => taskId.HasValue && granted.Contains(Permission.TasksModifying);

        private void SetProperties()
        {
            TaskPriorities = new ObservableCollection<TaskPriorityDTO>() { new TaskPriorityDTO { Id = null, Name = " -- wybierz --" } };

            foreach (var p in taskService.GetTaskPriorities())
                TaskPriorities.Add(p);

            TaskStatuses = new ObservableCollection<TaskStatusDTO>() { new TaskStatusDTO { Id = null, Name = " -- wybierz --" } };

            foreach (var s in taskService.GetTaskStatuses())
                TaskStatuses.Add(s);

            TaskGenres = new ObservableCollection<TaskGenreDTO>() { new TaskGenreDTO { Id = null, Name = " -- wybierz --" } };

            foreach (var g in taskService.GetTaskGenres())
                TaskGenres.Add(g);
        }

        public override void UnsubscribePrismEvents()
        {
            base.UnsubscribePrismEvents();
            eventAggregator.GetEvent<TaskDataRequestEvent>().Unsubscribe(taskDataRequestToken);
        }
    }
}
