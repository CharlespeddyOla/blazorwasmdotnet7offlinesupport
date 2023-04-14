using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppAcademics.Shared.Models.Settings
{
    public class SETReligion 
    {
        public int ReligionID { get; set; }
        public string Religion { get; set; }
        public int Id { get; set; }
    }

    public class ReligionValidator : AbstractValidator<SETReligion>
    {
        public ReligionValidator()
        {
            RuleFor(r => r.Religion).NotEmpty().WithMessage("Please Enter the religion");
        }
    }
}
