using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Comments;
using WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Entry;
using WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Grades;
using WebAppAcademics.Client.OfflineRepo.Admin.School;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Administration.School;
using static MudBlazor.CategoryTypes;

namespace WebAppAcademics.Client.Pages.Academics.Exam.Marks.Comments
{
    public partial class CheckPointComments
    {
        #region [Injection Declaration]
        bool Initialized = false;
        bool IsOnline = true;

        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] INetworkStatus _networkStatusService { get; set; }
        [Inject] SchoolDBSyncRepo _schoolService { get; set; }
        [Inject] ClassListDBSyncRepo _classListService { get; set; }
        [Inject] CheckPointGradesDBSyncRepo _checkpointGradesService { get; set; }
        [Inject] IGCSEGradesDBSyncRepo _igcseGradesService { get; set; }
        [Inject] CognitiveDBSyncRepo _cognitiveMarksService { get; set; }
        [Inject] CheckPointIGCSECommentsDBSyncRepo _commentsService { get; set; }

        [Inject] IAPIServices<ACDStudentsMarksCognitive> studentMarksCognitiveService { get; set; }
        [Inject] IAPIServices<ACDReportCommentCheckPointIGCSE> checkpointigcseCommentService { get; set; }


        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int termid { get; set; }
        int roleid { get; set; }
        int staffid { get; set; }
        string schTerm { get; set; }
        int schid { get; set; }
        int classid { get; set; }
        int classTeacherID { get; set; }
        int schoolSession { get; set; }

        string selectedSchool { get; set; }
        string selectedClass { get; set; }
        string gradeLetter { get; set; }

        bool IsCheckPointClass { get; set; }
        bool IsIGCSEClass { get; set; }

        decimal i { get; set; } = 0;
        int j { get; set; } = 0;
        bool IsShow { get; set; } = true;
        string progressbarInfo { get; set; } = string.Empty;

        bool isLoading { get; set; } = false;
        string loadingmessage { get; set; } = "";

        string offlineprohressbarinfo { get; set; } = string.Empty;
        #endregion

        #region [Models Declaration]
        List<ADMSchlList> schools = new();
        List<ADMSchClassList> classListAll = new();
        List<ADMSchClassList> classList = new();
        List<ACDSettingsGradeCheckPoint> checkpointGradeList = new();
        List<ACDSettingsGradeIGCSE> igcseGradeList = new();
        List<ACDStudentsMarksCognitive> cognitiveMarksAll = new();
        List<ACDStudentsMarksCognitive> cognitiveMarks = new();
        List<ScoreRanking> scoreRanking = new();
        List<ACDReportCommentCheckPointIGCSE> commentsCheckPointIGCSEAll = new();
        List<ACDReportCommentCheckPointIGCSE> commentsCheckPointIGCSE = new();

        ACDReportCommentCheckPointIGCSE checkpoinigcseComment = new();
        ACDReportCommentCheckPointIGCSE selectedCheckPointIGCSEComment = null;

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
            Layout.currentPage = "Check Point / IGCSE Comments Entry";
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
            classListAll = (await _classListService.GetAllOfflineAsync()).ToList();
            checkpointGradeList = (await _checkpointGradesService.GetAllOfflineAsync()).ToList();
            igcseGradeList = (await _igcseGradesService.GetAllOfflineAsync()).ToList();
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
            await LoadSchools();
            await LoadClassList();
            await LoadCheckPointGrades();
            await LoadIGCSEGrades();
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

