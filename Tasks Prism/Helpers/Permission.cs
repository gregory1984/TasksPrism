using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks_Model.DTO;

namespace Tasks_Prism.Helpers
{
    /// <summary>
    /// Browsing is a primary permission. Only if browsing is granted we can check other permissions.
    /// </summary>
    public enum Permission
    {
        UsersBrowsing = 1,
        UsersAdding = 2,
        UsersModifying = 3,
        UsersRemoving = 4,
        UsersPasswordReseting = 5,

        RolesBrowsing = 6,
        RolesAdding = 7,
        RolesModifying = 8,
        RolesRemoving = 9,

        TasksBrowsing = 10,
        TasksAdding = 11,
        TasksModifying = 12,
        TasksRemoving = 13
    }

    public static class PermissionHelper
    {
        public static IList<Permission> ConvertFromDTO(this IList<PermissionDTO> dto)
        {
            var enums = new List<Permission>();
            foreach (var d in dto)
            {
                enums.Add((Permission)d.Id);
            }

            return enums;
        }

        public static bool ContainsAnyOf(this IList<Permission> authenticatedUserPermissions, IList<Permission> requiredPermissions)
        {
            foreach (var r in requiredPermissions)
            {
                if (authenticatedUserPermissions.Contains(r))
                    return true;
            }
            return false;
        }
    }
}
