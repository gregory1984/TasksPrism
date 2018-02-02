using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Unity;
using MahApps.Metro.Controls;
using Tasks_Prism.Helpers;
using Tasks_Prism.ViewModels.Administration.Roles;

namespace Tasks_Prism.Views.Administration.Roles
{
    public partial class RolesDeletingForm : UserControl
    {
        public RolesDeletingForm()
        {
            InitializeComponent();

            var viewmodel = DataContext as RolesDeletingFormViewModel;

            viewmodel.RoleDeletionRequest += (rolename) =>
            {
                var result = MessageBox.Show($"Czy na pewno usunąć rolę '{rolename}'?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                return result == MessageBoxResult.Yes;
            };

            viewmodel.RoleDeleted += (rolename) => MessageBoxes.Information($"Rola '{rolename}'została pomyślnie usunięta.");

            viewmodel.RoleHasUsers += (rolename)
                => MessageBoxes.Warning($"Rola '{rolename}' nie może zostać usunięta, ponieważ posiada przypisanych do siebie użytkowników.");

            Unloaded += (sender, e) => viewmodel.UnsubscribePrismEvents();
        }
    }
}
