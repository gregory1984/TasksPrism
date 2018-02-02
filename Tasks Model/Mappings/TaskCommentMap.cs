using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Tasks_Model.Entities;

namespace Tasks_Model.Mappings
{
    public class TaskCommentMap : ClassMap<TaskComment>
    {
        public TaskCommentMap()
        {
            Id(x => x.Id).Unique().Not.Nullable();
            Map(x => x.Content).Not.Nullable().Length(4000);
            Map(x => x.Date).Not.Nullable();

            References(x => x.User);
            References(x => x.Task);
        }
    }
}
