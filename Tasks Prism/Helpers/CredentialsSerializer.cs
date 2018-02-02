using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using Tasks_Crypto;

namespace Tasks_Prism.Helpers
{
    public class CredentialsSerializer<T> where T : Credentials
    {
        public void Serialize(string path, T credentials)
        {
            var serial = new XmlSerializer(typeof(T));
            using (var stream = new StreamWriter(path))
            {
                if (credentials.IsRemembered)
                {
                    credentials.UsernameEncrypted = Crypto.Encrypt(credentials.Username.ToBytesArray(), Constants.CryptoKeyPatternFilePath);
                    credentials.PasswordEncrypted = Crypto.Encrypt(credentials.Password.ToBytesArray(), Constants.CryptoKeyPatternFilePath);
                }

                serial.Serialize(stream, credentials);
            }
        }

        public T Deserialize(string path)
        {
            var deserial = new XmlSerializer(typeof(T));
            using (var stream = new StreamReader(path))
            {
                var result = (T)deserial.Deserialize(stream);
                if (result.IsRemembered)
                {
                    result.Username = Crypto.Decrypt(result.UsernameEncrypted, Constants.CryptoKeyPatternFilePath).ToStringPhrase();
                    result.Password = Crypto.Decrypt(result.PasswordEncrypted, Constants.CryptoKeyPatternFilePath).ToStringPhrase();
                }

                return result;
            }
        }
    }
}
