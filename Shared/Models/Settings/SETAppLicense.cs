using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Settings
{
    public  class SETAppLicense
    {
        public int UserID { get; set; }
        public string LicenseStatus { get; set; }
        public string ExpiredStatus { get; set; }
        public string PerpetualStatus { get; set; }
        public string LicenseDate { get; set; }
        public string LicenseDuration { get; set; }
        public string License { get; set; }
        public string ActivationCode { get; set; }
        public int Id { get; set; }
    }

    public class SETAppLicenseValidator : AbstractValidator<SETAppLicense>
    {
        public SETAppLicenseValidator()
        {
            RuleFor(s => s.UserID).NotEmpty().WithMessage("Please Enter the User ID");
            RuleFor(c => c.License).NotEmpty().WithMessage("Please Enter the License Key");
            RuleFor(c => c.ActivationCode).NotEmpty().WithMessage("Please Enter Paste the Activation Code");
        }
    }
}
