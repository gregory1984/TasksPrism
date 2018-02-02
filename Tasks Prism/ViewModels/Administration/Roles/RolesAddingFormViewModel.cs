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
    public class RolesAddingFormViewModel : ViewModelBase
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

        public delegate void RolenameEmptyDelegate();
        public event RolenameEmptyDelegate RolenameEmpty;

        public delegate void RolenameExistsDelegate(string rolename);
        public event RolenameExistsDelegate RolenameExists;

        public delegate void RoleAddedDelegate(string rolename);
        public event RoleAddedDelegate RoleAdded;

        public delegate void NoRolesBrowsingSelectedDelegate();
        public event NoRolesBrowsingSelectedDelegate NoRolesBrowsingSelected;

        public delegate void NoUsersBrowsingSelectedDelegate();
        public event NoUsersBrowsingSelectedDelegate NoUsersBrowsingSelected;

        public delegate void NoTasksBrowsingSelectedDelegate();
        public event NoTasksBrowsingSelectedDelegate NoTasksBrowsingSelected;
        #endregion

        private readonly IAdministrationService administrationService;

        public RolesAddingFormViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IUserService userService, IAdministrationService administrationService)
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
                    SetPermissions();
                    ClearForm();
                    SetPrivileges();
                });
            }));
        }

        private DelegateCommand addRole;
        public DelegateCommand AddRole
        {
            get => addRole ?? (addRole = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    var role = new UserRoleDTO
                    {
                        IsSystemRole = false,
                        Name = Rolename,
                    };

                    var permissions = new List<PermissionDTO>();
                    foreach (PermissionViewModel p in Permissions.Where(p => p.IsChecked))
                    {
                        permissions.Add(new PermissionDTO(p.Id, p.Name));
                    }

                    role.Permissions = permissions;

                    var status = administrationService.AddUserRole(role);
                    switch (status)
                    {
                        case UserRoleStatus.Added: { RoleAdded?.Invoke(Rolename); ClearForm(); break; }
                        case UserRoleStatus.NoPermissionsSelected: { NoPermissionsSelected?.Invoke(); break; }
                        case UserRoleStatus.RolenameExists: { RolenameExists?.Invoke(Rolename); break; }
                        case UserRoleStatus.ElementEmpty: { RolenameEmpty?.Invoke(); break; }
                        case UserRoleStatus.NoRolesBrowsingSelected: { NoRolesBrowsingSelected?.Invoke(); break; }
                        case UserRoleStatus.NoUsersBrowsingSelected: { NoUsersBrowsingSelected?.Invoke(); break; }
                        case UserRoleStatus.NoTasksBrowsingSelected: { NoTasksBrowsingSelected?.Invoke(); break; }
                    }
                });
            }));
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

            if (granted.Contains(Permission.RolesAdding))
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
            SelectedPermission = null;

            Rolename = "";
            foreach (PermissionViewModel p in Permissions)
            {
                p.IsChecked = false;
            }
        }
    }
}
