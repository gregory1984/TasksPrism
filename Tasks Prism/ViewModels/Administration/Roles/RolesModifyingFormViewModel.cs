using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Mvvm;
using Prism.Commands;
using Tasks_Prism.Events;
using Tasks_Prism.Helpers;
using Tasks_Prism.ViewModels.Base;
using Tasks_Prism.ViewModels.Administration.Helpers;
using Tasks_Model.DTO;
using Tasks_Model.Interfaces;

namespace Tasks_Prism.ViewModels.Administration.Roles
{
    public class RolesModifyingFormViewModel : ViewModelBase
    {
        #region Privileges
        private Visibility warningVisibility;
        public Visibility WarningVisibility
        {
            get { return warningVisibility; }
            set { SetProperty(ref warningVisibility, value); }
        }

        private Visibility fromVisibility;
        public Visibility FormVisibility
        {
            get { return fromVisibility; }
            set { SetProperty(ref fromVisibility, value); }
        }

        private Visibility modifyRoleButtonVisibility;
        public Visibility ModifyRoleButtonVisibility
        {
            get { return modifyRoleButtonVisibility; }
            set { SetProperty(ref modifyRoleButtonVisibility, value); }
        }

        private bool isEnabled;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set { SetProperty(ref isEnabled, value); }
        }
        #endregion

        #region Properties
        private IList<UserRoleDTO> roles;
        public IList<UserRoleDTO> Roles
        {
            get { return roles; }
            set { SetProperty(ref roles, value); }
        }

        private UserRoleDTO selectedRole;
        public UserRoleDTO SelectedRole
        {
            get { return selectedRole; }
            set
            {
                SetProperty(ref selectedRole, value);
                ModifyRole.RaiseCanExecuteChanged();
                FillForm(selectedRole);
                SetPrivileges();
            }
        }

        private string rolename = "";
        public string Rolename
        {
            get { return rolename; }
            set { SetProperty(ref rolename, value); }
        }

        private IList<PermissionViewModel> permissions;
        public IList<PermissionViewModel> Permissions
        {
            get { return permissions; }
            set { SetProperty(ref permissions, value); }
        }

        private PermissionViewModel selectedPermission;
        public PermissionViewModel SelectedPermission
        {
            get { return selectedPermission; }
            set { SetProperty(ref selectedPermission, value); }
        }
        #endregion

        #region Delegates
        public delegate void NoPemissionsSelectedDelegate();
        public event NoPemissionsSelectedDelegate NoPermissionsSelected;

        public delegate void RolenameExistsDelegate(string rolename);
        public event RolenameExistsDelegate RolenameExists;

        public delegate void RolenameEmptyDelegate();
        public event RolenameEmptyDelegate RolenameEmpty;

        public delegate bool RoleModificationRequestDelegate(string rolename);
        public event RoleModificationRequestDelegate RoleModificationRequest;

        public delegate void RoleModifiedDelegate(string rolename);
        public event RoleModifiedDelegate RoleModified;

        public delegate void NoRolesBrowsingSelectedDelegate();
        public event NoRolesBrowsingSelectedDelegate NoRolesBrowsingSelected;

        public delegate void NoUsersBrowsingSelectedDelegate();
        public event NoUsersBrowsingSelectedDelegate NoUsersBrowsingSelected;

        public delegate void NoTasksBrowsingSelectedDelegate();
        public event NoTasksBrowsingSelectedDelegate NoTasksBrowsingSelected;
        #endregion

        private readonly IAdministrationService administrationService;

