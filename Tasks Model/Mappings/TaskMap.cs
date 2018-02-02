using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Tasks_Model.Entities;

namespace Tasks_Model.Mappings
{
    public class TaskMap : ClassMap<Entities.Task>
    {
        public TaskMap()
        {
            Id(x => x.Id).Unique().Not.Nullable();
            Map(x => x.Topic).Not.Nullable().Length(100);
            Map(x => x.Content).Not.Nullable().Length(4000);
            Map(x => x.StartPeriod).Not.Nullable();
            Map(x => x.EndPeriod).Nullable();
            Map(x => x.Author).Not.Nullable().Length(200);

            References(x => x.Genre);
            References(x => x.Status);
            References(x => x.Priority);

            HasMany(x => x.Comments).Inverse().Cascade.All();
            HasManyToMany(x => x.Users).Cascade.SaveUpdate();
        }
    }
}
