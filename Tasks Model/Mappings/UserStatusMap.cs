using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Tasks_Model.Entities;

namespace Tasks_Model.Mappings
{
    public class UserStatusMap : ClassMap<UserStatus>
    {
        public UserStatusMap()
        {
            Id(x => x.Id).Not.Nullable().Unique();
            Map(x => x.Name).Not.Nullable().Unique().Length(100);

            HasMany(x => x.Users).Inverse().Cascade.SaveUpdate();
        }
    }
}
