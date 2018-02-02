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
using Tasks_Prism.ViewModels.Tasks.Helpers;
using Tasks_Prism.Helpers;
using Tasks_Model.DTO;
using Tasks_Model.Interfaces;

namespace Tasks_Prism.ViewModels.Tasks.Controls
{
    public class TaskParticipantsViewModel : ViewModelBase
    {
        #region Properties
        private IList<TaskParticipantViewModel> taskParticipants;
        public IList<TaskParticipantViewModel> TaskParticipants
        {
            get { return taskParticipants; }
            set { SetProperty(ref taskParticipants, value); }
        }

        private TaskParticipantViewModel selectedTaskParticipant;
        public TaskParticipantViewModel SelectedTaskParticipant
        {
            get { return selectedTaskParticipant; }
            set { SetProperty(ref selectedTaskParticipant, value); }
        }
        #endregion

        #region Event tokens
        private SubscriptionToken taskDataRequestToken;
        #endregion

        private readonly IAdministrationService administrationService;

        public TaskParticipantsViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IUserService userService, IAdministrationService administrationService)
            : base(eventAggregator, unityContainer, userService)
        {
            this.administrationService = administrationService;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                taskDataRequestToken = eventAggregator.GetEvent<TaskDataRequestEvent>().Subscribe(() =>
                {
                    var pariticipants = new TaskParticipantsCollectedPayload
                    {
                        TaskParticipantsIds = TaskParticipants
                            .Where(p => p.IsChecked)
                            .Select(p => p.Id)
                            .ToList<int>()
                    };

                    eventAggregator.GetEvent<TaskParticipantsCollectedEvent>().Publish(pariticipants);
                });

                eventAggregator.ExecuteSafety(() =>
                {
                    SetParticipants();

                    var taskData = unityContainer.Resolve<TaskCreationDTO>(UnityNames.ModifiedTaskData);

                    if (taskData.Id.HasValue)
                    {
                        foreach (TaskParticipantViewModel p in TaskParticipants)
                            p.IsChecked = taskData.ParticipantsIds.Contains(p.Id) ? true : false;
                    }

                    SetPrivileges(taskData.Id);
                });
            }));
        }

        private void SetParticipants()
        {
            TaskParticipants = new ObservableCollection<TaskParticipantViewModel>();

            foreach (var u in administrationService.GetUsersSimpleData(all: false))
                TaskParticipants.Add(new TaskParticipantViewModel(u.Id.Value, u.Username, u.Name, u.Surname, false, userService));
        }

        public void SetPrivileges(int? taskId)
        {
            IList<Permission> granted = userService.AuthenticatedUserPermissions.ConvertFromDTO();

            foreach (var p in TaskParticipants)
            {
                if (granted.Contains(Permission.TasksBrowsing))
                {
                    p.IsEnabled = false;
                    if (IsTaskAddingMode(taskId, granted) || IsTaskModifyingMode(taskId, granted))
                    {
                        p.IsEnabled = true;
                    }
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
        }
    }
}
