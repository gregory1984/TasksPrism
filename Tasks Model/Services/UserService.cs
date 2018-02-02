using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Transform;
using NHibernate.Criterion;
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
    public class UserService : IUserService
    {
        public UserDTO AuthenticatedUser { get; private set; }

        private IList<PermissionDTO> authenticatedUserPermissions;
        public IList<PermissionDTO> AuthenticatedUserPermissions
        {
            get
            {
                if (authenticatedUserPermissions == null)
                {
                    authenticatedUserPermissions = GetPermissions();
                }
                return authenticatedUserPermissions;
            }
        }

        public void Authenticate(string username, string password)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                User u = null;
                UserStatus s = null;

                var user = session.QueryOver(() => u)
                    .JoinAlias(() => u.Status, () => s)
                    .Where(() => u.Username == username)
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
                    .SingleOrDefault<UserDTO>();

                if (user != null && user.HashedPassword == Crypto.GenerateSHA256(password + user.Salt))
                {
                    UserRole r = null;

                    var userRoles = session.QueryOver(() => r)
                        .JoinAlias(() => r.Users, () => u)
                        .Where(() => u.Id == user.Id)
                        .SelectList(l => l
                            .Select(() => r.Id)
                            .Select(() => r.Name)
                            .Select(() => r.IsSystemRole))
                        .TransformUsing(Transformers.AliasToBeanConstructor(typeof(UserRoleDTO).GetConstructors()[1]))
                        .List<UserRoleDTO>();

                    user.Roles = userRoles;
                    AuthenticatedUser = user;
                }
                else AuthenticatedUser = new UserDTO();
            }
        }

        public PasswordChangingStatus ChangePassword(string currentPassword, string newPassword, string retypedPassword)
        {
            if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(retypedPassword))
                return PasswordChangingStatus.ElementEmpty;

            if (Crypto.GenerateSHA256(currentPassword + AuthenticatedUser.Salt) != AuthenticatedUser.HashedPassword)
                return PasswordChangingStatus.CurrentPasswordIncorrect;

            if (newPassword != retypedPassword)
                return PasswordChangingStatus.NewPasswordsNotSame;

            if (newPassword.Length < Constants.MinPasswordLength)
                return PasswordChangingStatus.MinLengthViolation;

            AuthenticatedUser.HashedPassword = Crypto.GenerateSHA256(newPassword + AuthenticatedUser.Salt);

            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                var user = session.QueryOver<User>().Where(u => u.Id == AuthenticatedUser.Id.Value).SingleOrDefault();
                user.HashedPassword = AuthenticatedUser.HashedPassword;

                session.Update(user);
                session.Flush();

                return PasswordChangingStatus.Changed;
            }
        }

        private IList<PermissionDTO> GetPermissions()
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                Permission p = null;
                UserRole r = null;
                User u = null;

                var permissions = session.QueryOver(() => p)
                    .JoinAlias(() => p.Roles, () => r)
                    .JoinAlias(() => r.Users, () => u)
                    .Where(Res.Eq(Pro.Property("u.Id"), AuthenticatedUser.Id.Value))
                    .SelectList(l => l
                        .Select(Pro.Group(() => p.Id))
                        .Select(Pro.Group(() => p.Name)))
                    .TransformUsing(Transformers.AliasToBeanConstructor(typeof(PermissionDTO).GetConstructors()[1]))
                    .List<PermissionDTO>();

                return permissions;
            }
        }

        public void ResetPermissions()
        {
            authenticatedUserPermissions = null;
        }
    }
}
