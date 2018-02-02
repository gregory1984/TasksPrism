using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks_Model.Entities;

namespace Tasks_Model.DTO
{
    public class UserStatusDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public UserStatusDTO() { }
        public UserStatusDTO(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
