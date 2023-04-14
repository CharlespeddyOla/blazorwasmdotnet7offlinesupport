using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Academics.CBT
{
    public class CBTExamType
    {
        public int ExamTypeID { get; set; }
        public string ExamType { get; set; }
        public int Id { get; set; }
    }

    public class CBTExams
    {
        public int ExamID { get; set; }
        public int TermID { get; set; }
        public int SchID { get; set; }
        public string School { get; set; }
        public int ClassListID { get; set; }
        public string SchClass { get; set; }
        public string ClassName { get; set; }
        public int SubjectID { get; set; }
        public string Subject { get; set; }
        public DateTime? ExamDate { get; set; }
        public int ReportTypeID { get; set; }
        public string ReportType { get; set; }
        public int ExamTypeID { get; set; }
        public string ExamType { get; set; }
        public string ExamCode { get; set; }
        public string ExamName { get; set; }
        public string ExamInstruction { get; set; }
        public double PassingPercentage { get; set; }
        public bool FixExamTime { get; set; }
        public double ExamTime { get; set; }
        public bool QTimer { get; set; }
        public bool ExamTimer { get; set; }
        public bool AllowCalc { get; set; }
        public byte[] ExamPassword { get; set; }
        public string Password { get; set; }
        public bool ExamDefault { get; set; }
        public string AcademicSession { get; set; }
        public int StaffID { get; set; }
        public string SubjectTeacher { get; set; }
        public int Id { get; set; }
    }

    public class CBTExamsValidator : AbstractValidator<CBTExams>
    {
        public CBTExamsValidator()
        {
            RuleFor(cbt => cbt.ExamDate).NotEmpty().WithMessage("Please Select Exam Date");
            RuleFor(cbt => cbt.ReportType).NotEmpty().WithMessage("Please Select Report Type");
            RuleFor(cbt => cbt.ExamType).NotEmpty().WithMessage("Please Select Exam Type");
            RuleFor(cbt => cbt.School).NotEmpty().WithMessage("Please Select School");
            RuleFor(cbt => cbt.ClassName).NotEmpty().WithMessage("Please Select Class Group");
            RuleFor(cbt => cbt.Subject).NotEmpty().WithMessage("Please Select Subject");
            RuleFor(cbt => cbt.ExamCode).NotEmpty().WithMessage("Please Enter Exam Code");
            RuleFor(cbt => cbt.ExamName).NotEmpty().WithMessage("Please Enter Exam Name");
            RuleFor(cbt => cbt.ExamInstruction).NotEmpty().WithMessage("Please Enter Exam Instruction");
            //RuleFor(cbt => cbt.Password).NotNull().WithMessage("Please Enter Password");
            RuleFor(cbt => cbt.PassingPercentage)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Passing Percentage must be greater than 0")
                .GreaterThan(-1).WithMessage("Passing Percentage must be greater than 0");
            RuleFor(cbt => cbt.ExamTime)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Enter Exam Time must be greater than 0")
                .GreaterThan(-1).WithMessage("Enter Exam Time must be greater than 0");
        }
    }

    public class CBTQuestionType
    {
        public int QTypeID { get; set; }
        public string QType { get; set; }
        public bool QTypeStatus { get; set; }
        public int Id { get; set; }
    }

    public class CBTQuestions
    {
        public int QID { get; set; }
        public int ExamID { get; set; }
        public string ExamCode { get; set; }
        public string ExamName { get; set; }
        public int QTypeID { get; set; }
        public string QType { get; set; }
        public int QNo { get; set; }
        public string Section { get; set; }
        public string Question { get; set; }
        public string Equation { get; set; }
        public string CurrentQuestion { get; set; }
        public int QPoints { get; set; }
        public int QTime { get; set; }
        public byte[] SImage { get; set; }
        public byte[] QImage { get; set; }
        public int NAns { get; set; }
        public string ImageUrl { get; set; }
        public bool DeleteQuestion { get; set; }
        public int Id { get; set; }
    }

    public class CBTQuestionsValidator : AbstractValidator<CBTQuestions>
    {
        public CBTQuestionsValidator()
        {
            //RuleFor(q => q.QPoints)
            //     .Cascade(CascadeMode.Stop)
            //     .NotEmpty().WithMessage("Points (Marks) must be greater than 0")
            //     .GreaterThan(-1).WithMessage("Points (Marks) must be greater than 0");
            //RuleFor(q => q.QTime)
            //    .Cascade(CascadeMode.Stop)
            //    .NotEmpty().WithMessage("Question Time Allocation must be greater than 0")
            //    .GreaterThan(-1).WithMessage("Question Time Allocation must be greater than 0");
            RuleFor(q => q.QType).NotEmpty().WithMessage("Please Select Question Type");
            RuleFor(q => q.Question).NotNull().WithMessage("Please Enter The Question");
            
        }
    }

    public class CBTAnswers
    {
        public int AnsID { get; set; }
        public int ExamID { get; set; }
        public int QTypeID { get; set; }
        public int QID { get; set; }
        public string AnsLetter { get; set; }
        public string Answers { get; set; }
        public string Equation { get; set; }
        public byte[] AnsImage { get; set; }
        public bool CorrectAns { get; set; }
        public string ImageUrl { get; set; }
        public string CorrectAnswer { get; set; }
        public bool DeleteAnswers { get; set; }
        public string Choices { get; set; }
        public int Id { get; set; }

    }

    public class CBTStudentAnswers
    {
        public int StudentAnswerID { get; set; }
        public int STDID { get; set; }
        public int ExamID { get; set; }
        public int QTypeID { get; set; }
        public int QID { get; set; }
        public int QNo { get; set; }
        public string Answer { get; set; }
        public bool MultipleAnswer { get; set; }
        public bool QAnswered { get; set; }
        public bool Correct { get; set; }
        public int QPoints { get; set; }
        public int StudentScoreID { get; set; }
        public int AnsID { get; set; }
        public bool CBTToUse { get; set; }
        public int Id { get; set; }
    }
       
    public class CBTStudentScores
    {
        public int StudentScoreID { get; set; }
        public int ExamID { get; set; }
        public int STDID { get; set; }
        public DateTime? ExamDate { get; set; }
        public int NQuestions { get; set; }
        public int NUnAnsQuestions { get; set; }
        public int NWrongAns { get; set; }
        public int NCorrectAns { get; set; }
        public double ScorePercentage { get; set; } 
        public double PassingPercentage { get; set; }
        public bool QTimer { get; set; }
        public bool ExamTimer { get; set; }
        public string TimeAllocated { get; set; }
        public string TimeUsed { get; set; }
        public bool CBTToUse { get; set; }
        public int SubjectID { get; set; }
        public string Subject { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public byte[] studentPhoto { get; set; }
        public int TermID { get; set; }
        public int ReportTypeID { get; set; }
        public int SN { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public int Id { get; set; }
    }

    public class CBTSelectedAnswer
    {
        public int QID { get; set; }
        public int QNo { get; set; }
        public int AnsID { get; set; }
        public string AnsLetter { get; set; }
        public bool QAnswered { get; set; }
        public bool Correct { get; set; }
        public byte[] AnsImage { get; set; }
        public int Id { get; set; }
    }

    public class CBTLatex
    {
        public int LatexID { get; set; }
        public string LatexGroup { get; set; }
        public string LatexSymbol { get; set; }
        public string Description { get; set; }
        public string Package { get; set; }
        public int Id { get; set; }
    }

    public class CBTConnectionInfo
    {
        public int ConnectionID { get; set; }
        public string ConnectionName { get; set; }
        public string ConnectionValue { get; set; }
        public int Id { get; set; }
    }

    public class CBTExamTakenFlags
    {
        public int FlagID { get; set; }
        public int STDID { get; set; }
        public int ExamID { get; set; }
        public int TermID { get; set; }
        public bool Flag { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public int SubjectID { get; set; }
        public string Subject { get; set; }
        public int Id { get; set; }
    }
}
