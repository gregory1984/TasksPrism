using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Tasks_Model.Entities;

namespace Tasks_Model.Mappings
{
    public class PermissionMap : ClassMap<Permission>
    {
        public PermissionMap()
        {
            Id(x => x.Id).Unique().Not.Nullable();
            Map(x => x.Name).Unique().Not.Nullable().Length(100);

            HasManyToMany(x => x.Roles).Inverse().Cascade.SaveUpdate();
        }
    }
}
