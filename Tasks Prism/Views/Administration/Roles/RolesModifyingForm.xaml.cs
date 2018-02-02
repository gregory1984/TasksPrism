using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Unity;
using MahApps.Metro.Controls;
using Tasks_Prism.Helpers;
using Tasks_Prism.ViewModels.Administration.Roles;

namespace Tasks_Prism.Views.Administration.Roles
{
    public partial class RolesModifyingForm : UserControl
    {
        public RolesModifyingForm()
        {
            InitializeComponent();

            var viewmodel = DataContext as RolesModifyingFormViewModel;

            viewmodel.NoPermissionsSelected += () => MessageBoxes.Warning("Nie wybrano uprawnień dla modyfikowanej roli.");
            viewmodel.RolenameExists += (rolename) => MessageBoxes.Warning($"Rola o nazwie '{rolename}' już istnieje w systemie.");
            viewmodel.RolenameEmpty += () => MessageBoxes.Warning("Nie wprowdzono nowej nazwy dla wybranej roli.");

            viewmodel.RoleModificationRequest += (rolename) =>
            {
                var result = MessageBox.Show($"Czy na pewno zapisać zmiany dotyczące roli '{rolename}'?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                return result == MessageBoxResult.Yes;
            };

            viewmodel.RoleModified += (rolename) => MessageBoxes.Information($"Dane dotyczące roli '{rolename}' zostały pomyślnie zmienione.");

            viewmodel.NoRolesBrowsingSelected += () => MessageBoxes.Warning("Wybrano uprawnienia do zarządzania rolami " +
                "bez udostępnienia możliwości ich przeglądania.");

            viewmodel.NoUsersBrowsingSelected += () => MessageBoxes.Warning("Wybrano uprawnienia do zarządzania użytkownikami " +
                "bez udostępnienia możliwości ich przeglądania.");

            viewmodel.NoTasksBrowsingSelected += () => MessageBoxes.Warning("Wybrano uprawnienia do zarządzania zleceniami " +
                "bez udostępnienia możliwości ich przeglądania.");

            Unloaded += (sender, e) => viewmodel.UnsubscribePrismEvents();
        }
    }
}
