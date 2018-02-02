using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Tasks_Model.Entities;

namespace Tasks_Model.Mappings
{
    public class UserPreferenceMap : ClassMap<UserPreference>
    {
        public UserPreferenceMap()
        {
            Id(x => x.Id).Unique().Not.Nullable();
            Map(x => x.AutoRefreshingFrequency).Not.Nullable();
            Map(x => x.TasksPerPage).Not.Nullable();
            Map(x => x.DisplayOnlyTasksWithMyParticipation).Not.Nullable();
            Map(x => x.DisplayTasksRefreshingProgressBar).Not.Nullable();
            Map(x => x.EnableTasksListAutoRefreshing).Not.Nullable();
            Map(x => x.HideCanceledTasks).Not.Nullable();

            References(x => x.User);
        }
    }
}
