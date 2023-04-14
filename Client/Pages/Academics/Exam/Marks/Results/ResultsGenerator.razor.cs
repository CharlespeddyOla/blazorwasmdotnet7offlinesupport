using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Academics.Subjects;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Administration.Students;
using WebAppAcademics.Shared.Models.Settings;
using WebAppAcademics.Client.Extensions;
using Newtonsoft.Json.Converters;
using System.Dynamic;
using MudBlazor;
using Newtonsoft.Json.Linq;
using static MudBlazor.CategoryTypes;

namespace WebAppAcademics.Client.Pages.Academics.Exam.Marks.Results
{
    public partial class ResultsGenerator
    {
        #region [Injection Declaration]
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] IJSRuntime iJSRuntime { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<SETSchSessions> schoolSessionsService { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassList> classService { get; set; }
        [Inject] IAPIServices<ACDSettingsGrade> gradeService { get; set; }
        [Inject] IAPIServices<ACDSettingsRating> ratingService { get; set; }
        [Inject] IAPIServices<ACDSettingsGradeMock> seniorMockGradeService { get; set; }
        [Inject] IAPIServices<ACDSettingsGradeOthers> juniorMockGradeService { get; set; }
        [Inject] IAPIServices<ACDSettingsGradeCheckPoint> checkpointGradeService { get; set; }
        [Inject] IAPIServices<ACDSettingsGradeIGCSE> igcseGradeService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksCognitive> studentMarksCognitiveService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksAssessment> studentOtherMarksService { get; set; }
        [Inject] IAPIServices<ACDStudentsResultCognitive> cognitiveResultsService { get; set; }
        [Inject] IAPIServices<ACDStudentsResultAssessmentBool> assessmentResultsService { get; set; }
        [Inject] IAPIServices<ACDBroadSheet> braodsheetService { get; set; }
        [Inject] IAPIServices<ACDReportCommentMidTerm> midTermCommentService { get; set; }
        [Inject] IAPIServices<ACDReportCommentsTerminal> termEndCommentService { get; set; }
        [Inject] IAPIServices<ACDReportCommentCheckPointIGCSE> checkpointigcseCommentService { get; set; }
        [Inject] IAPIServices<SETSchInformation> schoolInfoService { get; set; }
        [Inject] IAPIServices<SETReports> reportsSelectionService { get; set; }
        [Inject] IAPIServices<ACDSubjects> subjectService { get; set; }
        [Inject] IAPIServices<byte[]> reportServices { get; set; }
        [Inject] IAPIServices<FileChunkDTO> fileUploadServices { get; set; }
        [Inject] IAPIServices<ADMStudents> studentService { get; set; }
        [Inject] IAPIServices<SETSchInformation> _schoolInfoService { get; set; }
        [Inject] IAPIServices<string> stringValueService { get; set; }
        [Inject] IAPIServices<dynamic> bradsheetMarkListService { get; set; }
        [Inject] IAPIServices<bool> deleteService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int menuId { get; set; }
        int SelectedOption { get; set; }
        int termid { get; set; }
        int maxTermID { get; set; }
        int roleid { get; set; }
        int staffid { get; set; }        
        int schoolSession { get; set; }
        int schtermid { get; set; }
        int studentCount { get; set; }
        int schid { get; set; }
        int classid { get; set; }
        int schoolPrincipalID { get; set; }
        int classTeacherID { get; set; }
        int ExpectedAttendance { get; set; }
        int daysAbsent { get; set; }
        int resultTemplateId { get; set; }

        string schTerm { get; set; }
        string academicSession { get; set; }
        string selectedSchool { get; set; }
        string selectedClass { get; set; }
        string selectedStudent { get; set; }
        string GradeLetter { get; set; }
        string GradeRemarks { get; set; }
        string OverAllGradeLetter { get; set; }
        string teacherComment { get; set; }
        string principlaComment { get; set; }
        string resultTitle { get; set; }
        string combinedSubjectsName { get; set; }
        string fileNameDescription { get; set; }

        bool IsFinalYearClass { get; set; }
        bool IsJuniorFinalYearClass { get; set; }
        bool IsCheckPointClass { get; set; }
        bool IsIGCSEClass { get; set; }

        decimal i { get; set; } = 0;
        int j { get; set; } = 0;
        bool IsShow { get; set; } = true;
        string progressbarInfo { get; set; } = string.Empty;

        bool ShowPreview { get; set; } = false;
        bool isLoading { get; set; } = false;
        string loadingmessage { get; set; } = "Waiting for your action...";

        #endregion

        #region [Models Declaration]
        List<SETSchSessions> sessions = new();
        List<ADMSchlList> schools = new();
        List<ADMSchClassList> classList = new();
        List<ACDSettingsGrade> gradeList = new();
        List<ACDSettingsRating> ratingList = new();
        List<ACDSettingsGradeMock> seniorMockGradeList = new();
        List<ACDSettingsGradeOthers> juniorMockGradeList = new();
        List<ACDSettingsGradeCheckPoint> checkpointGradeList = new();
        List<ACDSettingsGradeIGCSE> igcseGradeList = new();
        List<ACDStudentsMarksCognitive> cognitiveMarks = new();
        List<ACDStudentsMarksCognitive> marksListPreviousTerms = new();
        List<ACDStudentsMarksCognitiveFirstTerm> marksListFirstTerm = new();
        List<ACDStudentsMarksCognitiveSecondTerm> marksListSecondTerm = new();
        List<ACDStudentsMarksAssessment> studentOtherMarks = new();
        List<ACDStudentsResultCognitive> studentCognitiveResults = new();
        List<ACDReportCommentsTerminal> commentsTermEnd = new();
        List<ACDReportCommentMidTerm> commentsMidTerm = new();
        List<ACDReportCommentCheckPointIGCSE> commentsCheckPointIGCSE = new();
        List<ACDSubjectsRanking> _subjectRanking = new();
        List<ACDSubjects> subjects = new();
        List<string> fieldnames = new();
        List<dynamic> broadsheetMarkList = new();

        SETSchInformation schInfo = new();
        ACDBroadSheet broadsheet = new();
        ACDStudentsResultCognitive cognitiveResults = new();
        ACDStudentsResultAssessmentBool accessmentResults = new();
        SETReports reportSelection = new();

        #endregion

        protected override async Task OnInitializedAsync()
        {
            _scoreRanking = new List<ScoreRanking>();
            Layout.currentPage = "Student Results";
            _jsModule = await iJSRuntime.InvokeAsync<IJSObjectReference>("import", "./scripts/reports.js");
            menuId = 1;
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            schtermid = await localStorageService.ReadEncryptedItemAsync<int>("calendarid");
            schTerm = await localStorageService.ReadEncryptedItemAsync<string>("schTerm");
            schoolSession = await localStorageService.ReadEncryptedItemAsync<int>("schoolsession");
            academicSession = await localStorageService.ReadEncryptedItemAsync<string>("academicsession");           
            staffid = await localStorageService.ReadEncryptedItemAsync<int>("staffid");
            roleid = await localStorageService.ReadEncryptedItemAsync<int>("roleid");
            await LoadDefaultList();
            await base.OnInitializedAsync();
        }

        #region [Section - Results Generator]

        #region [Section - Load Events]
        async Task LoadDefaultList()
        {
            sessions = await schoolSessionsService.GetAllAsync("Settings/GetAccademicSessions/1/0");
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            maxTermID = sessions.FirstOrDefault(s => s.TermID == sessions.Max(t => t.TermID)).TermID;
            gradeList = await gradeService.GetAllAsync("AcademicsMarkSettings/GetGrades/1");
            ratingList = await ratingService.GetAllAsync("AcademicsMarkSettings/GeRatingSettings/1");
            seniorMockGradeList = await seniorMockGradeService.GetAllAsync("AcademicsMarkSettings/GetMockGrades/1");
            juniorMockGradeList = await juniorMockGradeService.GetAllAsync("AcademicsMarkSettings/GeOtherGradeSettings/1");
            checkpointGradeList = await checkpointGradeService.GetAllAsync("AcademicsMarkSettings/GetCheckPointGrades/1");
            igcseGradeList = await igcseGradeService.GetAllAsync("AcademicsMarkSettings/GetIGCSEGrades/1");
            schInfo = await schoolInfoService.GetByIdAsync("Settings/GetSchoolDetails/", 1);
            ExpectedAttendance = sessions.FirstOrDefault(s => s.TermID == termid).Attendance;
        }

        async Task LoadClassList()
        {
            if (roleid == 1) //Administrator
            {
                classList = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + schid + "/0");
            }
            else if (roleid == 10) //Class Teacher
            {
                var _classlist = await classService.GetAllAsync("AdminSchool/GetClassList/0/" + schid + "/0");
                classList = _classlist.Where(c => c.StaffID == staffid).ToList();
            }
        }
          
        async Task LoadCognitiveMarks(int switchid, int _termid)
        {
            cognitiveMarks = await studentMarksCognitiveService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/" + switchid + "/" +
                                                                  _termid + "/" + schid + "/" + classid + "/0/0/0");
        }

        async Task OnSchoolChanged(IEnumerable<string> e)
        {
            selectedSchool = e.ElementAt(0);
            schid = schools.FirstOrDefault(s => s.School == selectedSchool).SchID;
            schoolPrincipalID = schools.FirstOrDefault(s => s.School == selectedSchool).StaffID;

            classid = 0;
            classTeacherID = 0;
            selectedClass = string.Empty;
            classList.Clear();
            await LoadClassList();
                       
            cognitiveMarks.Clear();
            studentOtherMarks.Clear();
            fieldnames.Clear();
            broadsheetMarkList.Clear();
            commentsMidTerm.Clear();
            commentsTermEnd.Clear();
            commentsCheckPointIGCSE.Clear();
            combinedSubjectsName = string.Empty;
            subjects = await subjectService.GetAllAsync("AcademicsSubjects/GetSubjects/2/" + schid + "/0/0/" + true);
            combinedSubjectsName = subjects.Where(s => s.SbjMerge == true).Select(s => s.SbjMergeName).Distinct().FirstOrDefault();
        }

        async Task OnClassChanged(IEnumerable<string> e)
        {
            selectedClass = e.ElementAt(0);
            classid = classList.FirstOrDefault(c => c.ClassName == selectedClass).ClassID;
            classTeacherID = classList.FirstOrDefault(c => c.ClassName == selectedClass).StaffID;
            IsJuniorFinalYearClass = classList.FirstOrDefault(c => c.ClassID == classid).JuniorFinalYearClass;
            IsFinalYearClass = classList.FirstOrDefault(c => c.ClassID == classid).FinalYearClass;
            IsCheckPointClass = classList.FirstOrDefault(c => c.ClassID == classid).CheckPointClass;
            IsIGCSEClass = classList.FirstOrDefault(c => c.ClassID == classid).IGCSEClass;
            ResultsPreviewID();
            
            cognitiveMarks.Clear();
            studentOtherMarks.Clear();
            fieldnames.Clear();
            broadsheetMarkList.Clear();
            commentsMidTerm.Clear();
            commentsTermEnd.Clear();
            commentsCheckPointIGCSE.Clear();

            await CommentSelection();
            //await LoadStudents(classid);
        }

