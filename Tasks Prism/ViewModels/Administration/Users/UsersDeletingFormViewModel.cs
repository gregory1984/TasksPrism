using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class UsersDeletingFormViewModel : ViewModelBase
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
                DeleteUser.RaiseCanExecuteChanged();
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

        private string userStatus;
        public string UserStatus
        {
            get { return userStatus; }
            set { SetProperty(ref userStatus, value); }
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
        public delegate void UserHasTasksDelegate(string username);
        public event UserHasTasksDelegate UserHasTasks;

        public delegate bool UserDeletionRequestDelegate(string username);
        public event UserDeletionRequestDelegate UserDeletionRequest;

        public delegate void UserDeletedDelegate();
        public event UserDeletedDelegate UserDeleted;
        #endregion

        private readonly IAdministrationService administrationService;

        public UsersDeletingFormViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IUserService userService, IAdministrationService administrationService)
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
                    SetUsers();
                    SetUserRoles();
                    SetPrivileges();
                });
            }));
        }

        private DelegateCommand deleteUser;
        public DelegateCommand DeleteUser
        {
            get => deleteUser ?? (deleteUser = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    if ((bool)UserDeletionRequest?.Invoke(selectedUser.Username))
                    {
                        var status = administrationService.DeleteUser(selectedUser.Id.Value);
                        switch (status)
                        {
                            case UserAccountStatus.Deleted: { UserDeleted?.Invoke(); ClearForm(); SetUsers(); break; }
                            case UserAccountStatus.HasTasks: { UserHasTasks?.Invoke(selectedUser.Username); break; }
                        }
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

            if (granted.Contains(Permission.UsersRemoving))
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

        private void SetUsers()
        {
            Users = new ObservableCollection<UserDTO>(administrationService.GetUsers().Where(u => u.Username != Tasks_Model.Helpers.Constants.AdminUsername));
        }

        private void SetUserRoles()
        {
            UserRoles = new ObservableCollection<UserRoleViewModel>();
            foreach (var r in administrationService.GetUserRoles())
            {
                UserRoles.Add(new UserRoleViewModel(r.Id, false, r.IsSystemRole, r.Name));
            }
        }

        private void FillForm(UserDTO selectedUser)
        {
            if (selectedUser != null)
            {
                Name = selectedUser.Name;
                Surname = selectedUser.Surname;
                Phone = selectedUser.Phone;
                EMail = selectedUser.EMail;
                UserStatus = selectedUser.Status.Name;

                IList<int> roleIds = selectedUser.Roles.Select(r => r.Id).ToList();
                foreach (var role in UserRoles)
                {
                    role.IsChecked = roleIds.Contains(role.Id) ? true : false;
                }
            }
        }

        private void ClearForm()
        {
            Name = Surname = Phone = EMail = UserStatus = "";
            SelectedUser = null;

            foreach (var role in UserRoles)
            {
                role.IsChecked = false;
            }
        }
    }
}
