using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Cesxhin.AnimeSaturn.Application.Generic
{
    public static class Hash
    {
        public static string GetHash(string path)
        {
            using (SHA256 hash = SHA256.Create())
            {
                var content = File.ReadAllBytes(path);

                return BytesToStr(hash.ComputeHash(content));
            }
        }

        private static string BytesToStr(byte[] bytes)
        {
            StringBuilder str = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
                str.AppendFormat("{0:X2}", bytes[i]);

            return str.ToString();
        }
    }
}
