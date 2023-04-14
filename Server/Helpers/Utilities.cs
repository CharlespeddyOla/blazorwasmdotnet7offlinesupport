using System.Security.Cryptography;
using System.Text;

namespace WebAppAcademics.Server.Helpers
{
    public class Utilities
    {
        public byte[] GetImage(string sBase64String)
        {
            byte[] bytes = null;
            if (!string.IsNullOrEmpty(sBase64String))
            {
                bytes = Convert.FromBase64String(sBase64String);
            }

            return bytes;
        }

        public string NewTerm(int termcount)
        {
            string result = "First";

            if (termcount == 1)
            {
                result = "Second";
            }
            else if (termcount == 2)
            {
                result = "Third";
            }

            return result;
        }

        public static string Encrypt(string password)
        {
            var provider = MD5.Create();
            //string salt = "S0m321R@nd0m&$?@Salt";
            string salt = "S0m3R@nd0mSalt";
            byte[] bytes = provider.ComputeHash(Encoding.UTF32.GetBytes(salt + password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
