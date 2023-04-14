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
    public class ResultsTermEndController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        SwitchModel _switch = new();
        int _schTermID { get; set; }

        public ResultsTermEndController(IUnitOfWork unitOfWork)
        {
           this.unitOfWork = unitOfWork;
        }

        #region [Section - Variables Declaration]
        string _resultTitle { get; set; }
        byte[] _schLogo { get; set; }
        byte[] _checkBox { get; set; }
        string _schName { get; set; }
        string _schSlogan { get; set; }
        string _schAddress1 { get; set; }
        string _schAddress2 { get; set; }
        string _schPhones { get; set; }
        string _academicSession { get; set; }
        byte[] _studentPhoto { get; set; }
        string _caTitle { get; set; }
        string _examTitle { get; set; }
        string _resultGrades { get; set; }
        byte[] _signTeacher { get; set; }
        byte[] _signPrincipal { get; set; }
        #endregion

        #region [Section - Declaration]
        Document _document;
        PdfWriter writer;
        PdfPCell _pdfPCell;
        Font _fontStyle;
        MemoryStream _memoryStream = new();
        List<ACDStudentsResultCognitive> _studentResults = new();
        List<ACDStudentsResultAssessmentBool> _accessmentResults = new();
        List<ACDSettingsRating> _ratingList = new();
        #endregion

        [HttpGet]
        [Route("GetPDFResults/{schtermid}/{stdid}")]
        public async Task<IActionResult> GetPDFResults(int schtermid, int stdid)
        {
            try
            {
                _switch.SwitchID = 1;
                var data = await unitOfWork.CognitiveResults.GetAllAsync(_switch);
                var accessmentData = await unitOfWork.OtherMarksResults.GetAllAsync(_switch);

                if (stdid == 0)
                {
                    _studentResults = data.ToList();
                    _accessmentResults = accessmentData.ToList();
                }
                else
                {
                    _studentResults = data.Where(s => s.STDID == stdid).ToList();
                    _accessmentResults = accessmentData.Where(s => s.STDID == stdid).ToList();
                }

                var ratingData = await unitOfWork.RatingSettings.GetAllAsync(_switch);
                var studentIDs = _studentResults.Select(s => s.STDID).Distinct();
                var reportHeaderFooter = await unitOfWork.ResultHeaderFooterSettings.GetByIdAsync(1);

                _ratingList = ratingData.ToList();
                _caTitle = reportHeaderFooter.HeaderCA;
                _examTitle = reportHeaderFooter.HeaderExam;
                _resultGrades = reportHeaderFooter.Footer;

                ResultHeaderDetails();

                _document = new Document(PageSize.A4, 10f, 10f, 5f, 5f);
                writer = PdfWriter.GetInstance(_document, _memoryStream);

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
                    switch (schtermid)
                    {
                        case 1: //First Term Results
                            _document.Add(FirstTermResultBody(item));
                            break;
                        case 2: //Second Term Results
                            _document.Add(SecondTermResultBody(item));
                            break;
                        case 3: //Third Term Results
                            _document.Add(ThirdTermResultBody(item));
                            break;
                    }
                    _document.Add(ResultGrades());

                    //Add Footer Table Here
                    _document.Add(ResultFooter(item));
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
            _signTeacher = _studentResults.FirstOrDefault().ClassTeacherSign;
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
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95
            };
            pdfPtable.SetWidths(new float[] { 10f, 20f, 20f, 11f });
            LineDash solid = new SolidLine();

            _pdfPCell = new PdfPCell(this.PreSubHeaderTable(_stdid))
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

        private PdfPTable PreSubHeaderTable(int _stdid)
        {
            int maxColumn = 4;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95
            };
            pdfPtable.SetWidths(new float[] { 8f, 12f, 5f, 5f });

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("Admission No.:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).StudentNo, _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("Class:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).ClassName, _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("Student Name:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).FullName, _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("No. In Class:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).No_In_Class.ToString(), _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("Class Teacher's Name:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).ClassTeacher, _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("No. of Subject:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).No_Of_Sbj.ToString(), _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("No. of Times School Opened:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).Attendance.ToString(), _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("Percentage:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).AVGPer.ToString(), _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("No. of Times Present:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 0);

            int timePresent = _studentResults.FirstOrDefault(s => s.STDID == _stdid).Attendance - _studentResults.FirstOrDefault(s => s.STDID == _stdid).DaysAbsent;
            _pdfPCell = new PdfPCell(new Phrase(timePresent.ToString(), _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("Sex:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).Gender, _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("Next Term Begins:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).NextTermBegins?.ToString("dd-MMM-yyyy"), _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("Overall Score:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).OverAllScore.ToString("#,#0"), _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("Next Term Ends:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).NextTermEnds?.ToString("dd-MMM-yyyy"), _fontStyle))
            {
                Colspan = 3,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();


            return pdfPtable;
        }

        #endregion

        #region [Section - Result Body]

        private PdfPTable FirstTermResultBody(int _stdid)
        {
            int maxColumn = 11;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95
            };
            pdfPtable.SetWidths(new float[] { 2.5f, 10f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 10f });
            pdfPtable.SpacingBefore = 3f;

            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 1);

            #region [Table Header]
            _pdfPCell = new PdfPCell(new Phrase("Code", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM                
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Subject", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                PaddingLeft = 5f,
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("C.A.", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Exam Score", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("1st Term Score", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Grade", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Position", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Highest Score In Class", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                FixedHeight  = 10f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Lowest Score In Class", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                FixedHeight = 10f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Class Average", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Teacher's Remarks", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            #endregion

            #region [Table Sub Header]
            _pdfPCell = new PdfPCell(new Phrase("Max. Obtainable", _fontStyle))
            {
                Colspan = 2,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            pdfPtable.AddCell(_pdfPCell);
               
            _pdfPCell = new PdfPCell(new Phrase(_caTitle, _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(_examTitle, _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("%", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("G", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("P", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("H", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("L", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(" AVG", _fontStyle))
            {
                Colspan = 2,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            #endregion

            #region [Table Body]

            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 0);

            foreach (var item in _studentResults.Where(s => s.STDID == _stdid).OrderBy(s => s.SortID))
            {
                _pdfPCell = new PdfPCell(new Phrase(item.SubjectCode, _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.Subject, _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    PaddingLeft = 5f,
                    BackgroundColor = BaseColor.White
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

                _pdfPCell = new PdfPCell(new Phrase(item.POS.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.MaxMark.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.MinMark.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.ClassAvg.ToString("#0.00"), _fontStyle))
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
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                pdfPtable.CompleteRow();
            }

            #endregion

            return pdfPtable;
        }

        private PdfPTable SecondTermResultBody(int _stdid)
        {
            int maxColumn = 12;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95
            };
            pdfPtable.SetWidths(new float[] { 2.5f, 10f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 8f });
            pdfPtable.SpacingBefore = 3f;

            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 1);

            #region [Table Header]
            _pdfPCell = new PdfPCell(new Phrase("Code", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Subject", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                PaddingLeft = 5f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("1st Term Score", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("C.A.", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Exam Score", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("2nd Term Score", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Grade", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Position", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Highest Score In Class", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                FixedHeight = 10f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Lowest Score In Class", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                FixedHeight = 10f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Class Average", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Teacher's Remarks", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            #endregion

            #region [Table Sub Header]
            _pdfPCell = new PdfPCell(new Phrase("Max. Obtainable", _fontStyle))
            {
                Colspan = 2,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("100", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);
                    
            _pdfPCell = new PdfPCell(new Phrase(_caTitle, _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(_examTitle, _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("%", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("G", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("P", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("H", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("L", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(" AVG", _fontStyle))
            {
                Colspan = 2,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            #endregion

            #region [Table Body]

            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 0);

            foreach (var item in _studentResults.Where(s => s.STDID == _stdid).OrderBy(s => s.SortID))
            {
                _pdfPCell = new PdfPCell(new Phrase(item.SubjectCode, _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.Subject, _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    PaddingLeft = 5f,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.FTerm.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
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

                _pdfPCell = new PdfPCell(new Phrase(item.POS.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.MaxMark.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.MinMark.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.ClassAvg.ToString("#0.00"), _fontStyle))
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
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                pdfPtable.CompleteRow();
            }

            #endregion

            return pdfPtable;
        }

        private PdfPTable ThirdTermResultBody(int _stdid)
        {
            int maxColumn = 14;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95
            };
            pdfPtable.SetWidths(new float[] { 2.5f, 10f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 4f });
            pdfPtable.SpacingBefore = 3f;

            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 1);

            #region [Table Header]
            _pdfPCell = new PdfPCell(new Phrase("Code", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Subject", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                PaddingLeft = 5f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("1st Term Score", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("2nd Term Score", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("C.A.", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Exam Score", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("3rd Term Score", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Cumulative Score", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Grade", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Position", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Highest Score In Class", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                FixedHeight = 10f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Lowest Score In Class", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                FixedHeight = 10f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Class Average", _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Teacher's Remarks", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            #endregion

            #region [Table Sub Header]
            _pdfPCell = new PdfPCell(new Phrase("Max. Obtainable", _fontStyle))
            {
                Colspan = 2,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("100", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("100", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(_caTitle, _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(_examTitle, _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("%", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("%", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("G", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("P", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("H", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("L", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(" AVG", _fontStyle))
            {
                Colspan = 2,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            #endregion

            #region [Table Body]

            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 0);

            foreach (var item in _studentResults.Where(s => s.STDID == _stdid).OrderBy(s => s.SortID))
            {
                _pdfPCell = new PdfPCell(new Phrase(item.SubjectCode, _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.Subject, _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    PaddingLeft = 5f,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.FTerm.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.STerm.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
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

                int _FTermCount = 0;
                int _STermCount = 0;
                int _tTermCount = 0;
                int _divisor = 0;
                int _CMS = 0; //Cumulative Score

                decimal _FTermScore = Convert.ToDecimal(item.FTerm);
                decimal _STermScore = Convert.ToDecimal(item.STerm);
                decimal _TTermScore = Convert.ToDecimal(item.TotalMark);

                if (item.FTerm > 0)
                {
                    _FTermCount = 1;
                }

                if (item.STerm > 0)
                {
                    _STermCount = 1;
                }

                if (item.TTerm > 0)
                {
                    _tTermCount = 1;
                }

                _divisor = _FTermCount + _STermCount + _tTermCount;

                if (_divisor > 0)
                {
                    _CMS = Convert.ToInt32(Math.Round((_FTermScore + _STermScore + _TTermScore) / _divisor, MidpointRounding.AwayFromZero));
                }
                else
                {
                    _CMS = 0;
                }

                _pdfPCell = new PdfPCell(new Phrase(_CMS.ToString(), _fontStyle))
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

                _pdfPCell = new PdfPCell(new Phrase(item.POS.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.MaxMark.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.MinMark.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.ClassAvg.ToString("#0.00"), _fontStyle))
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
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                pdfPtable.CompleteRow();
            }

            #endregion

            return pdfPtable;
        }

        private PdfPTable ResultGrades()
        {
            int maxColumn = 4;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95
            };
            pdfPtable.SetWidths(new float[] { 8f, 12f, 5f, 5f });
            pdfPtable.SpacingBefore = 3f;

            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_resultGrades, _fontStyle))
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        #endregion

        #region [Section - Result Footer]

        #region [Footer Body]       
        private PdfPTable ResultFooter(int _stdid)
        {
            int maxColumn = 3;
            PdfPTable pdfPtable = new(maxColumn);
            pdfPtable.WidthPercentage = 95;
            pdfPtable.SetWidths(new float[] { 35f, 25f, 25f });
            pdfPtable.SpacingBefore = 1.5f;

            _pdfPCell = new PdfPCell(this.LeftFooter(_stdid))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(this.RightFooter(_stdid))
            {
                Colspan = maxColumn - 1,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingLeft = 3f,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        private PdfPTable LeftFooter(int _stdid)
        {
            int maxColumn = 1;
            PdfPTable pdfPtable = new(maxColumn);
            pdfPtable.SetWidths(new float[] { 35f });

            _pdfPCell = new PdfPCell(this.PsychoMotor(_stdid))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _pdfPCell = new PdfPCell(this.OtherAccessment(_stdid))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER,
                PaddingTop = 3f
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _pdfPCell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER,
                PaddingTop = 3f
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        private PdfPTable RightFooter(int _stdid)
        {
            int maxColumn = 2;
            PdfPTable pdfPtable = new(maxColumn);
            pdfPtable.WidthPercentage = 95;
            //pdfPtable.SpacingBefore = 3f;

            _pdfPCell = new PdfPCell(this.PreRightFooter(_stdid))
            {
                Colspan = maxColumn,
                Rowspan = 3,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        private PdfPTable PreRightFooter(int _stdid)
        {
            int maxColumn = 2;
            PdfPTable pdfPtable = new(maxColumn);

            _pdfPCell = new PdfPCell(this.AddScoreChartTable(_stdid))
            {
                Colspan = 2,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _pdfPCell = new PdfPCell(this.CommentsTeacher(_stdid))
            {
                Colspan = 2,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _pdfPCell = new PdfPCell(this.CommentsPrincipal(_stdid))
            {
                Colspan = 2,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _pdfPCell = new PdfPCell(this.Signature())
            {
                Colspan = 2,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();


            return pdfPtable;
        }

        #endregion

        #region [Assessments Result]
       
        private PdfPCell CheckBoxImage(int typeId)
        {            
            int maxColumn = 1;

            switch (typeId)
            {
                case 1: //Checked Box
                    GetImageFile chkBoxImage = new("CheckedBox.png");
                    _checkBox = chkBoxImage.ImageFile();
                    break;
                case 2: //UnChecked Box
                    GetImageFile unchkBoxImage = new("UnCheckedBox.png");
                    _checkBox = unchkBoxImage.ImageFile();
                    break;
            }

            Image img = Image.GetInstance(_checkBox);
            img.ScaleAbsolute(10f, 10f);
            _pdfPCell = new PdfPCell(img)
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_TOP,
                PaddingTop = 0.5f
            };

            return _pdfPCell;
        }

        private PdfPTable PsychoMotor(int _stdid)
        {
            int maxColumn = 7;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 35
            };
            pdfPtable.SetWidths(new float[] { 14f, 1.6f, 1.6f, 1.6f, 1.6f, 1.6f, 1.6f });
            pdfPtable.HorizontalAlignment = Element.ALIGN_LEFT;

            _fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);

            #region [Table Header]
            _pdfPCell = new PdfPCell(new Phrase("Activities", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(_ratingList.FirstOrDefault(r => r.RatingID == 1).RatingLevel, _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(_ratingList.FirstOrDefault(r => r.RatingID == 2).RatingLevel, _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(_ratingList.FirstOrDefault(r => r.RatingID == 3).RatingLevel, _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(_ratingList.FirstOrDefault(r => r.RatingID == 4).RatingLevel, _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(_ratingList.FirstOrDefault(r => r.RatingID == 5).RatingLevel, _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(_ratingList.FirstOrDefault(r => r.RatingID == 6).RatingLevel, _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();
            #endregion

            #region [Table Body]

            _fontStyle = FontFactory.GetFont("Tahoma", 7f, 0);

            foreach (var item in _accessmentResults.Where(m => m.SbjClassID == 2 && m.STDID == _stdid))
            {
                _pdfPCell = new PdfPCell(new Phrase(item.Subject, _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_TOP
                };
                pdfPtable.AddCell(_pdfPCell);

                if (item.Five)
                {
                    pdfPtable.AddCell(CheckBoxImage(1));
                }
                else
                {
                    pdfPtable.AddCell(CheckBoxImage(2));
                }

                if (item.Four)
                {
                    pdfPtable.AddCell(CheckBoxImage(1));
                }
                else
                {
                    pdfPtable.AddCell(CheckBoxImage(2));
                }

                if (item.Three)
                {
                    pdfPtable.AddCell(CheckBoxImage(1));
                }
                else
                {
                    pdfPtable.AddCell(CheckBoxImage(2));
                }

                if (item.Two)
                {
                    pdfPtable.AddCell(CheckBoxImage(1));
                }
                else
                {
                    pdfPtable.AddCell(CheckBoxImage(2));
                }

                if (item.One)
                {
                    pdfPtable.AddCell(CheckBoxImage(1));
                }
                else
                {
                    pdfPtable.AddCell(CheckBoxImage(2));
                }

                if (item.Zero)
                {
                    pdfPtable.AddCell(CheckBoxImage(1));
                }
                else
                {
                    pdfPtable.AddCell(CheckBoxImage(2));
                }

                pdfPtable.CompleteRow();
            }

            #endregion

            return pdfPtable;
        }

        private PdfPTable OtherAccessment(int _stdid)
        {
            int maxColumn = 7;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 35
            };
            pdfPtable.SetWidths(new float[] { 14f, 1.6f, 1.6f, 1.6f, 1.6f, 1.6f, 1.6f });
            pdfPtable.HorizontalAlignment = Element.ALIGN_LEFT;

            _fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);

            #region [Table Header]
            _pdfPCell = new PdfPCell(new Phrase("Activities", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(_ratingList.FirstOrDefault(r => r.RatingID == 1).RatingLevel, _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(_ratingList.FirstOrDefault(r => r.RatingID == 2).RatingLevel, _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(_ratingList.FirstOrDefault(r => r.RatingID == 3).RatingLevel, _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(_ratingList.FirstOrDefault(r => r.RatingID == 4).RatingLevel, _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(_ratingList.FirstOrDefault(r => r.RatingID == 5).RatingLevel, _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(_ratingList.FirstOrDefault(r => r.RatingID == 6).RatingLevel, _fontStyle))
            {
                Rotation = -270,
                HorizontalAlignment = Element.ALIGN_BOTTOM,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();
            #endregion

            #region [Table Body]

            _fontStyle = FontFactory.GetFont("Tahoma", 7f, 0);

            foreach (var item in _accessmentResults.Where(m => m.SbjClassID == 3 && m.STDID == _stdid))
            {
                _pdfPCell = new PdfPCell(new Phrase(item.Subject, _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                pdfPtable.AddCell(_pdfPCell);

                if (item.Five)
                {
                    pdfPtable.AddCell(CheckBoxImage(1));
                }
                else
                {
                    pdfPtable.AddCell(CheckBoxImage(2));
                }

                if (item.Four)
                {
                    pdfPtable.AddCell(CheckBoxImage(1));
                }
                else
                {
                    pdfPtable.AddCell(CheckBoxImage(2));
                }

                if (item.Three)
                {
                    pdfPtable.AddCell(CheckBoxImage(1));
                }
                else
                {
                    pdfPtable.AddCell(CheckBoxImage(2));
                }

                if (item.Two)
                {
                    pdfPtable.AddCell(CheckBoxImage(1));
                }
                else
                {
                    pdfPtable.AddCell(CheckBoxImage(2));
                }

                if (item.One)
                {
                    pdfPtable.AddCell(CheckBoxImage(1));
                }
                else
                {
                    pdfPtable.AddCell(CheckBoxImage(2));
                }

                if (item.Zero)
                {
                    pdfPtable.AddCell(CheckBoxImage(1));
                }
                else
                {
                    pdfPtable.AddCell(CheckBoxImage(2));
                }

                pdfPtable.CompleteRow();
            }

            #endregion

            return pdfPtable;
        }

        #endregion

        #region [Score Chart]
        private PdfPTable AddScoreChartTable(int _stdid)
        {
            int maxColumn = 1;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95
            };

            _pdfPCell = new PdfPCell(this.ScoreChart(_stdid))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _pdfPCell = new PdfPCell(this.ChartTitle(_stdid))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        private PdfPTable ScoreChart(int _stdid)
        {
            int maxColumn = 1;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95
            };
            LineDash solid = new SolidLine();

            _pdfPCell = new PdfPCell(this.CreateBarChart(_stdid))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,
                PaddingTop = 5f,
                PaddingBottom = 5f,
                CellEvent = new TableBorders(solid, null, null, solid)
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        private PdfPTable CreateBarChart(int _stdid)
        {
            int count = _studentResults.Where(m => m.STDID == _stdid).Count();
            PdfPTable pdfPtable = new(count);
            float[] columnWidths = new float[count];
            for (int w = 0; w < count; w++)
            {
                columnWidths[w] = 10f;
            }
            pdfPtable.SetWidths(columnWidths);

            int _colortId = 0;

            double y = (54.375 / count) * 10;

            foreach (var item in _studentResults.Where(m => m.STDID == _stdid).OrderBy(s => s.SortID))
            {
                _colortId++;

                _pdfPCell = new PdfPCell(this.BarChart((float)y, (float)item.TotalMark, _colortId))
                {
                    Colspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_BOTTOM,
                    Border = PdfPCell.NO_BORDER,
                    PaddingLeft = 3f
                };
                pdfPtable.AddCell(_pdfPCell);
            }

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        private PdfPTable ChartTitle(int _stdid)
        {
            int count = _studentResults.Where(m => m.STDID == _stdid).Count();
            PdfPTable pdfPtable = new(count);
            float[] columnWidths = new float[count];
            for (int w = 0; w < count; w++)
            {
                columnWidths[w] = 10f;
            }
            pdfPtable.SetWidths(columnWidths);

            _fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);

            foreach (var item in _studentResults.Where(m => m.STDID == _stdid).ToList())
            {
                _pdfPCell = new PdfPCell(new Phrase(item.SubjectCode, _fontStyle));
                _pdfPCell.Colspan = 1;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = PdfPCell.NO_BORDER;
                _pdfPCell.Rotation = -270;
                _pdfPCell.PaddingLeft = 3f;
                pdfPtable.AddCell(_pdfPCell);
            }

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        private PdfPTable BarChart(float _width, float _heigth, int _chartcolorId)
        {
            ChartColors _color = ChartColors.GetChartColorsArray().Where(id => id.ColorId == _chartcolorId).Single();
            int r = _color.Red;
            int b = _color.Blue;
            int g = _color.Green;

            GetImageFile barImage = new("Vertical-Line-PNG.png");
            int maxColumn = 1;
            PdfPTable pdfPtable = new(maxColumn);
            Image img = Image.GetInstance(barImage.ImageFile());
            img.ScaleAbsolute(_width, _heigth);
            BaseColor color = new(r, b, g);

            _pdfPCell = new PdfPCell(img)
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,
                BackgroundColor = color
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        #endregion

        #region [Comments And Signature]
        private PdfPTable CommentsTeacher(int _stdid)
        {
            int maxColumn = 2;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95,
                SpacingBefore = 3f
            };

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);

            _pdfPCell = new PdfPCell(new Phrase("Teacher's Comments: " + _studentResults.FirstOrDefault(s => s.STDID == _stdid).Comments_Teacher, _fontStyle))
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        private PdfPTable CommentsPrincipal(int _stdid)
        {
            int maxColumn = 2;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95,
                SpacingBefore = 3f
            };

            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);

            _pdfPCell = new PdfPCell(new Phrase("Principal's Comments: " + _studentResults.FirstOrDefault(s => s.STDID == _stdid).Comments_Principal, _fontStyle))
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        private PdfPTable Signature()
        {
            int maxColumn = 2;
            PdfPTable pdfPtable = new(maxColumn);

            if (_signTeacher != null)
            {
                Image signTeacher = Image.GetInstance(_signTeacher);
                signTeacher.ScaleAbsolute(35f, 35f);
                _pdfPCell = new PdfPCell(signTeacher)
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

            if (_signPrincipal != null)
            {
                Image signPrincipal = Image.GetInstance(_signPrincipal);
                signPrincipal.ScaleAbsolute(35f, 35f);
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

            _pdfPCell = new PdfPCell(new Phrase("Class Teacher's Sign.", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Principal's Sign.", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        #endregion

        #endregion

    }
}
