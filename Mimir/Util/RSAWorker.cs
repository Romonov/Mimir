using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.X509;
using RUL;
using RUL.Encode;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml;

namespace Mimir.Util
{
    /// <summary>
    /// RSA秘钥管理类
    /// </summary>
    class RSAWorker
    {
        private static Logger log = new Logger("RSAWorker");

        public static RSACryptoServiceProvider PublicKey = new RSACryptoServiceProvider();
        public static RSACryptoServiceProvider PrivateKey = new RSACryptoServiceProvider();

        /// <summary>
        /// 生成RSA秘钥对
        /// </summary>
        public static void GenKey()
        {
            try
            {
                log.Info("Generating RSA secret keys, it will take at least 5 seconds...");

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(4096);

                using (StreamWriter privateKey = new StreamWriter("PrivateKey.xml"))
                {
                    privateKey.WriteLine(rsa.ToXmlString(true));
                    privateKey.Flush();
                }
                using (StreamWriter publicKey = new StreamWriter("PublicKey.xml"))
                {
                    publicKey.WriteLine(rsa.ToXmlString(false));
                    publicKey.Flush();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// 加载RSA秘钥对
        /// </summary>
        public static void LoadKey()
        {
            try
            {
                log.Info("Loading secret keys.");
                PublicKey.FromXmlString(File.ReadAllText("PublicKey.xml"));
                PrivateKey.FromXmlString(File.ReadAllText("PrivateKey.xml"));
                log.Info("Secret keys load successfully.");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// 使用RSA私钥给数据签名
        /// </summary>
        /// <param name="data">要签名的数据</param>
        /// <returns>签名结果</returns>
        public static string Sign(string data)
        {
            byte[] byteData = Encoding.Default.GetBytes(data);
            byte[] signedData = PrivateKey.SignData(byteData, new SHA1CryptoServiceProvider());
            return Convert.ToBase64String(signedData);
        }

        /// <summary>
        /// 把XML格式的秘钥转为Java格式
        /// </summary>
        /// <param name="XML"></param>
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