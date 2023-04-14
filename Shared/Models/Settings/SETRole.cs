using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Settings
{
    public class SETRole
    {
        public int RoleID { get; set; }
        public string RoleDesc { get; set; }
        public int Id { get; set; }
    }

    public class AccessRoleValidator : AbstractValidator<SETRole>
    {
        public AccessRoleValidator()
        {
            RuleFor(r => r.RoleDesc).NotEmpty().WithMessage("Please Enter A Role");
        }
    }
}
