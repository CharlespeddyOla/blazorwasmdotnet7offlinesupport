using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppAcademics.Server.ReportHelpers
{
    public class FooterSign : PdfPageEventHelper
    {
        protected PdfPTable footerSign;

        public FooterSign(PdfPTable footerSign)
        {
            this.footerSign = footerSign;
        }

        public override void OnEndPage(PdfWriter writer, Document doc)
        {
            LineDash solid = new SolidLine();

            PdfPTable footerTblSign = new(1)
            {
                TotalWidth = 175
            };

            footerTblSign.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell footerSignCell = new(this.footerSign)
            {
                Border = PdfPCell.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingBottom = 40f,
            };

            footerTblSign.AddCell(footerSignCell);
            footerTblSign.WriteSelectedRows(0, -1, 200, 120, writer.DirectContent);
        }
    }
}
