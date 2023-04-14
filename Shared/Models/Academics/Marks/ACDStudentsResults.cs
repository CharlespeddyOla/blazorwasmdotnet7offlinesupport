using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Academics.Marks
{
    public class ACDStudentsResultCognitive
    {
        public int STDID { get; set; }
        public int ClassID { get; set; }
        public int SubjectID { get; set; }
        public string SubjectCode { get; set; }
        public string Subject { get; set; }
        public int CA1 { get; set; }
        public int CA2 { get; set; }
        public int CA3 { get; set; }
        public int CA { get; set; }
        public int Exam { get; set; }
        public int TotalMark { get; set; }
        public string Grade { get; set; }
        public string Remarks { get; set; }
        public int POS { get; set; }
        public int MaxMark { get; set; }
        public int MinMark { get; set; }
        public decimal ClassAvg { get; set; }
        public int FTerm { get; set; }
        public int STerm { get; set; }
        public int TTerm { get; set; }
        public byte[] StudentPhoto { get; set; }
        public string StudentNo { get; set; }
        public string FullName { get; set; }
        public string ClassTeacher { get; set; }
        public string YouthClub { get; set; }
        public string YouthRole { get; set; }
        public int Attendance { get; set; }
        public int DaysAbsent { get; set; }
        public DateTime? NextTermBegins { get; set; }
        public DateTime? NextTermEnds { get; set; }
        public string ClassName { get; set; }
        public int No_In_Class { get; set; }
        public int No_Of_Sbj { get; set; }
        public decimal AVGPer { get; set; }
        public int Position { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public int OverAllScore { get; set; }
        public string Comments_Teacher { get; set; }
        public byte[] ClassTeacherSign { get; set; }
        public string Comments_Principal { get; set; }
        public byte[] PrincipalSign { get; set; }
        public string SubjectTeacher { get; set; }
        public int OutOf { get; set; }
        public decimal YearAvg { get; set; }
        public int StudentID { get; set; }
        public int ClassTeacherID { get; set; }
        public int SchPrincipalID { get; set; }
        public string SchPrincipal { get; set; }
        public decimal PRJ { get; set; }
        public decimal CLW { get; set; }
        public decimal ATD { get; set; }
        public int PMark_CA1 { get; set; }
        public int PMark_CA2 { get; set; }
        public int PMark_CA3 { get; set; }
        public int PMark_Project { get; set; }
        public int PMark_ClassWork { get; set; }
        public int PMark_Attendance { get; set; }
        public int PMark_Exam { get; set; }
        public string MarkType { get; set; }
        public int AlphabetID { get; set; }
        public string AcademicSession { get; set; }
        public string CurrentTerm { get; set; }
        public int SchInfoID { get; set; }
        public string SchName { get; set; }
        public string SchSlogan { get; set; }
        public string SchAddress { get; set; }
        public string SchAddressLine2 { get; set; }
        public string SchPhones { get; set; }
        public string SchEmails { get; set; }
        public string SchWebsites { get; set; }
        public byte[] SchLogo { get; set; }
        public int SortID { get; set; }
        public int Id { get; set; }
    }

    public class ACDStudentsResultAssessmentBool
    {
        public int STDID { get; set; }
        public int ClassID { get; set; }
        public int SubjectID { get; set; }
        public int SbjClassID { get; set; }
        public string SbjClassification { get; set; }
        public string SubjectCode { get; set; }
        public string Subject { get; set; }
        public int RatingValue { get; set; }
        public bool Five { get; set; }
        public bool Four { get; set; }
        public bool Three { get; set; }
        public bool Two { get; set; }
        public bool One { get; set; }
        public bool Zero { get; set; }
        public int Id { get; set; }
    }

    public class ACDStudentsResultAssessmentText
    {
        public int STDID { get; set; }
        public int ClassID { get; set; }
        public int SubjectID { get; set; }
        public decimal TermRating { get; set; }
        public string GradeLetter { get; set; }
        public string RatingRemarks { get; set; }
        public int Id { get; set; }
    }

    public class ACDBroadSheet
    {
        public string scriptFilePath { get; set; }
        public int STDID { get; set; }
        public int SubjectCount { get; set; }
        public int MarkObtained { get; set; }
        public int Position { get; set; }
        public decimal AverageMark { get; set; }
        public int Id { get; set; }
    }

}
