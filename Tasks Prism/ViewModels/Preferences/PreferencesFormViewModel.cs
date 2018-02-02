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

namespace Tasks_Prism.ViewModels.Preferences
{
    public class PreferencesFormViewModel : ViewModelBase
    {
        #region Preferences
        private int autoRefreshingFrequency;
        public int AutoRefreshingFrequency
        {
            get => autoRefreshingFrequency;
            set => SetProperty(ref autoRefreshingFrequency, value);
        }

        private int tasksPerPage;
        public int TasksPerPage
        {
            get => tasksPerPage;
            set => SetProperty(ref tasksPerPage, value);
        }

        private bool displayOnlyTasksWithMyParticipation;
        public bool DisplayOnlyTasksWithMyParticipation
        {
            get => displayOnlyTasksWithMyParticipation;
            set => SetProperty(ref displayOnlyTasksWithMyParticipation, value);
        }

        private bool enableTasksListAutoRefreshing;
        public bool EnableTasksListAutoRefreshing
        {
            get => enableTasksListAutoRefreshing;
            set => SetProperty(ref enableTasksListAutoRefreshing, value);
        }

        private bool displayTasksRefreshingProgressBar;
        public bool DisplayTasksRefreshingProgressBar
        {
            get => displayTasksRefreshingProgressBar;
            set => SetProperty(ref displayTasksRefreshingProgressBar, value);
        }

        private bool hideCanceledTasks;
        public bool HideCanceledTasks
        {
            get => hideCanceledTasks;
            set => SetProperty(ref hideCanceledTasks, value);
        }
        #endregion

        private readonly IPreferencesService preferencesService;

        public PreferencesFormViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IPreferencesService preferencesService, IUserService userService)
            : base(eventAggregator, unityContainer, userService)
        {
            this.preferencesService = preferencesService;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() => SetUserPreferences());
            }));
        }

        private DelegateCommand reset;
        public DelegateCommand Reset
        {
            get => reset ?? (reset = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    preferencesService.Reset(userService.AuthenticatedUser.Id.Value);
                    preferencesService.LoadPreferences(userService.AuthenticatedUser.Id.Value);

                    SetUserPreferences();
                    eventAggregator.GetEvent<PreferencesSavedEvent>().Publish();
                });
            }));
        }

        private DelegateCommand savePreferences;
        public DelegateCommand SavePreferences
        {
            get => savePreferences ?? (savePreferences = new DelegateCommand(() =>
            {
                preferencesService.SavePreferences(new UserPreferencesDTO
                {
                    AutoRefreshingFrequency = AutoRefreshingFrequency,
                    DisplayOnlyTasksWithMyParticipation = DisplayOnlyTasksWithMyParticipation,
                    DisplayTasksRefreshingProgressBar = DisplayTasksRefreshingProgressBar,
                    EnableTasksListAutoRefreshing = EnableTasksListAutoRefreshing,
                    HideCanceledTasks = HideCanceledTasks,
                    TasksPerPage = TasksPerPage,
                    UserId = userService.AuthenticatedUser.Id.Value
                });

                eventAggregator.GetEvent<PreferencesSavedEvent>().Publish();
            }));
        }

        private DelegateCommand close;
        public DelegateCommand Close
        {
            get => close ?? (close = new DelegateCommand(() =>
            {
                eventAggregator.GetEvent<ClosePreferencesWindowEvent>().Publish();
            }));
        }

        private void SetUserPreferences()
        {
            AutoRefreshingFrequency = preferencesService.Preferences.AutoRefreshingFrequency;
            DisplayOnlyTasksWithMyParticipation = preferencesService.Preferences.DisplayOnlyTasksWithMyParticipation;
            DisplayTasksRefreshingProgressBar = preferencesService.Preferences.DisplayTasksRefreshingProgressBar;
            EnableTasksListAutoRefreshing = preferencesService.Preferences.EnableTasksListAutoRefreshing;
            HideCanceledTasks = preferencesService.Preferences.HideCanceledTasks;
            TasksPerPage = preferencesService.Preferences.TasksPerPage;
        }
    }
}