        async Task LoadCheckPointGrades()
        {
            if (IsOnline)
            {
                var list = await _checkpointGradesService.GetAllAsync("AcademicsMarkSettings/GetCheckPointGrades/1");
                if (list != null)
                {
                    checkpointGradeList = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task LoadIGCSEGrades()
        {
            if (IsOnline)
            {
                var list = await _igcseGradesService.GetAllAsync("AcademicsMarkSettings/GetIGCSEGrades/1");
                if (list != null)
                {
                    igcseGradeList = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task LoadCheckPointIGCSEComments()
        {
            if (IsOnline)
            {
                _processing = true;
                offlineprohressbarinfo = "Please wait generating comments templates(s) for offline use...";
                commentsCheckPointIGCSEAll = await _commentsService.GetAllAsync(
                    "AcademicsResultsComments/GetCheckPointIGCSEComments/0/" +  termid + "/0/0");
                _processing = false;
            }
        }

        async Task GenerateOfflineTemplate()
        {
            stopwatchvalue = new TimeSpan();
            Task task1 = Task.Run(() => LoadCheckPointIGCSEComments());
            Task task2 = Task.Run(() => StartStopWatch());

            await Task.WhenAll(task1, task2);

            task1.Dispose();
            task2.Dispose();

            await Task.CompletedTask;
        }
        #endregion

        #region [Section - Comments Processing Operations]
        async Task CheckPointIGCSEScoreRanking()
        {
            scoreRanking = new List<ScoreRanking>();
            scoreRanking.Clear();

            if (IsOnline)
            {
                cognitiveMarks = await studentMarksCognitiveService.GetAllAsync(
                    "AcademicsMarks/GetCognitiveMarks/3/" + termid + "/" + schid + "/" + classid + "/0/0/0");
            }
            else
            {
                cognitiveMarks = cognitiveMarksAll.Where(m => m.EntryStatus_ICGCS == true && m.ClassID == classid).ToList();
            }

            var scoreList = cognitiveMarks
                .GroupBy(s => s.STDID)
                .OrderByDescending(avg => avg.Average(u => u.Mark_ICGC))
                .Select((avg, i) => new
                {
                    STDID = avg.Key,
                    SubjectCount = avg.Count(),
                    MarkObtained = avg.Sum(u => u.Mark_ICGC),
                    MarkObtainable = avg.Count() * 100,
                    AverageMark = avg.Average(u => u.Mark_ICGC),
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

        async Task RunOnlineCheckPointIGCSEComment()
        {
            IsShow = false;
            i = 0;
            j = 0;
            progressbarInfo = "Please wait loading CheckPoint Or IGCSE Comments...";

            int maxValue = scoreRanking.Count();

            foreach (var item in scoreRanking)
            {
                j++;
                i = ((decimal)(j) / maxValue) * 100;

                if (IsCheckPointClass)
                {
                    var _grade = checkpointGradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark);

                    if (_grade != null)
                    {
                        gradeLetter = checkpointGradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark).GradeLetter;
                    }
                    else
                    {
                        gradeLetter = string.Empty;
                    }
                }
                else if (IsIGCSEClass)
                {
                    var _grade = igcseGradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark);

                    if (_grade != null)
                    {
                        gradeLetter = igcseGradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark).GradeLetter;
                    }
                    else
                    {
                        gradeLetter = string.Empty;
                    }
                }

                if (IsCheckPointClass || IsIGCSEClass)
                {
                    var _commentsCheckPointIGCSE = await checkpointigcseCommentService.GetAllAsync(
                                                                          "AcademicsResultsComments/GetCheckPointIGCSEComments/2/" +
                                                                          termid + "/0/" + item.STDID);

                    if (_commentsCheckPointIGCSE.Count() == 0)
                    {
                        //Add Student Comment
                        checkpoinigcseComment.TermID = termid;
                        checkpoinigcseComment.SchSession = schoolSession;
                        checkpoinigcseComment.ClassID = classid;
                        checkpoinigcseComment.ClassTeacherID = classTeacherID;
                        checkpoinigcseComment.STDID = item.STDID;
                        checkpoinigcseComment.Comments = string.Empty;
                        checkpoinigcseComment.AVGPer = item.AverageMark;
                        checkpoinigcseComment.Position = item.Position;
                        checkpoinigcseComment.Grade = gradeLetter;

                        var response = await checkpointigcseCommentService.SaveAsync("AcademicsResultsComments/AddCheckPointIGCSEComment/", checkpoinigcseComment);
                        checkpoinigcseComment.CommentID = response.CommentID;
                        checkpoinigcseComment.Id = response.CommentID;
                        await checkpointigcseCommentService.UpdateAsync("AcademicsResultsComments/UpdateCheckPointIGCSEComment/", 4, checkpoinigcseComment);
                    }
                    else
                    {
                        //Update Student Comment
                        checkpoinigcseComment.CommentID = _commentsCheckPointIGCSE.FirstOrDefault().CommentID; ;
                        checkpoinigcseComment.Comments = _commentsCheckPointIGCSE.FirstOrDefault().Comments;
                        checkpoinigcseComment.AVGPer = item.AverageMark; // pos.AVGPer;
                        checkpoinigcseComment.Position = item.Position;
                        checkpoinigcseComment.Grade = gradeLetter;

                        await checkpointigcseCommentService.UpdateAsync("AcademicsResultsComments/UpdateCheckPointIGCSEComment/", 1, checkpoinigcseComment);
                    }
                }

                StateHasChanged();
            }
                       
            commentsCheckPointIGCSE = await checkpointigcseCommentService.GetAllAsync(
                "AcademicsResultsComments/GetCheckPointIGCSEComments/1/" + termid + "/" + classid + "/0");

            IsShow = true;
        }

        async Task RunOfflineCheckPointIGCSEComment()
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

                if (IsCheckPointClass)
                {
                    var _grade = checkpointGradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark);

                    if (_grade != null)
                    {
                        gradeLetter = checkpointGradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark).GradeLetter;
                    }
                    else
                    {
                        gradeLetter = string.Empty;
                    }
                }
                else if (IsIGCSEClass)
                {
                    var _grade = igcseGradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark);

                    if (_grade != null)
                    {
                        gradeLetter = igcseGradeList.FirstOrDefault(g => g.LowerGrade <= item.AverageMark && g.HigherGrade >= item.AverageMark).GradeLetter;
                    }
                    else
                    {
                        gradeLetter = string.Empty;
                    }
                }

                commentsCheckPointIGCSE.Add(new ACDReportCommentCheckPointIGCSE
                {
                    SN = sn++,
                    CommentID = commentsCheckPointIGCSEAll.SingleOrDefault(c => c.STDID == item.STDID).CommentID,
                    TermID = commentsCheckPointIGCSEAll.SingleOrDefault(c => c.STDID == item.STDID).TermID,
                    ClassID = commentsCheckPointIGCSEAll.SingleOrDefault(c => c.STDID == item.STDID).ClassID,
                    ClassTeacherID = commentsCheckPointIGCSEAll.SingleOrDefault(c => c.STDID == item.STDID).ClassTeacherID,
                    STDID = commentsCheckPointIGCSEAll.SingleOrDefault(c => c.STDID == item.STDID).STDID,
                    AdmissionNo = commentsCheckPointIGCSEAll.SingleOrDefault(c => c.STDID == item.STDID).AdmissionNo,
                    StudentName = commentsCheckPointIGCSEAll.SingleOrDefault(c => c.STDID == item.STDID).StudentName,
                    Comments = commentsCheckPointIGCSEAll.SingleOrDefault(c => c.STDID == item.STDID).Comments,
                    AVGPer = item.AverageMark, // pos.AVGPer;
                    Position = item.Position,
                    Grade = gradeLetter,
                    Id = commentsCheckPointIGCSEAll.SingleOrDefault(c => c.STDID == item.STDID).Id
                });

                StateHasChanged();
            }

            IsShow = true;
            await Task.CompletedTask;
        }

