using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Util
{
    /// <summary>
    /// Hash函数工具类
    /// </summary>
    public static class HashWorker
    {
        public static string Md5(string str)
        {
            using (var md5 = MD5.Create())
            {
                var result = "";
                var buffer = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                for (int i = 0; i < buffer.Length; i++)
                {
                    result += buffer[i].ToString("X2");
                }
                return result.ToLower();
            }
        }

        public static string Sha512(string str)
        {
            using (var sha512 = SHA512.Create())
            {
                var result = "";
                var buffer = sha512.ComputeHash(Encoding.ASCII.GetBytes(str));
                for (int i = 0; i < buffer.Length; i++)
                {
                    result += buffer[i].ToString("X2");
                }
                return result.ToLower();
            }
        }

        public static string Sha256(byte[] bytes)
        {
            using (var sha256 = SHA256.Create())
            {
                var result = "";
                var buffer = sha256.ComputeHash(bytes);
                for (int i = 0; i < buffer.Length; i++)
                {
                    result += buffer[i].ToString("X2");
                }
                return result.ToLower();
            }
        }

        public static string HashPassword(string password, string salt)
        {
            return Sha512(Md5(password) + salt);
        }
    }
}
