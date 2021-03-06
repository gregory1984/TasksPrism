﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Tasks_Model.Entities;

namespace Tasks_Model.Mappings
{
    public class DatabaseInitializationMap : ClassMap<DatabaseInitialization>
    {
        public DatabaseInitializationMap()
        {
            Id(x => x.Id).Not.Nullable().Unique();
            Map(x => x.InitDate).Not.Nullable();
        }
    }
}
