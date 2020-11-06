using System;

namespace Crypto
{
    public interface IEncryptor
    {
        public byte[] Encrypt(ReadOnlySpan<byte> data);
    }
}
