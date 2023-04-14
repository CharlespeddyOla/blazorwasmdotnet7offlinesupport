using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Models.Settings
{
    public class SETReports
    {
        public int ReportID { get; set; }
        public string ReportCode { get; set; }
        public string ReportFileName { get; set; }
        public string ReportDescr { get; set; }
        public string ReportClass { get; set; }
        public bool Delete { get; set; }
        public int Id { get; set; }
    }
}
