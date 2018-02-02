using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Model.DTO
{
    public class TaskGenreDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; }

        public TaskGenreDTO() { }

        public TaskGenreDTO(int? id, string name)
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
