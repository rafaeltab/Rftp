using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace RftpLib
{
    public static class Encrypting
    {


        public static readonly int SaltSize = 32;
        public const int IterationIndex = 0;

        public static string Encrypt(string password)
        {
            var cp = new RNGCryptoServiceProvider();
            byte[] salt = new byte[SaltSize];
            cp.GetBytes(salt);

            var hash = GetPbkdf2Bytes(password, salt, 1000, 20);

            string full = "";

            string saltS = Convert.ToBase64String(salt);
            string hashS = Convert.ToBase64String(hash);

            Random r = new Random(2361723);
            int one = r.Next(0, saltS.Length/3);
            int Two = r.Next(one + 1, (int) (saltS.Length / 1.5));
            int Three = r.Next(Two+1, saltS.Length);

            full = saltS + "8Q93ctObfFclaAOV2fLoV7stlD21QJZxmPBxzaIh4QIZus3OagC7r8dUwvCG" + hashS;

            return full;
        }

        public static bool ValidatePassword(string password, string correctHash)
        {
            string[] devider = { "8Q93ctObfFclaAOV2fLoV7stlD21QJZxmPBxzaIh4QIZus3OagC7r8dUwvCG" };
            var split = correctHash.Split(devider,StringSplitOptions.None);

            var salt = Convert.FromBase64String(split[0]);
            var hash = Convert.FromBase64String(split[1]);

            var testHash = GetPbkdf2Bytes(password, salt, 1000, hash.Length);
            return SlowEquals(hash, testHash);
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            var diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }

        private static byte[] GetPbkdf2Bytes(string password, byte[] salt, int iterations, int outputBytes)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt);
            pbkdf2.IterationCount = iterations;
            return pbkdf2.GetBytes(outputBytes);
        }
    }
}
