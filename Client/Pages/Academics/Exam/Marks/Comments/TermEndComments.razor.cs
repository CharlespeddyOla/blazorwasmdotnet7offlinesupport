using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Comments;
using WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Entry;
using WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Grades;
using WebAppAcademics.Client.OfflineRepo.Admin.School;
using WebAppAcademics.Client.OfflineRepo.Settings;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Settings;
using static MudBlazor.CategoryTypes;

namespace WebAppAcademics.Client.Pages.Academics.Exam.Marks.Comments
{
    public partial class TermEndComments
    {
        #region [Injection Declaration]
        bool Initialized = false;
        bool IsOnline = true;

        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] INetworkStatus _networkStatusService { get; set; }
        [Inject] SessionsDBSyncRepo _sessionsService { get; set; }
        [Inject] SchoolDBSyncRepo _schoolService { get; set; }
        [Inject] ClassListDBSyncRepo _classListService { get; set; }
        [Inject] GeneralGradesDBSyncRepo _generalGradesListService { get; set; }
        [Inject] JuniorMockGradesDBSyncRepo _juniorMockGradesListService { get; set; }
        [Inject] SeniorMockGradesDBSyncRepo _seniorMockGradesListService { get; set; }
        [Inject] CognitiveDBSyncRepo _cognitiveMarksService { get; set; }
        [Inject] TermEndCommentsDBSyncRepo _commentsService { get; set; }


        [Inject] IAPIServices<ACDStudentsMarksCognitive> studentMarksCognitiveService { get; set; }
        [Inject] IAPIServices<ACDReportCommentsTerminal> termEndCommentService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int termid { get; set; }
        int roleid { get; set; }
        int staffid { get; set; }
        int schoolSession { get; set; }
        string schTerm { get; set; }
        int schid { get; set; }
        int classid { get; set; }
        int schoolPrincipalID { get; set; }
        int classTeacherID { get; set; }
        int ExpectedAttendance { get; set; }

        string selectedSchool { get; set; }
        string selectedClass { get; set; }
        string gradeLetter { get; set; }
        string autoCommentTeacher { get; set; }
        string autoCommentPrincipal { get; set; }

        bool IsFinalYearClass { get; set; }
        bool IsJuniorFinalYearClass { get; set; }

        decimal i { get; set; } = 0;
        int j { get; set; } = 0;
        bool IsShow { get; set; } = true;
        string progressbarInfo { get; set; } = string.Empty;

        bool isLoading { get; set; } = false;
        string loadingmessage { get; set; } = "";

        DialogOptions dialogOptions = new() { FullWidth = true };
        bool visible { get; set; }
        void Submit() => visible = false;

        string offlineprohressbarinfo { get; set; } = string.Empty;
        #endregion

        #region [Models Declaration]
        List<SETSchSessions> sessions = new();
        List<ADMSchlList> schools = new();
        List<ADMSchClassList> classListAll = new();
        List<ADMSchClassList> classList = new();
        List<ACDSettingsGrade> gradeList = new();
        List<ACDSettingsGradeMock> seniorMockGradeList = new();
        List<ACDSettingsGradeOthers> juniorMockGradeList = new();
        List<ScoreRanking> scoreRanking = new();
        List<ACDReportCommentsTerminal> commentsTermEndAll = new();
        List<ACDReportCommentsTerminal> commentsTermEnd = new();
        List<ACDStudentsMarksCognitive> cognitiveMarksAll = new();
        List<ACDStudentsMarksCognitive> cognitiveMarks = new();

        ACDReportCommentsTerminal termEndComment = new();
        ACDReportCommentsTerminal selectedTermEndComment = null;

        #endregion

        protected async void OnlineStatusChanged(object sender, OnlineStatusEventArgs args)
        {
            IsOnline = args.IsOnline;
            if (args.IsOnline == false)
            {
                // reload from IndexedDB
                schools = (await _schoolService.GetAllOfflineAsync()).ToList();
                if (schools.Count > 0)
                {
                    await OfflineListProcessing();
                }
                else
                {
                    await Swal.FireAsync("No Offline Data",
                        "No Mark Entry Data. Please, go Online To Generate Mark Entry Templates.", "error");
                }
            }
            else
            {
                if (Initialized)
                    // reload from API
                    await Task.CompletedTask;
                else
                    Initialized = true;
            }
            await InvokeAsync(StateHasChanged);
        }

