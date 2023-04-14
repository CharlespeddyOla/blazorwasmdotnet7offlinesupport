using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Administration.Staff
{
    public class ADMEmployeeLocation
    {
        public int LocID { get; set; }
        public string Location { get; set; }
        public int StaffCount { get; set; }
        public bool DeleteName { get; set; }
        public int Id { get; set; }
    }

    public class LocationValidator : AbstractValidator<ADMEmployeeLocation>
    {
        public LocationValidator()
        {
            RuleFor(loc => loc.Location).NotEmpty().WithMessage("Location Name is required");
        }
    }
}
