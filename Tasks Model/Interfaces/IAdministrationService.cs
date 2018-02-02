using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks_Model.DTO;

namespace Tasks_Model.Interfaces
{
    public enum UserAccountStatus
    {
        UsernameExists,
        PasswordMinLengthViolation,
        ElementEmpty,
        NoRolesSelected,
        HasTasks,
        Added,
        Modified,
        Deleted,
    }

    public enum UserRoleStatus
    {
        RolenameExists,
        NoPermissionsSelected,
        NoRolesBrowsingSelected,
        NoUsersBrowsingSelected,
        NoTasksBrowsingSelected,
        ElementEmpty,
        HasUsers,
        Added,
        Modified,
        Deleted
    }

    public interface IAdministrationService
    {
        IList<UserRoleDTO> GetUserRoles();
        IList<UserStatusDTO> GetUserStatuses();
        IList<UserDTO> GetUsers();
        IList<UserSimpleDTO> GetUsersSimpleData(bool all = true);
        IList<PermissionDTO> GetPermissions();

        void ResetPassword(int userId);
        UserAccountStatus AddUser(UserDTO uset);
        UserAccountStatus ModifyUser(UserDTO user);
        UserAccountStatus DeleteUser(int userId);

        UserRoleStatus AddUserRole(UserRoleDTO role);
        UserRoleStatus ModifyUserRole(UserRoleDTO role);
        UserRoleStatus DeleteUserRole(int roleId);
    }
}
