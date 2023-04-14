using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Administration.School
{

    public class ADMSchClassList
    {
        public int ClassID { get; set; }
        public int SchInfoID { get; set; }
        public int SchID { get; set; }
        public int ClassListID { get; set; }
        public int DisciplineID { get; set; }
        public int StaffID { get; set; }
        public int CATID { get; set; }
        public bool FinalYearClass { get; set; }
        public bool JuniorFinalYearClass { get; set; }
        public bool CheckPointClass { get; set; }
        public bool IGCSEClass { get; set; }
        public string School { get; set; }
        public string Discipline { get; set; }
        public string CATName { get; set; }
        public string CATCode { get; set; }
        public bool UseConvension { get; set; }
        public string SchClass { get; set; }
        public string ConvensionalName { get; set; }
        public string ClassName { get; set; }
        public int ClassCount { get; set; }
        public string ClassTeacherTitle { get; set; }
        public string ClassTeacher { get; set; }
        public string ClassTeacherWithNo { get; set; }
        public string ClassTeacherInitials { get; set; }
        public int PrincipalID { get; set; }
        public string PrincipalTitle { get; set; }
        public string Principal { get; set; }
        public string PrincipalWithNo { get; set; }       
        public string PrincipalInitials { get; set; }
        public string ClassGroupName { get; set; }        
        public int SN { get; set; }
        public int Id { get; set; }
    }

    public class ClassDetailsValidator : AbstractValidator<ADMSchClassList>
    {
        public ClassDetailsValidator()
        {
            RuleFor(c => c.School).NotEmpty().WithMessage("Please Select School");
            RuleFor(c => c.ClassGroupName).NotEmpty().WithMessage("Please Select A Class");
            //RuleFor(c => c.SchClass).NotEmpty().When(c => c.UseConvension == false).WithMessage("Please Select Class Group");
            //RuleFor(c => c.ConvensionalName).NotEmpty().When(c => c.UseConvension == true).WithMessage("Please Select Class Group");
            RuleFor(c => c.CATName).NotEmpty().WithMessage("Please Select Class Name");
            RuleFor(c => c.Discipline).NotEmpty().WithMessage("Please Select Class Discipline");
            RuleFor(c => c.ClassTeacherWithNo).NotEmpty().WithMessage("Please Select Class Teacher");
        }
    }

}
