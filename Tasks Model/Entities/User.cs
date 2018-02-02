using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Model.Entities
{
    public class User
    {
        public virtual int Id { get; protected set; }
        public virtual string Username { get; set; }
        public virtual string HashedPassword { get; set; }
        public virtual string Salt { get; set; }
        public virtual string Name { get; set; }
        public virtual string Surname { get; set; }
        public virtual string Phone { get; set; }
        public virtual string EMail { get; set; }
        public virtual bool IsSystemUser { get; set; }
        public virtual UserStatus Status { get; set; }
        public virtual IList<UserPreference> Preferences { get; set; }
        public virtual IList<UserRole> Roles { get; set; }
        public virtual IList<Task> Tasks { get; set; }
        public virtual IList<TaskComment> Comments { get; set; }
    }
}
