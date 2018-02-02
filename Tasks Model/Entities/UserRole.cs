using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Model.Entities
{
    public class UserRole
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual bool IsSystemRole { get; set; }
        public virtual IList<User> Users { get; set; }
        public virtual IList<Permission> Permissions { get; set; }
    }
}
