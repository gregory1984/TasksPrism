using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Prism.Events.Payloads
{
    public class TaskDatesCollectedPayload
    {
        public DateTime StartDate { get; set; }
        public TimeSpan StartHour { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? EndHour { get; set; }
    }
}