        async Task RunCheckPointIGCSEComments()
        {
            isLoading = true;
            loadingmessage = "Please wait, computing grades...";
            if (IsOnline)
            {
                await CheckPointIGCSEScoreRanking();
                await RunOnlineCheckPointIGCSEComment();
            }
            else
            {
                cognitiveMarks = cognitiveMarksAll.Where(m => m.EntryStatus_ICGCS == true && m.ClassID == classid).ToList();
                await CheckPointIGCSEScoreRanking();
                commentsCheckPointIGCSEAll = (await _commentsService.GetAllOfflineAsync()).ToList();
                await RunOfflineCheckPointIGCSEComment();
            }
            isLoading = false;
        }

        void CgeckPointIGCSEAutoComments()
        {
            string _applyAutoComments = string.Empty;

            foreach (var item in commentsCheckPointIGCSE)
            {
                decimal _AverageMark = item.AVGPer;

                if (IsCheckPointClass)
                {
                    var _grade = checkpointGradeList.FirstOrDefault(g => g.LowerGrade <= _AverageMark && g.HigherGrade >= _AverageMark);

                    if (_grade != null)
                    {
                        _applyAutoComments = checkpointGradeList.FirstOrDefault(g => g.LowerGrade <= _AverageMark && g.HigherGrade >= _AverageMark).AutoComments;
                    }
                    else
                    {
                        _applyAutoComments = string.Empty;
                    }
                }
                else if (IsIGCSEClass)
                {
                    var _grade = igcseGradeList.FirstOrDefault(g => g.LowerGrade <= _AverageMark && g.HigherGrade >= _AverageMark);

                    if (_grade != null)
                    {
                        _applyAutoComments = igcseGradeList.FirstOrDefault(g => g.LowerGrade <= _AverageMark && g.HigherGrade >= _AverageMark).AutoComments;
                    }
                    else
                    {
                        _applyAutoComments = string.Empty;
                    }
                }

                commentsCheckPointIGCSE.FirstOrDefault(s => s.STDID == item.STDID).Comments = _applyAutoComments;
            }
        }

