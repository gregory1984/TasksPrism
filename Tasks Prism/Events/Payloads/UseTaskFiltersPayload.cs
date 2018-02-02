using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks_Model.Searching;

namespace Tasks_Prism.Events.Payloads
{
    public class UseTaskFiltersPayload
    {
        public int? TaskId { get; set; }

        public string Topic { get; set; } = "";
        public string Content { get; set; } = "";
        public string Comment { get; set; } = "";

        public DateTime? StartDate { get; set; }
        public TimeSpan? StartHour { get; set; }

        public DateTime? EndDate { get; set; }
        public TimeSpan? EndHour { get; set; }

        public int? TaskStatusId { get; set; }
        public int? TaskPriorityId { get; set; }

        public string TaskAuthorUsername { get; set; }
        public int? TaskParticipatorId { get; set; }
        public int? CommentAuthorId { get; set; }
    }
}
