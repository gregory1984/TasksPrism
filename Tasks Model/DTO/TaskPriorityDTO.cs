using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Model.DTO
{
    public class TaskPriorityDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; }

        public TaskPriorityDTO() { }

        public TaskPriorityDTO(int? id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
