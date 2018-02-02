using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Tasks_Model.Entities;

namespace Tasks_Model.Mappings
{
    public class TaskPriorityMap : ClassMap<TaskPriority>
    {
        public TaskPriorityMap()
        {
            Id(x => x.Id).Unique().Not.Nullable();
            Map(x => x.Name).Unique().Not.Nullable().Length(200);

            HasMany(x => x.Tasks).Inverse().Cascade.SaveUpdate();
        }
    }
}
