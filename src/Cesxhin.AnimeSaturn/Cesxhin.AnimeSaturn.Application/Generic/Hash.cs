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
                try
                {
                    File.OpenRead(path).Close();

                    var content = File.ReadAllBytes(path);

                    return BytesToStr(hash.ComputeHash(content));
                }
                catch(IOException)
                {
                    return null;
                }
            }
        }

        private static string BytesToStr(byte[] bytes)
        {
            StringBuilder str = new();

            for (int i = 0; i < bytes.Length; i++)
                str.AppendFormat("{0:X2}", bytes[i]);

            return str.ToString();
        }
    }
}
