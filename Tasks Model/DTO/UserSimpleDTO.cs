using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks_Model.DTO
{
    public class UserSimpleDTO
    {
        public int? Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public UserSimpleDTO() { }

        public UserSimpleDTO(int id, string username, string name, string surname)
        {
            Id = id;
            Username = username;
            Name = name;
            Surname = surname;
        }

        public override string ToString()
        {
            return Name + " " + Surname;
        }
    }
}
