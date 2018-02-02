using System.Windows;
using Microsoft.Practices.Unity;
using MahApps.Metro.Controls;
using Tasks_Prism.Views.Main;
using Tasks_Prism.Helpers;
using Tasks_Prism.ViewModels.Login;

namespace Tasks_Prism.Views.Login
{
    public partial class LoginWindow : MetroWindow
    {
        public LoginWindow(IUnityContainer unityContainer)
        {
            InitializeComponent();

            var viewmodel = DataContext as LoginWindowViewModel;
            viewmodel.ExceptionOccured += (ex) => MessageBoxes.CriticalQuestion(ex.ToString());

            viewmodel.ShowMainWindow += () =>
            {
                unityContainer.Resolve<MainWindow>().Show();
                Close();
            };

            viewmodel.BadCredentials += () => MessageBoxes.Warning("Nieprawidłowa nazwa użytkownika i hasło.");
            viewmodel.UserBlocked += (username) => MessageBoxes.Warning($"Użytkownik '{username}' został zablokowany przez administratora systemu.");

            Unloaded += (sender, e) => viewmodel.UnsubscribePrismEvents();
        }
    }
}
