using AppSoftware.LicenceEngine.Common;
using AppSoftware.LicenceEngine.KeyVerification;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace WebAppAcademics.Server.Helpers
{
    public class ProductKeyVerification
    {
        public static bool LicenseVerifiction(string licenceKey)
        {
            bool result = false;

            var keyByteSets = new[]
                {
                    new KeyByteSet(keyByteNumber: 1, keyByteA: 58, keyByteB: 6, keyByteC: 97),
                    new KeyByteSet(keyByteNumber: 5, keyByteA: 62, keyByteB: 4, keyByteC: 234),
                    new KeyByteSet(keyByteNumber: 8, keyByteA: 6, keyByteB: 88, keyByteC: 32)
                };

            var pkvKeyVerifier = new PkvKeyVerifier();
            var pkvKeyVerificationResult = pkvKeyVerifier.VerifyKey(

                   key: licenceKey?.Trim(),
                   keyByteSetsToVerify: keyByteSets,

                   // The TOTAL number of KeyByteSets used to generate the licence key in SampleKeyGenerator

                   totalKeyByteSets: 8,

                   // Add blacklisted seeds here if required (these could be user IDs for example)

                   blackListedSeeds: null
               );

            Console.WriteLine($"Verification result: {pkvKeyVerificationResult}");

            Console.WriteLine("\nPress any key to verify another licence key.");

            if (pkvKeyVerificationResult.ToString() == "KeyIsValid")
            {
                result = true;
            }
            
            return result; 
        }
    }
}
