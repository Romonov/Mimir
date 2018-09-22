using RUL;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Mimir.Common
{
    class RSAWorker
    {
        public static RSACryptoServiceProvider PublicKey = new RSACryptoServiceProvider();
        public static RSACryptoServiceProvider PrivateKey = new RSACryptoServiceProvider();

        /// <summary>
        /// 生成秘钥
        /// </summary>
        public static bool GenKey()
        {
            try
            {
                Logger.Info("Generating RSA secret keys, it will take at least 5 seconds...");

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(4096);

                StreamWriter privateKey = new StreamWriter("PrivateKey.xml");
                privateKey.WriteLine(rsa.ToXmlString(true));
                privateKey.FlushAsync();

                StreamWriter publicKey = new StreamWriter("PublicKey.xml");
                publicKey.WriteLine(rsa.ToXmlString(false));
                publicKey.FlushAsync();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 加载秘钥
        /// </summary>
        /// <returns>成功与否</returns>
        public static bool LoadKey()
        {
            try
            {
                Logger.Info("Loading secret keys.");
                PublicKey.FromXmlString(File.ReadAllText("PublicKey.xml"));
                PrivateKey.FromXmlString(File.ReadAllText("PrivateKey.xml"));
                Logger.Info("Secret keys load successfully.");
            }
            catch(Exception e)
            {
                Logger.Error(e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// RSA 公钥加密
        /// </summary>
        /// <param name="str">明文</param>
        /// <returns>密文</returns>
        public static string Encrypt(string str)
        {
            byte[] publicValue = PublicKey.Encrypt(Encoding.Default.GetBytes(str), false);
            string publicStr = Convert.ToBase64String(publicValue);
            return publicStr;
        }

        /// <summary>
        /// RSA 私钥解密
        /// </summary>
        /// <param name="str">密文</param>
        /// <returns>明文</returns>
        public string Decrypt(string str)
        {
            byte[] privateValue = PrivateKey.Decrypt(Convert.FromBase64String(str), false);
            string privateStr = Encoding.Default.GetString(privateValue);
            return privateStr;
        }
    }
}
