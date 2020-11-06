using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Crypto
{
    /// <summary>
    /// Version1 uses AES GCM
    /// </summary>
    public class Symmetric_AesGcm : IEncryptor, IDecryptor
    {
        const int KEY_SIZE = 32; // 256 bits
        const int KEY_SALT_SIZE = KEY_SIZE;
        const int NONCE_SIZE = 12; // 96 bits
        const int KEY_DERIVATION_ITERATIONS = 2037;
        const int TAG_SIZE = 16; // 128 bits
        const byte VERSION = 0x01;
        const int VERSION_OFFSET = 0;
        const int TAG_OFFSET = 1;
        const int NONCE_OFFSET = 1 + TAG_SIZE;
        const int KEY_SALT_OFFSET = NONCE_OFFSET + NONCE_SIZE;
        const int DATA_OFFSET = KEY_SALT_OFFSET + KEY_SALT_SIZE;

        readonly string _key;
        readonly byte[] _associated_data;

        public Symmetric_AesGcm(string key, string associated_data = null)
        {
            _key = key;
            _associated_data = associated_data == null ? null : Encoding.UTF8.GetBytes(associated_data);
        }

        public static byte[] Encrypt(ReadOnlySpan<byte> data, string key, byte[] associated_data = null)
        {
            var key_salt = new byte[KEY_SALT_SIZE];
            RandomNumberGenerator.Fill(key_salt);
            var derived_key = DeriveKey(key, key_salt);
            var tag = new byte[TAG_SIZE];
            var nonce = new byte[NONCE_SIZE];
            var cipher_data = new byte[data.Length];

            using var aes = new AesGcm(derived_key);
            aes.Encrypt(nonce, data, cipher_data, tag, associated_data);

            return Concat(new[] { VERSION }, tag, nonce, key_salt, cipher_data);
        }

        public static byte[] Decrypt(ReadOnlySpan<byte> cipher_data, string key, byte[] associated_data = null) 
        {
            var version = cipher_data[VERSION_OFFSET];
            if (version != VERSION) throw new Exception($"Version incorrect for AesGcm version {VERSION}");
            var tag = cipher_data.Slice(TAG_OFFSET, TAG_SIZE);
            var nonce = cipher_data.Slice(NONCE_OFFSET, NONCE_SIZE);
            var key_salt = cipher_data.Slice(KEY_SALT_OFFSET, KEY_SIZE);
            var encrypted_data = cipher_data[DATA_OFFSET..];
            var derived_key = DeriveKey(key, key_salt.ToArray());
            var data = new byte[encrypted_data.Length];

            using var aes = new AesGcm(derived_key);
            aes.Decrypt(nonce, encrypted_data, tag, data, associated_data);

            return data;
        }

        public static byte[] DeriveKey(string key, byte[] salt)
        {
            var key_data = Encoding.UTF8.GetBytes(key);
            using var key_derivation = new Rfc2898DeriveBytes(key_data, salt, KEY_DERIVATION_ITERATIONS, HashAlgorithmName.SHA512);
            return key_derivation.GetBytes(KEY_SIZE);
        }

        public static byte[] Concat(params byte[][] arrays)
        {
            var length = arrays.Sum(f => f.Length);
            var result = new byte[length];
            var result_offset = 0;
            for(var i = 0; i < arrays.Length; i++)
            {
                Array.Copy(arrays[i], 0, result, result_offset, arrays[i].Length);
                result_offset += arrays[i].Length;
            }
            return result;
        }

        public byte[] Encrypt(ReadOnlySpan<byte> data) => Encrypt(data, _key, _associated_data);

        public byte[] Decrypt(ReadOnlySpan<byte> data) => Decrypt(data, _key, _associated_data);
    }
}
