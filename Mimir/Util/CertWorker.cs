using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using RUL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Util
{
    class CertWorker
    {
        private static Logger log = new Logger("CertWorker");

        public static void Gen()
        {
            log.Info("Generating ssl certificates...");

            // Generate RSA key pair
            var rsaGenerator = new RsaKeyPairGenerator();
            var randomGenerator = new CryptoApiRandomGenerator();
            var secureRandom = new SecureRandom(randomGenerator);
            var keyParameters = new KeyGenerationParameters(secureRandom, 1024);
            rsaGenerator.Init(keyParameters);
            var keyPair = rsaGenerator.GenerateKeyPair();

            // Generate certificate
            var attributes = new Hashtable
            {
                [X509Name.CN] = Program.ServerName,
                [X509Name.O] = "Romonov"
            };

            var ordering = new ArrayList
            {
                X509Name.CN,
                X509Name.O
            };

            var certificateGenerator = new X509V3CertificateGenerator();
            certificateGenerator.SetSerialNumber(BigInteger.ProbablePrime(120, new Random()));
            certificateGenerator.SetIssuerDN(new X509Name(ordering, attributes));
            certificateGenerator.SetNotBefore(DateTime.Today.Subtract(new TimeSpan(1, 0, 0, 0)));
            certificateGenerator.SetNotAfter(DateTime.Today.AddDays(365));
            certificateGenerator.SetSubjectDN(new X509Name(ordering, attributes));
            certificateGenerator.SetPublicKey(keyPair.Public);
            certificateGenerator.SetSignatureAlgorithm("SHA1WithRSA");
            certificateGenerator.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(false));
            certificateGenerator.AddExtension(X509Extensions.AuthorityKeyIdentifier, true, new AuthorityKeyIdentifier(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public)));
            certificateGenerator.AddExtension(X509Extensions.ExtendedKeyUsage.Id, false, new ExtendedKeyUsage(new ArrayList() { new DerObjectIdentifier("1.3.6.1.5.5.7.3.2") }));

            var x509Certificate = certificateGenerator.Generate(keyPair.Private);
            byte[] pkcs12Bytes = DotNetUtilities.ToX509Certificate(x509Certificate).Export(X509ContentType.Pkcs12, Program.SslCertPassword);

            using (FileStream fs = new FileStream(Program.SslCertName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.Write(pkcs12Bytes, 0, pkcs12Bytes.Length);
                fs.Flush();
            }
            
        }
    }
}
