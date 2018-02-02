using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Tasks_Model.Entities;

namespace Tasks_Model.Mappings
{
    public class UserRoleMap : ClassMap<UserRole>
    {
        public UserRoleMap()
        {
            Id(x => x.Id).Not.Nullable().Unique();
            Map(x => x.Name).Not.Nullable().Unique().Length(100);
            Map(x => x.IsSystemRole).Not.Nullable();

            HasManyToMany(x => x.Users).Inverse().Cascade.SaveUpdate();
            HasManyToMany(x => x.Permissions).Cascade.SaveUpdate();
        }
    }
}
