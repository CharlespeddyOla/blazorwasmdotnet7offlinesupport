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
    public class ResultsCheckPointController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        SwitchModel _switch = new();

        public ResultsCheckPointController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        #region [Section - Variables Declaration]
        string _resultTitle { get; set; }
        byte[] _schLogo { get; set; }
        string _schName { get; set; }
        string _schSlogan { get; set; }
        string _schAddress1 { get; set; }       
        string _schPhones { get; set; }
        string _schEmail { get; set; }
        string _academicSession { get; set; }
        byte[] _studentPhoto { get; set; }
        byte[] _signPrincipal { get; set; }
        byte[] _cambridgeLogo { get; set; }
        #endregion

        #region [Section - Declaration]
        Document _document;
        PdfWriter writer { get; set; }
        PdfPCell _pdfPCell;
        Font _fontStyle;
        MemoryStream _memoryStream = new();
        List<ACDStudentsResultCognitive> _studentResults = new();
        List<ACDSettingsGradeCheckPoint> _checkpointGradeList = new();
        List<ACDSettingsGradeIGCSE> _igcseGradeList = new();
        #endregion

        [HttpGet]
        [Route("GetPDFResults/{previewid}/{stdid}")]
        public async Task<IActionResult> GetPDFResults(int previewid, int stdid)
        {
            try
            {
                _switch.SwitchID = 1;
                var data = await unitOfWork.CognitiveResults.GetAllAsync(_switch);
                var dataCheckPointGrades = await unitOfWork.CheckPointGradeSettings.GetAllAsync(_switch);
                var dataIGCSEGrades = await unitOfWork.IGCSEGradeSettings.GetAllAsync(_switch);

                if (stdid == 0)
                {
                    _studentResults = data.ToList();
                }
                else
                {
                    _studentResults = data.Where(s => s.STDID == stdid).ToList();
                }

                _checkpointGradeList = dataCheckPointGrades.ToList();
                _igcseGradeList = dataIGCSEGrades.ToList();
                ResultHeaderDetails();

                var studentIDs = _studentResults.Select(s => s.STDID).Distinct();

                _document = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
                writer = PdfWriter.GetInstance(_document, _memoryStream);

                FooterSignAndInfo footer = new(Signature(), _schSlogan);
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
            _schPhones = _studentResults.FirstOrDefault().SchPhones;
            _schEmail = _studentResults.FirstOrDefault().SchEmails;
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
                PaddingBottom = 10f,
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
                    PaddingLeft = 10f,
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
                    PaddingLeft = 10f,
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
            PdfPTable pdfPtable = new PdfPTable(maxColumn);
            Image img = Image.GetInstance(_schLogo);
            img.ScaleAbsolute(60f, 60f);

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
            PdfPTable pdfPtable = new PdfPTable(maxColumn);

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

            _pdfPCell = new PdfPCell(new Phrase(_schAddress1, _fontStyle))
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();                      

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);

            _pdfPCell = new PdfPCell(new Phrase(_schPhones, _fontStyle))
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);

            _pdfPCell = new PdfPCell(new Phrase(_schEmail, _fontStyle))
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _pdfPCell = new PdfPCell(this.AddCambridgeLogo())
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 12f, 1);

            _pdfPCell = new PdfPCell(new Phrase(_resultTitle, _fontStyle))
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
            PdfPTable pdfPtable = new PdfPTable(maxColumn);
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

        private PdfPTable AddCambridgeLogo()
        {
            GetImageFile getImgFile = new("ChcekPointIGCSELogo.png");
            int maxColumn = 1;
            PdfPTable pdfPtable = new PdfPTable(maxColumn);
            Image img = Image.GetInstance(getImgFile.ImageFile());
            img.ScaleAbsolute(250f, 70f);

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
                WidthPercentage = 95,
                SpacingBefore = 5f
            };
            pdfPtable.SetWidths(new float[] { 10f, 20f, 20f, 11f });
            LineDash solid = new SolidLine();
            
            _pdfPCell = new PdfPCell(this.PreSubHeaderTable(_stdid))
            {
                Colspan = 4,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,
                //PaddingTop = 35f,
                FixedHeight = 30f,
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
            pdfPtable.SetWidths(new float[] { 5f, 12f, 5f, 10f });

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("Student Name:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingLeft = 5f,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).FullName, _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, Font.BOLD, BaseColor.Black);

            _pdfPCell = new PdfPCell(new Phrase("Class:", _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 0);

            _pdfPCell = new PdfPCell(new Phrase(_studentResults.FirstOrDefault(s => s.STDID == _stdid).ClassName, _fontStyle))
            {
                Colspan = 1,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
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
                case 9:
                    _pdfPCell = new PdfPCell(this.CheckPointGrades())
                    {
                        Colspan = maxColumn,
                        //Rowspan = 2,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        Border = PdfPCell.NO_BORDER
                    };
                    pdfPtable.AddCell(_pdfPCell);

                    pdfPtable.CompleteRow();
                    break;
                case 10:
                    _pdfPCell = new PdfPCell(this.IGCSEGrades())
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
            int maxColumn = 3;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 40
            };
            pdfPtable.SetWidths(new float[] { 10f, 3f, 2.5f });

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 1);

            #region [Table Header]
            
            _pdfPCell = new PdfPCell(new Phrase("SUBJECT", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                PaddingLeft = 5f,
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("STUDENT SCORE", _fontStyle))
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

            pdfPtable.CompleteRow();

            #endregion

            #region [Table Body]

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 0);

            foreach (var item in _studentResults.Where(s => s.STDID == _stdid).OrderBy(s => s.SortID))
            {               
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

        private PdfPTable CheckPointGrades()
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

            foreach (var item in _checkpointGradeList)
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

        private PdfPTable IGCSEGrades()
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

            foreach (var item in _igcseGradeList)
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

        #region [Comments And Signature]

        private PdfPTable AddComment(int _stdid)
        {
            int maxColumn = 2;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95
            };
            pdfPtable.SpacingBefore = 35f;

            _pdfPCell = new PdfPCell(this.CommentsPrincipal(_stdid))
            {
                Colspan = 2,
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
            pdfPtable.SetWidths(new float[] { 6f, 55f });

            _fontStyle = FontFactory.GetFont("Tahoma", 11f,1);

            _pdfPCell = new PdfPCell(new Phrase("Remark: ", _fontStyle))
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
