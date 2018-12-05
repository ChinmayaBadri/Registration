using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Registration.Utilities
{
    public class EncryptDecrypt
    {
        /// <summary>
        /// Gets encrypted content
        /// </summary>
        /// <param name="source"> source to be encrypted </param>
        /// <param name="EncryptDecryptPassword"> service password </param>
        /// <returns> return encrypted content </returns>
        public string Encrypt(string source, string EncryptDecryptPassword)
        {
            byte[] initializer = Encoding.ASCII.GetBytes("@1B2c3D4e5F6g7H8");
            string strencryKey = "!QAZ2wsx#EDC";
            byte[] stringiation = Encoding.ASCII.GetBytes(strencryKey);
            int size = 192;
            byte[] bytStr = Encoding.UTF8.GetBytes(source);

            PasswordDeriveBytes pwdItem = new PasswordDeriveBytes(EncryptDecryptPassword, stringiation, "SHA1", 5);
            byte[] bytKeys = pwdItem.GetBytes(size / 8);
            RijndaelManaged rmEncryption = new RijndaelManaged();
            rmEncryption.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = rmEncryption.CreateEncryptor(bytKeys, initializer);
            MemoryStream stream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(bytStr, 0, bytStr.Length);
            cryptoStream.FlushFinalBlock();
            byte[] bytSource = stream.ToArray();
            stream.Close();
            cryptoStream.Close();
            string ciphered = Convert.ToBase64String(bytSource);
            return ciphered;
        }

        /// <summary>
        /// Gets decrypted content
        /// </summary>
        /// <param name="source"></param>
        /// <param name="EncryptDecryptPassword"> service password </param>
        /// <returns> return encrypted content </returns>
        public string Decrypt(string ciphered, string EncryptDecryptPassword)
        {
            byte[] initializer = Encoding.ASCII.GetBytes("@1B2c3D4e5F6g7H8");
            string strencryKey = "!QAZ2wsx#EDC";
            byte[] stringiation = Encoding.ASCII.GetBytes(strencryKey);
            byte[] bytStr = null;
            int size = 192;
            try
            {
                bytStr = Convert.FromBase64String(ciphered.Replace(" ", "+"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            PasswordDeriveBytes pwdItem = new PasswordDeriveBytes(EncryptDecryptPassword, stringiation, "SHA1", 5);
            byte[] bytKeys = pwdItem.GetBytes(size / 8);
            RijndaelManaged rmEncryption = new RijndaelManaged();
            rmEncryption.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = rmEncryption.CreateDecryptor(bytKeys, initializer);
            MemoryStream stream = new MemoryStream(bytStr);
            CryptoStream cryptoStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[bytStr.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            stream.Close();
            cryptoStream.Close();

            string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            return plainText;
        }
    }
}
