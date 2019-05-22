using Mimir.Models;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Mimir.Util
{
    /// <summary>
    /// RSA秘钥和签名工具类
    /// </summary>
    public class SignatureWorker
    {
        /// <summary>
        /// 生成并保存RSA秘钥对
        /// </summary>
        public static void GenKey(MimirContext db)
        {
            RSACryptoServiceProvider key = new RSACryptoServiceProvider(4096);

            var publicKey = RSACryptoServiceProviderExtensions.ToXmlString(key);
            var privateKey = RSACryptoServiceProviderExtensions.ToXmlString(key, true);
            foreach (var option in db.Options)
            {
                if (option.Option == "PublicKeyXml")
                {
                    option.Value = publicKey;
                }

                if (option.Option == "PrivateKeyXml")
                {
                    option.Value = privateKey;
                }

                if (option.Option == "PublicKey")
                {
                    option.Value = PublicKeyToJavaFormat(privateKey);
                }
            }
            db.SaveChanges();
        }

        /// <summary>
        /// 把XML格式的秘钥转为Java格式
        /// </summary>
        /// <param name="XML">XML秘钥</param>
        /// <returns>Java秘钥</returns>
        private static string PublicKeyToJavaFormat(string XML)
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

        /// <summary>
        /// 使用RSA私钥给数据签名
        /// </summary>
        /// <param name="str">要签名的数据</param>
        /// <returns>签名结果</returns>
        public static string Sign(string str)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(str);
            byte[] signedData = Program.PrivateKeyProvider.SignData(byteData, new SHA1CryptoServiceProvider());
            return Convert.ToBase64String(signedData);
        }
    }
}
