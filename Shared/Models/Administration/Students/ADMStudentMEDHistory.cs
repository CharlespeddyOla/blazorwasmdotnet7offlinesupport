using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppAcademics.Shared.Models.Administration.Students
{
    public class ADMStudentMEDHistory
    {
        public int MEDHistoryID { get; set; }
        public int SchInfoID { get; set; }
        public int STDID { get; set; }
        public int MEDID { get; set; }
        public string MEDName { get; set; }
        public bool MEDValue { get; set; }
        public string MEDTextValue { get; set; }
        public int Id { get; set; }
    }
}