        protected override async Task OnInitializedAsync()
        {
            _networkStatusService.OnlineStatusChanged += OnlineStatusChanged;
            Layout.currentPage = "End of Term Comments Entry";
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            schTerm = await localStorageService.ReadEncryptedItemAsync<string>("schTerm");
            schoolSession = await localStorageService.ReadEncryptedItemAsync<int>("schoolsession");
            staffid = await localStorageService.ReadEncryptedItemAsync<int>("staffid");
            roleid = await localStorageService.ReadEncryptedItemAsync<int>("roleid");
           
            await base.OnInitializedAsync();
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            // execute conditionally for loading data, otherwise this will load
            // every time the page refreshes
            if (firstRender)
            {
                // Do work to load page data and set properties
                if (IsOnline)
                {
                    await OnlineListProcessing();
                }
            }
        }

        #region [Section - Load Offline Data]

        async Task LoadOffLineList()
        {
            offlineprohressbarinfo = string.Empty;
            _processing = true;
            offlineprohressbarinfo = "Please wait loading offline data...";
            sessions = (await _sessionsService.GetAllOfflineAsync()).ToList();
            classListAll = (await _classListService.GetAllOfflineAsync()).ToList();
            gradeList = (await _generalGradesListService.GetAllOfflineAsync()).ToList();
            juniorMockGradeList = (await _juniorMockGradesListService.GetAllOfflineAsync()).ToList();
            seniorMockGradeList = (await _seniorMockGradesListService.GetAllOfflineAsync()).ToList();
            ExpectedAttendance = sessions.SingleOrDefault(s => s.TermID == termid).Attendance;
            SetRequestURL();
            _processing = false;
            await Swal.FireAsync("Offline Data", "Offline Data Succesfully Loaded.", "success");
        }

        async Task OfflineListProcessing()
        {
            stopwatchvalue = new TimeSpan();
            Task task1 = Task.Run(() => LoadOffLineList());
            Task task2 = Task.Run(() => StartStopWatch());

            await Task.WhenAll(task1, task2);

            task1.Dispose();
            task2.Dispose();

            await Task.CompletedTask;
        }
        #endregion

        #region [Section - Load Online Data]
        async Task LoadAllOnlineList()
        {
            _processing = true;
            offlineprohressbarinfo = "Please wait while synchronizing Offline Data with Online Data...";
            await Task.Delay(2000);
            await LoadSessions();
            await LoadSchools();
            await LoadClassList();
            await LoadGeneralGrades();
            await LoadJuniorMockGrades();
            await LoadSeniorMockGrades();
            ExpectedAttendance = sessions.SingleOrDefault(s => s.TermID == termid).Attendance;
            SetRequestURL();
            await _commentsService.SyncLocalToServer();
            _processing = false;

            await Swal.FireAsync("Data Synchronization", "Synchronization Completed Succesfully.", "success");
        }

        async Task OnlineListProcessing()
        {
            stopwatchvalue = new TimeSpan();
            Task task1 = Task.Run(() => LoadAllOnlineList());
            Task task2 = Task.Run(() => StartStopWatch());

            await Task.WhenAll(task1, task2);

            task1.Dispose();
            task2.Dispose();

            await Task.CompletedTask;
        }