        async Task ApplyAutoCommentsCgeckPointIGCSE()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Apply Auto Comments Operation",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                CgeckPointIGCSEAutoComments();
            }
        }

        async Task UpdateCheckPointIGCSEComment()
        {
            checkpoinigcseComment.CommentID = selectedCheckPointIGCSEComment.CommentID;
            checkpoinigcseComment.TermID = selectedCheckPointIGCSEComment.TermID;
            checkpoinigcseComment.ClassID = selectedCheckPointIGCSEComment.ClassID;
            checkpoinigcseComment.ClassTeacherID = selectedCheckPointIGCSEComment.ClassTeacherID;
            checkpoinigcseComment.STDID = selectedCheckPointIGCSEComment.STDID;
            checkpoinigcseComment.AdmissionNo = selectedCheckPointIGCSEComment.AdmissionNo;
            checkpoinigcseComment.StudentName = selectedCheckPointIGCSEComment.StudentName;
            checkpoinigcseComment.Comments = selectedCheckPointIGCSEComment.Comments;
            checkpoinigcseComment.AVGPer = selectedCheckPointIGCSEComment.AVGPer; // pos.AVGPer;
            checkpoinigcseComment.Position = selectedCheckPointIGCSEComment.Position;
            checkpoinigcseComment.Grade = selectedCheckPointIGCSEComment.Grade;
            checkpoinigcseComment.Id = selectedCheckPointIGCSEComment.Id;
            await checkpointigcseCommentService.UpdateAsync("AcademicsResultsComments/UpdateCheckPointIGCSEComment/", 2, checkpoinigcseComment);
            Snackbar.Add("Comment For " + selectedCheckPointIGCSEComment.StudentName + " Has Been Successfully Saved");
        }

        async Task SaveCheckPointIGCSEComments()
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
                    int maxValue = commentsCheckPointIGCSE.Count();

                    foreach (var item in commentsCheckPointIGCSE)
                    {
                        j++;
                        i = ((decimal)(j) / maxValue) * 100;

                        checkpoinigcseComment.CommentID = item.CommentID;
                        checkpoinigcseComment.TermID = item.TermID;
                        checkpoinigcseComment.ClassID = item.ClassID;
                        checkpoinigcseComment.ClassTeacherID = item.ClassTeacherID;
                        checkpoinigcseComment.STDID = item.STDID;
                        checkpoinigcseComment.AdmissionNo = item.AdmissionNo;
                        checkpoinigcseComment.StudentName = item.StudentName;
                        checkpoinigcseComment.Comments = item.Comments;
                        checkpoinigcseComment.AVGPer = item.AVGPer; // pos.AVGPer;
                        checkpoinigcseComment.Position = item.Position;
                        checkpoinigcseComment.Grade = item.Grade;
                        checkpoinigcseComment.Id = item.Id;
                        await checkpointigcseCommentService.UpdateAsync("AcademicsResultsComments/UpdateCheckPointIGCSEComment/", 2, checkpoinigcseComment);
                        StateHasChanged();
                    }

                    IsShow = true;
                    await Swal.FireAsync("Save Comments", "Teacher's Comments For All The Student In The Selected Class Successfully Saved.", "success");
                }
            }
            else
            {
                await Swal.FireAsync("CheckPoint / IGCSE Comments", "Please select a class", "info");
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
            commentsCheckPointIGCSE.Clear();
        }

        #endregion

        #region [Section - Click Events]
        async Task OnSchoolChanged(IEnumerable<string> e)
        {
            selectedSchool = e.ElementAt(0);
            schid = schools.FirstOrDefault(s => s.School == selectedSchool).SchID;

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
            commentsCheckPointIGCSE.Clear();

            await Task.CompletedTask;
        }

        async Task OnClassChanged(IEnumerable<string> e)
        {
            selectedClass = e.ElementAt(0);
            classid = classList.FirstOrDefault(c => c.ClassName == selectedClass).ClassID;
            classTeacherID = classList.FirstOrDefault(c => c.ClassName == selectedClass).StaffID;
            IsCheckPointClass = classList.FirstOrDefault(c => c.ClassID == classid).CheckPointClass;
            IsIGCSEClass = classList.FirstOrDefault(c => c.ClassID == classid).IGCSEClass;

            cognitiveMarks.Clear();
            commentsCheckPointIGCSE.Clear();

            if (!IsOnline)
            {
                cognitiveMarksAll = (await _cognitiveMarksService.GetAllOfflineAsync()).ToList();
                if (cognitiveMarksAll.Count == 0)
                {
                    await Swal.FireAsync("CheckPoint / IGCSE Comments", "Cogtive Marks Not Loaded for Offline Use. " +
                        "Please go Online to generate Mark Entry Template(s) for Cognitive Marks.", "error");
                }
                else
                {
                    await RunCheckPointIGCSEComments();
                }
            }
            else
            {
                await RunCheckPointIGCSEComments();
            }
        }

        string SelectedCommentType()
        {
            string result = string.Empty;

            if (IsCheckPointClass)
            {
                result = "Check Point Comments for " + schTerm + " Term";
            }
            else if (IsIGCSEClass)
            {
                result = "IGSCE Comments for " + schTerm + " Term";
            }

            return result;
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
            APICallParameters.RequestUriUpdate = "AcademicsResultsComments/UpdateCheckPointIGCSEComment/";
        }

        void IDisposable.Dispose()
        {
            _networkStatusService.OnlineStatusChanged -= OnlineStatusChanged;
        }
    }
}
