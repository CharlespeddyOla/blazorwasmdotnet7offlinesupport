using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Administration.Staff
{
    public class ADMEmployeeDepts
    {
        public int EmployeeGroupID { get; set; }
        public string EmployeeGroup { get; set; }
        public string AccessLevel { get; set; }
        public bool MarkAsTeacher { get; set; }
        public bool MarkAsPrincipal { get; set; }
        public int StaffCount { get; set; }
        public bool DeleteName { get; set; }
        public int Id { get; set; }
    }

    public class DepartmentValidator : AbstractValidator<ADMEmployeeDepts>
    {
        public DepartmentValidator()
        {
            RuleFor(dept => dept.EmployeeGroup).NotEmpty().WithMessage("Department Name is required");
        }
    }

}
