using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks_Model.Helpers;
using Tasks_Model.Entities;

namespace Tasks_Model.DTO
{
    public enum AuthenticationStatus
    {
        LoggedIn,
        BadCredentials,
        Blocked
    }

    public class UserDTO
    {
        public int? Id { get; set; }
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public string PlainPassword { get; set; } // Only for user creation;
        public string Salt { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string EMail { get; set; }
        public bool IsSystemUser { get; set; }
        public UserStatusDTO Status { get; set; }
        public IList<UserRoleDTO> Roles { get; set; }

        public AuthenticationStatus AuthenticationStatus
        {
            get
            {
                if (Id.HasValue)
                {
                    return Status.Name == Constants.UserStatus_Active ? AuthenticationStatus.LoggedIn : AuthenticationStatus.Blocked;
                }
                return AuthenticationStatus.BadCredentials;
            }
        }

        public UserDTO() { }
        public UserDTO(int? id, string username, string hashedPassword, string salt, string name, string surname, string phone, string email, int statusId, string statusName, bool isSystemUser)
        {
            Id = id;
            Username = username;
            HashedPassword = hashedPassword;
            Salt = salt;
            Name = name;
            Surname = surname;
            Phone = phone;
            EMail = email;
            IsSystemUser = isSystemUser;

            Status = new UserStatusDTO
            {
                Id = statusId,
                Name = statusName
            };
        }

        public override string ToString()
        {
            return Username;
        }
    }
}
