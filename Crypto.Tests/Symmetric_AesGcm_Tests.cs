using NUnit.Framework;
using System.Security.Cryptography;
using System.Text;

namespace Crypto.Tests
{
    public class Symmetric_AesGcm_Tests
    {
        const string test_string = "hello, world. 1234567890 !@#$%^&*()-=_+[]{}\\|;':\"`~,.<>/?";
        const string password = "my pass@123";
        const string password_alt = "old pill box";
        readonly byte[] plain_text_data = Encoding.UTF8.GetBytes(test_string);
        readonly byte[] authenticated_data = Encoding.UTF8.GetBytes("only auth here");
        readonly byte[] authenticated_data_alt = Encoding.UTF8.GetBytes("or auth here");

        [Test]
        public void Encrypt_Decrypt_Test()
        {
            var encrypted = Symmetric_AesGcm.Encrypt(plain_text_data, password);
            var decrypted = Symmetric_AesGcm.Decrypt(encrypted, password);
            var recovered_text_data = Encoding.UTF8.GetString(decrypted);
            Assert.That(test_string, Is.EqualTo(recovered_text_data));
        }

        [Test]
        public void Decrypt_Bad_Password_Test()
        {
            var encrypted = Symmetric_AesGcm.Encrypt(plain_text_data, password);

            // test to make sure the incorrect password cannot decrypt the data
            var ex = Assert.Throws<CryptographicException>(() => { Symmetric_AesGcm.Decrypt(encrypted, password_alt); });
            Assert.That(ex.Message, Is.EqualTo("The computed authentication tag did not match the input authentication tag."));
        }

        [Test]
        public void Encrypt_Decrypt_AutheticatedData_Test()
        {
            var encrypted = Symmetric_AesGcm.Encrypt(plain_text_data, password, authenticated_data);
            var decrypted = Symmetric_AesGcm.Decrypt(encrypted, password, authenticated_data);
            var recovered_text_data = Encoding.UTF8.GetString(decrypted);
            Assert.That(test_string, Is.EqualTo(recovered_text_data));

            encrypted = Symmetric_AesGcm.Encrypt(plain_text_data, password, authenticated_data);

            // test to make sure data encrypted with authenticated data doesn't decrypt when null authenticated data is passed in
            var ex = Assert.Throws<CryptographicException>(() => { Symmetric_AesGcm.Decrypt(encrypted, password, null); });
            Assert.That(ex.Message, Is.EqualTo("The computed authentication tag did not match the input authentication tag."));

            // test to make sure data encrypted with authenticated data doesn't decrypt when wrong authenticated data is passed in
            ex = Assert.Throws<CryptographicException>(() => { Symmetric_AesGcm.Decrypt(encrypted, password, authenticated_data_alt); });
            Assert.That(ex.Message, Is.EqualTo("The computed authentication tag did not match the input authentication tag."));
        }

        [Test]
        public void HexString_Encrypt_Decrypt_Test()
        {
            var crypto = new Symmetric_AesGcm(password);
            var encrypted = crypto.HexStringEncrypt(plain_text_data);
            var decrypted = crypto.HexStringDecrypt(encrypted);
            var recovered_text_data = Encoding.UTF8.GetString(decrypted);
            Assert.That(test_string, Is.EqualTo(recovered_text_data));
        }
    }
}