        async Task CommentSelection()
        {
            switch (SelectedOption)
            {
                case 1: //Mid-Term Results
                    commentsMidTerm = await midTermCommentService.GetAllAsync("" +
                        "AcademicsResultsComments/GetMidTermComments/1/" + termid + "/" + classid + "/0");
                    break;
                case 2: //End of Term Results
                    commentsTermEnd = await termEndCommentService.GetAllAsync(
                        "AcademicsResultsComments/GetTermEndComments/1/" + termid + "/" + classid + "/0");                 
                    break;
                case 3: //Check Point Results
                    commentsCheckPointIGCSE = await checkpointigcseCommentService.GetAllAsync(
                        "AcademicsResultsComments/GetCheckPointIGCSEComments/1/" + termid + "/" + classid + "/0");
                    break;
                case 4: //IGCSE Results
                    commentsCheckPointIGCSE = await checkpointigcseCommentService.GetAllAsync(
                       "AcademicsResultsComments/GetCheckPointIGCSEComments/1/" + termid + "/" + classid + "/0");
                    break;
            }
        }


        #endregion

        #region [Section - Mid-Term Results Processing]
        async Task ProcessMidTermResults()
        {
            ShowPreview = true;
            loadingmessage = "Please wait, Performing Backgroud Processing...";
            isLoading = true;
            StateHasChanged();

            try
            {
                await Task.Delay(1000);
                await cognitiveResultsService.DeleteAsync("AcademicsResults/DeleteCognitiveResult/", 0);               
                await LoadCognitiveMarks(1, termid);
            }
            finally
            {
                isLoading = false;
            }

            IsShow = false;
            i = 0;
            j = 0;
            progressbarInfo = "Please wait, Computing Cognitive Marks...";

            int maxValue = cognitiveMarks.Count();
            studentCount = cognitiveMarks.Where(m => m.EntryStatus_MidTerm == true).Select(s => s.STDID).Distinct().Count();

            foreach (var item in cognitiveMarks)
            {
                j++;
                i = ((decimal)(j) / maxValue) * 100;

                if (item.EntryStatus_MidTerm)
                {                   
                    decimal _totalMarks = Math.Round(item.Mark_Mid, MidpointRounding.AwayFromZero) +
                                            Math.Round(item.Mark_MidCBT, MidpointRounding.AwayFromZero);
                    decimal _mark = Math.Round(_totalMarks, MidpointRounding.AwayFromZero);
                    int kount = cognitiveMarks.Count(c => c.STDID == item.STDID);
                    decimal totalObtained = cognitiveMarks.Where(c => c.STDID == item.STDID).Sum(s => (
                                                Math.Round(s.Mark_Mid, MidpointRounding.AwayFromZero) +
                                                Math.Round(s.Mark_MidCBT, MidpointRounding.AwayFromZero)));
                    decimal AVGPer = cognitiveMarks.Where(d => d.STDID == item.STDID)
                                                .Average(s =>
                                                Math.Round(s.Mark_Mid, MidpointRounding.AwayFromZero) +
                                                Math.Round(s.Mark_MidCBT, MidpointRounding.AwayFromZero));

                    if (IsJuniorFinalYearClass)
                    {
                        var _grade = juniorMockGradeList.FirstOrDefault(g => g.LowerGrade <= _mark && g.HigherGrade >= _mark);

                        if (_grade != null)
                        {
                            GradeLetter = _grade.GradeLetter;
                            GradeRemarks = _grade.GradeRemark;
                        }
                        else
                        {
                            GradeLetter = string.Empty;
                            GradeRemarks = string.Empty;
                        }

                        var avgAll = juniorMockGradeList.FirstOrDefault(g => g.LowerGrade <= AVGPer && g.HigherGrade >= AVGPer);
                        if (avgAll != null)
                        {
                            OverAllGradeLetter = avgAll.GradeLetter;
                        }
                        else
                        {
                            OverAllGradeLetter = string.Empty;
                        }

                        reportSelection = await reportsSelectionService.GetByIdAsync("Settings/GetReport/", 1);
                    }
                    else if (IsFinalYearClass)
                    {
                        var _grade = seniorMockGradeList.FirstOrDefault(g => g.LowerGrade <= _mark && g.HigherGrade >= _mark);
                        if (_grade != null)
                        {
                            GradeLetter = _grade.GradeLetter;
                            GradeRemarks = _grade.GradeRemark;
                        }
                        else
                        {
                            GradeLetter = string.Empty;
                            GradeRemarks = string.Empty;
                        }

                        var avgAll = seniorMockGradeList.FirstOrDefault(g => g.LowerGrade <= AVGPer && g.HigherGrade >= AVGPer);
                        if (avgAll != null)
                        {
                            OverAllGradeLetter = avgAll.GradeLetter;
                        }
                        else
                        {
                            OverAllGradeLetter = string.Empty;
                        }

                        reportSelection = await reportsSelectionService.GetByIdAsync("Settings/GetReport/", 2);
                    }
                    else
                    {
                        var _grade = gradeList.FirstOrDefault(g => g.LowerGrade <= _mark && g.HigherGrade >= _mark);
                        if (_grade != null)
                        {
                            GradeLetter = _grade.GradeLetter;
                            GradeRemarks = _grade.GradeRemark;
                        }
                        else
                        {
                            GradeLetter = string.Empty;
                            GradeRemarks = string.Empty;
                        }

                        var avgAll = gradeList.FirstOrDefault(g => g.LowerGrade <= AVGPer && g.HigherGrade >= AVGPer);
                        if (avgAll != null)
                        {
                            OverAllGradeLetter = avgAll.GradeLetter;
                        }
                        else
                        {
                            OverAllGradeLetter = string.Empty;
                        }

                        reportSelection = await reportsSelectionService.GetByIdAsync("Settings/GetReport/", 3);
                    }
                                       
                    var _teacherComments = commentsMidTerm.SingleOrDefault(c => c.STDID == item.STDID);
                    if (_teacherComments != null)
                    {
                        teacherComment = _teacherComments.Comments_Teacher;
                    }
                    else
                    {
                        teacherComment = string.Empty;
                    }

                    cognitiveResults.STDID = item.STDID;
                    cognitiveResults.ClassID = item.ClassID;
                    cognitiveResults.SubjectID = item.SubjectID;
                    cognitiveResults.SubjectCode = item.SubjectCode;
                    cognitiveResults.Subject = item.Subject;
                    cognitiveResults.SortID = item.SortID;
                    cognitiveResults.CA = Convert.ToInt32(Math.Round(item.Mark_MidCBT, MidpointRounding.AwayFromZero));
                    cognitiveResults.Exam = Convert.ToInt32(Math.Round(item.Mark_Mid, MidpointRounding.AwayFromZero));
                    cognitiveResults.TotalMark = Convert.ToInt32(Math.Round(item.Mark_Mid + item.Mark_MidCBT, MidpointRounding.AwayFromZero));
                    cognitiveResults.Grade = GradeLetter;
                    cognitiveResults.Remarks = GradeRemarks;
                    cognitiveResults.MaxMark = kount * 100;
                    cognitiveResults.MinMark = Convert.ToInt32(Math.Round(totalObtained));
                    cognitiveResults.No_Of_Sbj = kount;
                    cognitiveResults.AVGPer = AVGPer;
                    cognitiveResults.YouthClub = OverAllGradeLetter;
                    cognitiveResults.Comments_Teacher = teacherComment;
                    cognitiveResults.ClassTeacherSign = item.signClassTeacher;
                    cognitiveResults.PrincipalSign = item.signPrincipal;
                    cognitiveResults.StudentPhoto = item.studentPhoto;
                    cognitiveResults.StudentNo = item.AdmissionNo;
                    cognitiveResults.FullName = item.StudentName;
                    cognitiveResults.ClassName = item.ClassName;
                    cognitiveResults.ClassTeacher = item.ClassTeacher;
                    cognitiveResults.Gender = item.Gender;
                    cognitiveResults.Age = item.Age;
                    cognitiveResults.No_In_Class = studentCount;
                    cognitiveResults.SchName = schInfo.SchName;
                    cognitiveResults.SchSlogan = schInfo.SchSlogan;
                    cognitiveResults.SchAddress = schInfo.SchAddress;
                    cognitiveResults.SchAddressLine2 = schInfo.SchAddressLine2;
                    cognitiveResults.SchEmails = schInfo.SchEmails;
                    cognitiveResults.SchPhones = schInfo.SchPhones;
                    cognitiveResults.SchWebsites = schInfo.SchWebsites;
                    cognitiveResults.SchLogo = schInfo.SchLogo;
                    cognitiveResults.AcademicSession = item.AcademicSession;
                    cognitiveResults.CurrentTerm = item.CurrentTerm;
                    cognitiveResults.SubjectTeacher = reportSelection.ReportDescr;
                    cognitiveResults.AlphabetID = termid;

                    await cognitiveResultsService.SaveAsync("AcademicsResults/AddCognitiveResult/", cognitiveResults);
                }

                StateHasChanged();
            }

            IsShow = true;
        }

