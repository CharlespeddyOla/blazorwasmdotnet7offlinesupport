using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Administration.School
{
    public class ADMSchClassCategory
    {
        public int CATID { get; set; }
        public string CATName { get; set; }
        public string CATCode { get; set; }
        public int Id { get; set; }
    }

    public class ClassCategoryValidator : AbstractValidator<ADMSchClassCategory>
    {
        public ClassCategoryValidator()
        {
            RuleFor(c => c.CATName).NotEmpty().WithMessage("Class Name is required");
        }
    }

}
