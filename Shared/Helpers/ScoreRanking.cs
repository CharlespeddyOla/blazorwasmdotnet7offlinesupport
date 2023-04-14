using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Helpers
{
    public class ScoreRanking
    {
        public int STDID { get; set; }
        public int SubjectCount { get; set; }
        public int MarkObtained { get; set; }
        public int MarkObtainable { get; set; }
        public decimal AverageMark { get; set; }
        public int Position { get; set; }
        
    }
}
