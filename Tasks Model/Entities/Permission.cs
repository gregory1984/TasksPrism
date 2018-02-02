using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Model.Entities
{
    public class Permission
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual IList<UserRole> Roles { get; set; }
    }
}