        public RolesModifyingFormViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IUserService userService, IAdministrationService administrationService)
            : base(eventAggregator, unityContainer, userService)
        {
            this.administrationService = administrationService;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    SetRoles();
                    SetPermissions();
                    ClearForm();
                    SetPrivileges();
                });
            }));
        }

        private DelegateCommand modifyRole;
        public DelegateCommand ModifyRole
        {
            get => modifyRole ?? (modifyRole = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    if ((bool)RoleModificationRequest?.Invoke(selectedRole.Name))
                    {
                        var role = new UserRoleDTO
                        {
                            Id = selectedRole.Id,
                            IsSystemRole = false,
                            Name = selectedRole.Name,
                            NewName = Rolename
                        };

                        var permissions = new List<PermissionDTO>();
                        foreach (var p in Permissions.Where(p => p.IsChecked))
                        {
                            permissions.Add(new PermissionDTO(p.Id, p.Name));
                        }

                        role.Permissions = permissions;

                        var status = administrationService.ModifyUserRole(role);
                        switch (status)
                        {
                            case UserRoleStatus.Modified: { RoleModified?.Invoke(selectedRole.Name); ClearForm(); SetRoles(); break; }
                            case UserRoleStatus.NoPermissionsSelected: { NoPermissionsSelected?.Invoke(); break; }
                            case UserRoleStatus.RolenameExists: { RolenameExists?.Invoke(Rolename); break; }
                            case UserRoleStatus.ElementEmpty: { RolenameEmpty?.Invoke(); break; }
                            case UserRoleStatus.NoRolesBrowsingSelected: { NoRolesBrowsingSelected?.Invoke(); break; }
                            case UserRoleStatus.NoUsersBrowsingSelected: { NoUsersBrowsingSelected?.Invoke(); break; }
                            case UserRoleStatus.NoTasksBrowsingSelected: { NoTasksBrowsingSelected?.Invoke(); break; }
                        }
                    }
                });
            },

            () => selectedRole != null && !selectedRole.IsSystemRole));
        }

        private DelegateCommand close;
        public DelegateCommand Close
        {
            get => close ?? (close = new DelegateCommand(() =>
            {
                eventAggregator.GetEvent<CloseAdministrationWindowEvent>().Publish();
            }));
        }

        private void SetPrivileges()
        {
            IList<Permission> granted = userService.AuthenticatedUserPermissions.ConvertFromDTO();

            if (granted.Contains(Permission.RolesBrowsing))
            {
                IsEnabled = false;
                FormVisibility = Visibility.Visible;
                WarningVisibility = Visibility.Collapsed;
                ModifyRoleButtonVisibility = Visibility.Collapsed;

                if (granted.Contains(Permission.RolesModifying))
                {
                    IsEnabled = true;
                    ModifyRoleButtonVisibility = Visibility.Visible;

                    DisableChangingSystemRole();
                }

                foreach (PermissionViewModel p in Permissions)
                    p.IsEnabled = IsEnabled;
            }
            else
            {
                FormVisibility = Visibility.Collapsed;
                WarningVisibility = Visibility.Visible;
            }
        }

        private void SetRoles()
        {
            Roles = new ObservableCollection<UserRoleDTO>(administrationService.GetUserRoles());
        }

        private void SetPermissions()
        {
            Permissions = new ObservableCollection<PermissionViewModel>();

            foreach (PermissionDTO p in administrationService.GetPermissions())
            {
                Permissions.Add(new PermissionViewModel(p.Id, false, p.Name, eventAggregator));
            }
        }

        private void ClearForm()
        {
            SelectedRole = null;
            SelectedPermission = null;

            Rolename = "";
            foreach (PermissionViewModel p in Permissions)
            {
                p.IsChecked = false;
            }
        }

        private void FillForm(UserRoleDTO selectedRole)
        {
            if (selectedRole != null)
            {
                Rolename = selectedRole.Name;

                IList<int> permIds = selectedRole.Permissions.Select(p => p.Id).ToList();
                foreach (PermissionViewModel p in Permissions)
                {
                    p.IsChecked = permIds.Contains(p.Id) ? true : false;
                }
            }
        }

        private void DisableChangingSystemRole()
        {
            if (selectedRole != null)
            {
                IsEnabled = !selectedRole.IsSystemRole;
                foreach (PermissionViewModel p in Permissions)
                {
                    p.IsEnabled = !selectedRole.IsSystemRole;
                }
            }
        }
    }
}
