using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Model.DTO
{
    public class PermissionDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public PermissionDTO() { }
        public PermissionDTO(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
