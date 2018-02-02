using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Mvvm;
using Prism.Commands;
using Tasks_Prism.Events;
using Tasks_Prism.Helpers;
using Tasks_Model.DTO;
using Tasks_Model.Interfaces;
using Tasks_Prism.ViewModels.Base;

namespace Tasks_Prism.ViewModels.Preferences
{
    public class PreferencesWindowViewModel : ViewModelBase
    {
        #region Properties
        private string windowTitle = "";
        public string WindowTitle
        {
            get => windowTitle;
            set => SetProperty(ref windowTitle, value);
        }
        #endregion

        #region Delegates
        public delegate void PreferencesSavedDelegate();
        public event PreferencesSavedDelegate PreferencesSaved;

        public delegate void PreferencesWindowClosedDelegate();
        public event PreferencesWindowClosedDelegate PreferencesWindowClosed;
        #endregion

        #region Event tokens
        private SubscriptionToken preferencesSavedToken;
        private SubscriptionToken closePreferencesWindowToken;
        #endregion

        private readonly IPreferencesService preferencesService;

        public PreferencesWindowViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IPreferencesService preferencesService, IUserService userService)
            : base(eventAggregator, unityContainer, userService)
        {
            this.preferencesService = preferencesService;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                preferencesSavedToken = eventAggregator.GetEvent<PreferencesSavedEvent>()
                    .Subscribe(() => PreferencesSaved?.Invoke());

                closePreferencesWindowToken = eventAggregator.GetEvent<ClosePreferencesWindowEvent>()
                    .Subscribe(() => PreferencesWindowClosed?.Invoke());

                eventAggregator.ExecuteSafety(() => SetWindowTitle());
            }));
        }

        private void SetWindowTitle()
            => WindowTitle = "Ustawienia";

        public override void UnsubscribePrismEvents()
        {
            eventAggregator.GetEvent<PreferencesSavedEvent>().Unsubscribe(preferencesSavedToken);
            eventAggregator.GetEvent<ClosePreferencesWindowEvent>().Unsubscribe(closePreferencesWindowToken);
        }
    }
}
