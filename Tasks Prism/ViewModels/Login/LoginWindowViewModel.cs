using System;
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

namespace Tasks_Prism.ViewModels.Login
{
    public class LoginWindowViewModel : ViewModelBase
    {
        #region Properties
        private string username = "";
        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        private string windowTitle = "";
        public string WindowTitle
        {
            get => windowTitle;
            set => SetProperty(ref windowTitle, value);
        }

        private bool isRemembered;
        public bool IsRemembered
        {
            get => isRemembered;
            set => SetProperty(ref isRemembered, value);
        }
        #endregion

        #region Delegates
        public delegate void ShowMainWindowDelegate();
        public event ShowMainWindowDelegate ShowMainWindow;

        public delegate void UserBlockedDelegate(string username);
        public event UserBlockedDelegate UserBlocked;

        public delegate void BadCredentialsDelegate();
        public event BadCredentialsDelegate BadCredentials;
        #endregion

        private readonly IDatabaseService databaseService;
        private readonly IPreferencesService preferencesService;

        public LoginWindowViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IUserService userService, IDatabaseService databaseService, IPreferencesService preferencesService)
            : base(eventAggregator, unityContainer, userService)
        {
            this.databaseService = databaseService;
            this.preferencesService = preferencesService;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                SubscribeExceptionHandling();
                eventAggregator.ExecuteSafety(() =>
                {
                    databaseService.Initialize();
                    SetWindowTitle();
                });
            }));
        }

        private DelegateCommand<PasswordBox> contentRendered;
        public DelegateCommand<PasswordBox> ContentRendered
        {
            get => contentRendered ?? (contentRendered = new DelegateCommand<PasswordBox>(p => LoadCredentials(p)));
        }

        private DelegateCommand<PasswordBox> login;
        public DelegateCommand<PasswordBox> Login
        {
            get => login ?? (login = new DelegateCommand<PasswordBox>(p =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    if (IsCorrect(Username, p.Password))
                    {
                        userService.Authenticate(Username, p.Password);
                        switch (userService.AuthenticatedUser.AuthenticationStatus)
                        {
                            case AuthenticationStatus.BadCredentials:
                                {
                                    BadCredentials?.Invoke();
                                    break;
                                }
                            case AuthenticationStatus.Blocked:
                                {
                                    UserBlocked?.Invoke(Username);
                                    break;
                                }
                            case AuthenticationStatus.LoggedIn:
                                {
                                    preferencesService.LoadPreferences(userService.AuthenticatedUser.Id.Value);
                                    SaveCredentials(p.Password);
                                    ShowMainWindow?.Invoke();
                                    break;
                                }
                        }
                    }
                });
            }));
        }

        private void SetWindowTitle()
            => WindowTitle = "Zlecenia " + unityContainer.Resolve<VersionData>(UnityNames.VersionData).VersionNumber;

        private bool IsCorrect(string username, string password)
            => !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);

        #region Save and load remembered credentials
        private void SaveCredentials(string password)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    var serializer = unityContainer.Resolve<CredentialsSerializer<Credentials>>(UnityNames.CredentialsSerializer);
                    if (IsRemembered)
                        serializer.Serialize(Constants.CredentialsFilePath, new Credentials(Username, password, IsRemembered));
                    else
                        serializer.Serialize(Constants.CredentialsFilePath, new Credentials(IsRemembered));
                });
            });
        }

        private void LoadCredentials(PasswordBox passwordbox)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    var serializer = unityContainer.Resolve<CredentialsSerializer<Credentials>>(UnityNames.CredentialsSerializer);
                    var credentials = serializer.Deserialize(Constants.CredentialsFilePath);

                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (credentials.IsRemembered)
                        {
                            Username = credentials.Username;
                            passwordbox.Password = credentials.Password;
                        }
                        IsRemembered = credentials.IsRemembered;
                    }));
                });
            });
        }
        #endregion
    }
}
