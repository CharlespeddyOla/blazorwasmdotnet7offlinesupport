using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppAcademics.Shared.Models.Settings
{
    public class SETPrefix 
    {
        public int PrefixID { get; set; }
        public string PrefixType { get; set; }
        public string PrefixName { get; set; }
        public int PrefixDigits { get; set; }
        public string Message { get; set; }
        public bool AutoPrefix { get; set; }
        public int Id { get; set; }
    }
}
