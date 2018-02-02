using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Model.Searching
{
    public class TaskSearchCriteria
    {
        public bool ShowTasksOnly { get; set; }
        public bool ShowTonersOnly { get; set; }
        public bool ShowUpdatesOnly { get; set; }
        public bool ShowInstallationsOnly { get; set; }
        public bool ShowWithoutCanceledTasks { get; set; }

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
        public int? TaskParticipantId { get; set; }
        public int? CommentAuthorId { get; set; }

        public void Reset()
        {
            TaskAuthorUsername = Topic = Content = Comment = "";
            ShowInstallationsOnly = ShowTasksOnly = ShowTonersOnly = ShowUpdatesOnly = ShowWithoutCanceledTasks = false;
            TaskId = TaskStatusId = TaskPriorityId = TaskParticipantId = CommentAuthorId = null;

            StartDate = EndDate = null;
            StartHour = EndHour = null;
        }
    }
}