        async Task MidTermResultsComputation()
        {
            if (!String.IsNullOrWhiteSpace(selectedClass))
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Create Student Results",
                    Text = "Do You Want To Continue With This Operation?",
                    Icon = SweetAlertIcon.Warning,
                    ShowCancelButton = true,
                    ConfirmButtonText = "Yes",
                    CancelButtonText = "No"
                });

                if (result.IsConfirmed)
                {
                    await ProcessMidTermResults();
                    await Swal.FireAsync("Create Results", "Selected Class Results Successfully Created. " +
                        "Please, use the Preview Results button to display results.", "success");
                }
            }
            else
            {
                await Swal.FireAsync("Create Results", "Please, selecte a class.", "error");
            }
        }


        #endregion

        #region [Section - Term End Results Processing]

        #region [Cognitive Processing]
        async Task PreviouseTermsTotalMark()
        {
            if (schtermid == 2)
            {
                //First Term Mark Total
                marksListPreviousTerms.Clear();
                await LoadCognitiveMarks(2, termid - 1);
                marksListPreviousTerms = cognitiveMarks;

                if (marksListPreviousTerms.Count() > 0)
                {
                    foreach (var firstterm in marksListPreviousTerms)
                    {
                        marksListFirstTerm.Add(new ACDStudentsMarksCognitiveFirstTerm()
                        {
                            STDID = firstterm.STDID,
                            SubjectID = firstterm.SubjectID,
                            TotalMark = firstterm.Mark_CA1 + firstterm.Mark_CA2 + firstterm.Mark_CA3 + firstterm.Mark_CBT + firstterm.Mark_Exam
                        }); ;
                    }
                }
            }
            else if (schtermid == 3)
            {
                //First Term Mark Total
                await LoadCognitiveMarks(2, termid - 2);
                marksListPreviousTerms = cognitiveMarks;

                if (marksListPreviousTerms.Count() > 0)
                {
                    foreach (var firstterm in marksListPreviousTerms)
                    {
                        marksListFirstTerm.Add(new ACDStudentsMarksCognitiveFirstTerm()
                        {
                            STDID = firstterm.STDID,
                            SubjectID = firstterm.SubjectID,
                            TotalMark = firstterm.Mark_CA1 + firstterm.Mark_CA2 + firstterm.Mark_CA3 + firstterm.Mark_CBT + firstterm.Mark_Exam
                        }); ;
                    }
                }

                //Second Term Mark Total
                marksListPreviousTerms.Clear();
                await LoadCognitiveMarks(2, termid - 1);
                marksListPreviousTerms = cognitiveMarks;

                if (marksListPreviousTerms.Count() > 0)
                {
                    foreach (var secondterm in marksListPreviousTerms)
                    {
                        marksListSecondTerm.Add(new ACDStudentsMarksCognitiveSecondTerm()
                        {
                            STDID = secondterm.STDID,
                            SubjectID = secondterm.SubjectID,
                            TotalMark = secondterm.Mark_CA1 + secondterm.Mark_CA2 + secondterm.Mark_CA3 + secondterm.Mark_CBT + secondterm.Mark_Exam
                        }); ;
                    }
                }
            }
        }

        private void SubjectRanking()
        {
            _subjectRanking.Clear();

            var scoreList = cognitiveMarks
                           .GroupBy(u => u.SubjectID)
                           .SelectMany(g => g.OrderByDescending(u => (u.Mark_CA1 + u.Mark_CA2 + u.Mark_CA3 + u.Mark_CBT + u.Mark_Exam))
                          .Select((j, i) => new
                          {
                              STDID = j.STDID,
                              SubjectID = j.SubjectID,
                              SubjectCode = j.SubjectCode,
                              Subject = j.Subject,
                              CA1 = j.Mark_CA1,
                              CA2 = j.Mark_CA2,
                              CA3 = j.Mark_CA3,
                              CBT = j.Mark_CBT,
                              CA = (j.Mark_CA1 + j.Mark_CA2 + j.Mark_CA3 + j.Mark_CBT),
                              Exam = j.Mark_Exam,
                              TotalMark = (j.Mark_CA1 + j.Mark_CA2 + j.Mark_CA3 + j.Mark_CBT + j.Mark_Exam),
                              StudentPhoto = j.studentPhoto,
                              StudentNo = j.AdmissionNo,
                              FullName = j.StudentName,
                              ClassTeacher = j.ClassTeacher,
                              YouthClub = j.YouthClub,
                              YouthRole = j.YouthRole,
                              ClassName = j.ClassName,
                              Age = j.Age,
                              Gender = j.Gender,
                              AcademicSession = j.AcademicSession,
                              CurrentTerm = j.CurrentTerm,
                              SubjectCount = g.Count(),
                              Position = i + 1,
                              MaxMark = g.Max(u => (u.Mark_CA1 + u.Mark_CA2 + u.Mark_CA3 + u.Mark_CBT + u.Mark_Exam)),
                              MinMark = g.Min(u => (u.Mark_CA1 + u.Mark_CA2 + u.Mark_CA3 + u.Mark_CBT + u.Mark_Exam)),
                              ClassAverage = g.Sum(u => (u.Mark_CA1 + u.Mark_CA2 + u.Mark_CA3 + u.Mark_CBT + u.Mark_Exam)) / g.Count()
                          })).ToList();

            var subjectIDList = cognitiveMarks.GroupBy(u => u.SubjectID)
                                        .Select((s, i) => new
                                        {
                                            SubjectID = s.Key
                                        }).ToList();

            foreach (var sbjid in subjectIDList)
            {
                var distinctScores = scoreList.Where(s => s.SubjectID == sbjid.SubjectID).Select(avg => avg.TotalMark).Distinct().ToList();
                distinctScores.Sort((a, b) => b.CompareTo(a));

                int rank = 1;
                foreach (var value in distinctScores)
                {
                    foreach (var item in scoreList.Where(s => s.SubjectID == sbjid.SubjectID))
                    {
                        if (item.TotalMark == value)
                        {
                            _subjectRanking.Add(new ACDSubjectsRanking()
                            {
                                STDID = item.STDID,
                                SubjectID = item.SubjectID,
                                SubjectCount = item.SubjectCount,
                                TotalMark = item.TotalMark,
                                Position = rank,
                                MaxMark = Convert.ToInt32(item.MaxMark),
                                MinMark = Convert.ToInt32(item.MinMark),
                                ClassAverage = item.ClassAverage
                            });
                        }
                    }

                    rank++;
                }
            }
        }

        async Task ProcessTermEndResults()
        {
            ShowPreview = true;
            loadingmessage = "Please wait, Performing Backgroud Processing...";
            isLoading = true;
            StateHasChanged();

            try
            {
                await Task.Delay(1000);
                await cognitiveResultsService.DeleteAsync("AcademicsResults/DeleteCognitiveResult/", 0);
                await PreviouseTermsTotalMark();
                await LoadCognitiveMarks(2, termid);
                SubjectRanking();
            }
            finally
            {
                isLoading = false;
            }

            IsShow = false;
            i = 0;
            j = 0;
            progressbarInfo = "Please wait, Computing Cognitive Marks...";

            int maxValue = cognitiveMarks.Count();
            studentCount = cognitiveMarks.Where(m => m.EntryStatus_TermEnd == true).Select(s => s.STDID).Distinct().Count();

            foreach (var item in cognitiveMarks)
            {
                j++;
                i = ((decimal)(j) / maxValue) * 100;

                if (item.EntryStatus_TermEnd)
                {
                    int _studentAttendance = sessions.FirstOrDefault(a => a.TermID == termid).Attendance;
                    DateTime? _NextTermBegins = sessions.FirstOrDefault(a => a.TermID == (termid + 1)).StartDate;
                    DateTime? _NextTermEnds = sessions.FirstOrDefault(a => a.TermID == (termid + 1)).EndDate;

                    decimal _totalMarks = Math.Round(item.Mark_CA1, MidpointRounding.AwayFromZero) +
                        Math.Round(item.Mark_CA2, MidpointRounding.AwayFromZero) +
                        Math.Round(item.Mark_CA3, MidpointRounding.AwayFromZero) +
                        Math.Round(item.Mark_CBT, MidpointRounding.AwayFromZero) +
                        Math.Round(item.Mark_Exam, MidpointRounding.AwayFromZero);
                    decimal _mark = Math.Round(_totalMarks, MidpointRounding.AwayFromZero);
                    int studentSubjectCount = cognitiveMarks.Count(u => u.STDID == item.STDID);
                    decimal AVGPer = cognitiveMarks.Where(d => d.STDID == item.STDID)
                                                .Average(u =>
                                                Math.Round(u.Mark_CA1, MidpointRounding.AwayFromZero) +
                                                Math.Round(u.Mark_CA2, MidpointRounding.AwayFromZero) +
                                                Math.Round(u.Mark_CA3, MidpointRounding.AwayFromZero) +
                                                Math.Round(u.Mark_CBT, MidpointRounding.AwayFromZero) +
                                                Math.Round(u.Mark_Exam, MidpointRounding.AwayFromZero));
                    decimal OverAllScore = cognitiveMarks.Where(d => d.STDID == item.STDID)
                                                        .Sum(u => Math.Round(u.Mark_CA1, MidpointRounding.AwayFromZero) +
                                                        Math.Round(u.Mark_CA2, MidpointRounding.AwayFromZero) +
                                                        Math.Round(u.Mark_CA3, MidpointRounding.AwayFromZero) +
                                                        Math.Round(u.Mark_CBT, MidpointRounding.AwayFromZero) +
                                                        Math.Round(u.Mark_Exam, MidpointRounding.AwayFromZero));

                    if (IsJuniorFinalYearClass)
                    {
                        var _grade = juniorMockGradeList.FirstOrDefault(g => g.LowerGrade <= _mark && g.HigherGrade >= _mark);
                        if (_grade != null)
                        {
                            GradeLetter = _grade.GradeLetter;
                            GradeRemarks = _grade.GradeRemark;
                        }
                        else
                        {
                            GradeLetter = string.Empty;
                            GradeRemarks = string.Empty;
                        }
                        reportSelection = await reportsSelectionService.GetByIdAsync("Settings/GetReport/", 4);
                    }
                    else if (IsFinalYearClass)
                    {
                        var _grade = seniorMockGradeList.FirstOrDefault(g => g.LowerGrade <= _mark && g.HigherGrade >= _mark);
                        if (_grade != null)
                        {
                            GradeLetter = _grade.GradeLetter;
                            GradeRemarks = _grade.GradeRemark;
                        }
                        else
                        {
                            GradeLetter = string.Empty;
                            GradeRemarks = string.Empty;
                        }
                        reportSelection = await reportsSelectionService.GetByIdAsync("Settings/GetReport/", 5);
                    }
                    else
                    {
                        var _grade = gradeList.FirstOrDefault(g => g.LowerGrade <= _mark && g.HigherGrade >= _mark);
                        if (_grade != null)
                        {
                            GradeLetter = _grade.GradeLetter;
                            GradeRemarks = _grade.GradeRemark;
                        }
                        else
                        {
                            GradeLetter = string.Empty;
                            GradeRemarks = string.Empty;
                        }

                        reportSelection = await reportsSelectionService.GetByIdAsync("Settings/GetReport/", 6);
                    }

                    var _termEndComments = commentsTermEnd.SingleOrDefault(c => c.STDID == item.STDID);

                    if (_termEndComments != null)
                    {
                        daysAbsent = _termEndComments.DaysAbsent;
                        teacherComment = _termEndComments.Comments_Teacher;
                        principlaComment = _termEndComments.Comments_Principal;
                    }
                    else
                    {
                        daysAbsent = 0;
                        teacherComment = string.Empty;
                        principlaComment = string.Empty;
                    }

                    cognitiveResults.STDID = item.STDID;
                    cognitiveResults.ClassID = classid;
                    cognitiveResults.SubjectID = item.SubjectID;
                    cognitiveResults.SubjectCode = item.SubjectCode;
                    cognitiveResults.Subject = item.Subject;
                    cognitiveResults.CA1 = Convert.ToInt32(Math.Round(item.Mark_CA1, MidpointRounding.AwayFromZero));
                    cognitiveResults.CA2 = Convert.ToInt32(Math.Round(item.Mark_CA2, MidpointRounding.AwayFromZero));
                    cognitiveResults.CA3 = Convert.ToInt32(Math.Round(item.Mark_CA3, MidpointRounding.AwayFromZero));
                    cognitiveResults.CLW = Convert.ToInt32(Math.Round(item.Mark_CBT, MidpointRounding.AwayFromZero));
                    cognitiveResults.CA = Convert.ToInt32(Math.Round((item.Mark_CA1 + item.Mark_CA2 + item.Mark_CA3 + item.Mark_CBT), MidpointRounding.AwayFromZero));
                    cognitiveResults.Exam = Convert.ToInt32(Math.Round(item.Mark_Exam, MidpointRounding.AwayFromZero));
                    cognitiveResults.TotalMark = Convert.ToInt32(Math.Round(_totalMarks, MidpointRounding.AwayFromZero));
                    cognitiveResults.Grade = GradeLetter;
                    cognitiveResults.Remarks = GradeRemarks;
                    cognitiveResults.POS = _subjectRanking.FirstOrDefault(y => y.STDID == item.STDID && y.SubjectID == item.SubjectID).Position;
                    cognitiveResults.MaxMark = _subjectRanking.FirstOrDefault(y => y.STDID == item.STDID && y.SubjectID == item.SubjectID).MaxMark;
                    cognitiveResults.MinMark = _subjectRanking.FirstOrDefault(y => y.STDID == item.STDID && y.SubjectID == item.SubjectID).MinMark;
                    cognitiveResults.ClassAvg = _subjectRanking.FirstOrDefault(y => y.STDID == item.STDID && y.SubjectID == item.SubjectID).ClassAverage;

                    if (marksListFirstTerm.Count > 0)
                    {
                        cognitiveResults.FTerm = 0;
                        bool firstTotalTermMark = marksListFirstTerm.Where(m => m.STDID == item.STDID && m.SubjectID == item.SubjectID).Any();
                        if (firstTotalTermMark)
                        {
                            cognitiveResults.FTerm = Convert.ToInt32(Math.Round(marksListFirstTerm.FirstOrDefault(m => m.STDID == item.STDID && m.SubjectID == item.SubjectID).TotalMark, MidpointRounding.AwayFromZero));
                        }
                    }

                    if (marksListSecondTerm.Count > 0)
                    {
                        cognitiveResults.STerm = 0;
                        bool secondTotalTermMark = marksListSecondTerm.Where(m => m.STDID == item.STDID && m.SubjectID == item.SubjectID).Any();
                        if (secondTotalTermMark)
                        {
                            cognitiveResults.STerm = Convert.ToInt32(Math.Round(marksListSecondTerm.FirstOrDefault(m => m.STDID == item.STDID && m.SubjectID == item.SubjectID).TotalMark, MidpointRounding.AwayFromZero));
                        }
                    }

                    if (schtermid == 3)
                    {
                        cognitiveResults.TTerm = Convert.ToInt32(Math.Round(_totalMarks, MidpointRounding.AwayFromZero));
                    }

                    cognitiveResults.StudentPhoto = item.studentPhoto;
                    cognitiveResults.StudentNo = item.AdmissionNo;
                    cognitiveResults.FullName = item.StudentName;
                    cognitiveResults.ClassTeacher = item.ClassTeacher;
                    cognitiveResults.YouthClub = item.YouthClub;
                    cognitiveResults.YouthRole = item.YouthRole;
                    cognitiveResults.Attendance = _studentAttendance;
                    cognitiveResults.DaysAbsent = daysAbsent;
                    cognitiveResults.NextTermBegins = _NextTermBegins;
                    cognitiveResults.NextTermEnds = _NextTermEnds;
                    cognitiveResults.ClassName = item.ClassName;
                    cognitiveResults.No_In_Class = studentCount;
                    cognitiveResults.No_Of_Sbj = studentSubjectCount;
                    cognitiveResults.AVGPer = AVGPer;
                    cognitiveResults.Position = 0;
                    cognitiveResults.Age = item.Age;
                    cognitiveResults.Gender = item.Gender;
                    cognitiveResults.OverAllScore = Convert.ToInt32(Math.Round(OverAllScore, MidpointRounding.AwayFromZero));
                    cognitiveResults.Comments_Teacher = teacherComment;
                    cognitiveResults.ClassTeacherSign = item.signClassTeacher;
                    cognitiveResults.Comments_Principal = principlaComment;
                    cognitiveResults.PrincipalSign = item.signPrincipal;
                    cognitiveResults.AcademicSession = item.AcademicSession;
                    cognitiveResults.CurrentTerm = item.CurrentTerm;
                    cognitiveResults.SchName = schInfo.SchName;
                    cognitiveResults.SchSlogan = schInfo.SchSlogan;
                    cognitiveResults.SchAddress = schInfo.SchAddress;
                    cognitiveResults.SchAddressLine2 = schInfo.SchAddressLine2;
                    cognitiveResults.SchEmails = schInfo.SchEmails;
                    cognitiveResults.SchPhones = schInfo.SchPhones;
                    cognitiveResults.SchWebsites = schInfo.SchWebsites;
                    cognitiveResults.SchLogo = schInfo.SchLogo;
                    cognitiveResults.SortID = item.SortID;
                    cognitiveResults.SubjectTeacher = reportSelection.ReportDescr;
                    cognitiveResults.AlphabetID = termid;

                    await cognitiveResultsService.SaveAsync("AcademicsResults/AddCognitiveResult/", cognitiveResults);
                }

                StateHasChanged();
            }

            IsShow = true;
        }

        #endregion

        #region [PsychoMotor Processing]
        private static bool Determine_Rating_Value_5(int ratingValue)
        {
            bool result = false;

            if (ratingValue == 5)
            {
                result = true;
            }

            return result;
        }

        private static bool Determine_Rating_Value_4(int ratingValue)
        {
            bool result = false;

            if (ratingValue == 4)
            {
                result = true;
            }

            return result;
        }

        private static bool Determine_Rating_Value_3(int ratingValue)
        {
            bool result = false;

            if (ratingValue == 3)
            {
                result = true;
            }

            return result;
        }

        private static bool Determine_Rating_Value_2(int ratingValue)
        {
            bool result = false;

            if (ratingValue == 2)
            {
                result = true;
            }

            return result;
        }

        private static bool Determine_Rating_Value_1(int ratingValue)
        {
            bool result = false;

            if (ratingValue == 1)
            {
                result = true;
            }

            return result;
        }

        private static bool Determine_Rating_Value_0(int ratingValue)
        {
            bool result = false;

            if (ratingValue == 0)
            {
                result = true;
            }

            return result;
        }

        async Task ProcessPsychoAndOtherAccessments()
        {
            await assessmentResultsService.DeleteAsync("AcademicsResults/DeleteOtherMarksResult/", 0);
            var _studentOtherMarks = await studentOtherMarksService.GetAllAsync("AcademicsMarks/GetOtherMarks/1/" +
                                                                            termid + "/" + schid + "/" + classid + "/0/0/0/0");

            studentOtherMarks = _studentOtherMarks.Where(m => m.Rating > 0).ToList();

            IsShow = false;
            i = 0;
            j = 0;
            progressbarInfo = "Please wait, Computing PsychoMotor And Other Marks...";

            int maxValue = studentOtherMarks.Count();

            foreach (var item in studentOtherMarks)
            {
                j++;
                i = ((decimal)(j) / maxValue) * 100;

                if (item.Rating > 0)
                {
                    int _ratingValue = ratingList.FirstOrDefault(r => r.RatingID == item.RatingID).Rating;

                    accessmentResults.STDID = item.STDID;
                    accessmentResults.ClassID = classid;
                    accessmentResults.SbjClassID = item.SbjClassID;
                    accessmentResults.SbjClassification = item.SubjectClassification;
                    accessmentResults.SubjectCode = item.SubjectCode;
                    accessmentResults.Subject = item.Subject;
                    accessmentResults.RatingValue = Convert.ToInt32(Math.Round(item.Rating, MidpointRounding.AwayFromZero));
                    accessmentResults.Five = Determine_Rating_Value_5(_ratingValue);
                    accessmentResults.Four = Determine_Rating_Value_4(_ratingValue);
                    accessmentResults.Three = Determine_Rating_Value_3(_ratingValue);
                    accessmentResults.Two = Determine_Rating_Value_2(_ratingValue);
                    accessmentResults.One = Determine_Rating_Value_1(_ratingValue);
                    accessmentResults.Zero = Determine_Rating_Value_0(_ratingValue);

                    await assessmentResultsService.SaveAsync("AcademicsResults/AddOtherMarksResults/", accessmentResults);
                }

                StateHasChanged();
            }

            IsShow = true;
        }

        #endregion

        async Task TermEndResultsComputation()
        {
            if (!String.IsNullOrWhiteSpace(selectedClass))
            {
                bool nextTermexist = sessions.Where(a => a.TermID == (termid + 1)).Any();
                if (nextTermexist)
                {
                    SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                    {
                        Title = "Create Student Results",
                        Text = "Do You Want To Continue With This Operation?",
                        Icon = SweetAlertIcon.Warning,
                        ShowCancelButton = true,
                        ConfirmButtonText = "Yes",
                        CancelButtonText = "No"
                    });

                    if (result.IsConfirmed)
                    {
                        await ProcessTermEndResults();
                        await ProcessPsychoAndOtherAccessments();
                        await Swal.FireAsync("Create Results", "Selected Class Results Successfully Created. " +
                            "Please, use the Preview Results button to display results.", "success");
                    }
                }
                else
                {
                    await Swal.FireAsync("Next Term Begins And End Dates Does Not Exist", "Please, Create Next Term Details.", "error");
                }
            }
            else
            {
                await Swal.FireAsync("Create Results", "Please, selecte a class.", "error");
            }
        }

        #endregion

        #region [Section - Check Point / IGCSE Results Processing]

        async Task ProcessCheckPointIGCSEResults()
        {
            ShowPreview = true;
            loadingmessage = "Please wait, Performing Backgroud Processing...";
            isLoading = true;
            StateHasChanged();

            try
            {
                await Task.Delay(1000);
                await cognitiveResultsService.DeleteAsync("AcademicsResults/DeleteCognitiveResult/", 0);
                await LoadCognitiveMarks(3, termid);
            }
            finally
            {
                isLoading = false;
            }

            IsShow = false;
            i = 0;
            j = 0;
            progressbarInfo = "Please wait, Computing Cognitive Marks...";

            int maxValue = cognitiveMarks.Count();
            studentCount = cognitiveMarks.Where(m => m.EntryStatus_ICGCS == true).Select(s => s.STDID).Distinct().Count();

            foreach (var item in cognitiveMarks)
            {
                j++;
                i = ((decimal)(j) / maxValue) * 100;

                if (item.EntryStatus_ICGCS)
                {
                    decimal _mark = Math.Round(item.Mark_ICGC, MidpointRounding.AwayFromZero);
                    int kount = cognitiveMarks.Count(c => c.STDID == item.STDID);
                   
                    if (IsCheckPointClass)
                    {
                        var _grade = checkpointGradeList.FirstOrDefault(g => g.LowerGrade <= _mark && g.HigherGrade >= _mark);
                        if (_grade != null)
                        {
                            GradeLetter = _grade.GradeLetter;
                        }
                        else
                        {
                            GradeLetter = string.Empty;
                        }

                        reportSelection = await reportsSelectionService.GetByIdAsync("Settings/GetReport/", 9);
                    }
                    else if (IsIGCSEClass)
                    {
                        var _grade = igcseGradeList.FirstOrDefault(g => g.LowerGrade <= _mark && g.HigherGrade >= _mark);
                        if (_grade != null)
                        {
                            GradeLetter = _grade.GradeLetter;
                        }
                        else
                        {
                            GradeLetter = string.Empty;
                        }

                        reportSelection = await reportsSelectionService.GetByIdAsync("Settings/GetReport/", 10);
                    }

                    var _Comments = commentsCheckPointIGCSE.SingleOrDefault(c => c.STDID == item.STDID);
                    if (_Comments != null)
                    {
                        principlaComment = _Comments.Comments;
                    }
                    else
                    {
                        principlaComment = string.Empty;
                    }

                    cognitiveResults.STDID = item.STDID;
                    cognitiveResults.ClassID = item.ClassID;
                    cognitiveResults.SubjectID = item.SubjectID;
                    cognitiveResults.SubjectCode = item.SubjectCode;
                    cognitiveResults.Subject = item.Subject;
                    cognitiveResults.SortID = item.SortID;
                    cognitiveResults.TotalMark = Convert.ToInt32(Math.Round(item.Mark_ICGC, MidpointRounding.AwayFromZero));
                    cognitiveResults.Grade = GradeLetter;
                    cognitiveResults.PrincipalSign = item.signPrincipal;
                    cognitiveResults.Comments_Principal = principlaComment;
                    cognitiveResults.StudentPhoto = item.studentPhoto;
                    cognitiveResults.StudentNo = item.AdmissionNo;
                    cognitiveResults.FullName = item.StudentName;
                    cognitiveResults.ClassName = item.ClassName;
                    cognitiveResults.SchName = schInfo.SchName;
                    cognitiveResults.SchSlogan = schInfo.SchSlogan;
                    cognitiveResults.SchAddress = schInfo.SchAddress;
                    cognitiveResults.SchAddressLine2 = schInfo.SchAddressLine2;
                    cognitiveResults.SchEmails = schInfo.SchEmails;
                    cognitiveResults.SchPhones = schInfo.SchPhones;
                    cognitiveResults.SchWebsites = schInfo.SchWebsites;
                    cognitiveResults.SchLogo = schInfo.SchLogo;
                    cognitiveResults.AcademicSession = item.AcademicSession;
                    cognitiveResults.CurrentTerm = item.CurrentTerm;
                    cognitiveResults.SubjectTeacher = reportSelection.ReportDescr;
                    cognitiveResults.AlphabetID = termid;

                    await cognitiveResultsService.SaveAsync("AcademicsResults/AddCognitiveResult/", cognitiveResults);
                }

                StateHasChanged();
            }

            if (IsCheckPointClass)
            {
                CombinedSubjectsTotal();
            }
                
            IsShow = true;
        }

        async void CombinedSubjectsTotal()
        {
            var _scienceScores = cognitiveMarks
                                .Where(s => s.SbjMerge == true)
                                .GroupBy(g => g.STDID)
                                .OrderByDescending(avg => avg.Average(u => u.Mark_ICGC))
                                .Select((avg, i) => new
                                {
                                    STDID = avg.Key,
                                    Mark = avg.Average(u => u.Mark_ICGC)
                                }).ToList();


            foreach (var item in _scienceScores)
            {
                var _Comments = commentsCheckPointIGCSE.SingleOrDefault(c => c.STDID == item.STDID);
                if (_Comments != null)
                {
                    teacherComment = _Comments.Comments;
                }
                else
                {
                    teacherComment = string.Empty;
                }

                cognitiveResults.STDID = item.STDID;
                cognitiveResults.StudentNo = cognitiveMarks.FirstOrDefault(s => s.STDID == item.STDID).AdmissionNo;
                cognitiveResults.FullName = cognitiveMarks.FirstOrDefault(s => s.STDID == item.STDID).StudentName;
                cognitiveResults.StudentPhoto = cognitiveMarks.FirstOrDefault(s => s.STDID == item.STDID).studentPhoto;
                cognitiveResults.ClassID = classid;
                cognitiveResults.SubjectID = 99999;
                cognitiveResults.Subject = combinedSubjectsName;
                cognitiveResults.SortID = 99999;
                cognitiveResults.TotalMark = Convert.ToInt32(Math.Round(item.Mark));
                cognitiveResults.Comments_Principal = teacherComment;

                decimal _mark = Math.Round(item.Mark, MidpointRounding.AwayFromZero);
                var _grade = checkpointGradeList.FirstOrDefault(g => g.LowerGrade <= _mark && g.HigherGrade >= _mark);
                if (_grade != null)
                {
                    GradeLetter = _grade.GradeLetter;
                }
                else
                {
                    GradeLetter = string.Empty;
                }

                cognitiveResults.Grade = GradeLetter;

                await cognitiveResultsService.SaveAsync("AcademicsResults/AddCognitiveResult/", cognitiveResults);
            }
        }

        async Task CheckPointIGCSEResultsComputation()
        {
            if (!String.IsNullOrWhiteSpace(selectedClass))
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Create Student Results",
                    Text = "Do You Want To Continue With This Operation?",
                    Icon = SweetAlertIcon.Warning,
                    ShowCancelButton = true,
                    ConfirmButtonText = "Yes",
                    CancelButtonText = "No"
                });

                if (result.IsConfirmed)
                {
                    await ProcessCheckPointIGCSEResults();
                    await Swal.FireAsync("Create Results", "Selected Class Results Successfully Created. " +
                        "Please, use the Preview Results button to display results.", "success");
                }
            }
            else
            {
                await Swal.FireAsync("Create Results", "Please, selecte a class.", "error");
            }
        }

        #endregion

        #region [Section - Click Events]

        int ResultsPreviewID()
        {
            int resultid = 0;
            string startYear = academicSession.Substring(0, 4);
            string endYear = academicSession.Substring(5, 4);

            switch (SelectedOption)
            {
                case 1: //Mid-Term Results
                    if (IsJuniorFinalYearClass)
                    {
                        resultid = 1; //Mid_Term_Mock_Report.rpt                        
                    }
                    else if (IsFinalYearClass)
                    {
                        resultid = 2; //Mid_Term_Mock_Report.rpt
                    }
                    else
                    {
                        resultid = 3; //Mid_Term_Report.rpt
                    }

                    if (schtermid == 1)
                    {
                        resultTitle = "First Term: Mid-Term Results";
                        fileNameDescription = "_" + startYear + "_" + endYear + "_FirstTerm_MidTerm_Results";
                    }
                    else if (schtermid == 2)
                    {
                        resultTitle = "Second Term: Mid-Term Results";
                        fileNameDescription = "_" + startYear + "_" + endYear + "_SecondTerm_MidTerm_Results";
                    }
                    else if (schtermid == 3)
                    {
                        resultTitle = "Third Term: Mid-Term Results";
                        fileNameDescription = "_" + startYear + "_" + endYear + "_ThirdTerm_MidTerm_Results";
                    }
                    break;
                case 2: //End of Term Results
                    if (IsJuniorFinalYearClass)
                    {
                        resultid = 4; //Mock_Report_Junior.rpt
                    }
                    else if (IsFinalYearClass)
                    {
                        resultid = 5; //Mock_Report.rpt
                    }
                    else
                    {
                        if (schtermid == 1)
                        {
                            resultid = 6; //"1_Term_Report.rpt
                        }
                        else if (schtermid == 2)
                        {
                            resultid = 7; //2_Term_Report.rpt
                        }
                        else if (schtermid == 3)
                        {
                            resultid = 8; //3_Term_Report.rpt
                        }
                    }
                    if (schtermid == 1)
                    {                        
                        resultTitle = startYear + " - "  + endYear + " First Term Results";
                        fileNameDescription = "_" + startYear + "_" + endYear + "_FirstTerm_Results";
                    }
                    else if (schtermid == 2)
                    {
                        resultTitle = startYear + " - " + endYear + " Second Term Results";
                        fileNameDescription = "_" + startYear + "_" + endYear + "_SecondTerm_Results";
                    }
                    else if (schtermid == 3)
                    {
                        resultTitle = startYear + " - " + endYear + " Third Term Results";
                        fileNameDescription = "_" + startYear + "_" + endYear + "_ThirdTerm_Results";
                    }
                    break;
                case 3: //Check Point Results
                    resultid = 9;//CheckPoint_Report.rpt
                    if (schtermid == 1)
                    {
                        resultTitle = "First Term CheckPoint Results";
                        fileNameDescription = "_" + startYear + "_" + endYear + "_FirstTerm_CheckPoint_Results";
                    }
                    else if (schtermid == 2)
                    {
                        resultTitle = "Second Term CheckPoint Results";
                        fileNameDescription = "_" + startYear + "_" + endYear + "_SecondTerm_CheckPoint_Results";
                    }
                    else if (schtermid == 3)
                    {
                        resultTitle = "Third Term CheckPoint Results";
                        fileNameDescription = "_" + startYear + "_" + endYear + "_ThirdTerm_CheckPoint_Results";
                    }
                    break;
                case 4: //IGCSE Results
                    resultid = 10;//IGCSE_Report.rpt
                    if (schtermid == 1)
                    {
                        resultTitle = "First Term IGCSE Results";
                        fileNameDescription = "_" + startYear + "_" + endYear + "_FirstTerm_IGCSE_Results";
                    }
                    else if (schtermid == 2)
                    {
                        resultTitle = "Second Term IGCSE Results";
                        fileNameDescription = "_" + startYear + "_" + endYear + "_SecondTerm_IGCSE_Results";
                    }
                    else if (schtermid == 3)
                    {
                        resultTitle = "Third Term IGCSE Results";
                        fileNameDescription = "_" + startYear + "_" + endYear + "_ThirdTerm_IGCSE_Results";
                    }
                    break;
            }

            return resultid;
        }

        async Task ResultsComputation()
        {
            switch (SelectedOption)
            {
                case 1: //Mid-Term Results
                    await MidTermResultsComputation();
                    break;
                case 2: //End of Term Results
                    await TermEndResultsComputation();
                    break;
                case 3: //Check Point Results
                    await CheckPointIGCSEResultsComputation();
                    break;
                case 4: //IGCSE Results
                    await CheckPointIGCSEResultsComputation();
                    break;
            }
        }

        async Task Refresh()
        {
            selectedSchool = string.Empty;
            schid = 0;
            schools.Clear();
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            classid = 0;
            classTeacherID = 0;
            selectedClass = string.Empty;
            classList.Clear();

            cognitiveMarks.Clear();
            studentOtherMarks.Clear();
            fieldnames.Clear();
            broadsheetMarkList.Clear();
            commentsMidTerm.Clear();
            commentsTermEnd.Clear();
            commentsCheckPointIGCSE.Clear();
            combinedSubjectsName = string.Empty;
            subjects.Clear();

            SelectedOption = 0;
            stopwatchvalue = new TimeSpan();

            SelectedStudent = string.Empty;
            Stdid = 0;
        }

        async Task OptRefresh()
        {
            selectedSchool = string.Empty;
            schid = 0;
            schools.Clear();
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            classid = 0;
            classTeacherID = 0;
            selectedClass = string.Empty;
            classList.Clear();

            cognitiveMarks.Clear();
            studentOtherMarks.Clear();
            fieldnames.Clear();
            broadsheetMarkList.Clear();
            commentsMidTerm.Clear();
            commentsTermEnd.Clear();
            commentsCheckPointIGCSE.Clear();
            combinedSubjectsName = string.Empty;
            subjects.Clear();
                      
            stopwatchvalue = new TimeSpan();

            SelectedStudent = string.Empty;
            Stdid = 0;
        }


        async Task OptSelectionClick()
        {
            selectedSchool = string.Empty;
            schid = 0;
            schools.Clear();
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            classid = 0;
            classTeacherID = 0;
            selectedClass = string.Empty;
            classList.Clear();
        }

        void GoBack()
        {           
            menuId = 1;
        }

        async Task ResultPage()
        {            
            if (String.IsNullOrWhiteSpace(selectedClass))
            {
                await Swal.FireAsync("Academic Result", "Please Select A Class.", "error");
            }
            else
            {
                int resultcount = await cognitiveResultsService.CountAsync("AcademicsResults/GetCount/1/", 1);

                if (resultcount > 0)
                {
                    menuId = 2;
                    SelectedStudent = string.Empty;
                    Students.Clear();
                    await LoadStudents(classid);
                }
                else
                {
                    await Swal.FireAsync("Academic Result", "Either Results Has Not Been Generated Or " +
                        "There Are No Mark Records Selected Class - " + selectedClass + ".", "error");
                }
            }                      
        }

        async Task BroadsheetPage()
        {
            if (String.IsNullOrWhiteSpace(selectedClass))
            {
                await Swal.FireAsync("Academic Result", "Please Select A Class.", "error");
            }
            else
            {
                int resultcount = await cognitiveResultsService.CountAsync("AcademicsResults/GetCount/1/", 1);

                if (resultcount > 0)
                {
                    menuId = 3;
                }
                else
                {
                    await Swal.FireAsync("Academic Result", "Either Results Has Not Been Generated Or " +
                        "There Are No Mark Records Selected Class - " + selectedClass + ".", "error");
                }
            }
        }

        #endregion

        #endregion

        #region [Section - Go To Result Page]
        #region [Variables Declaration]
        private IJSObjectReference _jsModule { get; set; }

        byte[] pdfFile { get; set; }
        private bool _processing = false;

        TimeSpan stopwatchvalue = new();
        bool Is_stopwatchrunning { get; set; } = false;
        int MaxTermID { get; set; }
        int Stdid { get; set; }
        string SelectedStudent { get; set; }

        #endregion

        #region [Models Declaration]       
        List<SETSchSessions> SchSessions = new();
        List<ADMStudents> Students = new();
        List<ACDStudentsMarksCognitive> cognitiveMarksSTDID = new();

        FileChunkDTO fileDTO = new();
        ADMStudents student = new();
        #endregion

        #region [Section - Load And Click Events]

        async Task LoadStudents(int _classid)
        {           
            if (MaxTermID == termid)
            {
                Students = await studentService.GetAllAsync("AdminStudent/GetStudents/3/0/" + _classid + "/0/1");
            }
            else
            {
                //Students = await studentService.GetAllAsync("AdminStudent/GetStudents/7/0/" + _classid + "/0/0");
                if (SelectedOption == 1)
                {
                    cognitiveMarksSTDID = await studentMarksCognitiveService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/19/" +
                                        termid + "/" + schid + "/" + classid + "/0/0/0");

                }
                else if (SelectedOption == 2)
                {
                    cognitiveMarksSTDID = await studentMarksCognitiveService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/20/" +
                                        termid + "/" + schid + "/" + classid + "/0/0/0");
                }
                else
                {
                    cognitiveMarksSTDID = await studentMarksCognitiveService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/21/" +
                                       termid + "/" + schid + "/" + classid + "/0/0/0");
                }

                foreach (var item in cognitiveMarksSTDID)
                {
                    Students.Add(new ADMStudents()
                    {
                        STDID = item.STDID,
                        StudentName = item.StudentName,
                    });
                }
            }
        }

        string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }
            var splitted = name?.Split(' ');
            var initials = $"{splitted[0][0]}{(splitted.Length > 1 ? splitted[splitted.Length - 1][0] : (char?)null)}";
            return initials;
        }

        async Task OnStudentChanged(IEnumerable<string> e)
        {
            SelectedStudent = e.ElementAt(0);
            Stdid = Students.FirstOrDefault(c => c.StudentName == SelectedStudent).STDID;

            int resultcount = await cognitiveResultsService.CountAsync("AcademicsResults/GetCount/0/", Stdid);

            if (resultcount > 0)
            {
                ProcessAResult();
            }
            else
            {
                await Swal.FireAsync("Academic Result", "No Mark For " + SelectedStudent + ".", "error");
            }
        }

        async void ProcessAllResults()
        {
            stopwatchvalue = new TimeSpan();
            Task task1 = Task.Run(() => ProcessingAllStudentsResults());
            Task task2 = Task.Run(() => StartStopWatch());

            await Task.WhenAll(task1, task2);

            task1.Dispose();
            task2.Dispose();

            await Task.CompletedTask;
        }

        #endregion

        #region [Section - Students Results In PDF]
        async Task GenerateAllStudentsResults()
        {
            Stdid = 0;
            pdfFile = null;

            switch (ResultsPreviewID())
            {
                case 1: //Mid-Term Results - Junior Final Year Class
                    pdfFile = await reportServices.GetResults("ResultsMidTerm/GetPDFResults/1/0");
                    break;
                case 2: //Mid-Term Results - Senior Final Year Class
                    pdfFile = await reportServices.GetResults("ResultsMidTerm/GetPDFResults/2/0");
                    break;
                case 3: //Mid-Term Results - Other Classes
                    pdfFile = await reportServices.GetResults("ResultsMidTerm/GetPDFResults/3/0");
                    break;
                case 4: //End of Term Results - Junior Final Year Class
                    pdfFile = await reportServices.GetResults("ResultsMock/GetPDFResults/4/0");
                    break;
                case 5: //End of Term Results - Senior Final Year Class
                    pdfFile = await reportServices.GetResults("ResultsMock/GetPDFResults/5/0");
                    break;
                case 6: //End of Term Results - First Term
                    pdfFile = await reportServices.GetResults("ResultsTermEnd/GetPDFResults/1/0");
                    break;
                case 7: //End of Term Results - Second Term
                    pdfFile = await reportServices.GetResults("ResultsTermEnd/GetPDFResults/2/0");
                    break;
                case 8: //End of Term Results - Third Term
                    pdfFile = await reportServices.GetResults("ResultsTermEnd/GetPDFResults/3/0");
                    break;
                case 9: //End of Term Results - Check Point
                    pdfFile = await reportServices.GetResults("ResultsCheckPoint/GetPDFResults/9/0");
                    break;
                case 10: //End of Term Results - IGCSE
                    pdfFile = await reportServices.GetResults("ResultsCheckPoint/GetPDFResults/10/0");
                    break;
            }

            //View PDF
            //await _jsModule.InvokeAsync<string>(
            //   "viewPdf",
            //   "iframeId",
            //   Convert.ToBase64String(pdfFile)
            //   );

            //Open PDF In New Tab
            await _jsModule.InvokeAsync<string>(
              "openPDFNewTab",
              Convert.ToBase64String(pdfFile)
              );

            //Download PDF
            await _jsModule.InvokeAsync<string>(
              "downloadPdf",
              selectedClass + fileNameDescription + ".pdf",
              Convert.ToBase64String(pdfFile)
              );
        }

        async Task DownloadResults()
        {
            Stdid = 0;
            pdfFile = null;

            switch (ResultsPreviewID())
            {
                case 1: //Mid-Term Results - Junior Final Year Class
                    pdfFile = await reportServices.GetResults("ResultsMidTerm/GetPDFResults/1/0");
                    break;
                case 2: //Mid-Term Results - Senior Final Year Class
                    pdfFile = await reportServices.GetResults("ResultsMidTerm/GetPDFResults/2/0");
                    break;
                case 3: //Mid-Term Results - Other Classes
                    pdfFile = await reportServices.GetResults("ResultsMidTerm/GetPDFResults/3/0");
                    break;
                case 4: //End of Term Results - Junior Final Year Class
                    pdfFile = await reportServices.GetResults("ResultsMock/GetPDFResults/4/0");
                    break;
                case 5: //End of Term Results - Senior Final Year Class
                    pdfFile = await reportServices.GetResults("ResultsMock/GetPDFResults/5/0");
                    break;
                case 6: //End of Term Results - First Term
                    pdfFile = await reportServices.GetResults("ResultsTermEnd/GetPDFResults/1/0");
                    break;
                case 7: //End of Term Results - Second Term
                    pdfFile = await reportServices.GetResults("ResultsTermEnd/GetPDFResults/2/0");
                    break;
                case 8: //End of Term Results - Third Term
                    pdfFile = await reportServices.GetResults("ResultsTermEnd/GetPDFResults/3/0");
                    break;
                case 9: //End of Term Results - Check Point
                    pdfFile = await reportServices.GetResults("ResultsCheckPoint/GetPDFResults/9/0");
                    break;
                case 10: //End of Term Results - IGCSE
                    pdfFile = await reportServices.GetResults("ResultsCheckPoint/GetPDFResults/10/0");
                    break;
            }

            //Download PDF
            await _jsModule.InvokeAsync<string>(
              "downloadPdf",
              selectedClass + "_StudentResults.pdf",
              Convert.ToBase64String(pdfFile)
              );
        }

        async Task ProcessingAllStudentsResults()
        {
            _processing = true;
            await Task.Delay(2000);
            await GenerateAllStudentsResults();
            _processing = false;
        }

        async Task GenerateAStudentResult()
        {
            pdfFile = null;

            switch (ResultsPreviewID())
            {
                case 1: //Mid-Term Results - Junior Final Year Class
                    pdfFile = await reportServices.GetResults("ResultsMidTerm/GetPDFResults/1/" + Stdid);
                    break;
                case 2: //Mid-Term Results - Senior Final Year Class
                    pdfFile = await reportServices.GetResults("ResultsMidTerm/GetPDFResults/2/" + Stdid);
                    break;
                case 3: //Mid-Term Results - Other Classes
                    pdfFile = await reportServices.GetResults("ResultsMidTerm/GetPDFResults/3/" + Stdid);
                    break;
                case 4: //End of Term Results - Junior Final Year Class
                    pdfFile = await reportServices.GetResults("ResultsMock/GetPDFResults/4/" + Stdid);
                    break;
                case 5: //End of Term Results - Senior Final Year Class
                    pdfFile = await reportServices.GetResults("ResultsMock/GetPDFResults/5/" + Stdid);
                    break;
                case 6: //End of Term Results - First Term
                    pdfFile = await reportServices.GetResults("ResultsTermEnd/GetPDFResults/1/" + Stdid);
                    break;
                case 7: //End of Term Results - Second Term
                    pdfFile = await reportServices.GetResults("ResultsTermEnd/GetPDFResults/2/" + Stdid);
                    break;
                case 8: //End of Term Results - Third Term
                    pdfFile = await reportServices.GetResults("ResultsTermEnd/GetPDFResults/3/" + Stdid);
                    break;
                case 9: //End of Term Results - Check Point
                    pdfFile = await reportServices.GetResults("ResultsCheckPoint/GetPDFResults/9/" + Stdid);
                    break;
                case 10: //End of Term Results - IGCSE
                    pdfFile = await reportServices.GetResults("ResultsCheckPoint/GetPDFResults/10/" + Stdid);
                    break;
            }

            //View PDF
            await _jsModule.InvokeAsync<string>(
               "viewPdf",
               "iframeId",
               Convert.ToBase64String(pdfFile)
               );
        }

        async Task ProcessingAStudentResult()
        {
            _processing = true;
            await Task.Delay(1000);
            await GenerateAStudentResult();
            _processing = false;
        }

        async void ProcessAResult()
        {
            stopwatchvalue = new TimeSpan();
            Task task1 = Task.Run(() => ProcessingAStudentResult());
            Task task2 = Task.Run(() => StartStopWatch());

            await Task.WhenAll(task1, task2);

            task1.Dispose();
            task2.Dispose();
            await Task.CompletedTask;
        }

        async Task StartStopWatch()
        {
            Is_stopwatchrunning = true;

            while (Is_stopwatchrunning)
            {
                await Task.Delay(1000);
                if (Is_stopwatchrunning)
                {
                    await InvokeAsync(() =>
                    {
                        stopwatchvalue = stopwatchvalue.Add(new TimeSpan(0, 0, 1));
                        if (_processing == false)
                        {
                            Is_stopwatchrunning = false;
                            StateHasChanged();
                        }

                        StateHasChanged();
                    });                    
                }
            }
        }

        #endregion

        #region [Section - Export Students Results For Parents]

        async Task ExportAStudentResult(int _stdid)
        {
            pdfFile = null;

            switch (ResultsPreviewID())
            {
                case 1: //Mid-Term Results - Junior Final Year Class
                    pdfFile = await reportServices.GetResults("ResultsMidTerm/GetPDFResults/1/" + _stdid);
                    break;
                case 2: //Mid-Term Results - Senior Final Year Class
                    pdfFile = await reportServices.GetResults("ResultsMidTerm/GetPDFResults/2/" + _stdid);
                    break;
                case 3: //Mid-Term Results - Other Classes
                    pdfFile = await reportServices.GetResults("ResultsMidTerm/GetPDFResults/3/" + _stdid);
                    break;
                case 4: //End of Term Results - Junior Final Year Class
                    pdfFile = await reportServices.GetResults("ResultsMock/GetPDFResults/4/" + _stdid);
                    break;
                case 5: //End of Term Results - Senior Final Year Class
                    pdfFile = await reportServices.GetResults("ResultsMock/GetPDFResults/5/" + _stdid);
                    break;
                case 6: //End of Term Results - First Term
                    pdfFile = await reportServices.GetResults("ResultsTermEnd/GetPDFResults/1/" + _stdid);
                    break;
                case 7: //End of Term Results - Second Term
                    pdfFile = await reportServices.GetResults("ResultsTermEnd/GetPDFResults/2/" + _stdid);
                    break;
                case 8: //End of Term Results - Third Term
                    pdfFile = await reportServices.GetResults("ResultsTermEnd/GetPDFResults/3/" + _stdid);
                    break;
                case 9: //End of Term Results - Check Point
                    pdfFile = await reportServices.GetResults("ResultsCheckPoint/GetPDFResults/9/" + _stdid);
                    break;
                case 10: //End of Term Results - IGCSE
                    pdfFile = await reportServices.GetResults("ResultsCheckPoint/GetPDFResults/10/" + _stdid);
                    break;
            }
        }

        async Task ExportAllStudentsResults()
        {
            IsShow = false;
            i = 0;
            j = 0;
            progressbarInfo = "Please wait, Exporting Results For Parents...";

            studentCognitiveResults = await cognitiveResultsService.GetAllAsync("AcademicsResults/GetCognitiveResults/1");
            int maxValue = studentCognitiveResults.Select(s => s.STDID).Distinct().Count();

            foreach (var item in studentCognitiveResults.Select(s => s.STDID).Distinct())
            {
                j++;
                i = ((decimal)(j) / maxValue) * 100;

                await ExportAStudentResult(item);

                if (pdfFile != null)
                {
                    var chunk = new FileChunkDTO
                    {
                        Data = pdfFile,
                        FileName = pad_an_int(item, 4) + ResultType() + "_" + SelectedOption + ".pdf",
                        FolderName = "Results",
                        Offset = 0,
                        FirstChunk = true
                    };

                    await fileUploadServices.ExportResultsAsync("Files/UploadFileChunk", chunk);

                    student.STDID = item;
                    student.ResultTypeID = SelectedOption;
                    student.ResultTermID = termid;
                    await studentService.UpdateAsync("AdminStudent/UpdateStudent/", 16, student);
                }

                StateHasChanged();
            }

            IsShow = true;

            await Swal.FireAsync("Export Results For Parents", "Operation Completed Successfully.", "success");
        }

        static string pad_an_int(int N, int P)
        {
            // string used in Format() method
            string s = "{0:";
            for (int i = 0; i < P; i++)
            {
                s += "0";
            }
            s += "}";

            // use of string.Format() method
            string value = string.Format(s, N);

            // return output
            return value;
        }

        string ResultType()
        {
            string result = string.Empty;

            switch (SelectedOption)
            {
                case 1:
                    result = "Mid-Term Exam";
                    break;
                case 2:
                    result = "End of Term Exam";
                    break;
                case 3:
                    result = "Check Point Exam";
                    break;
                case 4:
                    result = "IGCSE Exam";
                    break;
            }

            return result;
        }

        async Task UploadFiles()
        {

            var chunk = new FileChunkDTO
            {
                Data = pdfFile,
                FileName = "StudentResults.pdf",
                Offset = 0,
                FirstChunk = true
            };

            //fileDTO.FileName = "StudentResults.pdf";
            //fileDTO.Data = pdfFile;
            await fileUploadServices.SaveAsync("Files/UploadFileChunk", chunk);
        }

        #endregion

        #endregion

        #region [Section - Broadsheet]
        #region [Variables Declaration]

        int broadsheetMenudId { get; set; }
        int _stdid { get; set; }
        string ScriptFilePath { get; set; }
        string broadsheetFileName { get; set; }
        bool fixedheader { get; set; } = true;

        #endregion

        #region [Models Declaration]
        List<ACDStudentsResultCognitive> _studentResults = new();
        List<ScoreRanking> _scoreRanking = new();
        #endregion

        string ResultTypeBroadsheet()
        {
            string result = string.Empty;
            string filename = academicSession.Substring(0, 4) + academicSession.Substring(5, 4) + "_" + schTerm + "_Term_" + selectedClass + ".xlsx";
            switch (SelectedOption)
            {
                case 1:
                    result = "Mid-Term Broadsheet For " + selectedClass;
                    broadsheetFileName = "Broadsheet_midterm_" + filename;
                    break;
                case 2:
                    result = "End of Term Broadsheet For " + selectedClass;
                    broadsheetFileName = "Broadsheet_termend_" + filename;
                    break;
                case 3:
                    result = "Check Point Broadsheet For " + selectedClass;
                    broadsheetFileName = "Broadsheet_checkpoint_" + filename;
                    break;
                case 4:
                    result = "IGCSE Broadsheet For " + selectedClass;
                    broadsheetFileName = "Broadsheet_igcse_" + filename;
                    break;
            }

            return result;
        }

        string ResultTypeDisplayBroadsheet()
        {
            string result = string.Empty;

            switch (SelectedOption)
            {
                case 1:
                    result = "Mid-Term: " + academicSession;
                    break;
                case 2:
                    result = "End of Term: " + academicSession;
                    break;
                case 3:
                    result = "CheckPoint: " + academicSession;
                    break;
                case 4:
                    result = "IGCSE: " + academicSession;
                    break;
            }

            return result;
        }

        async Task DisplayBroadsheet(int _menudId)
        {
            broadsheetMenudId = _menudId;

            await deleteService.DeleteAsync("AcademicsResults/DeleteBroadSheet/", 1);

            ScriptFilePath = await stringValueService.GetStringAsync("Files/GetSqlFilePath/generatebroadsheettotal.sql");
            broadsheet.scriptFilePath = ScriptFilePath;

            await braodsheetService.SaveAsync("AcademicsResults/ExecuteScript", broadsheet);
            fieldnames = await stringValueService.GetAllAsync("AcademicsResults/GetFieldNames");

            if (fieldnames.Count > 0)
            {
                broadsheetMarkList = await bradsheetMarkListService.GetAllAsync("AcademicsResults/GetBroadSheet");
            }
            else
            {
                await Swal.FireAsync("Empty Marks", "No Mark Entry For " + selectedClass + " Class.", "info");
            }
        }

        async Task Test()
        {
            dynamic dyn;

            foreach (var item in broadsheetMarkList)
            {
                dyn = JsonConvert.DeserializeObject(item.ToString());

                foreach (var r in dyn.rows)
                {
                    foreach (var d in r)
                    {
                        await Swal.FireAsync("Export To Excel", d.Value, "success");                        
                    }
                }
            }

        }

        async Task DeserializeScpres()
        {
            await BroadsheetScoreRanking();
            string _resulttype = ResultType();
            string fileName = ResultTypeBroadsheet();
            byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Broadsheet");
            int _row = 0;

            foreach (var item in broadsheetMarkList)
            {
                dynamic json = JsonConvert.DeserializeObject<ExpandoObject>(item.ToString(), new ExpandoObjectConverter());

                foreach (var value in json)
                {
                    //await Swal.FireAsync("Export To Excel", $"{value.Key}: {value.Value}", "success");

                    if (value.Value != null)
                    {                        
                        if (value.Key == "STDID")
                        {
                            _row++;
                            int studentId = Convert.ToInt32(value.Value);
                            workSheet.Cells[_row, 1].Value = studentId;

                            var xxxxx = _scoreRanking.SingleOrDefault(s => s.STDID == studentId);
                            if (xxxxx != null)
                            {
                                workSheet.Cells[_row, 2].Value = xxxxx.SubjectCount;
                            }

                            //await Swal.FireAsync("Export To Excel", "STDID: " + studentId, "success");
                        }
                    }
                }
            }

            fileContents = package.GetAsByteArray();
            await iJSRuntime.InvokeAsync<string>(
              "saveAsFile",
              broadsheetFileName,
              Convert.ToBase64String(fileContents)
              );

            package.Dispose();

            await Swal.FireAsync("Export To Excel", "Operation Completed Successfully.", "success");
        }

        #region [Broadsheet Summary]
        async Task GetBroadsheetSummary()
        {
            broadsheetMenudId = 0;

            if (SelectedOption == 4)
            {
                SelectedOption = 3;
            }

            pdfFile = null;
            pdfFile = await reportServices.GetResults("ResultsBroadsheet/GetBroadsheet/" + SelectedOption + "/" + termid + "/" + schid + "/" +
                                                        classid + "/" + ResultTypeBroadsheet());

            //View PDF
            await _jsModule.InvokeAsync<string>(
               "viewPdf",
               "iframeId",
               Convert.ToBase64String(pdfFile)
               );
        }
        #endregion

        #region [Export To Excel]
        async Task RetrieveCognitiveResults()
        {
            isLoading = true;
            StateHasChanged();

            try
            {
                await Task.Delay(500);
                _studentResults = await cognitiveResultsService.GetAllAsync("AcademicsResults/GetCognitiveResults/1");
            }
            finally
            {
                isLoading = false;
            }
        }

        async Task ExportBroadsheetToExcel()
        {
            await DisplayBroadsheet(0);
            await BroadsheetScoreRanking();
            string _resulttype = ResultType();
            string fileName = ResultTypeBroadsheet();
            byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var workSheet = package.Workbook.Worksheets.Add("Broadsheet");

                workSheet.Column(1).Width = 3.5;
                workSheet.Column(2).Width = 12.5;
                workSheet.Column(3).Width = 33;
                workSheet.PrinterSettings.PaperSize = ePaperSize.A4;
                workSheet.PrinterSettings.Orientation = eOrientation.Landscape;
                workSheet.PrinterSettings.LeftMargin = Convert.ToDecimal(0.111811023622047);
                workSheet.PrinterSettings.RightMargin = Convert.ToDecimal(0.111811023622047);
                workSheet.PrinterSettings.FitToPage = true;

                #region Header Row
                workSheet.Cells[1, 1].Value = schInfo.SchName;
                workSheet.Cells[1, 1].Style.Font.Size = 16;
                workSheet.Cells[1, 1].Style.Font.Bold = true;

                workSheet.Cells[2, 1].Value = schInfo.SchAddress;
                workSheet.Cells[2, 1].Style.Font.Size = 12;
                workSheet.Cells[2, 1].Style.Font.Italic = true;
                workSheet.Cells[2, 1].Style.Font.Bold = false;

                workSheet.Cells[3, 1].Value = ResultType();
                workSheet.Cells[3, 1].Style.Font.Size = 14;
                workSheet.Cells[3, 1].Style.Font.Bold = true;

                workSheet.Cells[4, 1].Value = academicSession;
                workSheet.Cells[4, 1].Style.Font.Size = 14;
                workSheet.Cells[4, 1].Style.Font.Bold = true;

                workSheet.Cells[6, 3].Value = "ANALYSIS";
                workSheet.Cells[6, 3].Style.Font.Size = 11;
                workSheet.Cells[6, 3].Style.Font.Italic = true;
                workSheet.Cells[6, 3].Style.Font.Bold = true;
                workSheet.Cells[6, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                workSheet.Cells[7, 3].Value = "Average Score";
                workSheet.Cells[7, 3].Style.Font.Size = 11;
                workSheet.Cells[7, 3].Style.Font.Italic = true;
                workSheet.Cells[7, 3].Style.Font.Bold = true;
                workSheet.Cells[7, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                workSheet.Cells[8, 3].Value = "Pass Ratio";
                workSheet.Cells[8, 3].Style.Font.Size = 11;
                workSheet.Cells[8, 3].Style.Font.Italic = true;
                workSheet.Cells[8, 3].Style.Font.Bold = true;
                workSheet.Cells[8, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                workSheet.Cells[9, 3].Value = "Pass Percentage";
                workSheet.Cells[9, 3].Style.Font.Size = 11;
                workSheet.Cells[9, 3].Style.Font.Italic = true;
                workSheet.Cells[9, 3].Style.Font.Bold = true;
                workSheet.Cells[9, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                #endregion

                #region Body Row

                int columnCount = fieldnames.Count;
                int rowCount = broadsheetMarkList.Count;

                #region Table Header
                workSheet.Cells[10, 1].Value = "S/N";
                workSheet.Cells[10, 1].Style.Font.Size = 10;
                workSheet.Cells[10, 1].Style.Font.Bold = true;

                workSheet.Cells[10, 2].Value = "Student No.";
                workSheet.Cells[10, 2].Style.Font.Size = 10;
                workSheet.Cells[10, 2].Style.Font.Bold = true;

                workSheet.Cells[10, 3].Value = "Student Name";
                workSheet.Cells[10, 3].Style.Font.Size = 10;
                workSheet.Cells[10, 3].Style.Font.Bold = true;

                for (int j = 4; j < columnCount; j++)
                {
                    workSheet.Cells[10, j].Value = fieldnames[j];
                    workSheet.Cells[10, j].Style.Font.Size = 10;
                    workSheet.Cells[10, j].Style.Font.Bold = true;
                    workSheet.Cells[10, j].Style.TextRotation = 90;
                    workSheet.Column(j).Width = 5;
                }

                workSheet.Cells[10, columnCount + 0].Value = "NO. OF SUBJECTS";
                workSheet.Cells[10, columnCount + 0].Style.Font.Size = 10;
                workSheet.Cells[10, columnCount + 0].Style.Font.Bold = true;
                workSheet.Cells[10, columnCount + 0].Style.Font.Bold = true;
                workSheet.Cells[10, columnCount + 0].Style.TextRotation = 90;
                workSheet.Column(columnCount + 0).Width = 5;

                workSheet.Cells[10, columnCount + 1].Value = "TOTAL";
                workSheet.Cells[10, columnCount + 1].Style.Font.Size = 10;
                workSheet.Cells[10, columnCount + 1].Style.Font.Bold = true;
                workSheet.Cells[10, columnCount + 1].Style.Font.Bold = true;
                workSheet.Cells[10, columnCount + 1].Style.TextRotation = 90;
                workSheet.Column(columnCount + 1).Width = 5;

                workSheet.Cells[10, columnCount + 2].Value = "AVERAGE";
                workSheet.Cells[10, columnCount + 2].Style.Font.Size = 10;
                workSheet.Cells[10, columnCount + 2].Style.Font.Bold = true;
                workSheet.Cells[10, columnCount + 2].Style.Font.Bold = true;
                workSheet.Cells[10, columnCount + 2].Style.TextRotation = 90;
                workSheet.Column(columnCount + 2).Width = 6;

                workSheet.Cells[10, columnCount + 3].Value = "POSITION";
                workSheet.Cells[10, columnCount + 3].Style.Font.Size = 10;
                workSheet.Cells[10, columnCount + 3].Style.Font.Bold = true;
                workSheet.Cells[10, columnCount + 3].Style.Font.Bold = true;
                workSheet.Cells[10, columnCount + 3].Style.TextRotation = 90;
                workSheet.Column(columnCount + 3).Width = 5;

                #endregion

                #region Table Body
                int _row = 0;
                int ScoreKount = 0;
                int PassKount = 0;
                decimal AVGScore = 0;

                foreach (var item in broadsheetMarkList)
                {
                    dynamic row = JsonConvert.DeserializeObject<dynamic>(item.ToString());
                    _row++;

                    int _col = 0;
                    foreach (var col in fieldnames)
                    {
                        _col++;

                        if (col != "STDID")
                        {
                            var rest = row[col];
                            string sno = Convert.ToString(row[col]);
                            if (_col == 1)
                            {
                                workSheet.Cells[10 + _row, _col].Value = Convert.ToInt32(row[col]);
                            }
                            else
                            {
                                if (_col < 5)
                                {
                                    workSheet.Cells[10 + _row, _col - 1].Value = Convert.ToString(row[col]);
                                }
                                else
                                {
                                    workSheet.Cells[10 + _row, _col - 1].Value = Convert.ToInt32(row[col]);
                                }
                            }
                        }
                        else
                        {
                            _stdid = Convert.ToInt32(row[col]);
                        }

                        if (_col == 4)
                        {
                            var studentRanking = _scoreRanking.SingleOrDefault(s => s.STDID == _stdid);
                            if (studentRanking != null)
                            {
                                workSheet.Cells[10 + _row, columnCount].Value = studentRanking.SubjectCount;
                                workSheet.Cells[10 + _row, columnCount + 1].Value = studentRanking.MarkObtained;
                                workSheet.Cells[10 + _row, columnCount + 2].Value = Convert.ToDecimal(studentRanking.AverageMark).ToString("#0.00");
                                workSheet.Cells[10 + _row, columnCount + 3].Value = studentRanking.Position;
                            }                            
                        }
                    }
                }

                int _col2 = 0;
                foreach (var col in fieldnames)
                {
                    _col2++;
                    ScoreKount = 0;
                    AVGScore = 0;
                    PassKount = 0;

                    foreach (var item in broadsheetMarkList)
                    {
                        dynamic row = JsonConvert.DeserializeObject<dynamic>(item.ToString());

                        if (_col2 > 4)
                        {
                            string fname = col;
                            int _mark = Convert.ToInt32(row[col]);
                            decimal passMark = 70;

                            ScoreKount++;
                            AVGScore = AVGScore + Convert.ToDecimal(_mark);

                            if (Convert.ToDecimal(_mark) >= passMark)
                            {
                                PassKount++;
                            }
                        }
                    }

                    if (_col2 > 4)
                    {
                        workSheet.Cells[7, _col2 - 1].Value = AVGScore / ScoreKount;

                        workSheet.Cells[8, _col2 - 1].Value = PassKount.ToString("#0") + ":" + ScoreKount.ToString("#0") + "";
                        var igE1 = workSheet.IgnoredErrors.Add(workSheet.Cells[8, _col2 - 1]);
                        igE1.NumberStoredAsText = true;

                        workSheet.Cells[9, _col2 - 1].Value = ((Convert.ToDecimal(PassKount) / Convert.ToDecimal(ScoreKount)) * 100).ToString("#0") + "%";
                        var igE2 = workSheet.IgnoredErrors.Add(workSheet.Cells[9, _col2 - 1]);
                        igE2.NumberStoredAsText = true;
                    }
                }

                #endregion

                #region Formatting

                //Draw Table Lines
                using (ExcelRange Rng = workSheet.Cells[10, 1, rowCount + 10, columnCount + 3])
                {
                    Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }

                using (ExcelRange Rng = workSheet.Cells[7, 4, 9, columnCount - 1])
                {
                    Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }

                //Horizontal Alignment For S/N Column
                using (ExcelRange Rng = workSheet.Cells[10, 1, rowCount + 10, 1])
                {
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                //Horizontal Alignment For Scores Columns
                using (ExcelRange Rng = workSheet.Cells[7, 4, rowCount + 10, columnCount + 3])
                {
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                //using (ExcelRange Rng = workSheet.Cells[10, 1, rowCount + 10, columnCount + 3])
                //{
                //    Rng.Sort(2, false);
                //}

                //using (ExcelRange Rng = workSheet.Cells[10, 1, rowCount + 10, columnCount + 3])
                //{

                //}

                #endregion

                //workSheet.Cells["A10:Y24"].Sort(x => x.SortBy.Column(25, eSortOrder.Descending));
                #endregion

                fileContents = package.GetAsByteArray();
            }

            //Download PDF
            //await _jsModule.InvokeAsync<string>(
            //  "downloadPdf",
            //  broadsheetFileName,
            //  Convert.ToBase64String(fileContents)
            //  );

            await iJSRuntime.InvokeAsync<string>(
              "saveAsFile",
              broadsheetFileName,
              Convert.ToBase64String(fileContents)
              );

            await Swal.FireAsync("Export To Excel", "Operation Completed Successfully.", "success");
        }

        async Task BroadsheetScoreRanking()
        {
            _scoreRanking.Clear();
            //_studentResults = await cognitiveResultsService.GetAllAsync(" AcademicsResults/GetCognitiveResults/1");
            ShowPreview = true;
            loadingmessage = "Please wait, Exporting Result To Excel...";
            await RetrieveCognitiveResults();

            var scoreList = _studentResults.GroupBy(s => s.STDID)
                                    .OrderByDescending(avg => avg.Average(u => u.TotalMark))
                                    .Select((avg, i) => new
                                    {
                                        STDID = avg.Key,
                                        SubjectCount = avg.Count(),
                                        MarkObtained = avg.Sum(u => u.TotalMark),
                                        MarkObtainable = avg.Count() * 100,
                                        AverageMark = avg.Average(u => u.TotalMark),
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
                            AverageMark = Convert.ToDecimal(item.AverageMark),
                        });
                    }
                }

                rank++;
            }

            ShowPreview = false;
        }

        #endregion


        #endregion
    }
}
