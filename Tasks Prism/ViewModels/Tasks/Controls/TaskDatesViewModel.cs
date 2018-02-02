using System;
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
    public class TaskDatesViewModel : ViewModelBase
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
        private DateTime startDate;
        public DateTime StartDate
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

        private TimeSpan startHour;
        public TimeSpan StartHour
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

        private bool isEndPeriodEnabled;
        public bool IsEndPeriodEnabled
        {
            get { return isEndPeriodEnabled; }
            set { SetProperty(ref isEndPeriodEnabled, value); }
        }
        #endregion

        #region Event tokens
        private SubscriptionToken taskDataRequestToken;
        private SubscriptionToken taskStatusSelectedToken;
        #endregion

        private TaskCreationDTO taskData;

        public TaskDatesViewModel(IUnityContainer unityContainer, IEventAggregator eventAggregator, IUserService userService)
            : base(eventAggregator, unityContainer, userService)
        {

        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                taskDataRequestToken = eventAggregator.GetEvent<TaskDataRequestEvent>().Subscribe(() =>
                {
                    var dates = new TaskDatesCollectedPayload
                    {
                        StartDate = this.StartDate,
                        StartHour = this.StartHour,
                        EndDate = this.EndDate,
                        EndHour = this.EndHour
                    };

                    eventAggregator.GetEvent<TaskDatesCollectedEvent>().Publish(dates);
                });

                taskStatusSelectedToken = eventAggregator.GetEvent<TaskStatusSelectedEvent>().Subscribe(taskStatusName =>
                {
                    if (taskStatusName == Tasks_Model.Helpers.Constants.TaskStatus_Opened)
                    {
                        IsEndPeriodEnabled = false;
                        EndDate = null;
                        EndHour = null;
                    }
                    else
                    {
                        IsEndPeriodEnabled = true;
                        if (taskData.Id.HasValue && taskData.EndPeriod.HasValue)
                        {
                            EndDate = taskData.EndPeriod.Value;
                            EndHour = taskData.EndPeriod.Value.TimeOfDay;
                        }
                        else
                        {
                            EndDate = DateTime.Today;
                            EndHour = DateTime.Now.TimeOfDay;
                        }
                    }

                    SetPrivileges(taskData.Id);
                });

                eventAggregator.ExecuteSafety(() =>
                {
                    taskData = unityContainer.Resolve<TaskCreationDTO>(UnityNames.ModifiedTaskData);

                    if (taskData.Id.HasValue)
                    {
                        StartDate = taskData.StartPeriod;
                        StartHour = taskData.StartPeriod.TimeOfDay;
                    }
                    else
                    {
                        StartDate = DateTime.Today;
                        StartHour = DateTime.Now.TimeOfDay;
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
                IsEnabled = IsEndPeriodEnabled = false;
                if (IsTaskAddingMode(taskId, granted) || IsTaskModifyingMode(taskId, granted))
                {
                    IsEnabled = IsEndPeriodEnabled = true;
                }
            }
        }

        private bool IsTaskAddingMode(int? taskId, IList<Permission> granted)
            => !taskId.HasValue && granted.Contains(Permission.TasksAdding);

        private bool IsTaskModifyingMode(int? taskId, IList<Permission> granted)
            => taskId.HasValue && granted.Contains(Permission.TasksModifying);

        public override void UnsubscribePrismEvents()
        {
            base.UnsubscribePrismEvents();
            eventAggregator.GetEvent<TaskDataRequestEvent>().Unsubscribe(taskDataRequestToken);
            eventAggregator.GetEvent<TaskStatusSelectedEvent>().Unsubscribe(taskStatusSelectedToken);
        }
    }
}