        async Task LoadSessions()
        {
            if (IsOnline)
            {
                var list = await _sessionsService.GetAllAsync("Settings/GetAccademicSessions/1/0");
                if (list != null)
                {
                    sessions = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task LoadSchools()
        {
            if (IsOnline)
            {
                var list = await _schoolService.GetAllAsync("AdminSchool/GetSchools/0");
                if (list != null)
                {
                    schools = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task LoadClassList()
        {
            if (IsOnline)
            {
                var list = await _classListService.GetAllAsync("AdminSchool/GetClassList/0/0/0");
                if (list != null)
                {
                    classListAll = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task LoadGeneralGrades()
        {
            if (IsOnline)
            {
                var list = await _generalGradesListService.GetAllAsync("AcademicsMarkSettings/GetGrades/1");
                if (list != null)
                {
                    gradeList = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task LoadJuniorMockGrades()
        {
            if (IsOnline)
            {
                var list = await _juniorMockGradesListService.GetAllAsync("AcademicsMarkSettings/GeOtherGradeSettings/1");
                if (list != null)
                {
                    juniorMockGradeList = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task LoadSeniorMockGrades()
        {
            if (IsOnline)
            {
                var list = await _seniorMockGradesListService.GetAllAsync("AcademicsMarkSettings/GetMockGrades/1");
                if (list != null)
                {
                    seniorMockGradeList = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task LoadTermEndComments()
        {
            if (IsOnline)
            {
                _processing = true;
                offlineprohressbarinfo = "Please wait generating comments templates(s) for offline use...";
                commentsTermEndAll = await _commentsService.GetAllAsync("AcademicsResultsComments/GetTermEndComments/0/" +
                                                                           termid + "/0/0");
                _processing = false;
            }
        }

        async Task GenerateOfflineTemplate()
        {
            stopwatchvalue = new TimeSpan();
            Task task1 = Task.Run(() => LoadTermEndComments());
            Task task2 = Task.Run(() => StartStopWatch());

            await Task.WhenAll(task1, task2);

            task1.Dispose();
            task2.Dispose();

            await Task.CompletedTask;
        }
        #endregion

        #region [Section - Comments Processing Operations]
        async Task TermEndScoreRanking()
        {
            scoreRanking = new List<ScoreRanking>();
            scoreRanking.Clear();

            if (IsOnline)
            {
                cognitiveMarks = await studentMarksCognitiveService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/2/" +
                                                                                   termid + "/" + schid + "/" + classid + "/0/0/0");
            }
            else
            {
                cognitiveMarks = cognitiveMarksAll.Where(m => m.EntryStatus_TermEnd == true && m.ClassID == classid).ToList();
            }

            var scoreList = cognitiveMarks.Where(u => (u.Mark_CA1 + u.Mark_CA2 + u.Mark_CA3 + u.Mark_CBT + u.Mark_Exam) > 0)
                .GroupBy(s => s.STDID)
                .OrderByDescending(avg => avg.Average(u => (u.Mark_CA1 + u.Mark_CA2 + u.Mark_CA3 + u.Mark_CBT + u.Mark_Exam)))
                .Select((avg, i) => new
                {
                    STDID = avg.Key,
                    SubjectCount = avg.Count(u => (u.Mark_CA1 + u.Mark_CA2 + u.Mark_CA3 + u.Mark_CBT + u.Mark_Exam) > 0),
                    MarkObtained = avg.Sum(u => (u.Mark_CA1 + u.Mark_CA2 + u.Mark_CA3 + u.Mark_CBT + u.Mark_Exam)),
                    MarkObtainable = avg.Count(u => (u.Mark_CA1 + u.Mark_CA2 + u.Mark_CA3 + u.Mark_CBT + u.Mark_Exam) > 0) * 100,
                    AverageMark = avg.Average(u => (u.Mark_CA1 + u.Mark_CA2 + u.Mark_CA3 + u.Mark_CBT + u.Mark_Exam)),
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
                        scoreRanking.Add(new ScoreRanking()
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

        async Task RunOnlineTermEndComment()
        {
            IsShow = false;
            i = 0;
            j = 0;
            progressbarInfo = "Please wait loading End of Term Comments...";

            int maxValue = scoreRanking.Count();

            foreach (var item in scoreRanking)
            {
                j++;
                i = ((decimal)(j) / maxValue) * 100;

                if (IsJuniorFinalYearClass)
                {
                    var _grade = juniorMockGradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark);

                    if (_grade != null)
                    {
                        gradeLetter = juniorMockGradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark).GradeLetter;
                    }
                    else
                    {
                        gradeLetter = string.Empty;
                    }
                }
                else if (IsFinalYearClass)
                {
                    var _grade = seniorMockGradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark);

                    if (_grade != null)
                    {
                        gradeLetter = seniorMockGradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark).GradeLetter;
                    }
                    else
                    {
                        gradeLetter = string.Empty;
                    }
                }
                else
                {
                    var _grade = gradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark);

                    if (_grade != null)
                    {
                        gradeLetter = gradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark).GradeLetter;
                    }
                    else
                    {
                        gradeLetter = string.Empty;
                    }
                }

                var _commentsTermEnd = await termEndCommentService.GetAllAsync("AcademicsResultsComments/GetTermEndComments/2/" +
                                                                           termid + "/0/" + item.STDID);

                if (_commentsTermEnd.Count() == 0)
                {
                    //Add Student Comment
                    termEndComment.TermID = termid;
                    termEndComment.SchSession = schoolSession;
                    termEndComment.ClassID = classid;
                    termEndComment.ClassTeacherID = classTeacherID;
                    termEndComment.STDID = item.STDID;
                    termEndComment.Attendance = ExpectedAttendance;
                    termEndComment.DaysAbsent = 0;
                    termEndComment.Comments_Teacher = string.Empty;
                    termEndComment.Comments_Principal = string.Empty;
                    termEndComment.AVGPer = item.AverageMark;
                    termEndComment.Position = item.Position;
                    termEndComment.Grade = gradeLetter;
                    termEndComment.MarkObtainable = item.MarkObtainable;
                    termEndComment.MarkObtained = item.MarkObtained;

                    var response = await termEndCommentService.SaveAsync("AcademicsResultsComments/AddTermEndComment/", termEndComment);
                    termEndComment.CommentID = response.CommentID;
                    termEndComment.Id = response.CommentID;
                    await termEndCommentService.UpdateAsync("AcademicsResultsComments/UpdateTermEndComment/", 4, termEndComment);
                }
                else
                {
                    //Update Student Comment
                    termEndComment.CommentID = _commentsTermEnd.FirstOrDefault().CommentID; ;
                    termEndComment.ClassID = classid;
                    termEndComment.ClassTeacherID = classTeacherID;
                    termEndComment.MarkObtained = item.MarkObtained;
                    termEndComment.MarkObtainable = item.SubjectCount;
                    termEndComment.Attendance = ExpectedAttendance;
                    termEndComment.DaysAbsent = _commentsTermEnd.FirstOrDefault().DaysAbsent;
                    termEndComment.Comments_Teacher = _commentsTermEnd.FirstOrDefault().Comments_Teacher;
                    termEndComment.Comments_Principal = _commentsTermEnd.FirstOrDefault().Comments_Principal;
                    termEndComment.AVGPer = item.AverageMark; // pos.AVGPer;
                    termEndComment.Position = item.Position;
                    termEndComment.Grade = gradeLetter;

                    await termEndCommentService.UpdateAsync("AcademicsResultsComments/UpdateTermEndComment/", 1, termEndComment);
                }
                StateHasChanged();
            }

            commentsTermEnd = await termEndCommentService.GetAllAsync("AcademicsResultsComments/GetTermEndComments/1/" +
                                                                      termid + "/" + classid + "/0");
            IsShow = true;
        }

        async Task RunOfflineTermEndComment()
        {
            IsShow = false;
            i = 0;
            j = 0;
            progressbarInfo = "Please wait loading End of Term Comments...";

            int maxValue = scoreRanking.Count();
            int sn = 1;
            foreach (var item in scoreRanking)
            {
                j++;
                i = ((decimal)(j) / maxValue) * 100;

                if (IsJuniorFinalYearClass)
                {
                    var _grade = juniorMockGradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark);

                    if (_grade != null)
                    {
                        gradeLetter = juniorMockGradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark).GradeLetter;
                    }
                    else
                    {
                        gradeLetter = string.Empty;
                    }
                }
                else if (IsFinalYearClass)
                {
                    var _grade = seniorMockGradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark);

                    if (_grade != null)
                    {
                        gradeLetter = seniorMockGradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark).GradeLetter;
                    }
                    else
                    {
                        gradeLetter = string.Empty;
                    }
                }
                else
                {
                    var _grade = gradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark);

                    if (_grade != null)
                    {
                        gradeLetter = gradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark).GradeLetter;
                    }
                    else
                    {
                        gradeLetter = string.Empty;
                    }
                }

                commentsTermEnd.Add(new ACDReportCommentsTerminal
                {
                    SN = sn++,
                    CommentID = commentsTermEndAll.SingleOrDefault(c => c.STDID == item.STDID).CommentID,
                    TermID = commentsTermEndAll.SingleOrDefault(c => c.STDID == item.STDID).TermID,
                    ClassID = commentsTermEndAll.SingleOrDefault(c => c.STDID == item.STDID).ClassID,
                    ClassTeacherID = commentsTermEndAll.SingleOrDefault(c => c.STDID == item.STDID).ClassTeacherID,
                    STDID = commentsTermEndAll.SingleOrDefault(c => c.STDID == item.STDID).STDID,
                    AdmissionNo = commentsTermEndAll.SingleOrDefault(c => c.STDID == item.STDID).AdmissionNo,
                    StudentName = commentsTermEndAll.SingleOrDefault(c => c.STDID == item.STDID).StudentName,
                    Attendance = ExpectedAttendance,
                    DaysAbsent = commentsTermEndAll.SingleOrDefault(c => c.STDID == item.STDID).DaysAbsent,
                    Comments_Teacher = commentsTermEndAll.SingleOrDefault(c => c.STDID == item.STDID).Comments_Teacher,
                    Comments_Principal = commentsTermEndAll.SingleOrDefault(c => c.STDID == item.STDID).Comments_Principal,
                    AVGPer = item.AverageMark, // pos.AVGPer;
                    Position = item.Position,
                    Grade = gradeLetter,
                    MarkObtained = item.MarkObtained,
                    MarkObtainable = item.SubjectCount,
                    Id = commentsTermEndAll.SingleOrDefault(c => c.STDID == item.STDID).Id
                });

                StateHasChanged();
            }

            IsShow = true;

            await Task.CompletedTask;
        }

        async Task RunTermEndComments()
        {
            isLoading = true;
            loadingmessage = "Please wait, computing grades...";
            if (IsOnline)
            {
                //await LoadCognitiveMarks();
                await TermEndScoreRanking();
                await RunOnlineTermEndComment();
            }
            else
            {
                //cognitiveMarksAll = (await _cognitiveMarksService.GetAllOfflineAsync()).ToList();
                cognitiveMarks = cognitiveMarksAll.Where(m => m.EntryStatus_TermEnd == true && m.ClassID == classid).ToList();
                await TermEndScoreRanking();
                commentsTermEndAll = (await _commentsService.GetAllOfflineAsync()).ToList();
                await RunOfflineTermEndComment();
            }
            isLoading = false;
        }

        void SelectAutoComments()
        {
            visible = true;
        }

        void ApplyTermEndAutoComments(int selectedOpt)
        {
            foreach (var item in commentsTermEnd)
            {
                var _autoComments = gradeList.FirstOrDefault(g => g.LowerGrade <= item.AVGPer && g.HigherGrade >= item.AVGPer);

                if (_autoComments == null)
                {
                    autoCommentTeacher = string.Empty;
                    autoCommentPrincipal = string.Empty;
                }
                else
                {
                    autoCommentTeacher = _autoComments.TeachersComment;
                    autoCommentPrincipal = _autoComments.PrincipalComment;
                }

                switch (selectedOpt)
                {
                    case 1:
                        commentsTermEnd.FirstOrDefault(s => s.STDID == item.STDID).Comments_Teacher = autoCommentTeacher;
                        break;
                    case 2:
                        commentsTermEnd.FirstOrDefault(s => s.STDID == item.STDID).Comments_Principal = autoCommentPrincipal;
                        break;
                    case 3:
                        commentsTermEnd.FirstOrDefault(s => s.STDID == item.STDID).Comments_Teacher = autoCommentTeacher;
                        commentsTermEnd.FirstOrDefault(s => s.STDID == item.STDID).Comments_Principal = autoCommentPrincipal;
                        break;
                }
            }
        }

        async Task CommittedItemChanges(ACDReportCommentsTerminal item)
        {
            termEndComment.CommentID = item.CommentID;
            termEndComment.TermID = item.TermID;
            termEndComment.ClassID = item.ClassID;
            termEndComment.ClassTeacherID = item.ClassTeacherID;
            termEndComment.STDID = item.STDID;
            termEndComment.AdmissionNo = item.AdmissionNo;
            termEndComment.StudentName = item.StudentName;
            termEndComment.Attendance = item.Attendance;
            termEndComment.Comments_Teacher = item.Comments_Teacher;
            termEndComment.Comments_Principal = item.Comments_Principal;
            termEndComment.DaysAbsent = item.DaysAbsent;
            termEndComment.AVGPer = item.AVGPer;
            termEndComment.Position = item.Position;
            termEndComment.Grade = item.Grade;
            termEndComment.MarkObtained = item.MarkObtained;
            termEndComment.MarkObtainable = item.MarkObtainable;
            termEndComment.Id = item.Id;

            await _commentsService.UpdateAsync("AcademicsResultsComments/UpdateTermEndComment/", 2, termEndComment);
        }

        async Task UpdateTermEndComments()
        {            
            termEndComment.CommentID = selectedTermEndComment.CommentID;
            termEndComment.TermID = selectedTermEndComment.TermID;            
            termEndComment.ClassID = selectedTermEndComment.ClassID;
            termEndComment.ClassTeacherID = selectedTermEndComment.ClassTeacherID;
            termEndComment.STDID = selectedTermEndComment.STDID;
            termEndComment.AdmissionNo = selectedTermEndComment.AdmissionNo;
            termEndComment.StudentName = selectedTermEndComment.StudentName;             
            termEndComment.Attendance = selectedTermEndComment.Attendance;
            termEndComment.Comments_Teacher = selectedTermEndComment.Comments_Teacher;
            termEndComment.Comments_Principal = selectedTermEndComment.Comments_Principal;
            termEndComment.DaysAbsent = selectedTermEndComment.DaysAbsent;
            termEndComment.AVGPer = selectedTermEndComment.AVGPer;
            termEndComment.Position = selectedTermEndComment.Position;
            termEndComment.Grade = selectedTermEndComment.Grade;
            termEndComment.MarkObtained = selectedTermEndComment.MarkObtained;
            termEndComment.MarkObtainable = selectedTermEndComment.MarkObtainable;
            termEndComment.Id = selectedTermEndComment.Id;

            await _commentsService.UpdateAsync("AcademicsResultsComments/UpdateTermEndComment/", 2, termEndComment);
            Snackbar.Add("Comments For " + selectedTermEndComment.StudentName + " Has Been Successfully Saved");
        }

        async Task SaveTermEndComments()
        {
            if (!String.IsNullOrWhiteSpace(selectedClass))
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Save Comments Operation",
                    Text = "Do You Want To Continue With This Operation?",
                    Icon = SweetAlertIcon.Warning,
                    ShowCancelButton = true,
                    ConfirmButtonText = "Yes, Contnue!",
                    CancelButtonText = "No"
                });

                if (result.IsConfirmed)
                {
                    IsShow = false;
                    i = 0;
                    j = 0;
                    progressbarInfo = "Please wait Saving Comments...";
                    int maxValue = commentsTermEnd.Count();

                    foreach (var item in commentsTermEnd)
                    {
                        j++;
                        i = ((decimal)(j) / maxValue) * 100;

                        termEndComment.CommentID = item.CommentID;
                        termEndComment.TermID = item.TermID;
                        termEndComment.ClassID = item.ClassID;
                        termEndComment.ClassTeacherID = item.ClassTeacherID;
                        termEndComment.STDID = item.STDID;
                        termEndComment.AdmissionNo = item.AdmissionNo;
                        termEndComment.StudentName = item.StudentName;                        
                        termEndComment.Attendance = item.Attendance;
                        termEndComment.Comments_Teacher = item.Comments_Teacher;
                        termEndComment.Comments_Principal = item.Comments_Principal;
                        termEndComment.DaysAbsent = item.DaysAbsent;
                        termEndComment.AVGPer = item.AVGPer;
                        termEndComment.Position = item.Position;
                        termEndComment.Grade = item.Grade;
                        termEndComment.MarkObtained = item.MarkObtained;
                        termEndComment.MarkObtainable = item.MarkObtainable;
                        termEndComment.Id = item.Id;
                        await _commentsService.UpdateAsync("AcademicsResultsComments/UpdateTermEndComment/", 2, termEndComment);
                        StateHasChanged();
                    }

                    IsShow = true;
                    await Swal.FireAsync("Save Comments", "Teacher's Comments For All The Student In The Selected Class Successfully Saved.", "success");
                }
            }
            else
            {
                await Swal.FireAsync("End of Term Comments", "Please select a class", "info");
            }
        }

        async Task Refresh()
        {
            selectedSchool = string.Empty;
            schid = 0;
            schools.Clear();
            await LoadSchools();

            if (!IsOnline)
            {
                schools = (await _schoolService.GetAllOfflineAsync()).ToList();
            }
            
            selectedClass = string.Empty;
            classid = 0;
            classList.Clear();

            cognitiveMarks.Clear();
            commentsTermEnd.Clear();
        }


        #endregion

        #region [Section - Click Events]
        async Task OnSchoolChanged(IEnumerable<string> e)
        {
            selectedSchool = e.ElementAt(0);
            schid = schools.FirstOrDefault(s => s.School == selectedSchool).SchID;
            schoolPrincipalID = schools.FirstOrDefault(s => s.School == selectedSchool).StaffID;

            classid = 0;
            classTeacherID = 0;
            selectedClass = string.Empty;
            classList.Clear();

            if (roleid == 1)
            {
                classList = classListAll.Where(c => c.SchID == schid).ToList();
            }
            else
            {
                classList = classListAll.Where(c => c.SchID == schid && c.StaffID == staffid).ToList();
            }

            cognitiveMarks.Clear();
            commentsTermEnd.Clear();

            await Task.CompletedTask;
        }

        async Task OnClassChanged(IEnumerable<string> e)
        {
            selectedClass = e.ElementAt(0);
            classid = classList.FirstOrDefault(c => c.ClassName == selectedClass).ClassID;
            classTeacherID = classList.FirstOrDefault(c => c.ClassName == selectedClass).StaffID;
            IsJuniorFinalYearClass = classList.FirstOrDefault(c => c.ClassID == classid).JuniorFinalYearClass;
            IsFinalYearClass = classList.FirstOrDefault(c => c.ClassID == classid).FinalYearClass;

            cognitiveMarks.Clear();
            commentsTermEnd.Clear();

            if (!IsOnline)
            {
                cognitiveMarksAll = (await _cognitiveMarksService.GetAllOfflineAsync()).ToList();
                if (cognitiveMarksAll.Count == 0)
                {
                    await Swal.FireAsync("End of Term Comments", "Cogtive Marks Not Loaded for Offline Use. " +
                        "Please go Online to generate Mark Entry Template(s) for Cognitive Marks.", "error");
                }
                else
                {
                    await RunTermEndComments();
                }
            }
            else
            {
                await RunTermEndComments();
            }
        }

        async Task GenerateMarkTemplates()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Offline Mark Entry",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                if (!string.IsNullOrEmpty(result.Value))
                {
                    await GenerateOfflineTemplate();
                    await Swal.FireAsync("Offline Mark Entry", "Offline Mark Entry Template(s) Successfully Generated.", "success");
                }
            }
        }

        #endregion

        #region [Section - Progress Bar: Timer]
        private bool _processing = false;
        TimeSpan stopwatchvalue = new();
        bool Is_stopwatchrunning { get; set; } = false;
        //int val = 0;    

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
                        //val = (int)stopwatchvalue.TotalHours + (int)stopwatchvalue.TotalMinutes + (int)stopwatchvalue.TotalSeconds;
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

        void SetRequestURL()
        {
            APICallParameters.Id = 1;
            APICallParameters.RequestUriUpdate = "AcademicsResultsComments/UpdateTermEndComment/";
        }

        void IDisposable.Dispose()
        {
            _networkStatusService.OnlineStatusChanged -= OnlineStatusChanged;
        }
    }
}
