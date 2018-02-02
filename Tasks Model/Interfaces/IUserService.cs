using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks_Model.DTO;

namespace Tasks_Model.Interfaces
{
    public enum PasswordChangingStatus
    {
        CurrentPasswordIncorrect,
        MinLengthViolation,
        ElementEmpty,
        NewPasswordsNotSame,
        Changed
    }

    public interface IUserService
    {
        UserDTO AuthenticatedUser { get; }
        IList<PermissionDTO> AuthenticatedUserPermissions { get; }

        void Authenticate(string username, string password);
        PasswordChangingStatus ChangePassword(string currentPassword, string newPassword, string retypedPassword);
        void ResetPermissions();
    }
}
