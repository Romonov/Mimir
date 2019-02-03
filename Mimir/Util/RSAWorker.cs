using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.X509;
using RUL;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml;

namespace Mimir.Util
{
    class RSAWorker
    {
        private static Logger log = new Logger("RSAWorker");

        public static RSACryptoServiceProvider PublicKey = new RSACryptoServiceProvider();
        public static RSACryptoServiceProvider PrivateKey = new RSACryptoServiceProvider();

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

        public static string Encrypt(string str)
        {
            byte[] publicValue = PublicKey.Encrypt(Encoding.Default.GetBytes(str), false);
            string publicStr = Convert.ToBase64String(publicValue);
            return publicStr;
        }

        public string Decrypt(string str)
        {
            byte[] privateValue = PrivateKey.Decrypt(Convert.FromBase64String(str), false);
            string privateStr = Encoding.Default.GetString(privateValue);
            return privateStr;
        }

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