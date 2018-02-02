using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Unity;
using MahApps.Metro.Controls;
using Tasks_Prism.Helpers;
using Tasks_Prism.ViewModels.Administration.Users;

namespace Tasks_Prism.Views.Administration.Users
{
    /// <summary>
    /// Interaction logic for UsersModifyingForm
    /// </summary>
    public partial class UsersModifyingForm : UserControl
    {
        public UsersModifyingForm()
        {
            InitializeComponent();

            var viewmodel = DataContext as UsersModifyingFormViewModel;

            viewmodel.PasswordResettingRequest += (username) =>
            {
                var description = "Zresetowanie poświadczenia spowoduje ustawienie wybranemu użytkownikowi hasła identycznego z jego nazwą użytkownika.";
                var result = MessageBox.Show(description + $"\n\nCzy zresetować hasło dla '{username}'?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                return result == MessageBoxResult.Yes;
            };

            viewmodel.PasswordResetedSuccessfully += () => MessageBoxes.Information("Hasło zostało pomyślnie zmienione.");
            viewmodel.ElementEmpty += () => MessageBoxes.Warning("Pola formularza nie mogą pozostać puste.");
            viewmodel.NoRolesSelected += () => MessageBoxes.Warning("Nie wybrano roli dla modyfikowanego użytkownika.");

            viewmodel.UserModificationRequest += (username) =>
            {
                var result = MessageBox.Show($"Czy na pewno zapisać zmiany dotyczące konta '{username}'?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                return result == MessageBoxResult.Yes;
            };

            viewmodel.UserModified += () => MessageBoxes.Information("Dane użytkownika zostały pomyślnie zmienione.");

            Unloaded += (sender, e) => viewmodel.UnsubscribePrismEvents();
        }
    }
}
