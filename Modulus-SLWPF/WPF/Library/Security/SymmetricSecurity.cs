using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Security
{
    internal class SymmetricSecurity<T> where T : SymmetricAlgorithm, new()
    {
        private readonly T _cryptoProvider;
        protected byte[] _iv;
        protected byte[] _key;
        private readonly UTF8Encoding _utf8;

        public SymmetricSecurity(byte[] key, byte[] iv)
        {
            _cryptoProvider = new T();
            _utf8 = new UTF8Encoding();
            _key = key;
            _iv = iv;
        }

        protected SymmetricSecurity()
        {
            _cryptoProvider = new T();
            _utf8 = new UTF8Encoding();

        }

        public byte[] Encrypt(byte[] input)
        {
            return Transform(input, _cryptoProvider.CreateEncryptor(_key, _iv));
        }

        public byte[] Decrypt(byte[] input)
        {
            return Transform(input, _cryptoProvider.CreateDecryptor(_key, _iv));
        }

        private readonly Random _r = new Random();
        public byte[] GenerateIV()
        {
            int len = 0;
            if (_cryptoProvider.GetType() == typeof(AesManaged))
                len = 16;

            byte[] res = new byte[len];
            _r.NextBytes(res);

            return res;
        }

        public byte[] GenerateKey()
        {
            KeySizes[] validKeySizes = _cryptoProvider.LegalKeySizes;
            int keySize = 0;
            if (_cryptoProvider.GetType() == typeof(AesManaged))
                keySize = validKeySizes[validKeySizes.Length - 1].MaxSize / 8;

            byte[] res = new byte[keySize];
            _r.NextBytes(res);

            return res;
        }

        public string Encrypt(string text)
        {
            byte[] input = _utf8.GetBytes(text);
            return Convert.ToBase64String(Transform(input, _cryptoProvider.CreateEncryptor(_key, _iv)));
        }

        public string Decrypt(string text)
        {
            byte[] input = Convert.FromBase64String(text);
            byte[] output = Transform(input, _cryptoProvider.CreateDecryptor(_key, _iv));
            return _utf8.GetString(output, 0, output.Length);
        }

        private static byte[] Transform(byte[] input, ICryptoTransform cryptoTransform)
        {
            MemoryStream memStream = new MemoryStream();
            CryptoStream cryptStream = new CryptoStream(memStream, cryptoTransform, CryptoStreamMode.Write);
            cryptStream.Write(input, 0, input.Length);
            cryptStream.FlushFinalBlock();
            memStream.Position = 0L;
            byte[] result = new byte[((int)(memStream.Length - 1L)) + 1];
            memStream.Read(result, 0, result.Length);
            memStream.Close();
            cryptStream.Close();
            return result;
        }
    }

    internal class AesSecurity : SymmetricSecurity<AesManaged>
    {
        public AesSecurity()
        {
            _key = new byte[]
               {
                 0x45, 0x9A, 0xCD, 0x2C, 0xDF, 0x48, 0xFE, 0x59,
                 0x17, 0xF3, 0xB4, 0x17, 0xC7, 0x34, 0xA4, 0xE9,
                 0x00, 0xB3, 0x3F, 0x64, 0xEB, 0x0C, 0x22, 0x21,
                 0x67, 0x9F, 0x78, 0x28, 0xE6, 0x25, 0xC2, 0xBE,
               };
            _iv = new byte[]
              {
                0xF5, 0xB7, 0x0A, 0xFA, 0xBB, 0x67, 0x18, 0x50,
                0x82, 0xB1, 0x65, 0xCE, 0xA0, 0x37, 0xA8, 0x88,
              };
        }
    }
}
