using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppAcademics.Shared.Models.Settings
{
    public class SETCountries 
    {
        public int CountryID { get; set; }
        public string CountryCode { get; set; }
        public string Country { get; set; }
        public int Id { get; set; }

    }

    public class CountryValidator : AbstractValidator<SETCountries>
    {
        public CountryValidator()
        {
            RuleFor(c => c.CountryCode).NotEmpty().WithMessage("Please Enter the Country Code");
            RuleFor(c => c.Country).NotEmpty().WithMessage("Please Enter the Country Name");
        }
    }
}

