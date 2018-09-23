using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using RUL;
using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Mimir.Common
{
    class CertWorker
    {
        /// <summary>
        /// 加载SSL证书
        /// </summary>
        public static void Load()
        {
            if (!Directory.Exists($@"{Program.Path}\Cert"))
            {
                Directory.CreateDirectory($@"{Program.Path}\Cert");
            }

            if (Program.IsCustomCert)
            {
                if (!File.Exists($@"{Program.Path}\Cert\{Program.SslCertName}"))
                {
                    Logger.Error("Cert file is missing, disabling ssl mode!");
                    Program.IsSslEnabled = false;
                }
            }
            else
            {
                if (!File.Exists($@"{Program.Path}\Cert\{Program.SslCertName}"))
                {
                    Logger.Info("Please keyin a password of self-signed certificate: ");
                    string Password = Console.ReadLine();
                    Logger.WriteToFile(Password);
                    Gen(Password);
                }
            }

            Program.ServerCertificate = new X509Certificate2($@"{Program.Path}\Cert\{Program.SslCertName}", Program.SslCertPassword);
        }

        public static void Gen(string Password)
        {
            Environment.Exit(3);
        }
    }
}
