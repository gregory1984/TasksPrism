using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Unity;
using MahApps.Metro.Controls;
using Tasks_Prism.Helpers;
using Tasks_Prism.ViewModels.Administration.Users;

namespace Tasks_Prism.Views.Administration.Users
{
    /// <summary>
    /// Interaction logic for UsersDeletingForm
    /// </summary>
    public partial class UsersDeletingForm : UserControl
    {
        public UsersDeletingForm()
        {
            InitializeComponent();

            var viewmodel = DataContext as UsersDeletingFormViewModel;

            viewmodel.UserDeletionRequest += (username) =>
            {
                var result = MessageBox.Show($"Czy na pewno usunąć konto '{username}'?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                return result == MessageBoxResult.Yes;
            };

            viewmodel.UserDeleted += () => MessageBoxes.Information("Konto użytkownika zostało pomyślnie usunięte.");
            viewmodel.UserHasTasks += (username) => MessageBoxes.Warning($"Konto użytkownika '{username}' nie może zostać usunięte, ponieważ jest przypisane do zleceń.");

            Unloaded += (sender, e) => viewmodel.UnsubscribePrismEvents();
        }
    }
}
