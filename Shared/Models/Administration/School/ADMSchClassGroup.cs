using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Administration.School
{
    public class ADMSchClassGroup
    {
        public int ClassListID { get; set; }
        public string SchClass { get; set; }
        public int SchID { get; set; }
        public string School { get; set; }
        public string ConvensionalName { get; set; }
        public bool UseConvension { get; set; }
        public string ClassName { get; set; }
        public int ClassGroupCount { get; set; }
        public string ClassGroupName { get; set; }
        public int Id { get; set; }
    }

    public class ClassGroupValidator : AbstractValidator<ADMSchClassGroup>
    {
        public ClassGroupValidator()
        {
            RuleFor(g => g.School).NotEmpty().WithMessage("School Name is required");
            RuleFor(g => g.SchClass).NotEmpty().WithMessage("Class is required");
            RuleFor(g => g.ConvensionalName).NotEmpty().WithMessage("Convensional Name is required");
        }
    }

}
