using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using WebAppAcademics.Server.Interfaces;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsBroadsheetController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        SwitchModel _switch = new();

        public ResultsBroadsheetController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region [Section - Variables Declaration]
        int _reporttype { get; set; }
        string _schName { get; set; }
        string _schAddress { get; set; }
        string _academicSession { get; set; }
        string _broadsheettitle { get; set; }
        #endregion

        #region [Section - Declaration]
        Document _document;
        PdfWriter writer;
        PdfPCell _pdfPCell;
        Font _fontStyle;
        MemoryStream _memoryStream = new();
        List<ACDStudentsMarksCognitive> _studentMarks = new();
        List<ScoreRanking> _scoreRanking = new();
        #endregion

        [HttpGet]
        [Route("GetBroadsheet/{id}/{termid}/{schid}/{classid}/{broadsheettitle}")]
        public async Task<IActionResult> GetBroadsheet(int id, int termid, int schid, int classid, string broadsheettitle)
        {
            try
            {
                var dataSchInfo = await unitOfWork.SETSchInformation.GetByIdAsync(1);
                _switch.SwitchID = id;
                _switch.TermID = termid;
                _switch.SchID = schid;
                _switch.ClassID = classid;
                _reporttype = id;

                await LoadData(id, termid, schid, classid, broadsheettitle);

                _document = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
                writer = PdfWriter.GetInstance(_document, _memoryStream);

                _document.Open();

                //Broadsheet Header Info
                _document.Add(BroadsheetHeader());

                //Broadsheet Summary
                _document.Add(BroadsheetSummary());

                _document.Close();
            }
            catch(Exception ex)
            {
                var msg = ex.Message;
            }

            return File(_memoryStream.ToArray(), "application/pdf");
        }

        async Task LoadData(int id, int termid, int schid, int classid, string broadsheettitle)
        {
            var dataSchInfo = await unitOfWork.SETSchInformation.GetByIdAsync(1);
            _schName = dataSchInfo.SchName;
            _schAddress = dataSchInfo.SchAddress;           
            _broadsheettitle = broadsheettitle;

            var dataAcademicSession = await unitOfWork.SETSchSessions.GetByIdAsync(termid);
            _academicSession = dataAcademicSession.AcademicSession;

            _switch.SwitchID = id;
            _switch.TermID = termid;
            _switch.SchID = schid;
            _switch.ClassID = classid;
            var dataStudentMarks = await unitOfWork.CognitiveMarkEntry.GetAllAsync(_switch);
            _studentMarks = dataStudentMarks.ToList();
        }

        #region [Section - Table Header]
        private PdfPTable BroadsheetHeader()
        {
            int maxColumn = 1;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95
            };
            pdfPtable.SetWidths(new float[] { 150f });

            _fontStyle = FontFactory.GetFont("Tahoma", 16f, 1);

            _pdfPCell = new PdfPCell(new Phrase(_schName, _fontStyle))
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 2);

            _pdfPCell = new PdfPCell(new Phrase(_schAddress, _fontStyle))
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 12f, 1);

            _pdfPCell = new PdfPCell(new Phrase(_broadsheettitle, _fontStyle))
            {
                Colspan = maxColumn,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Tahoma", 12f, 1);

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
        #endregion

        #region [Section - Table Body]

        private PdfPTable BroadsheetSummary()
        {
            if (_reporttype == 1)
            {
                ScoreRankingMidTerm();
            }
            else if (_reporttype == 2)
            {
                ScoreRanking();
            }
            else if (_reporttype == 3)
            {
                ScoreRankingCheckPointIGCSE();
            }
           
            int sn = 0;

            int maxColumn = 7;
            PdfPTable pdfPtable = new(maxColumn)
            {
                WidthPercentage = 95,
                SpacingBefore = 20f
            };
            pdfPtable.SetWidths(new float[] { 1.5f, 6f, 15f, 4f, 3f, 4f, 4f });

            #region [Header]
            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);

            _pdfPCell = new PdfPCell(new Phrase("S/N", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                //PaddingLeft = 5f,
                //FixedHeight = 25f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("ADMISSION NO.", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                PaddingLeft = 5f,
                //FixedHeight = 25f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("STUDENT NAME", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                PaddingLeft = 5f,
                //FixedHeight = 25f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("NO. OF SUBJECTS", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                //PaddingLeft = 5f,
                //FixedHeight = 25f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("TOTAL", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                PaddingRight = 5f,
                //FixedHeight = 25f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("AVERAGE", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                PaddingRight = 5f,
                //FixedHeight = 25f
            };
            pdfPtable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("POSITION", _fontStyle))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                PaddingRight = 5f,
                //FixedHeight = 25f
            };
            pdfPtable.AddCell(_pdfPCell);

            pdfPtable.CompleteRow();
            #endregion

            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 0);
            foreach (var item in _scoreRanking)
            {
                sn++;
                string admissionNo = _studentMarks.FirstOrDefault(s => s.STDID == item.STDID).AdmissionNo;
                string studentName = _studentMarks.FirstOrDefault(s => s.STDID == item.STDID).StudentName;

                _pdfPCell = new PdfPCell(new Phrase(sn.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    FixedHeight = 20f
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(admissionNo, _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    PaddingLeft = 5f,
                    FixedHeight = 20f
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(studentName, _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    PaddingLeft = 5f,
                    FixedHeight = 20f
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.SubjectCount.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    FixedHeight = 20f
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.MarkObtained.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    PaddingRight = 5f,
                    FixedHeight = 20f
                };
                pdfPtable.AddCell(_pdfPCell);                             

                _pdfPCell = new PdfPCell(new Phrase(item.AverageMark.ToString("#0.00"), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    PaddingRight = 5f,
                    FixedHeight = 20f
                };
                pdfPtable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(item.Position.ToString(), _fontStyle))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    FixedHeight = 20f
                };
                pdfPtable.AddCell(_pdfPCell);

                pdfPtable.CompleteRow();
            }

            return pdfPtable;
        }
        



        #endregion

        #region [Section - Broadsheet Summary Computation]
        void ScoreRanking()
        {
            _scoreRanking.Clear();

            var scoreList = _studentMarks.GroupBy(s => s.STDID)
                                    .OrderByDescending(avg => avg.Average(u => (
                                    Math.Round(u.Mark_CA1, MidpointRounding.AwayFromZero) +
                                    Math.Round(u.Mark_CA2, MidpointRounding.AwayFromZero) +
                                    Math.Round(u.Mark_CA3, MidpointRounding.AwayFromZero) +
                                    Math.Round(u.Mark_CBT, MidpointRounding.AwayFromZero) +
                                    Math.Round(u.Mark_Exam, MidpointRounding.AwayFromZero))))
                                    .Select((avg, i) => new
                                    {
                                        STDID = avg.Key,
                                        SubjectCount = avg.Count(),
                                        MarkObtained = avg.Sum(u =>
                                        (Math.Round(u.Mark_CA1, MidpointRounding.AwayFromZero) +
                                        Math.Round(u.Mark_CA2, MidpointRounding.AwayFromZero) +
                                        Math.Round(u.Mark_CA3, MidpointRounding.AwayFromZero) +
                                        Math.Round(u.Mark_CBT, MidpointRounding.AwayFromZero) +
                                        Math.Round(u.Mark_Exam, MidpointRounding.AwayFromZero))),
                                        MarkObtainable = avg.Count() * 100,
                                        AverageMark = avg.Average(u =>
                                        (Math.Round(u.Mark_CA1, MidpointRounding.AwayFromZero) +
                                        Math.Round(u.Mark_CA2, MidpointRounding.AwayFromZero) +
                                        Math.Round(u.Mark_CA3, MidpointRounding.AwayFromZero) +
                                        Math.Round(u.Mark_CBT, MidpointRounding.AwayFromZero) +
                                        Math.Round(u.Mark_Exam, MidpointRounding.AwayFromZero))),
                                        Position = i + 1
                                    }).ToList();

            var distinctScores = scoreList.Select(avg => avg.AverageMark).Distinct().ToList();
            distinctScores.Sort((a, b) => b.CompareTo(a));

            int rank = 1;
            foreach (var value in distinctScores)
            {
                foreach (var item in scoreList)
                {
                    if (item.AverageMark == value)
                    {
                        _scoreRanking.Add(new ScoreRanking()
                        {
                            Position = rank,
                            STDID = item.STDID,
                            SubjectCount = item.SubjectCount,
                            MarkObtained = Convert.ToInt32(item.MarkObtained),
                            MarkObtainable = Convert.ToInt32(item.MarkObtainable),
                            AverageMark = item.AverageMark,
                        });
                    }
                }

                rank++;
            }
        }

        void ScoreRankingMidTerm()
        {
            _scoreRanking.Clear();

            var scoreList = _studentMarks.GroupBy(s => s.STDID)
                                    .OrderByDescending(avg => avg.Average(u => (
                                    Math.Round(u.Mark_MidCBT, MidpointRounding.AwayFromZero) +
                                    Math.Round(u.Mark_Mid, MidpointRounding.AwayFromZero))))
                                    .Select((avg, i) => new
                                    {
                                        STDID = avg.Key,
                                        SubjectCount = avg.Count(),
                                        MarkObtained = avg.Sum(u =>
                                        (Math.Round(u.Mark_MidCBT, MidpointRounding.AwayFromZero) +
                                        Math.Round(u.Mark_Mid, MidpointRounding.AwayFromZero))),
                                        MarkObtainable = avg.Count() * 100,
                                        AverageMark = avg.Average(u =>
                                        (Math.Round(u.Mark_MidCBT, MidpointRounding.AwayFromZero) +
                                        Math.Round(u.Mark_Mid, MidpointRounding.AwayFromZero))),
                                        Position = i + 1
                                    }).ToList();

            var distinctScores = scoreList.Select(avg => avg.AverageMark).Distinct().ToList();
            distinctScores.Sort((a, b) => b.CompareTo(a));

            int rank = 1;
            foreach (var value in distinctScores)
            {
                foreach (var item in scoreList)
                {
                    if (item.AverageMark == value)
                    {
                        _scoreRanking.Add(new ScoreRanking()
                        {
                            Position = rank,
                            STDID = item.STDID,
                            SubjectCount = item.SubjectCount,
                            MarkObtained = Convert.ToInt32(item.MarkObtained),
                            MarkObtainable = Convert.ToInt32(item.MarkObtainable),
                            AverageMark = item.AverageMark,
                        });
                    }
                }

                rank++;
            }
        }

        void ScoreRankingCheckPointIGCSE()
        {
            _scoreRanking.Clear();

            var scoreList = _studentMarks.GroupBy(s => s.STDID)
                                    .OrderByDescending(avg => avg.Average(u => (
                                    Math.Round(u.Mark_ICGC, MidpointRounding.AwayFromZero))))
                                    .Select((avg, i) => new
                                    {
                                        STDID = avg.Key,
                                        SubjectCount = avg.Count(),
                                        MarkObtained = avg.Sum(u =>
                                        (Math.Round(u.Mark_ICGC, MidpointRounding.AwayFromZero))),
                                        MarkObtainable = avg.Count() * 100,
                                        AverageMark = avg.Average(u =>
                                        (Math.Round(u.Mark_ICGC, MidpointRounding.AwayFromZero))),
                                        Position = i + 1
                                    }).ToList();

            var distinctScores = scoreList.Select(avg => avg.AverageMark).Distinct().ToList();
            distinctScores.Sort((a, b) => b.CompareTo(a));

            int rank = 1;
            foreach (var value in distinctScores)
            {
                foreach (var item in scoreList)
                {
                    if (item.AverageMark == value)
                    {
                        _scoreRanking.Add(new ScoreRanking()
                        {
                            Position = rank,
                            STDID = item.STDID,
                            SubjectCount = item.SubjectCount,
                            MarkObtained = Convert.ToInt32(item.MarkObtained),
                            MarkObtainable = Convert.ToInt32(item.MarkObtainable),
                            AverageMark = item.AverageMark,
                        });
                    }
                }

                rank++;
            }
        }


        #endregion

    }
}
