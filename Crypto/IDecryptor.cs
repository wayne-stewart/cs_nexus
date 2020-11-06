using System;

namespace Crypto
{
    public interface IDecryptor
    {
        public byte[] Decrypt(ReadOnlySpan<byte> data);
    }
}
