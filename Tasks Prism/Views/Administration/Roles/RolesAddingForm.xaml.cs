using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Unity;
using MahApps.Metro.Controls;
using Tasks_Prism.Helpers;
using Tasks_Prism.ViewModels.Administration.Roles;

namespace Tasks_Prism.Views.Administration.Roles
{
    public partial class RolesAddingForm : UserControl
    {
        public RolesAddingForm()
        {
            InitializeComponent();

            var viewmodel = DataContext as RolesAddingFormViewModel;

            viewmodel.RoleAdded += (rolename) => MessageBoxes.Information($"Rola '{rolename}' została pomyślnie dodana do systemu.");
            viewmodel.NoPermissionsSelected += () => MessageBoxes.Warning("Nie wybrano uprawnień dla tworzonej roli.");
            viewmodel.RolenameExists += (rolename) => MessageBoxes.Warning($"Rola o nazwie '{rolename}' już istnieje w systemie.");
            viewmodel.RolenameEmpty += () => MessageBoxes.Warning("Nie wprowadzono nazwy dla tworzonej roli.");

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
