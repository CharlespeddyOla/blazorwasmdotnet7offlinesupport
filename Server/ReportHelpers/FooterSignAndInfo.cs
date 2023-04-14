using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppAcademics.Server.ReportHelpers
{
    public class FooterSignAndInfo : PdfPageEventHelper
    {
        protected PdfPTable footerSign;
        private readonly string footerInfo;

        public FooterSignAndInfo(PdfPTable footerSign, string footerInfo)
        {
            this.footerSign = footerSign;
            this.footerInfo = footerInfo;
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
            footerTblSign.WriteSelectedRows(0, -1, 200, 150, writer.DirectContent);

            PdfPTable footerTblInfo = new(1)
            {
                TotalWidth = 523
            };
            footerTblInfo.HorizontalAlignment = Element.ALIGN_CENTER;

            Paragraph footerInfoPara = new(footerInfo, FontFactory.GetFont(FontFactory.TIMES, 11, iTextSharp.text.Font.BOLDITALIC))
            {
                Alignment = Element.ALIGN_CENTER
            };

            PdfPCell footerInfoCell = new(footerInfoPara)
            {
                Border = PdfPCell.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                CellEvent = new TableBorders(null, null, solid, null)
            };

            footerTblInfo.AddCell(footerInfoCell);
            footerTblInfo.WriteSelectedRows(0, -1, 36, 50, writer.DirectContent);
        }
    }
}
