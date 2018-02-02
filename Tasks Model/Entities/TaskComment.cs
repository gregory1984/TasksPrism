using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tasks_Model.Entities
{
    public class TaskComment
    {
        public virtual int Id { get; protected set; }
        public virtual string Content { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual Task Task { get; set; }
        public virtual User User { get; set; }
    }
}
