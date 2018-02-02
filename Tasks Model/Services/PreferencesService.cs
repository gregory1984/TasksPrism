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

namespace Tasks_Model.Services
{
    public class PreferencesService : IPreferencesService
    {
        public UserPreferencesDTO Preferences { get; private set; }

        public void LoadPreferences(int userId)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                User u = null;
                UserPreference up = null;

                Preferences = session.QueryOver(() => up)
                    .JoinAlias(() => up.User, () => u)
                    .Where(() => u.Id == userId)
                    .SelectList(l => l
                        .Select(() => u.Id)
                        .Select(() => up.AutoRefreshingFrequency)
                        .Select(() => up.TasksPerPage)
                        .Select(() => up.DisplayOnlyTasksWithMyParticipation)
                        .Select(() => up.EnableTasksListAutoRefreshing)
                        .Select(() => up.DisplayTasksRefreshingProgressBar)
                        .Select(() => up.HideCanceledTasks))
                    .TransformUsing(Transformers.AliasToBeanConstructor(typeof(UserPreferencesDTO).GetConstructors()[1]))
                    .SingleOrDefault<UserPreferencesDTO>();
            }
        }

        public void SavePreferences(UserPreferencesDTO dto)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                var user = session.QueryOver<User>().Where(u => u.Id == dto.UserId).SingleOrDefault();
                var preferences = session.QueryOver<UserPreference>().Where(p => p.User == user).SingleOrDefault();

                preferences.AutoRefreshingFrequency = dto.AutoRefreshingFrequency;
                preferences.DisplayOnlyTasksWithMyParticipation = dto.DisplayOnlyTasksWithMyParticipation;
                preferences.DisplayTasksRefreshingProgressBar = dto.DisplayTasksRefreshingProgressBar;
                preferences.EnableTasksListAutoRefreshing = dto.EnableTasksListAutoRefreshing;
                preferences.HideCanceledTasks = dto.HideCanceledTasks;
                preferences.TasksPerPage = dto.TasksPerPage;

                Preferences = new UserPreferencesDTO(preferences);

                session.SaveOrUpdate(preferences);
                session.Flush();
            }
        }

        public void Reset(int userId)
        {
            using (var session = Hibernate.SessionFactory.OpenSession())
            {
                var user = session.QueryOver<User>().Where(u => u.Id == userId).SingleOrDefault();
                var preferences = session.QueryOver<UserPreference>().Where(p => p.User == user).SingleOrDefault();

                preferences.AutoRefreshingFrequency = 600;
                preferences.DisplayOnlyTasksWithMyParticipation = true;
                preferences.DisplayTasksRefreshingProgressBar = true;
                preferences.EnableTasksListAutoRefreshing = true;
                preferences.HideCanceledTasks = true;
                preferences.TasksPerPage = 50;

                Preferences = new UserPreferencesDTO(preferences);

                session.SaveOrUpdate(preferences);
                session.Flush();
            }
        }
    }
}
