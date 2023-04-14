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
    public class ResultsMockController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        SwitchModel _switch = new();

        public ResultsMockController(IUnitOfWork unitOfWork)
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
        string _resultGrades { get; set; }
        byte[] _signPrincipal { get; set; }
        #endregion

        #region [Section - Declaration]
        Document _document;
        PdfWriter writer;
        PdfPCell _pdfPCell;
        Font _fontStyle;
        MemoryStream _memoryStream = new();
        List<ACDStudentsResultCognitive> _studentResults = new();
        List<ACDSettingsGradeOthers> _juniorMockGradeList = new();
        List<ACDSettingsGradeMock> _seniorMockGradeList = new();
        #endregion


        [HttpGet]
        [Route("GetPDFResults/{previewid}/{stdid}")]
        public async Task<IActionResult> GetPDFResults(int previewid, int stdid)
        {
            try
            {
                _switch.SwitchID = 1;
                var data = await unitOfWork.CognitiveResults.GetAllAsync(_switch);
                var dataJuniorMockGrades = await unitOfWork.OtherGradeSettings.GetAllAsync(_switch);
                var dataSeniorMockGrades = await unitOfWork.MockGradeSettings.GetAllAsync(_switch);

                if (stdid == 0)
                {
                    _studentResults = data.ToList();
                }
                else
                {
                    _studentResults = data.Where(s => s.STDID == stdid).ToList();
                }

                _juniorMockGradeList = dataJuniorMockGrades.ToList();
                _seniorMockGradeList = dataSeniorMockGrades.ToList();

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
                    _document.Add(ResultBody(previewid, item));
                    _document.Add(AddScoreChartTable(item));
                    _document.Add(AddComment(item));
                }

                _document.Close();
            }
            catch (Exception ex)
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
        private PdfPTable ResultBody(int _previewid, int _stdid)
        {
            int maxColumn = 3;
            PdfPTable pdfPtable = new(maxColumn);
            pdfPtable.WidthPercentage = 95;
            pdfPtable.SetWidths(new float[] { 25f, 15f, 35f });
            pdfPtable.SpacingBefore = 15f;

            _pdfPCell = new PdfPCell(this.LeftResultBody(_stdid))
            {
                Colspan = 2,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(this.RightResultBody(_previewid))
            {
                Colspan = maxColumn - 2,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingLeft = 75f,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        private PdfPTable LeftResultBody(int _stdid)
        {
            int maxColumn = 2;
            PdfPTable pdfPtable = new(maxColumn);
            pdfPtable.SetWidths(new float[] { 25f, 15f });

            _pdfPCell = new PdfPCell(this.MainResultBody(_stdid))
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _pdfPCell = new PdfPCell()
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER,
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            return pdfPtable;
        }

        private PdfPTable RightResultBody(int _previewid)
        {
            int maxColumn = 1;
            PdfPTable pdfPtable = new(maxColumn);
            pdfPtable.WidthPercentage = 95;

            switch (_previewid)
            {
                case 4:
                    _pdfPCell = new PdfPCell(this.JnuiorGrades())
                    {
                        Colspan = maxColumn,
                        //Rowspan = 2,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        Border = PdfPCell.NO_BORDER
                    };
                    pdfPtable.AddCell(_pdfPCell);

                    pdfPtable.CompleteRow();
                    break;
                case 5:
                    _pdfPCell = new PdfPCell(this.SeniorGrades())
                    {
                        Colspan = maxColumn,
                        //Rowspan = 2,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        Border = PdfPCell.NO_BORDER
                    };
                    pdfPtable.AddCell(_pdfPCell);

                    pdfPtable.CompleteRow();
                    break;
            }

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

        private PdfPTable MainResultBody(int _stdid)
        {
            int maxColumn = 4;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 40
            };
            pdfPtable.SetWidths(new float[] { 2f, 10f, 3f, 2f });
           
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


            _pdfPCell = new PdfPCell(new Phrase("Student Score", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Grade", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM
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

                pdfPtable.CompleteRow();
            }

            #endregion

            return pdfPtable;
        }

        private PdfPTable JnuiorGrades()
        {
            int maxColumn = 3;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95
            };
            pdfPtable.SetWidths(new float[] { 3f, 3f, 5f });
           
            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 1);

            #region [Table Header]
            _pdfPCell = new PdfPCell(new Phrase("SCORE RANGE", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("GRADE", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("REMARK", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();
            #endregion

            #region [Table Body]

            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 0);

            foreach (var item in _juniorMockGradeList)
            {
                _pdfPCell = new PdfPCell(new Phrase(item.LowerGrade.ToString("#0")  + " - " + item.HigherGrade.ToString("#0"), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.GradeLetter, _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.GradeRemark, _fontStyle))
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

        private PdfPTable SeniorGrades()
        {
            int maxColumn = 3;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95
            };
            pdfPtable.SetWidths(new float[] { 3f, 3f, 5f });

            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 1);

            #region [Table Header]
            _pdfPCell = new PdfPCell(new Phrase("SCORE RANGE", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("GRADE", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("REMARK", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_BOTTOM
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();
            #endregion

            #region [Table Body]

            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 0);

            foreach (var item in _seniorMockGradeList)
            {
                _pdfPCell = new PdfPCell(new Phrase(item.LowerGrade.ToString("#0") + " - " + item.HigherGrade.ToString("#0"), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.GradeLetter, _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = BaseColor.White
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.GradeRemark, _fontStyle))
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

        #endregion

        #region [Score Chart]
        private PdfPTable AddScoreChartTable(int _stdid)
        {
            int maxColumn = 1;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 65
            };
            pdfPtable.SpacingBefore = 25f;

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
                _pdfPCell = new PdfPCell(new Phrase(item.SubjectCode, _fontStyle))
                {
                    Colspan = 1,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Border = PdfPCell.NO_BORDER,
                    Rotation = -270,
                    PaddingLeft = 3f
                };
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

        private PdfPTable AddComment(int _stdid)
        {
            int maxColumn = 1;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95
            };
            pdfPtable.SpacingBefore = 25f;

            _pdfPCell = new PdfPCell(this.CommentsPrincipal(_stdid))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
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
            pdfPtable.SetWidths(new float[] { 15f, 55f });

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 1);

            _pdfPCell = new PdfPCell(new Phrase("Principal's Comments: ", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).Comments_Principal, _fontStyle))
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
