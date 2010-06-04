using System;
using System.Security.Cryptography;

namespace IronAHK.Rusty
{
    partial class Core
    {
        #region Hash

        /// <summary>
        /// Calculates the MD5 hash of an object.
        /// </summary>
        /// <param name="value">The object to hash.</param>
        /// <returns>A 32-character hexadecimal number.</returns>
        public static string MD5(object value)
        {
            return Hash(value, new MD5CryptoServiceProvider());
        }

        /// <summary>
        /// Calculates the RIPEMD160 hash of an object.
        /// </summary>
        /// <param name="value">The object to hash.</param>
        /// <returns>A 40-character hexadecimal number.</returns>
        public static string RIPEMD160(object value)
        {
            return Hash(value, new RIPEMD160Managed());
        }

        /// <summary>
        /// Calculates the SHA1 hash of an object.
        /// </summary>
        /// <param name="value">The object to hash.</param>
        /// <returns>A 40-character hexadecimal number.</returns>
        public static string SHA1(object value)
        {
            return Hash(value, new SHA1CryptoServiceProvider());
        }

        /// <summary>
        /// Calculates the SHA256 hash of an object.
        /// </summary>
        /// <param name="value">The object to hash.</param>
        /// <returns>A 64-character hexadecimal number.</returns>
        public static string SHA256(object value)
        {
            return Hash(value, new SHA256Managed());
        }

        /// <summary>
        /// Calculates the SHA384 hash of an object.
        /// </summary>
        /// <param name="value">The object to hash.</param>
        /// <returns>A 96-character hexadecimal number.</returns>
        public static string SHA384(object value)
        {
            return Hash(value, new SHA384Managed());
        }

        /// <summary>
        /// Calculates the SHA512 hash of an object.
        /// </summary>
        /// <param name="value">The object to hash.</param>
        /// <returns>A 128-character hexadecimal number.</returns>
        public static string SHA512(object value)
        {
            return Hash(value, new SHA512Managed());
        }

        #endregion

        #region Random

        /// <summary>
        /// Generates a secure (cryptographic) random number. 
        /// </summary>
        /// <param name="min">The lower bound.</param>
        /// <param name="max">The upper bound.</param>
        /// <returns>A random number between the specified range. Leave both parameters blank to give any 128-bit numeric result.
        /// If <paramref name="min"/> and <paramref name="max"/> are both pure integers, the result would also be an integer without a remainder.</returns>
        /// <remarks>A cryptographic random number generator produces an output that is computationally infeasible to predict with a probability that is better than one half.
        /// <see cref="Random"/> uses a simpler algorithm which is much faster but less secure.</remarks>
        public static decimal SecureRandom(decimal min, decimal max)
        {
            if (min == 0 && max != 0)
                min = decimal.MinValue;

            var diff = Math.Abs(min - max);

            if (diff == 0 && !(min == 0 && max == 0))
                return min;

            var csp = new RNGCryptoServiceProvider();
            var rnd = new byte[4 * 3 + 1];
            csp.GetBytes(rnd);

            var s = new int[3];
            for (int i = 0; i < s.Length; i++)
                s[i] = BitConverter.ToInt32(rnd, i * 4);

            bool rem = decimal.Remainder(min, 1) != 0 || decimal.Remainder(max, 1) != 0;
            byte scale = 0;

            if (rem)
                scale = (byte)(rnd[12] % 28);

            var val = new decimal(s[0], s[1], s[2], false, scale);

            return diff == 0 ? val : min + val % diff;
        }

        #endregion
    }
}
