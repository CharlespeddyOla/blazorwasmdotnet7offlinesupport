using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Administration.School
{
    public class ADMSchClassDiscipline
    {
        public int DisciplineID { get; set; }
        public string Discipline { get; set; }
        public int Id { get; set; }
    }

    public class ClassDisciplineValidator : AbstractValidator<ADMSchClassDiscipline>
    {
        public ClassDisciplineValidator()
        {
            RuleFor(d => d.Discipline).NotEmpty().WithMessage("Class Discipline is required");
        }
    }

}
