using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using WebAppAcademics.Server.Helpers;
using WebAppAcademics.Shared.Helpers;

namespace WebAppAcademics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncryptDecryptController : ControllerBase
    {
        
        [HttpGet]
        [Route("Encrypt/{plaintext}")]
        public string Encrypt(string plaintext)
        {
            try
            {
                var encryptString = EncryptDecryptManager.EncryptString(plaintext);
                return encryptString;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }

            return default;
        }

        [HttpPost]
        [Route("Decrypt")]
        public FileChunkDTO Decrypt(FileChunkDTO inputcypertext)
        {
            try
            {
                string cipherText = inputcypertext.FileName;
                var decryptString = EncryptDecryptManager.DecryptString(cipherText);
                inputcypertext.FileName = decryptString;    
                return inputcypertext;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }

            return default;
        }

        [HttpPost]
        [Route("LicenseKeyVerification")]
        public FileChunkDTO LicenseKeyVerification(FileChunkDTO model)
        {
            try
            {
                model.LicenseVerified = ProductKeyVerification.LicenseVerifiction(model.LicenseKey);
                return model;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }

            return default;
        }


    }
}
