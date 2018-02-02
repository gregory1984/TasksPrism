using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Tasks_Model.Entities;

namespace Tasks_Model.Mappings
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Id(x => x.Id).Unique().Not.Nullable();
            Map(x => x.Username).Unique().Not.Nullable().Length(150);
            Map(x => x.HashedPassword).Not.Nullable().Length(512);
            Map(x => x.Salt).Not.Nullable().Length(512);
            Map(x => x.Name).Not.Nullable().Length(200);
            Map(x => x.Surname).Not.Nullable().Length(200);
            Map(x => x.Phone).Nullable().Length(50);
            Map(x => x.EMail).Nullable().Length(150);
            Map(x => x.IsSystemUser).Not.Nullable();

            References(x => x.Status);
            HasMany(x => x.Preferences).Inverse().Cascade.All();
            HasMany(x => x.Comments).Inverse().Cascade.SaveUpdate();

            HasManyToMany(x => x.Roles).Cascade.SaveUpdate();
            HasManyToMany(x => x.Tasks).Inverse().Cascade.SaveUpdate();
        }
    }
}
