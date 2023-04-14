using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Helpers
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

        public static string Encrypt(string rawData)
        {
            // Create a SHA256   
            // ComputeHash - returns byte array  
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));

            // Convert byte array to a string   
            StringBuilder builder = new();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
