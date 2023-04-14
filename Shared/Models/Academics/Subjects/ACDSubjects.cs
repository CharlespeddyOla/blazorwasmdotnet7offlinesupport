using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppAcademics.Shared.Models.Academics.Subjects
{
    public class CombinesSubjects
    {
        public int SubjectID { get; set; }
        public string Subject { get; set; }
        public int SchID { get; set; }
        public bool SbjMerge { get; set; }
        public int SbjMergeID { get; set; }
        public string SbjMergeName { get; set; }
        public override bool Equals(object o)
        {
            var other = o as CombinesSubjects;
            return other?.SubjectID == SubjectID;
        }
        public override int GetHashCode() => SubjectID.GetHashCode();
        public int Id { get; set; }
    }

    public class ACDSubjects
    {
        public int SubjectID { get; set; }
        public int SbjDeptID { get; set; }
        public int SbjClassID { get; set; }
        public int SchID { get; set; }
        public string SubjectCode { get; set; }
        public string Subject { get; set; }     
        public bool SubjectStatus { get; set; }
        public bool SbjMerge { get; set; }
        public int SbjMergeID { get; set; }
        public string SbjMergeName { get; set; }
        public int SortID { get; set; }
                
        public string SubjectDepartment { get; set; }
        public string SubjectClassification { get; set; }
        public bool SubjectClassificationStatus { get; set; }        
        public string School { get; set; }
        public int Id { get; set; }

    }

    public class SubjectDetailsValidator : AbstractValidator<ACDSubjects>
    {
        public SubjectDetailsValidator()
        {
            RuleFor(sbj => sbj.School).NotEmpty().WithMessage("Please Select School");
            RuleFor(sbj => sbj.SubjectDepartment).NotEmpty().WithMessage("Please Select Subject Depatment");
            RuleFor(sbj => sbj.SubjectClassification).NotEmpty().WithMessage("Please Select Subject Classification");
            RuleFor(sbj => sbj.Subject).NotEmpty().WithMessage("Please Enter The Subject Name");
        }
    }

    public class ACDSbjDept
    {
        public int SbjDeptID { get; set; }
        public string SbjDept { get; set; }
        public int SubjectCount { get; set; }
        public int Id { get; set; }
    }

    public class SubjectDeptDetailsValidator : AbstractValidator<ACDSbjDept>
    {
        public SubjectDeptDetailsValidator()
        {
            RuleFor(d => d.SbjDept).NotEmpty().WithMessage("Please Enter The Subject Department Name");
        }
    }

    public class ACDSbjClassification
    {
        public int SbjClassID { get; set; }
        public string SbjClassification { get; set; }
        public int SbjImageIndex { get; set; }
        public bool Remark { get; set; }
        public bool? Comment { get; set; }
        public int SubjectCount { get; set; }
        public bool Status { get; set; }
        public int Id { get; set; }
    }

    public class SubjectClassValidator : AbstractValidator<ACDSbjClassification>
    {
        public SubjectClassValidator()
        {
            RuleFor(c => c.SbjClassification).NotEmpty().WithMessage("Please Enter The Subject Classification");
        }
    }

    public class ACDSubjectsRanking
    {
        public int STDID { get; set; }
        public int SubjectID { get; set; }
        public string SubjectCode { get; set; }
        public string Subject { get; set; }
        public int SubjectCount { get; set; }
        public int CAMark { get; set; }
        public int ExamMark { get; set; }
        public decimal TotalMark { get; set; }
        public int Position { get; set; }
        public int MaxMark { get; set; }
        public int MinMark { get; set; }
        public string Grade { get; set; }
        public string Remark { get; set; }
        public decimal ClassAverage { get; set; }
        public int OverAllScore { get; set; }
        public decimal AverageMark { get; set; }
        public int Id { get; set; }
    }
}
