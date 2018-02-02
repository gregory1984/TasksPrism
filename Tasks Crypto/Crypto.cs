using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Tasks_Crypto
{
    public static class Crypto
    {
        public static string GenerateSHA256(string phrase)
        {
            using (var hash = SHA256.Create())
            {
                var builder = new StringBuilder();
                var bytes = hash.ComputeHash(Encoding.ASCII.GetBytes(phrase));
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string GenerateSalt()
        {
            using (var random = new RNGCryptoServiceProvider())
            {
                var size = 32;
                var bytes = new byte[size];
                random.GetNonZeroBytes(bytes);
                return Convert.ToBase64String(bytes);
            }
        }

        /// <param name="keyPatternFilePath">Encryption key will be computed based on SHA256 of this file.</param>
        public static byte[] Encrypt(byte[] plain, string keyPatternFilePath, bool encryption = true)
        {
            using (var aes = new AesCryptoServiceProvider())
            {
                var password = SHA256.Create().ComputeHash(File.ReadAllBytes(keyPatternFilePath)).ToString();
                using (var pass = new PasswordDeriveBytes(password, null))
                {
                    aes.Key = pass.GetBytes(aes.KeySize / 8);
                    aes.IV = pass.GetBytes(aes.BlockSize / 8);

                    using (var stream = new MemoryStream())
                    {
                        var process = encryption ? aes.CreateEncryptor() : aes.CreateDecryptor();
                        using (var crypto = new CryptoStream(stream, process, CryptoStreamMode.Write))
                        {
                            crypto.Write(plain, 0, plain.Length);
                            crypto.Clear();
                        }
                        return stream.ToArray();
                    }
                }
            }
        }

        /// <param name="keyPatternFilePath">Decryption key will be computed based on SHA256 of this file.</param>
        public static byte[] Decrypt(byte[] encrypted, string keyPatternFilePath)
        {
            return Encrypt(encrypted, keyPatternFilePath, false);
        }
    }
}
