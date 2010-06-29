using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace IronAHK.Rusty
{
    partial class Core
    {
        static string ToString(byte[] array)
        {
            var buf = new StringBuilder(array.Length * 2);

            foreach (var b in array)
                buf.Append(b.ToString("x").PadLeft(2, '0'));

            return buf.ToString();
        }

        static byte[] ToByteArray(object value)
        {
            if (value is string)
                return Encoding.Unicode.GetBytes((string)value);
            
            if (value is byte[])
                return (byte[])value;

            if (value == null)
                return new byte[] { };

            var formatter = new BinaryFormatter();
            var writer = new MemoryStream();
            formatter.Serialize(writer, value);
            return writer.ToArray();
        }

        static byte[] Encrypt(object value, object key, bool decrypt, SymmetricAlgorithm alg)
        {
            int size = 0;

            foreach (var legal in alg.LegalKeySizes)
                size = Math.Max(size, legal.MaxSize);

            var k = new byte[size / 8];

            var keyBytes = ToByteArray(key);

            if (keyBytes.Length < k.Length)
            {
                var padded = new byte[k.Length];
                keyBytes.CopyTo(padded, 0);
                keyBytes = padded;
            }

            for (int i = 0; i < k.Length; i++)
                k[i] = keyBytes[i];

            try
            {
                alg.Key = k;
            }
            catch (CryptographicException)
            {
                ErrorLevel = 2;
                return new byte[] { };
            }

            var iv = new byte[alg.IV.Length];
            var hash = new SHA1Managed().ComputeHash(keyBytes, 0, iv.Length);

            for (int i = 0; i < Math.Min(iv.Length, hash.Length); i++)
                iv[i] = hash[i];

            alg.IV = iv;

            var trans = decrypt ? alg.CreateDecryptor() : alg.CreateEncryptor();
            var buffer = ToByteArray(value);
            var result = trans.TransformFinalBlock(buffer, 0, buffer.Length);
            return result;
        }

        static string Hash(object value, HashAlgorithm alg)
        {
            var raw = ToByteArray(value);
            var result = alg.ComputeHash(raw);
            return ToString(result);
        }
    }
}
