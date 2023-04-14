using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppAcademics.Shared.Models.Settings
{
    public class SETMonthList
    {
        public int MonthID { get; set; }
        public string Month { get; set; }
        public string MonthCode { get; set; }
        public int NoofDays { get; set; }
        public int NoOfWeeks { get; set; }
        public int PRLStatus { get; set; }
        public int StartMonthID { get; set; }
        public string StartMonth { get; set; }
        public int EndMonthID { get; set; }
        public string EndMonth { get; set; }
        public int Id { get; set; }
    }
}
