using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppAcademics.Shared.Models.Academics.Marks
{
    public class ACDStudentsMarksAssessment
    {
        public int StudentMarkID { get; set; }
        public int TermID { get; set; }
        public int SchSession { get; set; }
        public int SchID { get; set; }
        public int ClassID { get; set; }
        public int StaffID { get; set; }
        public string Teacher { get; set; }
        public int ClassTeacherID { get; set; }
        public string ClassTeacher { get; set; }
        public int STDID { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public int SubjectID { get; set; }
        public string SubjectCode { get; set; }
        public string Subject { get; set; }
        public int SbjClassID { get; set; }
        public int SubjectClassID { get; set; }
        public string SubjectClassification { get; set; }
        public decimal Rating { get; set; }
        public int OptionID { get; set; }
        public int TextID { get; set; }
        public int RatingID { get; set; }
        public bool SbjSelection { get; set; }
        public bool Mark_Delete { get; set; }
        public int SN { get; set; }
      
        public string ClassName { get; set; }
        public string School { get; set; }
        public int Id { get; set; }
    }
}
