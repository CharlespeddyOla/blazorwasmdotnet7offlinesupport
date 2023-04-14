using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppAcademics.Server.ReportHelpers
{
    public class TableBorders : IPdfPCellEvent
    {
        protected LineDash left;
        protected LineDash right;
        protected LineDash top;
        protected LineDash bottom;

        public TableBorders(LineDash left, LineDash right, LineDash top, LineDash bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }

        public void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases)
        {
            PdfContentByte canvas = canvases[PdfPTable.LINECANVAS];
            if (top != null)
            {
                canvas.SaveState();
                top.applyLineDash(canvas);
                canvas.MoveTo(position.GetRight(1f), position.GetTop(1f));
                canvas.LineTo(position.GetLeft(1f), position.GetTop(1f));
                canvas.Stroke();
                canvas.RestoreState();
            }
            if (bottom != null)
            {
                canvas.SaveState();
                bottom.applyLineDash(canvas);
                canvas.MoveTo(position.GetRight(1f), position.GetBottom(1f));
                canvas.LineTo(position.GetLeft(1f), position.GetBottom(1f));
                canvas.Stroke();
                canvas.RestoreState();
            }
            if (right != null)
            {
                canvas.SaveState();
                right.applyLineDash(canvas);
                canvas.MoveTo(position.GetRight(1f), position.GetTop(1f));
                canvas.LineTo(position.GetRight(1f), position.GetBottom(1f));
                canvas.Stroke();
                canvas.RestoreState();
            }
            if (left != null)
            {
                canvas.SaveState();
                left.applyLineDash(canvas);
                canvas.MoveTo(position.GetLeft(1f), position.GetTop(1f));
                canvas.LineTo(position.GetLeft(1f), position.GetBottom(1f));
                canvas.Stroke();
                canvas.RestoreState();
            }
        }
    }
}


