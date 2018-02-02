using System;
using System.Windows;
using System.Linq;
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

namespace Tasks_Prism.ViewModels.Administration.Users
{
    public class UsersAddingFormViewModel : ViewModelBase
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
        private string username = "";
        public string Username
        {
            get { return username; }
            set { SetProperty(ref username, value); }
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
        public delegate void UserAddedDelegate(string username);
        public event UserAddedDelegate UserAdded;

        public delegate void ElementEmptyDelegate();
        public event ElementEmptyDelegate ElementEmpty;

        public delegate void NoRolesSelectedDelegate();
        public event NoRolesSelectedDelegate NoRolesSelected;

        public delegate void MinPasswordLengthViolationDelegate();
        public event MinPasswordLengthViolationDelegate MinPasswordLengthViolation;

        public delegate void UsernameExistsDelegate(string username);
        public event UsernameExistsDelegate UsernameExists;
        #endregion

        private readonly IAdministrationService administrationService;

        public UsersAddingFormViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IUserService userService, IAdministrationService administrationService)
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
                    SetUserStatuses();
                    SetUserRoles();
                    SetPrivileges();
                });
            }));
        }

        private DelegateCommand<PasswordBox> addUser;
        public DelegateCommand<PasswordBox> AddUser
        {
            get => addUser ?? (addUser = new DelegateCommand<PasswordBox>(p =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    var user = new UserDTO
                    {
                        Username = this.Username,
                        PlainPassword = p.Password,
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

                    var status = administrationService.AddUser(user);
                    switch (status)
                    {
                        case UserAccountStatus.Added: { UserAdded?.Invoke(Username); ClearForm(p); break; }
                        case UserAccountStatus.ElementEmpty: { ElementEmpty?.Invoke(); break; }
                        case UserAccountStatus.NoRolesSelected: { NoRolesSelected?.Invoke(); break; }
                        case UserAccountStatus.PasswordMinLengthViolation: { MinPasswordLengthViolation?.Invoke(); break; }
                        case UserAccountStatus.UsernameExists: { UsernameExists?.Invoke(Username); break; }
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

            if (granted.Contains(Permission.UsersAdding))
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

        private void SetUserStatuses()
        {
            UserStatuses = new ObservableCollection<UserStatusViewModel>();
            foreach (var s in administrationService.GetUserStatuses())
            {
                UserStatuses.Add(new UserStatusViewModel(s.Id, s.Name));
            }

            SelectedUserStatus = UserStatuses.SingleOrDefault(s => s.Name == Tasks_Model.Helpers.Constants.UserStatus_Active);
        }

        private void SetUserRoles()
        {
            UserRoles = new ObservableCollection<UserRoleViewModel>();
            foreach (var r in administrationService.GetUserRoles())
            {
                UserRoles.Add(new UserRoleViewModel(r.Id, false, r.IsSystemRole, r.Name));
            }

            UserRoles.SingleOrDefault(r => r.Name == Tasks_Model.Helpers.Constants.UserRoleName).IsChecked = true;
        }

        private void ClearForm(PasswordBox p)
        {
            Username = Name = Surname = EMail = Phone = p.Password = "";
            SelectedUserStatus = UserStatuses.SingleOrDefault(s => s.Name == Tasks_Model.Helpers.Constants.UserStatus_Active);

            foreach (var r in UserRoles)
            {
                r.IsChecked = false;
            }

            UserRoles.SingleOrDefault(r => r.Name == Tasks_Model.Helpers.Constants.UserRoleName).IsChecked = true;
        }
    }
}
