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
    public class RolesDeletingFormViewModel : ViewModelBase
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
                DeleteRole.RaiseCanExecuteChanged();
                FillForm(selectedRole);
            }
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
        public delegate void RoleHasUsersDelegate(string rolename);
        public event RoleHasUsersDelegate RoleHasUsers;

        public delegate bool RoleDeletionRequestDelegate(string rolename);
        public event RoleDeletionRequestDelegate RoleDeletionRequest;

        public delegate void RoleDeletedDelegate(string rolename);
        public event RoleDeletedDelegate RoleDeleted;
        #endregion

        private readonly IAdministrationService administrationService;

        public RolesDeletingFormViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IUserService userService, IAdministrationService administrationService)
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

        private DelegateCommand deleteRole;
        public DelegateCommand DeleteRole
        {
            get => deleteRole ?? (deleteRole = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    if ((bool)RoleDeletionRequest?.Invoke(selectedRole.Name))
                    {
                        var status = administrationService.DeleteUserRole(selectedRole.Id);
                        switch (status)
                        {
                            case UserRoleStatus.Deleted: { RoleDeleted?.Invoke(selectedRole.Name); ClearForm(); SetRoles(); break; }
                            case UserRoleStatus.HasUsers: { RoleHasUsers?.Invoke(selectedRole.Name); break; }
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

            if (granted.Contains(Permission.RolesRemoving))
            {
                FormVisibility = Visibility.Visible;
                WarningVisibility = Visibility.Collapsed;
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

            foreach (var p in administrationService.GetPermissions())
            {
                Permissions.Add(new PermissionViewModel(p.Id, false, p.Name, eventAggregator));
            }
        }

        private void ClearForm()
        {
            SelectedRole = null;

            foreach (var p in Permissions)
            {
                p.IsChecked = false;
            }
        }

        private void FillForm(UserRoleDTO selectedRole)
        {
            if (selectedRole != null)
            {
                IList<int> permIds = selectedRole.Permissions.Select(p => p.Id).ToList();
                foreach (var p in Permissions)
                {
                    p.IsChecked = permIds.Contains(p.Id) ? true : false;
                }
            }
        }
    }
}
