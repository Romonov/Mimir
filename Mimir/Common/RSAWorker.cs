using RUL;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Mimir.Common
{
    class RSAWorker
    {
        private static RSACryptoServiceProvider Key = new RSACryptoServiceProvider();

        public static void GenKey()
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

        public static bool LoadKey()
        {
            try
            {
                Logger.Info("Loading secret keys.");
                Key.FromXmlString("PublicKey.xml");
            }
            catch(Exception e)
            {
                Logger.Error(e.Message);
                return false;
            }
            return true;
        }
    }
}
