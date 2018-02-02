using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Model.Entities
{
    public class UserPreference
    {
        public virtual int Id { get; protected set; }
        public virtual int AutoRefreshingFrequency { get; set; }
        public virtual int TasksPerPage { get; set; }
        public virtual bool DisplayOnlyTasksWithMyParticipation { get; set; }
        public virtual bool EnableTasksListAutoRefreshing { get; set; }
        public virtual bool DisplayTasksRefreshingProgressBar { get; set; }
        public virtual bool HideCanceledTasks { get; set; }
        public virtual User User { get; set; }
    }
}
