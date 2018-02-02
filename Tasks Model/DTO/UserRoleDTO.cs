using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks_Model.Entities;

namespace Tasks_Model.DTO
{
    public class UserRoleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NewName { get; set; } //  Only for new role creation.
        public bool IsSystemRole { get; set; }
        public IList<PermissionDTO> Permissions { get; set; }

        public UserRoleDTO() { }
        public UserRoleDTO(int id, string name, bool isSystemRole)
        {
            Id = id;
            Name = name;
            IsSystemRole = isSystemRole;
        }

        public override string ToString()
        {
            return Name + (IsSystemRole ? " [rola systemowa]" : "");
        }
    }
}
