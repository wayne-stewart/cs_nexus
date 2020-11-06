using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Crypto
{
    public static class Extensions
    {
        public static string HexStringEncrypt(this IEncryptor crypto, ReadOnlySpan<byte> data)
        {
            if (crypto == null) throw new ArgumentNullException(nameof(crypto));
            if (data == null) throw new ArgumentNullException(nameof(data));

            var encrypted = crypto.Encrypt(data);
            return encrypted.BytesToHex();
        }

        public static byte[] HexStringDecrypt(this IDecryptor crypto, string cipher_text)
        {
            if (crypto == null) throw new ArgumentNullException(nameof(crypto));
            if (cipher_text == null) throw new ArgumentNullException(nameof(cipher_text));

            var data = cipher_text.HexToBytes();
            return crypto.Decrypt(data);
        }

        public static byte[] HexToBytes(this string s)
        {
            if (s == null) return null;

            if (s.Length % 2 != 0) throw new Exception($"Invalid Hex String Length: {s.Length}");
            var data = new byte[s.Length / 2];
            for (var i = 0; i < s.Length; i += 2)
            {
                var upper = (byte)s[i];
                var lower = (byte)s[i + 1];
                if (upper >= 0x41) upper -= (0x41 - 0x09);
                else upper -= 0x30;
                if (lower >= 0x41) lower -= (0x41 - 0x09);
                else lower -= 0x30;
                byte b = (byte)((upper << 4) | lower);
                data[i / 2] = b;
            }
            return data;
        }

        public static string BytesToHex(this byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var sb = new StringBuilder(data.Length * 2);
            for (var i = 0; i < data.Length; i++)
            {
                byte b = data[i];
                byte upper = (byte)(b >> 4);
                byte lower = (byte)(b & 0x0F);
                if (upper <= 9) sb.Append((char)(upper + 0x30));
                else sb.Append((char)(upper + 0x41 - 0x09));
                if (lower <= 9) sb.Append((char)(lower + 0x30));
                else sb.Append((char)(lower + 0x41 - 0x09));
            }
            return sb.ToString();
        }

        public static AsymmetricKeyParameter GetPrivateKey(this X509Certificate2 cert)
        {
            return DotNetUtilities.GetRsaKeyPair(cert.GetRSAPrivateKey()).Private;
        }

        /// <summary>
        /// Export a certificate in cer format. This format does not contain the private key.
        /// </summary>
        /// <param name="cert"></param>
        /// <returns>cer encoded certificate</returns>
        public static byte[] Export_CER(this X509Certificate2 cert)
        {
            if (cert == null) throw new ArgumentNullException(nameof(cert));

            return cert.Export(X509ContentType.Cert);
        }

        /// <summary>
        /// Export a certificate in pfx format. This format does include the private key
        /// </summary>
        /// <param name="cert"></param>
        /// <returns>pkcs#12 encoded certificate</returns>
        public static byte[] Export_PFX(this X509Certificate2 cert)
        {
            if (cert == null) throw new ArgumentNullException(nameof(cert));

            return cert.Export(X509ContentType.Pfx);
        }

        public static X509Certificate2 ConvertToX509_2(this PKI pki)
        {
            if (pki == null) throw new ArgumentNullException(nameof(pki));

            var private_key_info = Org.BouncyCastle.Pkcs.PrivateKeyInfoFactory.CreatePrivateKeyInfo(pki.PrivateKey);

            var asn1_object = (Asn1Sequence)private_key_info.ParsePrivateKey();

            if (asn1_object.Count != 9) throw new Org.BouncyCastle.OpenSsl.PemException($"malformed sequence in RSA private key - seq: {asn1_object.Count}");

            var rsa = Org.BouncyCastle.Asn1.Pkcs.RsaPrivateKeyStructure.GetInstance(asn1_object);
            var rsaparams = new Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters(rsa.Modulus, rsa.PublicExponent, rsa.PrivateExponent, rsa.Prime1, rsa.Prime2, rsa.Exponent1, rsa.Exponent2, rsa.Coefficient);

            using var x509_2 = new X509Certificate2(pki.Certificate.GetEncoded());
            var x509_2_copy = x509_2.CopyWithPrivateKey(DotNetUtilities.ToRSA(rsaparams));
            return x509_2_copy;
        }

        public static AsymmetricAlgorithm ToAsymmetricAlgorithm(this Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters private_key)
        {
            if (private_key == null) throw new ArgumentNullException(nameof(private_key));

            var csp_params = new CspParameters
            {
                KeyContainerName = Guid.NewGuid().ToString(),
                KeyNumber = (int)KeyNumber.Exchange,
                Flags = CspProviderFlags.UseMachineKeyStore
            };

            var rsa_provider = new RSACryptoServiceProvider(csp_params);
            var rsa_params = new RSAParameters 
            {
                Modulus = private_key.Modulus.ToByteArrayUnsigned(),
                P = private_key.P.ToByteArrayUnsigned(),
                Q = private_key.Q.ToByteArrayUnsigned(),
                DP = private_key.DP.ToByteArrayUnsigned(),
                DQ = private_key.DQ.ToByteArrayUnsigned(),
                InverseQ = private_key.QInv.ToByteArrayUnsigned(),
                D = private_key.Exponent.ToByteArrayUnsigned(),
                Exponent = private_key.PublicExponent.ToByteArrayUnsigned()
            };

            rsa_provider.ImportParameters(rsa_params);

            return rsa_provider;
        }

        public static void StoreForMe(this X509Certificate2 cert, string password = null)
        {
            if (cert == null) throw new ArgumentNullException(nameof(cert));
            using var cert_to_store = new X509Certificate2(cert.RawData, password, X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            cert_to_store.AddCertToStore(StoreName.My, StoreLocation.CurrentUser);
        }

        public static void StoreForEveryone(this X509Certificate2 cert, string password = null)
        {
            if (cert == null) throw new ArgumentNullException(nameof(cert));
            using var cert_to_store = new X509Certificate2(cert.RawData, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            cert_to_store.AddCertToStore(StoreName.My, StoreLocation.LocalMachine);
        }

        public static void AddCertToStore(this X509Certificate2 cert, StoreName store_name, StoreLocation store_location)
        {
            var store = new X509Store(store_name, store_location);
            store.Open(OpenFlags.ReadWrite);
            store.Add(cert);
            store.Close();
        }
    }
}
