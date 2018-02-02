using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.Transform;
using NHibernate.Criterion;
using NHibernate.Linq;
using Tasks_Crypto;
using Tasks_Model.Helpers;
using Tasks_Model.Entities;
using Tasks_Model.Database;
using Tasks_Model.DTO;
using Tasks_Model.Interfaces;
using Pro = NHibernate.Criterion.Projections;
using Res = NHibernate.Criterion.Restrictions;
using SqlType = NHibernate.NHibernateUtil;

namespace Tasks_Model.Services
{
    public class AdministrationService : IAdministrationService
    {
        private IList<UserStatusDTO> userStatuses;
        private IList<PermissionDTO> permissions;

        public IList<UserRoleDTO> GetUserRoles()
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                UserRole r = null;

                var userRoles = session.QueryOver(() => r)
                    .SelectList(l => l
                        .Select(() => r.Id)
                        .Select(() => r.Name)
                        .Select(() => r.IsSystemRole))
                    .TransformUsing(Transformers.AliasToBeanConstructor(typeof(UserRoleDTO).GetConstructors()[1]))
                    .List<UserRoleDTO>();

                foreach (var role in userRoles)
                {
                    Permission p = null;
                    UserRole ur = null;

                    var permissions = session.QueryOver(() => p)
                        .JoinAlias(() => p.Roles, () => ur)
                        .Where(() => ur.Id == role.Id)
                        .SelectList(l => l
                            .Select(() => p.Id)
                            .Select(() => p.Name))
                        .TransformUsing(Transformers.AliasToBeanConstructor(typeof(PermissionDTO).GetConstructors()[1]))
                        .List<PermissionDTO>();

                    role.Permissions = permissions;
                }

