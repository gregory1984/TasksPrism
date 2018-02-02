using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Model.Entities
{
    public class Task
    {
        public virtual int Id { get; protected set; }
        public virtual string Topic { get; set; }
        public virtual string Content { get; set; }
        public virtual DateTime StartPeriod { get; set; }
        public virtual DateTime? EndPeriod { get; set; }
        public virtual string Author { get; set; }
        public virtual TaskGenre Genre { get; set; }
        public virtual TaskStatus Status { get; set; }
        public virtual TaskPriority Priority { get; set; }
        public virtual IList<User> Users { get; set; }
        public virtual IList<TaskComment> Comments { get; set; }
    }
}
