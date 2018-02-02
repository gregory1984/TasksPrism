using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Model.DTO
{
    public class TaskStatusDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; }

        public TaskStatusDTO() { }

        public TaskStatusDTO(int? id, string name)
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
