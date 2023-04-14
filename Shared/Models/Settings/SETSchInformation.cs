using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppAcademics.Shared.Models.Settings
{
    public class SETSchInformation 
    {
        public int SchInfoID { get; set; }
        public string SchName { get; set; }
        public string SchCode { get; set; }
        public string SchType { get; set; }
        public string SchSlogan { get; set; }
        public string SchAddress { get; set; }
        public string SchAddressLine2 { get; set; }
        public string SchPhones { get; set; }
        public string SchPhones2 { get; set; }
        public string SchEmails { get; set; }
        public string SchWebsites { get; set; }
        public byte[] SchSplashScreen { get; set; }
        public byte[] SchLogo { get; set; }
        public bool DefaultSch { get; set; }
        public byte[] SchEmailPW { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string EmailSettings { get; set; }
        public int EmailPortSMTP { get; set; }
        public int EmailPortSSLTLS { get; set; }
        public int CountryID { get; set; }
        public string Country { get; set; }
        public int StateID { get; set; }
        public string State { get; set; }
        public int LGAID { get; set; }
        public string LGA { get; set; }
        public int StatusTypeID { get; set; }
        public string StatusType { get; set; }
        public string ImageUrl { get; set; }
        public string EmailPassword { get; set; }
        public int Id { get; set; }
    }

    public class SchoolInfoValidator : AbstractValidator<SETSchInformation>
    {
        public SchoolInfoValidator()
        {
            RuleFor(s => s.SchCode).NotEmpty().WithMessage("School Code is required");
            RuleFor(s => s.SchName).NotEmpty().WithMessage("Name of School is required");
            RuleFor(s => s.SchSlogan).NotEmpty().WithMessage("School Slogan is required");
            //RuleFor(s => s.SchType).NotEmpty().WithMessage("School Description is required");
            RuleFor(s => s.SchAddress).NotEmpty().WithMessage("School Address is required");
            RuleFor(s => s.SchPhones).NotEmpty().WithMessage("School Phone Number is required");
            RuleFor(s => s.SchPhones).MinimumLength(11).WithMessage("Phone Number Must Be 11 Digits");
            RuleFor(s => s.SchPhones).MaximumLength(11).WithMessage("Phone Number Must Be 11 Digits");
            RuleFor(s => s.SchEmails).NotEmpty().EmailAddress().WithMessage("Please specify a valid email");
            RuleFor(s => s.EmailSettings).NotEmpty().WithMessage("Please specify email server settings");
            RuleFor(s => s.EmailPortSMTP).NotEmpty().WithMessage("Please specify email SMTP Port");
            RuleFor(s => s.EmailPortSSLTLS).NotEmpty().WithMessage("Please specify email SSL/TLS Port");
            RuleFor(u => u.Password).NotEmpty().WithMessage("Your password cannot be empty")
                    .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                    .MaximumLength(16).WithMessage("Your password length must not exceed 16.");
            RuleFor(u => u.ConfirmPassword)
               .Equal(u => u.Password)
               .WithMessage("Passwords do not match");

            RuleFor(s => s.Country).NotEmpty().WithMessage("Please select Country");
            RuleFor(s => s.State).NotEmpty().WithMessage("Please select State");
            RuleFor(s => s.LGA).NotEmpty().WithMessage("Please select Local Government");
        }
    }
}
