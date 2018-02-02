using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Threading;
using System.Linq;
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

namespace Tasks_Prism.ViewModels.Administration.Users
{
    public class UsersModifyingFormViewModel : ViewModelBase
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

        private Visibility modifyUserButtonVisibility;
        public Visibility ModifyUserButtonVisibility
        {
            get { return modifyUserButtonVisibility; }
            set { SetProperty(ref modifyUserButtonVisibility, value); }
        }

        private Visibility resetPasswordButtonVisibility;
        public Visibility ResetPasswordButtonVisibility
        {
            get { return resetPasswordButtonVisibility; }
            set { SetProperty(ref resetPasswordButtonVisibility, value); }
        }

        private bool isEnabled;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set { SetProperty(ref isEnabled, value); }
        }
        #endregion

        #region Properties
        private IList<UserDTO> users;
        public IList<UserDTO> Users
        {
            get { return users; }
            set { SetProperty(ref users, value); }
        }

        private UserDTO selectedUser;
        public UserDTO SelectedUser
        {
            get { return selectedUser; }
            set
            {
                SetProperty(ref selectedUser, value);
                ResetPassword.RaiseCanExecuteChanged();
                ModifyUser.RaiseCanExecuteChanged();
                FillForm(selectedUser);
            }
        }

        private string name = "";
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string surname = "";
        public string Surname
        {
            get { return surname; }
            set { SetProperty(ref surname, value); }
        }

        private string eMail = "";
        public string EMail
        {
            get { return eMail; }
            set { SetProperty(ref eMail, value); }
        }

        private string phone = "";
        public string Phone
        {
            get { return phone; }
            set { SetProperty(ref phone, value); }
        }

        private IList<UserStatusViewModel> userStatuses;
        public IList<UserStatusViewModel> UserStatuses
        {
            get { return userStatuses; }
            set { SetProperty(ref userStatuses, value); }
        }

        private UserStatusViewModel selectedUserStatus;
        public UserStatusViewModel SelectedUserStatus
        {
            get { return selectedUserStatus; }
            set { SetProperty(ref selectedUserStatus, value); }
        }

        private IList<UserRoleViewModel> userRoles;
        public IList<UserRoleViewModel> UserRoles
        {
            get { return userRoles; }
            set { SetProperty(ref userRoles, value); }
        }

        private UserRoleViewModel selectedUserRole;
        public UserRoleViewModel SelectedUserRole
        {
            get { return selectedUserRole; }
            set { SetProperty(ref selectedUserRole, value); }
        }
        #endregion

        #region Delegates
        public delegate bool PasswordResettingRequestDelegate(string username);
        public event PasswordResettingRequestDelegate PasswordResettingRequest;

        public delegate void PasswordResetedSuccessfullyDelegate();
        public PasswordResetedSuccessfullyDelegate PasswordResetedSuccessfully;

        public delegate bool UserModificationRequestDelegate(string username);
        public event UserModificationRequestDelegate UserModificationRequest;

        public delegate void UserModifiedDelegate();
        public event UserModifiedDelegate UserModified;

        public delegate void ElementEmptyDelegate();
        public event ElementEmptyDelegate ElementEmpty;

        public delegate void NoRolesSelectedDelegate();
        public event NoRolesSelectedDelegate NoRolesSelected;
        #endregion

        private readonly IAdministrationService administrationService;

        public UsersModifyingFormViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IUserService userService, IAdministrationService administrationService)
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
                    SetUserRoles();
                    SetUserStatuses();
                    SetUsers();
                    ClearForm();
                    SetPrivileges();
                });
            }));
        }

        private DelegateCommand resetPassword;
        public DelegateCommand ResetPassword
        {
            get => resetPassword ?? (resetPassword = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    if ((bool)PasswordResettingRequest?.Invoke(selectedUser.Username))
                    {
                        administrationService.ResetPassword(selectedUser.Id.Value);
                        PasswordResetedSuccessfully?.Invoke();
                    }
                });
            },

            () => SelectedUser != null));
        }

        private DelegateCommand modifyUser;
        public DelegateCommand ModifyUser
        {
            get => modifyUser ?? (modifyUser = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    if ((bool)UserModificationRequest?.Invoke(selectedUser.Username))
                    {
                        var user = new UserDTO
                        {
                            Id = selectedUser.Id,
                            Name = this.Name,
                            Surname = this.Surname,
                            Phone = this.Phone,
                            EMail = this.EMail,
                            Status = new UserStatusDTO { Id = selectedUserStatus.Id }
                        };

                        var roles = new List<UserRoleDTO>();
                        foreach (var role in UserRoles.Where(r => r.IsChecked))
                        {
                            roles.Add(new UserRoleDTO { Id = role.Id });
                        }
                        user.Roles = roles;

                        var status = administrationService.ModifyUser(user);
                        switch (status)
                        {
                            case UserAccountStatus.Modified: { UserModified?.Invoke(); ClearForm(); SetUsers(); break; }
                            case UserAccountStatus.ElementEmpty: { ElementEmpty?.Invoke(); break; }
                            case UserAccountStatus.NoRolesSelected: { NoRolesSelected?.Invoke(); break; }
                        }
                    }
                });
            },

            () => SelectedUser != null));
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

            if (granted.Contains(Permission.UsersBrowsing))
            {
                IsEnabled = false;
                FormVisibility = Visibility.Visible;
                WarningVisibility = Visibility.Collapsed;
                ModifyUserButtonVisibility = Visibility.Collapsed;
                ResetPasswordButtonVisibility = Visibility.Collapsed;

                if (granted.Contains(Permission.UsersModifying))
                {
                    IsEnabled = true;
                    ModifyUserButtonVisibility = Visibility.Visible;
                }

                if (granted.Contains(Permission.UsersPasswordReseting))
                    ResetPasswordButtonVisibility = Visibility.Visible;

                foreach (UserRoleViewModel r in UserRoles)
                    r.IsEnabled = IsEnabled;
            }
            else
            {
                FormVisibility = Visibility.Collapsed;
                WarningVisibility = Visibility.Visible;
            }
        }

        private void SetUserStatuses()
        {
            UserStatuses = new ObservableCollection<UserStatusViewModel>();
            foreach (UserStatusDTO s in administrationService.GetUserStatuses())
            {
                UserStatuses.Add(new UserStatusViewModel(s.Id, s.Name));
            }
        }

        private void SetUserRoles()
        {
            UserRoles = new ObservableCollection<UserRoleViewModel>();
            foreach (UserRoleDTO r in administrationService.GetUserRoles())
            {
                UserRoles.Add(new UserRoleViewModel(r.Id, false, r.IsSystemRole, r.Name));
            }
        }

        private void SetUsers()
        {
            Users = new ObservableCollection<UserDTO>(administrationService.GetUsers().Where(u => u.Username != Tasks_Model.Helpers.Constants.AdminUsername));
        }

        private void FillForm(UserDTO selectedUser)
        {
            if (selectedUser != null)
            {
                Name = selectedUser.Name;
                Surname = selectedUser.Surname;
                Phone = selectedUser.Phone;
                EMail = selectedUser.EMail;

                SelectedUserStatus = UserStatuses.SingleOrDefault(s => s.Id == selectedUser.Status.Id);

                IList<int> roleIds = selectedUser.Roles.Select(r => r.Id).ToList();
                foreach (UserRoleViewModel role in UserRoles)
                {
                    role.IsChecked = roleIds.Contains(role.Id) ? true : false;
                }
            }
        }

        private void ClearForm()
        {
            Name = Surname = Phone = EMail = "";
            SelectedUser = null;
            SelectedUserStatus = UserStatuses.SingleOrDefault(s => s.Name == Tasks_Model.Helpers.Constants.UserStatus_Active);

            foreach (UserRoleViewModel role in UserRoles)
            {
                role.IsChecked = false;
            }
        }
    }
}
