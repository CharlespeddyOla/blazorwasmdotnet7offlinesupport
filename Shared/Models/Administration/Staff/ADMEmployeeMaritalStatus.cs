using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Administration.Staff
{
    public class ADMEmployeeMaritalStatus
    {
        public int MStatusID { get; set; }
        public string MStatus { get; set; }
        public int Id { get; set; }
    }

    public class MaritalStatusValidator : AbstractValidator<ADMEmployeeMaritalStatus>
    {
        public MaritalStatusValidator()
        {
            RuleFor(mstatus => mstatus.MStatus).NotEmpty().WithMessage("MaritAal Status is required");
        }
    }

}
