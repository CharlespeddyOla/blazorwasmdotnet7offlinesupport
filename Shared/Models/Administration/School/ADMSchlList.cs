using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Administration.School
{
    public class ADMSchlList
    {
        public int SchID { get; set; }
        public string School { get; set; }
        public int StaffID { get; set; }
        public int SchImageID { get; set; }
        public string Head { get; set; }
        public int HeadCode { get; set; }
        public bool Status { get; set; }
        public string SchoolHeadlTitle { get; set; }
        public string SchoolHead { get; set; }
        public string SchoolHeadWithNo { get; set; }
        public int SchoolCount { get; set; }
        public int Id { get; set; }

    }

    public class SchoolValidator : AbstractValidator<ADMSchlList>
    {
        public SchoolValidator()
        {
            RuleFor(sch => sch.School).NotEmpty().WithMessage("School Name is required");
            RuleFor(sch => sch.Head).NotEmpty().WithMessage("Head Type is required");
            RuleFor(sch => sch.SchoolHeadWithNo).NotEmpty().WithMessage("School Head is required");
        }
    }

}
