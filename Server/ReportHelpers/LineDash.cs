using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppAcademics.Server.ReportHelpers
{
    public interface LineDash
    {
        public void applyLineDash(PdfContentByte canvas);
    }
}

