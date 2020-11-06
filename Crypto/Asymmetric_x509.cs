using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using X509Certificate2 = System.Security.Cryptography.X509Certificates.X509Certificate2;
using StoreName = System.Security.Cryptography.X509Certificates.StoreName;
using StoreLocation = System.Security.Cryptography.X509Certificates.StoreLocation;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Parameters;

namespace Crypto
{
    public class PKI
    {
        public X509Certificate Certificate { get; set; }
        public AsymmetricKeyParameter PublicKey { get; set; }
        public AsymmetricKeyParameter PrivateKey { get; set; }
    }

    public sealed class Asymmetric_x509
    {
        const int KEY_STRENGTH = 2048;
        const int DURATION_IN_DAYS = 365;
        const int SERIAL_NUMBER_BIT_LENGTH = 120;

        public static PKI CreateCACertificate(string subject, string issuer)
        {
            var random_generator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(random_generator);

            var keypair_generator = new RsaKeyPairGenerator();
            keypair_generator.Init(new KeyGenerationParameters(new SecureRandom(), KEY_STRENGTH));
            var keypair = keypair_generator.GenerateKeyPair();

            var cert_generator = new X509V3CertificateGenerator();
            var serial_number = BigIntegers.CreateRandomBigInteger(SERIAL_NUMBER_BIT_LENGTH, random);
            cert_generator.SetSerialNumber(serial_number);
            var subjectdn = new X509Name(subject);
            cert_generator.SetSubjectDN(subjectdn); // common name
            cert_generator.SetIssuerDN(new X509Name(issuer)); // issuer's name
            cert_generator.SetNotBefore(DateTime.Now); // valid today
            cert_generator.SetNotAfter(DateTime.Now.AddDays(DURATION_IN_DAYS)); // expires in 1 year
            //cert_generator.SetSignatureAlgorithm // should not be needed if cert is generated with ISignatureFactory
            cert_generator.SetPublicKey(keypair.Public);

            var signature_factory = new Asn1SignatureFactory(PkcsObjectIdentifiers.Sha256WithRsaEncryption.Id, keypair.Private);

            //var cert_request = new Pkcs10CertificationRequest(signature_factory, subjectdn, keypair.Public, null);
            var cert = cert_generator.Generate(signature_factory);

            return new PKI
            {
                Certificate = cert,
                PublicKey = keypair.Public,
                PrivateKey = keypair.Private
            };
        }

        public static PKI CreateSelfSignedCertificate(string subject, string issuer, AsymmetricKeyParameter issuer_private_key)
        {
            var random_generator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(random_generator);
            var keypair_generator = new RsaKeyPairGenerator();
            keypair_generator.Init(new KeyGenerationParameters(random, KEY_STRENGTH));
            var keypair = keypair_generator.GenerateKeyPair();

            var cert_generator = new X509V3CertificateGenerator();

            var serial_number = BigIntegers.CreateRandomBigInteger(SERIAL_NUMBER_BIT_LENGTH, random);
            var subjectdn = new X509Name(subject);
            var issuerdn = new X509Name(issuer);

            cert_generator.SetSerialNumber(serial_number);
            cert_generator.SetSubjectDN(subjectdn);
            cert_generator.SetIssuerDN(issuerdn);
            cert_generator.SetNotBefore(DateTime.Now); // valid today
            cert_generator.SetNotAfter(DateTime.Now.AddDays(DURATION_IN_DAYS)); // expires in 1 year
            cert_generator.SetPublicKey(keypair.Public);

            // using the issuer_private_key for self-sign
            var signature_factory = new Asn1SignatureFactory(PkcsObjectIdentifiers.Sha256WithRsaEncryption.Id, issuer_private_key);
            var cert = cert_generator.Generate(signature_factory);

            return new PKI 
            {
                Certificate = cert,
                PublicKey = keypair.Public,
                PrivateKey = keypair.Private
            };
        }

        public static string WritePem(AsymmetricKeyParameter key)
        {
            var sb = new StringBuilder();
            var pem_writer = new PemWriter(new StringWriter(sb));
            pem_writer.WriteObject(key);
            pem_writer.Writer.Flush();
            return sb.ToString();
        }
    }
}
