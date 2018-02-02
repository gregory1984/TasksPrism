using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Model.Entities
{
    public class DatabaseInitialization
    {
        public virtual int Id { get; protected set; }
        public virtual DateTime InitDate { get; set; }
    }
}
