using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Mvvm;
using Prism.Commands;
using Tasks_Prism.Events;
using Tasks_Prism.ViewModels.Base;
using Tasks_Prism.Helpers;
using Tasks_Model.DTO;
using Tasks_Model.Interfaces;

namespace Tasks_Prism.ViewModels.Main
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Privileges
        private Visibility tasksVisibility;
        public Visibility TasksVisibility
        {
            get { return tasksVisibility; }
            set { SetProperty(ref tasksVisibility, value); }
        }
        #endregion

        #region Properties
        private string windowTitle = "";
        public string WindowTitle
        {
            get => windowTitle;
            set => SetProperty(ref windowTitle, value);
        }

        private string authenticatedUser = "";
        public string AuthenticatedUser
        {
            get => authenticatedUser;
            set => SetProperty(ref authenticatedUser, value);
        }
        #endregion

        #region Delegates
        public delegate void ShowPreferencesWindowDelegate();
        public event ShowPreferencesWindowDelegate ShowPreferencesWindow;

        public delegate void ShowAdministrationWindowDelegate();
        public event ShowAdministrationWindowDelegate ShowAdministrationWindow;

        public delegate void LogoutDelegate(Action logoutAction);
        public event LogoutDelegate Logout;

        public delegate void ShowTaskCreationWindowDelegate();
        public event ShowTaskCreationWindowDelegate ShowTaskCreationWindow;

        public delegate void QuitApplicationDelegate();
        public event QuitApplicationDelegate QuitApplication;

        public delegate void ShowAboutWindowDelegate();
        public event ShowAboutWindowDelegate ShowAboutWindow;
        #endregion

        #region Event tokens
        private SubscriptionToken showPreferencesWindowToken;
        private SubscriptionToken showAdministrationWindowToken;
        private SubscriptionToken logoutToken;
        private SubscriptionToken quitApplicationToken;
        private SubscriptionToken showAboutWindowToken;
        private SubscriptionToken showTaskCreationWindowToken;
        #endregion

        public MainWindowViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IUserService userService)
            : base(eventAggregator, unityContainer, userService)
        {
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                SubscribeExceptionHandling();

                showPreferencesWindowToken = eventAggregator.GetEvent<ShowPreferencesWindowEvent>()
                    .Subscribe(() => ShowPreferencesWindow?.Invoke());

                showAdministrationWindowToken = eventAggregator.GetEvent<ShowAdministrationWindowEvent>()
                    .Subscribe(() => ShowAdministrationWindow?.Invoke());

                logoutToken = eventAggregator.GetEvent<LogoutEvent>()
                    .Subscribe(() => Logout?.Invoke(() => userService.ResetPermissions()));

                quitApplicationToken = eventAggregator.GetEvent<QuitApplicationEvent>()
                    .Subscribe(() => QuitApplication?.Invoke());

                showAboutWindowToken = eventAggregator.GetEvent<ShowAboutWindowEvent>()
                    .Subscribe(() => ShowAboutWindow?.Invoke());

                showTaskCreationWindowToken = eventAggregator.GetEvent<ShowTaskCreationWindowEvent>().Subscribe(payload =>
                {
                    unityContainer.RegisterInstance(UnityNames.ActuallyModifiedTaskId, payload);
                    ShowTaskCreationWindow?.Invoke();
                });

                eventAggregator.ExecuteSafety(() =>
                {
                    SetWindowTitle();
                    SetAuthenticatedUserInfo();
                    SetPrivileges();
                });
            }));
        }

        private void SetWindowTitle()
            => WindowTitle = "Zlecenia " + unityContainer.Resolve<VersionData>(UnityNames.VersionData).VersionNumber + " - Okno główne";

        private void SetAuthenticatedUserInfo()
            => AuthenticatedUser = userService.AuthenticatedUser.Name + " " + userService.AuthenticatedUser.Surname;

        private void SetPrivileges()
        {
            IList<Permission> granted = userService.AuthenticatedUserPermissions.ConvertFromDTO();

            if (granted.Contains(Permission.TasksBrowsing))
                TasksVisibility = Visibility.Visible;
            else
                TasksVisibility = Visibility.Collapsed;
        }

        public override void UnsubscribePrismEvents()
        {
            base.UnsubscribePrismEvents();
            eventAggregator.GetEvent<ShowPreferencesWindowEvent>().Unsubscribe(showPreferencesWindowToken);
            eventAggregator.GetEvent<ShowAdministrationWindowEvent>().Unsubscribe(showAdministrationWindowToken);
            eventAggregator.GetEvent<LogoutEvent>().Unsubscribe(logoutToken);
            eventAggregator.GetEvent<QuitApplicationEvent>().Unsubscribe(quitApplicationToken);
            eventAggregator.GetEvent<ShowAboutWindowEvent>().Unsubscribe(showAboutWindowToken);
            eventAggregator.GetEvent<ShowTaskCreationWindowEvent>().Unsubscribe(showTaskCreationWindowToken);
        }
    }
}
