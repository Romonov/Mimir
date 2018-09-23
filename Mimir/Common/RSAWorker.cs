using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.X509;
using RUL;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

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

        /// <summary>  
        /// RSA 公钥转为Java格式
        /// </summary>  
        /// <param name="XML">XML文件名</param>  
        /// <returns></returns>  
        public static string RSAPublicKeyConverter(string XML)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(XML);
            BigInteger m = new BigInteger(1, Convert.FromBase64String(xmlDocument.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
            BigInteger p = new BigInteger(1, Convert.FromBase64String(xmlDocument.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
            RsaKeyParameters pub = new RsaKeyParameters(false, m, p);

            SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pub);
            byte[] serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();
            return Convert.ToBase64String(serializedPublicBytes);
        }
    }
}
