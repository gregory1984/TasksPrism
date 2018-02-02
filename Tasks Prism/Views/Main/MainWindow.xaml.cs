using System.Windows;
using Microsoft.Practices.Unity;
using MahApps.Metro.Controls;
using Tasks_Prism.Views.Administration;
using Tasks_Prism.Views.Preferences;
using Tasks_Prism.Views.Login;
using Tasks_Prism.Views.About;
using Tasks_Prism.Views.Tasks;
using Tasks_Prism.Helpers;
using Tasks_Prism.ViewModels.Main;

namespace Tasks_Prism.Views.Main
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow(IUnityContainer unityContainer)
        {
            InitializeComponent();

            var viewmodel = DataContext as MainWindowViewModel;
            viewmodel.ExceptionOccured += (ex) => MessageBoxes.CriticalQuestion(ex.ToString());

            viewmodel.ShowTaskCreationWindow += () => unityContainer.Resolve<TaskCreationWindow>().ShowDialog();

            viewmodel.QuitApplication += () => Close();
            viewmodel.Logout += (logoutAction) =>
            {
                var result = MessageBox.Show("Czy na pewno chcesz się wylogować?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    logoutAction();
                    Closing -= MainWindow_Closing;
                    unityContainer.Resolve<LoginWindow>().Show();
                    Close();
                }
            };

            viewmodel.ShowPreferencesWindow += () => unityContainer.Resolve<PreferencesWindow>().ShowDialog();
            viewmodel.ShowAdministrationWindow += () => unityContainer.Resolve<AdministrationWindow>().ShowDialog();
            viewmodel.ShowAboutWindow += () => unityContainer.Resolve<AboutWindow>().ShowDialog();

            Unloaded += (sender, e) => viewmodel.UnsubscribePrismEvents();
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Czy na pewno zamknąć aplikację?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}
