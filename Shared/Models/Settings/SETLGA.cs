using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppAcademics.Shared.Models.Settings
{
    public class SETLGA 
    {
        public int LGAID { get; set; }
        public string LGACode { get; set; }
        public string LGA { get; set; }
        public int StateID { get; set; }
        public string State { get; set; }
        public int Id { get; set; }
    }

    public class LGAValidator : AbstractValidator<SETLGA>
    {
        public LGAValidator()
        {
            RuleFor(l => l.LGACode).NotEmpty().WithMessage("Please Enter the LGA Code");
            RuleFor(l => l.LGA).NotEmpty().WithMessage("Please Enter the LGA Name");
        }
    }
}
