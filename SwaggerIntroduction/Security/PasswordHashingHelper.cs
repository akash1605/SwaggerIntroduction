using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace SwaggerIntroduction.Security
{
    public class PasswordHashingHelper
    {
        private readonly RNGCryptoServiceProvider _rngCryptoServiceProvider;
        private static string _passwordAdditive;
        public PasswordHashingHelper(string passwordAdditive)
        {
            _rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            _passwordAdditive = passwordAdditive;
        }

        public (string, string) GetHashedPassword(string password)
        {
            var saltBytes = GetSalt();
            var salt = GetSaltStringFrombytes(saltBytes);
            var hashedPassword = HashValues(password, saltBytes);

            return (salt, hashedPassword);
        }

        public byte[] GetSalt()
        {
            using (_rngCryptoServiceProvider)
            {
                var data = new byte[5];
                _rngCryptoServiceProvider.GetNonZeroBytes(data);
                return data;
            }
        }

        public byte[] GetSaltFromString(string salt)
        {
            var byteSalt = salt.Split(',').Select(byte.Parse);
            return byteSalt.ToArray();
        }

        public string HashValues(string password, byte[] salt)
        {
            return Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                password + _passwordAdditive,
                salt,
                KeyDerivationPrf.HMACSHA256,
                10000,
                256 / 8));
        }

        private static string GetSaltStringFrombytes(IEnumerable<byte> saltBytes)
        {
            string salt = null;
            foreach (var saltByte in saltBytes)
            {
                salt = string.IsNullOrEmpty(salt) ? saltByte.ToString() : $"{salt},{saltByte}";
            }

            return salt;
        }

    }
}
