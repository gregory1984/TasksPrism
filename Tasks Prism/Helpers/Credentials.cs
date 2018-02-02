using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Tasks_Prism.Helpers
{
    public class Credentials
    {
        public byte[] UsernameEncrypted { get; set; }
        public byte[] PasswordEncrypted { get; set; }

        [XmlIgnore]
        public string Username { get; set; }

        [XmlIgnore]
        public string Password { get; set; }
        public bool IsRemembered { get; set; }

        public Credentials() { }
        public Credentials(string username, string password, bool isRemembered)
        {
            Username = username;
            Password = password;
            IsRemembered = isRemembered;
        }

        public Credentials(bool isRemembered)
        {
            IsRemembered = isRemembered;
        }
    }
}
