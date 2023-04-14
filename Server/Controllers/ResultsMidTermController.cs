using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using WebAppAcademics.Server.Interfaces;
using WebAppAcademics.Server.ReportHelpers;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsMidTermController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        SwitchModel _switch = new();

        public ResultsMidTermController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region [Section - Variables Declaration]
        string _resultTitle { get; set; }
        byte[] _schLogo { get; set; }
        string _schName { get; set; }
        string _schSlogan { get; set; }
        string _schAddress1 { get; set; }
        string _schAddress2 { get; set; }
        string _schPhones { get; set; }
        string _academicSession { get; set; }
        string _caTitle { get; set; }
        string _examTitle { get; set; }
        byte[] _studentPhoto { get; set; }
        byte[] _signPrincipal { get; set; }
        #endregion

        #region [Section - Declaration]
        Document _document;
        PdfWriter writer;
        PdfPCell _pdfPCell;
        Font _fontStyle;
        MemoryStream _memoryStream = new();
        List<ACDStudentsResultCognitive> _studentResults = new();

        #endregion

        [HttpGet]
        [Route("GetPDFResults/{previewid}/{stdid}")]
        public async Task<IActionResult> GetPDFResults(int previewid, int stdid)
        {
            try
            {
                _switch.SwitchID = 1;
                var data = await unitOfWork.CognitiveResults.GetAllAsync(_switch);
                
                if (stdid == 0)
                {
                    _studentResults = data.ToList();                    
                }
                else
                {
                    _studentResults = data.Where(s => s.STDID == stdid).ToList();
                }

                var resultHeaderCA = await unitOfWork.MarkSettings.GetByIdAsync(8);
                var resultHeaderExam = await unitOfWork.MarkSettings.GetByIdAsync(1);

                _caTitle = "CA      (" + resultHeaderCA.Mark.ToString() + ")";
                _examTitle = "EXAM      (" + resultHeaderExam.Mark.ToString() + ")";

                ResultHeaderDetails();

                var studentIDs = _studentResults.Select(s => s.STDID).Distinct();

                _document = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
                writer = PdfWriter.GetInstance(_document, _memoryStream);

                FooterSign footer = new(Signature());
                writer.PageEvent = footer;

                _document.Open();

                foreach (var item in studentIDs)
                {
                    _studentPhoto = _studentResults.FirstOrDefault(s => s.STDID == item).StudentPhoto;

                    _document.NewPage();

                    //Add Header Table Here
                    _document.Add(HeaderTable());

                    //Add Sub Header Table Here
                    _document.Add(SubHeaderTable(item));

                    //Add Main Table (Body Table) Here
                    if (previewid == 3)
                    {
                        _document.Add(OtherResultBody(item));
                    }
                    else
                    {
                        _document.Add(FinalYearClassResultBody(item));
                    }

                    //Add Footer Table Here
                    _document.Add(ResultFooter(item));
                    _document.Add(AddComment(item));
                }

                _document.Close();
            }
            catch(Exception ex)
            {
                var msg = ex.Message;
            }
            return File(_memoryStream.ToArray(), "application/pdf");
        }


        #region [Section - Header Table]

        private void ResultHeaderDetails()
        {
            _schName = _studentResults.FirstOrDefault().SchName;
            _schAddress1 = _studentResults.FirstOrDefault().SchAddress;
            _schAddress2 = _studentResults.FirstOrDefault().SchAddressLine2;
            _schPhones = _studentResults.FirstOrDefault().SchPhones;
            _academicSession = _studentResults.FirstOrDefault().AcademicSession + " - " + _studentResults.FirstOrDefault().CurrentTerm;
            _schLogo = _studentResults.FirstOrDefault().SchLogo;
            _schSlogan = _studentResults.FirstOrDefault().SchSlogan;
            _resultTitle = _studentResults.FirstOrDefault().SubjectTeacher;
            _signPrincipal = _studentResults.FirstOrDefault().PrincipalSign;
        }

        private PdfPTable HeaderTable()
        {

            int maxColumn = 4;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95
            };
            pdfPtable.SetWidths(new float[] { 10f, 20f, 20f, 11f });
            LineDash solid = new SolidLine();

            _pdfPCell = new PdfPCell(this.PreHeaderTable())
            {
                Colspan = 4,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,
                PaddingTop = 5f,
                PaddingBottom = 5f,
                CellEvent = new TableBorders(solid, solid, solid, solid)
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        private PdfPTable PreHeaderTable()
        {
            int maxColumn = 4;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95
            };
            pdfPtable.SetWidths(new float[] { 7f, 25f, 25f, 12f });

            if (_schLogo != null)
            {
                _pdfPCell = new PdfPCell(this.AddSchoohLogo())
                {
                    Colspan = 1,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfPCell.NO_BORDER
                };
                pdfPtable.AddCell(_pdfPCell);
            }
            else
            {
                _pdfPCell = new PdfPCell
                {
                    Colspan = 1,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = PdfPCell.NO_BORDER
                };
                pdfPtable.AddCell(_pdfPCell);
            }

            _pdfPCell = new PdfPCell(this.PageTitle())
            {
                Colspan = maxColumn - 2,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            if (_studentPhoto != null)
            {
                _pdfPCell = new PdfPCell(this.AddStudentPhoto())
                {
                    Colspan = 1,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfPCell.NO_BORDER
                };
                pdfPtable.AddCell(_pdfPCell);
            }
            else
            {
                _pdfPCell = new PdfPCell
                {
                    Colspan = 1,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfPCell.NO_BORDER
                };
                pdfPtable.AddCell(_pdfPCell);
            }

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        private PdfPTable AddSchoohLogo()
        {
            int maxColumn = 1;
            PdfPTable pdfPtable = new(maxColumn);
            Image img = Image.GetInstance(_schLogo);
            img.ScaleAbsolute(50f, 50f);

            _pdfPCell = new PdfPCell(img)
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        private PdfPTable PageTitle()
        {
            int maxColumn = 2;
            PdfPTable pdfPtable = new(maxColumn);

            _fontStyle = FontFactory.GetFont("Tahoma", 14f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase(_schName, _fontStyle))
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);

            _pdfPCell = new PdfPCell(new Phrase(_schSlogan, _fontStyle))
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);

            _pdfPCell = new PdfPCell(new Phrase(_schAddress1, _fontStyle))
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            if (_schAddress2 != null)
            {
                _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);

                _pdfPCell = new PdfPCell(new Phrase(_schAddress2, _fontStyle))
                {
                    Colspan = maxColumn,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = PdfPCell.NO_BORDER
                };
                pdfPtable.AddCell(_pdfPCell);

                pdfPtable.CompleteRow();
            }

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);

            _pdfPCell = new PdfPCell(new Phrase(_schPhones, _fontStyle))
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 1);

            _pdfPCell = new PdfPCell(new Phrase(_resultTitle, _fontStyle))
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 1);

            _pdfPCell = new PdfPCell(new Phrase(_academicSession, _fontStyle))
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        private PdfPTable AddStudentPhoto()
        {
            int maxColumn = 1;
            PdfPTable pdfPtable = new(maxColumn);
            Image img = Image.GetInstance(_studentPhoto);
            img.ScaleAbsolute(90f, 96f);

            _pdfPCell = new PdfPCell(img)
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        #endregion

        #region [Section - Sub Header Table]
        private PdfPTable SubHeaderTable(int _stdid)
        {
            int maxColumn = 4;
            PdfPTable pdfPtable = new PdfPTable(maxColumn);
            pdfPtable.WidthPercentage = 95;
            pdfPtable.SetWidths(new float[] { 10f, 20f, 20f, 11f });
            LineDash solid = new SolidLine();

            _pdfPCell = new PdfPCell(this.PreSubHeaderTable(_stdid))
            {
                Colspan = 4,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,
                PaddingTop = 5f,
                PaddingBottom = 5f,
                CellEvent = new TableBorders(null, null, null, null)
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        private PdfPTable PreSubHeaderTable(int _stdid)
        {
            int maxColumn = 4;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95
            };
            pdfPtable.SetWidths(new float[] { 5f, 7f, 7f, 13f });

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("ADMISSION NO.:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                FixedHeight = 20f
                //Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).StudentNo, _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                //Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("SEX:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                //Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).Gender, _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                //Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("NAME:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                FixedHeight = 20f
                //Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).FullName, _fontStyle))
            {
                Colspan = 3,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                //Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("CLASS:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                FixedHeight = 20f
                //Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).ClassName, _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                //Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("CLASS TEACHER:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                //Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).ClassTeacher, _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                //Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        #endregion

        #region [Section - Result Body]

        private PdfPTable FinalYearClassResultBody(int _stdid)
        {
            int maxColumn = 4;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95,
                SpacingBefore = 3f
            };

            pdfPtable.SetWidths(new float[] { 12f, 2.5f, 2.5f, 15f });

            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 1);

            #region [Table Header]
            _pdfPCell = new PdfPCell(new Phrase("Subject", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                PaddingLeft = 5f,
                PaddingBottom = 3f,
                FixedHeight = 25f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("SCORE (%)", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                PaddingBottom = 3f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("GRADE", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                PaddingBottom = 3f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("REMARK", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                PaddingBottom = 3f,
                PaddingLeft = 5f
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            #endregion

            #region [Table Body]

            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 0);

            foreach (var item in _studentResults.Where(s => s.STDID == _stdid).OrderBy(s => s.SortID))
            {
                _pdfPCell = new PdfPCell(new Phrase(item.Subject, _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White,
                    PaddingLeft = 5f,
                    FixedHeight = 20f
                };
                pdfPtable.AddCell(_pdfPCell);
                               
                _pdfPCell = new PdfPCell(new Phrase(item.TotalMark.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.Grade, _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.Remarks, _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White,
                    PaddingLeft = 5f
                };
                pdfPtable.AddCell(_pdfPCell);

                pdfPtable.CompleteRow();
            }

            #endregion

            return pdfPtable;
        }

        private PdfPTable OtherResultBody(int _stdid)
        {
            int maxColumn = 6;           
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95,
                SpacingBefore = 3f
            };

            pdfPtable.SetWidths(new float[] { 12f, 2.5f, 2.5f, 2.5f, 2.5f, 15f });

            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 1);

            #region [Table Header]
            _pdfPCell = new PdfPCell(new Phrase("Subject", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                PaddingLeft = 5f,
                PaddingBottom = 3f,
                FixedHeight = 25f
            };
            pdfPtable.AddCell(_pdfPCell);

          
            _pdfPCell = new PdfPCell(new Phrase(_caTitle, _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                PaddingBottom = 3f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(_examTitle, _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                PaddingBottom = 3f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("SCORE (%)", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                PaddingBottom = 3f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("GRADE", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                PaddingBottom = 3f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("REMARK", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                PaddingLeft = 5f,
                PaddingBottom = 3f
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            #endregion

            #region [Table Body]

            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 0);

            foreach (var item in _studentResults.Where(s => s.STDID == _stdid).OrderBy(s => s.SortID))
            {               
                _pdfPCell = new PdfPCell(new Phrase(item.Subject, _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White,
                    PaddingLeft = 5f,
                    FixedHeight = 20f
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.CA.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.Exam.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.TotalMark.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.Grade, _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);
                              
                _pdfPCell = new PdfPCell(new Phrase(item.Remarks, _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White,
                    PaddingLeft = 5f
                };
                pdfPtable.AddCell(_pdfPCell);

                pdfPtable.CompleteRow();
            }

            #endregion

            return pdfPtable;
        }

        #endregion

        #region [Section - Result Footer]
        private PdfPTable ResultFooter(int _stdid)
        {
            int maxColumn = 2;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95
            };
            pdfPtable.SetWidths(new float[] { 8f, 12f });
            pdfPtable.SpacingBefore = 10f;

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("Total Mark Obtained:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingLeft = 5f,
                FixedHeight = 20f
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).MinMark.ToString(), _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingLeft = 5f,
                FixedHeight = 20f
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("Total Mark Obtainable", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingLeft = 5f,
                FixedHeight = 20f
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).MaxMark.ToString(), _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingLeft = 5f,
                FixedHeight = 20f
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("Number of Subject Offered", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingLeft = 5f,
                FixedHeight = 20f
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).No_Of_Sbj.ToString(), _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingLeft = 5f,
                FixedHeight = 20f
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("Average Performance", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingLeft = 5f,
                FixedHeight = 20f
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).AVGPer.ToString(), _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingLeft = 5f,
                FixedHeight = 20f
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("Overall Grade of Student's Performance", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingLeft = 5f,
                FixedHeight = 20f
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).YouthClub, _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingLeft = 5f,
                FixedHeight = 20f
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        #endregion

        #region [Comments And Signature]

        private PdfPTable AddComment(int _stdid)
        {
            int maxColumn = 1;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95,
                SpacingBefore = 10f
            };
            
            _pdfPCell = new PdfPCell(this.CommentsTeacher(_stdid))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                PaddingBottom = 5f,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        private PdfPTable CommentsTeacher(int _stdid)
        {
            int maxColumn = 2;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95,
                SpacingBefore = 3f
            };
            pdfPtable.SetWidths(new float[] { 20f, 55f });

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 1);

            _pdfPCell = new PdfPCell(new Phrase("Class Teacher's Comment: ", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).Comments_Teacher, _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        private PdfPTable Signature()
        {
            LineDash solid = new SolidLine();
            int maxColumn = 1;
            PdfPTable pdfPtable = new(maxColumn);

            if (_signPrincipal != null)
            {
                Image signPrincipal = Image.GetInstance(_signPrincipal);
                signPrincipal.ScaleAbsolute(40f, 40f);
                _pdfPCell = new PdfPCell(signPrincipal)
                {
                    Colspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = PdfPCell.NO_BORDER,
                    PaddingTop = 2f
                };
                pdfPtable.AddCell(_pdfPCell);
            }
            else
            {
                _pdfPCell = new PdfPCell
                {
                    Colspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = PdfPCell.NO_BORDER,
                    PaddingTop = 2f
                };
                pdfPtable.AddCell(_pdfPCell);
            }

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 2);

            _pdfPCell = new PdfPCell(new Phrase("Principal's Sign. and Date", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,
                CellEvent = new TableBorders(null, null, solid, null)
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        #endregion

    }
}
