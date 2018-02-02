using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Unity;
using MahApps.Metro.Controls;
using Tasks_Prism.Helpers;
using Tasks_Prism.ViewModels.Administration.Users;

namespace Tasks_Prism.Views.Administration.Users
{
    public partial class UsersAddingForm : UserControl
    {
        public UsersAddingForm()
        {
            InitializeComponent();

            var viewmodel = DataContext as UsersAddingFormViewModel;

            viewmodel.UserAdded += (username) => MessageBoxes.Information($"Użytkownik '{username}' został pomyślnie dodany do systemu.");
            viewmodel.ElementEmpty += () => MessageBoxes.Warning("Pola formularza nie mogą pozostać puste.");
            viewmodel.NoRolesSelected += () => MessageBoxes.Warning("Nie wybrano roli dla tworzonego użytkownika.");
            viewmodel.MinPasswordLengthViolation += ()
                => MessageBoxes.Warning($"Minimalna długość hasła to {Tasks_Model.Helpers.Constants.MinPasswordLength} znaków");
            viewmodel.UsernameExists += (username) => MessageBoxes.Warning($"Użytkownik o nazwie '{username}' już istnieje w systemie.");

            Unloaded += (sender, e) => viewmodel.UnsubscribePrismEvents();
        }
    }
}
