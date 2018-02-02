using System;
using System.Windows;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using MahApps.Metro.Controls;
using Tasks_Prism.Helpers;
using Tasks_Prism.ViewModels.Preferences;

namespace Tasks_Prism.Views.Preferences
{
    public partial class PreferencesWindow : MetroWindow
    {
        public PreferencesWindow()
        {
            InitializeComponent();

            var viewmodel = DataContext as PreferencesWindowViewModel;

            viewmodel.PreferencesWindowClosed += () => Close();

            viewmodel.PreferencesSaved += () =>
            {
                var message = "Ustawienia zostały zapisane.\nAplikacja zostanie uruchomiona ponownie.";
                MessageBoxes.Warning(message);

                var location = Application.ResourceAssembly.Location;
                Process.Start(location);
                Environment.Exit(0);
            };

            Unloaded += (sender, e) => viewmodel.UnsubscribePrismEvents();
        }
    }
}