                return userRoles;
            }
        }

        public IList<UserStatusDTO> GetUserStatuses()
        {
            if (userStatuses == null)
            {
                using (var session = Hibernate.SessionFactory.OpenSession())
                {
                    UserStatus s = null;

                    userStatuses = session.QueryOver(() => s)
                        .SelectList(l => l
                            .Select(() => s.Id)
                            .Select(() => s.Name))
                        .TransformUsing(Transformers.AliasToBeanConstructor(typeof(UserStatusDTO).GetConstructors()[1]))
                        .List<UserStatusDTO>();
                }
            }
            return userStatuses;
        }

        public IList<UserDTO> GetUsers()
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                User u = null;
                UserStatus s = null;

                var users = session.QueryOver(() => u)
                    .JoinAlias(() => u.Status, () => s)
                    .SelectList(l => l
                        .Select(() => u.Id)
                        .Select(() => u.Username)
                        .Select(() => u.HashedPassword)
                        .Select(() => u.Salt)
                        .Select(() => u.Name)
                        .Select(() => u.Surname)
                        .Select(() => u.Phone)
                        .Select(() => u.EMail)
                        .Select(() => s.Id)
                        .Select(() => s.Name)
                        .Select(() => u.IsSystemUser))
                    .TransformUsing(Transformers.AliasToBeanConstructor(typeof(UserDTO).GetConstructors()[1]))
                    .List<UserDTO>();

                foreach (var user in users)
                {
                    User us = null;
                    UserRole r = null;

                    var roles = session.QueryOver(() => r)
                        .JoinAlias(() => r.Users, () => us)
                        .Where(() => us.Id == user.Id)
                        .SelectList(l => l
                            .Select(() => r.Id)
                            .Select(() => r.Name)
                            .Select(() => r.IsSystemRole))
                        .TransformUsing(Transformers.AliasToBeanConstructor(typeof(UserRoleDTO).GetConstructors()[1]))
                        .List<UserRoleDTO>();

                    user.Roles = roles;
                }

                return users;
            }
        }

        public IList<UserSimpleDTO> GetUsersSimpleData(bool all = true)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                User u = null;

                var users = session.QueryOver(() => u)
                    .Where(Res.EqProperty(Pro.Property("u.IsSystemUser"),
                        Pro.Conditional(Res.Eq(Pro.Constant(all), true), Pro.Property("u.IsSystemUser"), Pro.Constant(false, SqlType.Boolean))))
                    .SelectList(l => l
                        .Select(() => u.Id)
                        .Select(() => u.Username)
                        .Select(() => u.Name)
                        .Select(() => u.Surname))
                    .OrderBy(() => u.Username).Asc
                    .TransformUsing(Transformers.AliasToBeanConstructor(typeof(UserSimpleDTO).GetConstructors()[1]))
                    .List<UserSimpleDTO>();

                return users;
            }
        }

        public IList<PermissionDTO> GetPermissions()
        {
            if (permissions == null)
            {
                using (var session = Hibernate.SessionFactory.OpenSession())
                {
                    permissions = session.QueryOver<Permission>()
                        .SelectList(l => l
                            .Select(p => p.Id)
                            .Select(p => p.Name))
                        .OrderBy(p => p.Id).Asc
                        .TransformUsing(Transformers.AliasToBeanConstructor(typeof(PermissionDTO).GetConstructors()[1]))
                        .List<PermissionDTO>();
                }
            }
            return permissions;
        }

        public void ResetPassword(int userId)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                var user = session.QueryOver<User>().Where(u => u.Id == userId).SingleOrDefault();
                user.HashedPassword = Crypto.GenerateSHA256(user.Username + user.Salt);

                session.Update(user);
                session.Flush();
            }
        }

        public UserAccountStatus AddUser(UserDTO user)
        {
            var fields = new List<string> { user.Username, user.PlainPassword, user.Name, user.Surname, user.EMail, user.Phone };
            if (fields.Any(f => string.IsNullOrWhiteSpace(f)))
                return UserAccountStatus.ElementEmpty;

            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                if (session.QueryOver<User>().Where(u => u.Username == user.Username).RowCount() > 0)
                    return UserAccountStatus.UsernameExists;

                if (user.PlainPassword.Length < Constants.MinPasswordLength)
                    return UserAccountStatus.PasswordMinLengthViolation;

                if (user.Roles.Count == 0)
                    return UserAccountStatus.NoRolesSelected;

                var salt = Crypto.GenerateSalt();
                var newUser = new User
                {
                    Username = user.Username,
                    HashedPassword = Crypto.GenerateSHA256(user.PlainPassword + salt),
                    Salt = salt,
                    Name = user.Name,
                    Surname = user.Surname,
                    EMail = user.EMail,
                    Phone = user.Phone,
                    IsSystemUser = false,
                    Status = session.QueryOver<UserStatus>().Where(s => s.Id == user.Status.Id).SingleOrDefault(),
                    Roles = session.QueryOver<UserRole>().Where(Restrictions.In("Id", user.Roles.Select(r => r.Id).ToList())).List()
                };

                var preference = new UserPreference
                {
                    AutoRefreshingFrequency = 600,
                    DisplayOnlyTasksWithMyParticipation = true,
                    DisplayTasksRefreshingProgressBar = true,
                    EnableTasksListAutoRefreshing = true,
                    HideCanceledTasks = true,
                    TasksPerPage = 50,
                    User = newUser
                };

                session.Save(newUser);
                session.Save(preference);
                session.Flush();

                return UserAccountStatus.Added;
            }
        }

        public UserAccountStatus ModifyUser(UserDTO user)
        {
            var fields = new List<string> { user.Name, user.Surname, user.EMail, user.Phone };
            if (fields.Any(f => string.IsNullOrWhiteSpace(f)))
                return UserAccountStatus.ElementEmpty;

            if (user.Roles.Count == 0)
                return UserAccountStatus.NoRolesSelected;

            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                var userToModify = session.QueryOver<User>().Where(u => u.Id == user.Id.Value).SingleOrDefault();

                userToModify.Name = user.Name;
                userToModify.Surname = user.Surname;
                userToModify.Phone = user.Phone;
                userToModify.EMail = user.EMail;
                userToModify.Status = session.QueryOver<UserStatus>().Where(s => s.Id == user.Status.Id).SingleOrDefault();

                List<int> roleIds = user.Roles.Select(dto => dto.Id).ToList();
                userToModify.Roles = session.QueryOver<UserRole>().Where(Restrictions.In("Id", roleIds)).List();

                session.Update(userToModify);
                session.Flush();

                return UserAccountStatus.Modified;
            }
        }

        public UserAccountStatus DeleteUser(int userId)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                var user = session.QueryOver<User>().Where(u => u.Id == userId).SingleOrDefault();

                if (user.Tasks.Count > 0 || user.Comments.Count > 0)
                    return UserAccountStatus.HasTasks;

                session.Delete(user);
                session.Flush();

                return UserAccountStatus.Deleted;
            }
        }

        public UserRoleStatus AddUserRole(UserRoleDTO role)
        {
            if (string.IsNullOrWhiteSpace(role.Name))
                return UserRoleStatus.ElementEmpty;

            if (role.Permissions.Count == 0)
                return UserRoleStatus.NoPermissionsSelected;

            var rolesManagingPermissions = new[] { 7, 8, 9 };
            if (role.Permissions.Any(p => rolesManagingPermissions.Contains(p.Id)) && !role.Permissions.Any(pe => pe.Id == 6))
                return UserRoleStatus.NoRolesBrowsingSelected;

            var usersManagingPermissions = new[] { 2, 3, 4, 5 };
            if (role.Permissions.Any(p => usersManagingPermissions.Contains(p.Id)) && !role.Permissions.Any(pe => pe.Id == 1))
                return UserRoleStatus.NoUsersBrowsingSelected;

            var tasksManagingPermissions = new[] { 11, 12, 13 };
            if (role.Permissions.Any(p => tasksManagingPermissions.Contains(p.Id)) && !role.Permissions.Any(pe => pe.Id == 10))
                return UserRoleStatus.NoTasksBrowsingSelected;

            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                if (session.QueryOver<UserRole>().Where(r => r.Name == role.Name).RowCount() > 0)
                    return UserRoleStatus.RolenameExists;

                var newRole = new UserRole
                {
                    IsSystemRole = false,
                    Name = role.Name,
                    Permissions = session.QueryOver<Permission>().Where(Res.In("Id", role.Permissions.Select(p => p.Id).ToList())).List()
                };

                session.Save(newRole);
                session.Flush();

                return UserRoleStatus.Added;
            }
        }

        public UserRoleStatus ModifyUserRole(UserRoleDTO role)
        {
            if (string.IsNullOrWhiteSpace(role.NewName))
                return UserRoleStatus.ElementEmpty;

            if (role.Permissions.Count == 0)
                return UserRoleStatus.NoPermissionsSelected;

            var rolesManagingPermissions = new[] { 7, 8, 9 };
            if (role.Permissions.Any(p => rolesManagingPermissions.Contains(p.Id)) && !role.Permissions.Any(pe => pe.Id == 6))
                return UserRoleStatus.NoRolesBrowsingSelected;

            var usersManagingPermissions = new[] { 2, 3, 4, 5 };
            if (role.Permissions.Any(p => usersManagingPermissions.Contains(p.Id)) && !role.Permissions.Any(pe => pe.Id == 1))
                return UserRoleStatus.NoUsersBrowsingSelected;

            var tasksManagingPermissions = new[] { 11, 12, 13 };
            if (role.Permissions.Any(p => tasksManagingPermissions.Contains(p.Id)) && !role.Permissions.Any(pe => pe.Id == 10))
                return UserRoleStatus.NoTasksBrowsingSelected;

            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                var count = session.QueryOver<UserRole>().Where(r => r.Name == role.NewName).RowCount();
                if (role.Name != role.NewName && count > 0)
                    return UserRoleStatus.RolenameExists;

                var roleToModify = session.QueryOver<UserRole>().Where(r => r.Id == role.Id).SingleOrDefault();

                roleToModify.Name = role.NewName;
                roleToModify.Permissions = session.QueryOver<Permission>().Where(Restrictions.In("Id", role.Permissions.Select(p => p.Id).ToList())).List();

                session.Update(roleToModify);
                session.Flush();

                return UserRoleStatus.Modified;
            }
        }

        public UserRoleStatus DeleteUserRole(int roleId)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                var role = session.QueryOver<UserRole>().Where(r => r.Id == roleId).SingleOrDefault();

                if (role.Users.Count > 0)
                    return UserRoleStatus.HasUsers;

                session.Delete(role);
                session.Flush();

                return UserRoleStatus.Deleted;
            }
        }
    }
}
