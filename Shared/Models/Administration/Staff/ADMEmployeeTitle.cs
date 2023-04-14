using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Administration.Staff
{
    public class ADMEmployeeTitle
    {
        public int TitleID { get; set; }
        public string Title { get; set; }
        public int Id { get; set; }
    }

    public class TitleValidator : AbstractValidator<ADMEmployeeTitle>
    {
        public TitleValidator()
        {
            RuleFor(t => t.Title).NotEmpty().WithMessage("Title is required");
        }
    }
}
