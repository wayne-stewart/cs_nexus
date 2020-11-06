using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto.Tests
{
    public class Asymetric_x509_Tests
    {
        [Test]
        public void CreateCACertificate_Test()
        {
            var cert = Asymmetric_x509.CreateCACertificate("CN=my root ca", "CN=wayne");
            Assert.That(cert, Is.Not.Null);
        }

        [Test]
        public void CreateSelfSignedCertificate_Test()
        {
            var pki = Asymmetric_x509.CreateCACertificate("CN=my root ca", "CN=wayne");
            var self_signed = Asymmetric_x509.CreateSelfSignedCertificate("CN=my cert", "CN=wayne", pki.PrivateKey);
            Assert.That(self_signed, Is.Not.Null);
        }
    }
}
