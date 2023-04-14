using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Administration.School
{
    public class ADMSchEducationInstitute
    {
        public int EDUID { get; set; }
        public string EDUInstitute { get; set; }
        public string Address { get; set; }
        public int Id { get; set; }
    }

    public class SchEducationInstituteValidator : AbstractValidator<ADMSchEducationInstitute>
    {
        public SchEducationInstituteValidator()
        {
            RuleFor(c => c.EDUInstitute).NotEmpty().WithMessage("School Name is required");
            RuleFor(c => c.Address).NotEmpty().WithMessage("School Address is required");
        }
    }
}
