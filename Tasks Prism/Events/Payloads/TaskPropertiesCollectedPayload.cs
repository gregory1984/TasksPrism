using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Prism.Events.Payloads
{
    public class TaskPropertiesCollectedPayload
    {
        public int? TaskStatusId { get; set; }
        public int? TaskGenreId { get; set; }
        public int? TaskPriorityId { get; set; }
    }
}
