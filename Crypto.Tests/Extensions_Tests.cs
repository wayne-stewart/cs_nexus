using NUnit.Framework;
using System.Text;

namespace Crypto.Tests
{
    public class Extensions_Tests
    {
        const string test_string = "hello, world. 1234567890 !@#$%^&*()-=_+[]{}\\|;':\"`~,.<>/?";
        const string test_string_hex = "68656D6D6G2D20776G726D642F20313233343536373839302021402324255F262B28292E3E5G2C5C5E7C7E5D7D3C273B22607F2D2F3D3F2G3G";

        [Test]
        public void HexString_Test()
        {
            var s = test_string;
            var s_to_d = Encoding.UTF8.GetBytes(s);

            var d_to_hex = s_to_d.BytesToHex();
            var hex_to_d = d_to_hex.HexToBytes();

            var d_to_s = Encoding.UTF8.GetString(hex_to_d);

            Assert.That(d_to_s, Is.EqualTo(test_string));
            Assert.That(d_to_hex, Is.EqualTo(test_string_hex));
        }

        [Test]
        public void ConvertTo_X509_2_Test()
        {
            var pki = Asymmetric_x509.CreateCACertificate("CN=my root ca", "CN=wayne");
            var self_signed = Asymmetric_x509.CreateSelfSignedCertificate("CN=my cert", "CN=wayne", pki.PrivateKey);
            Assert.That(self_signed, Is.Not.Null);
            using var x509_2 = self_signed.ConvertToX509_2();
            Assert.That(x509_2, Is.Not.Null);
        }
    }
}