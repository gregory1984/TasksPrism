using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Tasks_Model.Interfaces;
using Tasks_Prism.Events;

namespace Tasks_Prism.ViewModels.ProgressBar
{
    public class ProgressBarViewModel : BindableBase
    {
        #region Properties
        private Visibility visibility;
        public Visibility Visibility
        {
            get { return visibility; }
            set { SetProperty(ref visibility, value); }
        }

        private int max;
        public int Max
        {
            get { return max; }
            set { SetProperty(ref max, value); }
        }

        private int min;
        public int Min
        {
            get { return min; }
            set { SetProperty(ref min, value); }
        }

        private int progress;
        public int Progress
        {
            get { return progress; }
            set { SetProperty(ref progress, value); }
        }
        #endregion

        private readonly Timer refreshTimer;
        private readonly IPreferencesService preferencesService;
        private readonly IEventAggregator eventAggregator;

        public ProgressBarViewModel(IEventAggregator eventAggregator, IPreferencesService preferencesService)
        {
            this.preferencesService = preferencesService;
            this.eventAggregator = eventAggregator;
            this.refreshTimer = new Timer();

            if (preferencesService.Preferences.EnableTasksListAutoRefreshing)
            {
                Visibility = preferencesService.Preferences.DisplayTasksRefreshingProgressBar
                    ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }

            Min = Progress = 0;
            Max = preferencesService.Preferences.AutoRefreshingFrequency;
        }

        public void StartCountdown()
        {
            StopCountdown();

            if (preferencesService.Preferences.EnableTasksListAutoRefreshing)
            {
                refreshTimer.Interval = 1000;
                refreshTimer.Elapsed += (sender, e) =>
                {
                    if (Progress < Max) Progress++;
                    else
                    {
                        eventAggregator.GetEvent<RefreshTasksGridEvent>().Publish();
                        Progress = Min;
                    }
                };
                refreshTimer.Start();
            }
        }

        public void StopCountdown()
        {
            refreshTimer.Stop();
        }
    }
}
