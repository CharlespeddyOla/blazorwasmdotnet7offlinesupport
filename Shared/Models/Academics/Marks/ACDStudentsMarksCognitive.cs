using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppAcademics.Shared.Models.Academics.Marks
{
    public class ACDStudentsMarksCognitive
    {        
        public int StudentMarkID { get; set; }
        public int Id { get; set; }
        public int TermID { get; set; }
        public int SchSession { get; set; }
        public int ClassID { get; set; }
        public int SchID { get; set; }
        public int StaffID { get; set; }
        public int ClassTeacherID { get; set; }
        public int SchoolPrincipalID { get; set; }
        public int STDID { get; set; }
        public int StudentID { get; set; }
        public int SubjectID { get; set; }
        public decimal Mark_Mid { get; set; }
        public decimal Mark_MidCBT { get; set; }
        public decimal Mark_ICGC { get; set; }
        public decimal Mark_CA1 { get; set; }
        public decimal Mark_CA2 { get; set; }
        public decimal Mark_CA3 { get; set; }
        public decimal Mark_CBT { get; set; }
        public decimal Mark_Exam { get; set; }
        public decimal Total_Exam { get; set; }
        public int GradeID { get; set; }
        public int GradeID_ICGC { get; set; }
        public int GradeID_Mid { get; set; }
        public bool EntryStatus_MidTerm { get; set; }
        public bool EntryStatus_TermEnd { get; set; }
        public bool EntryStatus_ICGCS { get; set; }
        public bool SbjSelection { get; set; }
        public bool Mark_Delete { get; set; }

        public string AcademicSession { get; set; }
        public string CurrentTerm { get; set; }
        public string ClassName { get; set; }
        public string School { get; set; }
        public string Teacher { get; set; }
        public string ClassTeacher { get; set; }
        public string Principal { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public string StudentNameWithNo { get; set; }
        public string Email { get; set; }
        public byte[] studentPhoto { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string Subject { get; set; }
        public string SubjectCode { get; set; }
        public int SN { get; set; }
        public decimal Mark_TotalMidTerm { get; set; }
        public decimal AVGPer { get; set; }
        public int SortID { get; set; }
        public string YouthClub { get; set; }
        public string YouthRole { get; set; }
        public int OldID { get; set; }
        public byte[] signClassTeacher { get; set; }
        public byte[] signPrincipal { get; set; }
        public string StudentInitials { get; set; }
        public bool SbjMerge { get; set; }
        public int SbjMergeID { get; set; }
        public string SbjMergeName { get; set; }
        
    }

    public class ACDStudentsMarksCognitiveFirstTerm
    {
        public int STDID { get; set; }
        public int SubjectID { get; set; }
        public decimal TotalMark { get; set; }
        public int Id { get; set; }
    }

    public class ACDStudentsMarksCognitiveSecondTerm
    {
        public int STDID { get; set; }
        public int SubjectID { get; set; }
        public decimal TotalMark { get; set; }
        public int Id { get; set; }
    }
}
