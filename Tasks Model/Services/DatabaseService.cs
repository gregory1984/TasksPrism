using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using Tasks_Crypto;
using Tasks_Model.DTO;
using Tasks_Model.Helpers;
using Tasks_Model.Database;
using Tasks_Model.Interfaces;
using Tasks_Model.Entities;

namespace Tasks_Model.Services
{
    public class DatabaseService : IDatabaseService
    {
        public void Initialize()
        {
            if (IsDatabaseEmpty())
            {
                using (var session = Hibernate.SessionFactory.OpenSession())
                {
                    InsertInitialData(session);
                    InsertUserStatuses(session);
                    InsertUserRoles(session);
                    InsertPermissions(session);
                    InsertAdminRolePermissions(session);
                    InsertUserRolePermissions(session);
                    InsertAdminUser(session);
                    InsertAdminPreferences(session);
                    InsertTaskStatuses(session);
                    InsertTaskPriorities(session);
                    InsertTaskGenres(session);
                }
                MigrateUsers();
            }
        }

        private bool IsDatabaseEmpty()
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                return session.QueryOver<DatabaseInitialization>().RowCount() == 0;
            }
        }

        private void InsertInitialData(ISession session)
        {
            var init = new DatabaseInitialization { InitDate = DateTime.Now };
            session.Save(init);
            session.Flush();
        }

        private void InsertUserStatuses(ISession session)
        {
            var statuses = new List<UserStatus>
            {
                new UserStatus { Id = 1, Name = Constants.UserStatus_Active },
                new UserStatus { Id = 2, Name = Constants.UserStatus_Blocked }
            };

            foreach (var s in statuses)
            {
                session.Save(s);
                session.Flush();
            }
        }

        private void InsertUserRoles(ISession session)
        {
            var roles = new List<UserRole>
            {
                new UserRole { Id = 1, Name = Constants.AdminRoleName, IsSystemRole = true },
                new UserRole { Id = 2, Name = Constants.UserRoleName, IsSystemRole = true }
            };

            foreach (var r in roles)
            {
                session.Save(r);
                session.Flush();
            }
        }

        private void InsertPermissions(ISession session)
        {
            var usersManagementPermissions = new List<Permission>
            {
                new Permission { Id = 1, Name = "Użytkownicy - przeglądanie" },
                new Permission { Id = 2, Name = "Użytkownicy - dodawanie" },
                new Permission { Id = 3, Name = "Użytkownicy - modyfikowanie" },
                new Permission { Id = 4, Name = "Użytkownicy - usuwanie" },
                new Permission { Id = 5, Name = "Użytkownicy - resetowanie hasła" }
            };

            var rolesManagementPermissions = new List<Permission>
            {
                new Permission { Id = 6, Name = "Role - przeglądanie" },
                new Permission { Id = 7, Name = "Role - dodawanie" },
                new Permission { Id = 8, Name = "Role - modyfikowanie" },
                new Permission { Id = 9, Name = "Role - usuwanie" },
            };

            var tasksManagementPermissions = new List<Permission>
            {
                new Permission { Id = 10, Name = "Zlecenia - przeglądanie" },
                new Permission { Id = 11, Name = "Zlecenia - dodawanie" },
                new Permission { Id = 12, Name = "Zlecenia - modyfikowanie" },
                new Permission { Id = 13, Name = "Zlecenia - usuwanie" }
            };

            var all = new List<Permission>();
            all.AddRange(usersManagementPermissions);
            all.AddRange(rolesManagementPermissions);
            all.AddRange(tasksManagementPermissions);

            foreach (var p in all)
            {
                session.Save(p);
                session.Flush();
            }
        }

        private void InsertAdminRolePermissions(ISession session)
        {
            Permission p = null;
            UserRole r = null;

            var permissions = session.QueryOver(() => p)
                .Where(Restrictions.In(nameof(p.Id), new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 13 }))
                .List<Permission>();

            var adminRole = session.QueryOver(() => r)
                .Where(() => r.Name == Constants.AdminRoleName)
                .SingleOrDefault();

            adminRole.Permissions = permissions;

            session.Save(adminRole);
            session.Flush();
        }

        private void InsertUserRolePermissions(ISession session)
        {
            Permission p = null;
            UserRole r = null;

            var permissions = session.QueryOver(() => p)
                .Where(Restrictions.In(nameof(p.Id), new int[] { 10, 11, 12 }))
                .List<Permission>();

            var userRole = session.QueryOver(() => r)
                .Where(() => r.Name == Constants.UserRoleName)
                .SingleOrDefault();

            userRole.Permissions = permissions;

            session.Save(userRole);
            session.Flush();
        }

        private void InsertAdminUser(ISession session)
        {
            var salt = Crypto.GenerateSalt();
            var plainPassword = Constants.AdminPassword;
            var hashedPassword = Crypto.GenerateSHA256(plainPassword + salt);

            var admin = new User
            {
                Username = Constants.AdminUsername,
                HashedPassword = hashedPassword,
                Salt = salt,
                Status = session.QueryOver<UserStatus>().Where(s => s.Name == Constants.UserStatus_Active).SingleOrDefault(),
                Roles = session.QueryOver<UserRole>().List(),
                Name = "Administrator",
                Surname = "",
                Phone = "",
                EMail = "",
                IsSystemUser = true
            };

            session.Save(admin);
            session.Flush();
        }

        private void InsertAdminPreferences(ISession session)
        {
            var admin = session.QueryOver<User>().Where(u => u.Username == Constants.AdminUsername).SingleOrDefault();
            var preferences = new UserPreference
            {
                AutoRefreshingFrequency = 600,
                TasksPerPage = 50,
                DisplayOnlyTasksWithMyParticipation = true,
                DisplayTasksRefreshingProgressBar = true,
                EnableTasksListAutoRefreshing = true,
                HideCanceledTasks = true,
                User = admin
            };

            session.Save(preferences);
            session.Flush();
        }

        private void InsertTaskStatuses(ISession session)
        {
            var statuses = new List<TaskStatus>()
            {
                new TaskStatus { Id = 1, Name = Constants.TaskStatus_Canceled },
                new TaskStatus { Id = 2, Name = Constants.TaskStatus_Opened },
                new TaskStatus { Id = 3, Name = Constants.TaskStatus_Closed },
            };

            foreach (var s in statuses)
            {
                session.Save(s);
                session.Flush();
            }
        }

        private void InsertTaskPriorities(ISession session)
        {
            var priorities = new List<TaskPriority>()
            {
                new TaskPriority { Id = 1, Name = Constants.TaskPriority_Low },
                new TaskPriority { Id = 2, Name = Constants.TaskPriority_Normal},
                new TaskPriority { Id = 3, Name = Constants.TaskPriority_High},
            };

            foreach (var p in priorities)
            {
                session.Save(p);
                session.Flush();
            }
        }

        private void InsertTaskGenres(ISession session)
        {
            var genres = new List<TaskGenre>()
            {
                new TaskGenre { Id = 1, Name = Constants.TaskGenre_Update },
                new TaskGenre { Id = 2, Name = Constants.TaskGenre_Installation },
                new TaskGenre { Id = 3, Name = Constants.TaskGenre_Toner },
                new TaskGenre { Id = 4, Name = Constants.TaskGenre_Task },
            };

            foreach (var g in genres)
            {
                session.Save(g);
                session.Flush();
            }
        }

        /// <summary>
        /// Migrates users from 'Zlecenia' exported to CSV with MySQL Workbench.
        /// Do not modify exported CVS file!
        /// </summary>
        /// <param name="session">nHibernare session.</param>
        private void MigrateUsers()
        {
            if (File.Exists(Constants.Migration_UsersCSV))
            {
                IAdministrationService administrationService = new AdministrationService();

                foreach (var u in File.ReadAllLines(Constants.Migration_UsersCSV).ToList().Skip(1))
                {
                    var split = u.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    administrationService.AddUser(new UserDTO
                    {
                        Username = split[0],
                        PlainPassword = split[0].Length < Constants.MinPasswordLength ? split[0] + "0000" : split[0],
                        Name = split[2],
                        Surname = split[3],
                        Phone = split[4],
                        EMail = split[5],
                        IsSystemUser = false,
                        Status = new UserStatusDTO { Id = 1 },
                        Roles = new List<UserRoleDTO> { new UserRoleDTO { Id = 2 } }
                    });
                }
            }
        }
    }
}
