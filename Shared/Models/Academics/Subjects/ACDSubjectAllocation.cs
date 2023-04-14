using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Academics.Subjects
{
    public class ACDSbjAllocationStudents
    {
        public int SbjAllocID { get; set; }
        public int TermID { get; set; }
        public int SchSession { get; set; }
        public int STDID { get; set; }
        public bool SbjSelection { get; set; }
        public int SubjectID { get; set; }
        public string SubjectCode { get; set; }
        public string Subject { get; set; }
        public int ClassID { get; set; }
        public int ClassListID { get; set; }
        public int SbjClassID { get; set; }
        public string SbjClassification { get; set; }
        public string SbjDept { get; set; }
        public int SchID { get; set; }
        public string School { get; set; }
        public string SchClass { get; set; }
        public string ClassName { get; set; }
        public int DeleteStatus { get; set; }
        public int SN { get; set; }

        public string Surname { get; set; } //ADMStudents
        public string FirstName { get; set; } //ADMStudents
        public string MiddleName { get; set; } //ADMStudents
        public string StudentNo { get; set; }
        public string StudentName { get; set; }
        public int Id { get; set; }
    }

    public class ACDSbjAllocationTeachers
    {
        public int SbjAllocID { get; set; }
        public int TermID { get; set; }
        public int SchSession { get; set; }
        public int StaffID { get; set; }
        public bool SbjSelection { get; set; }
        public int SubjectID { get; set; }
        public string SubjectCode { get; set; }
        public string Subject { get; set; }
        public int SbjClassID { get; set; }
        public string SbjClassification { get; set; }
        public string SbjDept { get; set; }
        public int SchID { get; set; }
        public int ClassID { get; set; }
        public int ClassListID { get; set; }
        public string School { get; set; }
        public string SchClass { get; set; }
        public string ClassName { get; set; }
        public string ClassGroupName { get; set; }        
        public string SubjectTeacher { get; set; }
        public int StaffID_ClassTeacher { get; set; }
        public string ClassTeacher { get; set; }
        public int StaffID_Principal { get; set; }
        public string Principal { get; set; }
        public bool DeleteStatus { get; set; }
        public int SN { get; set; }

        public string Surname { get; set; } //ADMEmployee
        public string FirstName { get; set; } //ADMEmployee
        public string MiddleName { get; set; } //ADMEmployee
        public string Title { get; set; } //ADMEmployeeTitle
        public string SubjectTeacherInitials { get; set; }
        public int Id { get; set; }
    }
}
