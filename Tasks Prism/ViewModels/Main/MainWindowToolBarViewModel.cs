using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Mvvm;
using Prism.Commands;
using Tasks_Prism.Events;
using Tasks_Prism.Events.Payloads;
using Tasks_Prism.Events.TaskSearching;
using Tasks_Prism.ViewModels.Base;
using Tasks_Prism.Helpers;
using Tasks_Model.DTO;
using Tasks_Model.Interfaces;

namespace Tasks_Prism.ViewModels.Main
{
    public class MainWindowToolBarViewModel : ViewModelBase
    {
        #region Privileges
        private Visibility adminButtonVisibility;
        public Visibility AdminButtonVisibility
        {
            get { return adminButtonVisibility; }
            set { SetProperty(ref adminButtonVisibility, value); }
        }

        private Visibility taskAddingButtonVisibility;
        public Visibility TaskAddingButtonVisibility
        {
            get { return taskAddingButtonVisibility; }
            set { SetProperty(ref taskAddingButtonVisibility, value); }
        }
        #endregion

        public MainWindowToolBarViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IUserService userService)
            : base(eventAggregator, unityContainer, userService) { }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() => eventAggregator.ExecuteSafety(() => SetPrivileges())));
        }

        private DelegateCommand newTask;
        public DelegateCommand NewTask
        {
            get => newTask ?? (newTask = new DelegateCommand(() => eventAggregator.GetEvent<ShowTaskCreationWindowEvent>()
                .Publish(new ShowTaskCreationWindowPayload { TaskId = null })));
        }

        private DelegateCommand showPreferencesWindow;
        public DelegateCommand ShowPreferencesWindow
        {
            get => showPreferencesWindow ?? (showPreferencesWindow = new DelegateCommand(() => eventAggregator.GetEvent<ShowPreferencesWindowEvent>().Publish()));
        }

        private DelegateCommand showAdministrationWindow;
        public DelegateCommand ShowAdministrationWindow
        {
            get => showAdministrationWindow ?? (showAdministrationWindow = new DelegateCommand(() => eventAggregator.GetEvent<ShowAdministrationWindowEvent>().Publish()));
        }

        private DelegateCommand showMyTasks;
        public DelegateCommand ShowMyTasks
        {
            get => showMyTasks ?? (showMyTasks = new DelegateCommand(() => eventAggregator.GetEvent<ShowMyTasksEvent>().Publish()));
        }

        private DelegateCommand showTasksWithMyActivity;
        public DelegateCommand ShowTasksWithMyActivity
        {
            get => showTasksWithMyActivity ?? (showTasksWithMyActivity = new DelegateCommand(() => eventAggregator.GetEvent<ShowTasksWithMyActivityEvent>().Publish()));
        }

        private DelegateCommand showTasks;
        public DelegateCommand ShowTasks
        {
            get => showTasks ?? (showTasks = new DelegateCommand(() => eventAggregator.GetEvent<ShowTasksEvent>().Publish()));
        }

        private DelegateCommand showToners;
        public DelegateCommand ShowToners
        {
            get => showToners ?? (showToners = new DelegateCommand(() => eventAggregator.GetEvent<ShowTonersEvent>().Publish()));
        }

        private DelegateCommand showUpdates;
        public DelegateCommand ShowUpdates
        {
            get => showUpdates ?? (showUpdates = new DelegateCommand(() => eventAggregator.GetEvent<ShowUpdatesEvent>().Publish()));
        }

        private DelegateCommand showInstallations;
        public DelegateCommand ShowInstallations
        {
            get => showInstallations ?? (showInstallations = new DelegateCommand(() => eventAggregator.GetEvent<ShowInstallationsEvent>().Publish()));
        }

        private DelegateCommand logout;
        public DelegateCommand Logout
        {
            get => logout ?? (logout = new DelegateCommand(() => eventAggregator.GetEvent<LogoutEvent>().Publish()));
        }

        private DelegateCommand quitApplication;
        public DelegateCommand QuitApplication
        {
            get => quitApplication = (quitApplication = new DelegateCommand(() => eventAggregator.GetEvent<QuitApplicationEvent>().Publish()));
        }

        private DelegateCommand showAboutWindow;
        public DelegateCommand ShowAboutWindow
        {
            get => showAboutWindow ?? (showAboutWindow = new DelegateCommand(() => eventAggregator.GetEvent<ShowAboutWindowEvent>().Publish()));
        }

        private void SetPrivileges()
        {
            IList<Permission> granted = userService.AuthenticatedUserPermissions.ConvertFromDTO();
            IList<Permission> required = new List<Permission> { Permission.RolesBrowsing, Permission.UsersBrowsing };

            //  Browsing is required at least to get into the module.
            AdminButtonVisibility = granted.ContainsAnyOf(required) ? Visibility.Visible : Visibility.Collapsed;

            if (granted.Contains(Permission.TasksBrowsing))
            {
                TaskAddingButtonVisibility = Visibility.Collapsed;
                if (granted.Contains(Permission.TasksAdding))
                {
                    TaskAddingButtonVisibility = Visibility.Visible;
                }
            }
            else TaskAddingButtonVisibility = Visibility.Collapsed;
        }
    }
}
