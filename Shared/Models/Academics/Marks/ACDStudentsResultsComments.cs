using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Academics.Marks
{
    public class ACDReportCommentMidTerm
    {
        public int CommentID { get; set; }
        public int TermID { get; set; }
        public int SchSession { get; set; }
        public int ClassID { get; set; }
        public int ClassTeacherID { get; set; }
        public int STDID { get; set; }
        public int MarkObtainable { get; set; }
        public int MarkObtained { get; set; }
        public string Comments_Teacher { get; set; }
        public decimal AVGPer { get; set; }
        public int Position { get; set; }
        public string Grade { get; set; }
        public int SN { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public string AcademicSession { get; set; }
        public string ClassName { get; set; }
        public string ClassTeacher { get; set; }
        public int Id { get; set; }
    }

    public class ACDReportCommentsTerminal
    {
        public int CommentID { get; set; }
        public int TermID { get; set; }
        public int SchSession { get; set; }
        public int ClassID { get; set; }
        public int ClassTeacherID { get; set; }
        public int STDID { get; set; }
        public int Attendance { get; set; }
        public int DaysAbsent { get; set; }
        public string Comments_Teacher { get; set; }
        public string Comments_Principal { get; set; }
        public decimal AVGPer { get; set; }
        public int Position { get; set; }
        public string Grade { get; set; }
        public int SN { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public string AcademicSession { get; set; }
        public string ClassName { get; set; }
        public string ClassTeacher { get; set; }
        public int MarkObtainable { get; set; }
        public int MarkObtained { get; set; }
        public int Id { get; set; }
    }

    public class ACDReportCommentCheckPointIGCSE
    {
        public int CommentID { get; set; }
        public int TermID { get; set; }
        public int SchSession { get; set; }
        public int ClassID { get; set; }
        public int ClassTeacherID { get; set; }
        public int STDID { get; set; }
        public string Comments { get; set; }
        public decimal AVGPer { get; set; }
        public int Position { get; set; }
        public string Grade { get; set; }
        public int SN { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public string AcademicSession { get; set; }
        public string ClassName { get; set; }
        public string ClassTeacher { get; set; }
        public int Id { get; set; }
    }
}
