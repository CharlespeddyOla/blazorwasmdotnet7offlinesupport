using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppAcademics.Shared.Models.Settings
{
    public class SETStates 
    {
        public int StateID { get; set; }
        public string StateCode { get; set; }
        public string State { get; set; }
        public int CountryID { get; set; }
        public string Country { get; set; }
        public int Id { get; set; }
    }

    public class StateValidator : AbstractValidator<SETStates>
    {
        public StateValidator()
        {
            RuleFor(s => s.StateCode).NotEmpty().WithMessage("Please Enter the State Code");
            RuleFor(c => c.State).NotEmpty().WithMessage("Please Enter the Sate Name");
        }
    }
}
