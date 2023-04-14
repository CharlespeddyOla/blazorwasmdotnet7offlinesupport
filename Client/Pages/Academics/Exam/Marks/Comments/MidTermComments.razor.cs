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
using static MudBlazor.CategoryTypes;

namespace WebAppAcademics.Client.Pages.Academics.Exam.Marks.Comments
{
    public partial class MidTermComments
    {
        #region [Injection Declaration]
        bool Initialized = false;
        bool IsOnline = true;

        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] INetworkStatus _networkStatusService { get; set; }
        [Inject] SchoolDBSyncRepo _schoolService { get; set; }
        [Inject] ClassListDBSyncRepo _classListService { get; set; }
        [Inject] GeneralGradesDBSyncRepo _generalGradesListService { get; set; }
        [Inject] JuniorMockGradesDBSyncRepo _juniorMockGradesListService { get; set; }
        [Inject] SeniorMockGradesDBSyncRepo _seniorMockGradesListService { get; set; }
        [Inject] CognitiveDBSyncRepo _cognitiveMarksService { get; set; }
        [Inject] MidTermCommentsDBSyncRepo _commentsService { get; set; }


        [Inject] IAPIServices<ACDStudentsMarksCognitive> studentMarksCognitiveService { get; set; }
        [Inject] IAPIServices<ACDReportCommentMidTerm> midTermCommentService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int termid { get; set; }
        int roleid { get; set; }
        int staffid { get; set; }
        int schoolSession { get; set; }
        int schid { get; set; }
        int classid { get; set; }
        int classTeacherID { get; set; }

        string selectedSchool { get; set; }
        string selectedClass { get; set; }
        string gradeLetter { get; set; }
        string autoCommentTeacher { get; set; }
        string schTerm { get; set; }

        bool IsFinalYearClass { get; set; }
        bool IsJuniorFinalYearClass { get; set; }

        decimal i { get; set; } = 0;
        int j { get; set; } = 0;
        bool IsShow { get; set; } = true;
        string progressbarInfo { get; set; } = string.Empty;

        bool isLoading { get; set; } = false;
        string loadingmessage { get; set; } = "Waiting for your selection...";

        string offlineprohressbarinfo { get; set; } = string.Empty;
        #endregion

        #region [Models Declaration]
        List<ADMSchlList> schools = new();
        List<ADMSchClassList> classListAll = new();
        List<ADMSchClassList> classList = new();
        List<ACDSettingsGrade> gradeList = new();
        List<ACDSettingsGradeMock> seniorMockGradeList = new();
        List<ACDSettingsGradeOthers> juniorMockGradeList = new();
        List<ACDSettingsGradeCheckPoint> checkpointGradeList = new();
        List<ACDSettingsGradeIGCSE> igcseGradeList = new();
        List<ACDStudentsMarksCognitive> cognitiveMarksAll = new();
        List<ACDStudentsMarksCognitive> cognitiveMarks = new();
        List<ACDReportCommentMidTerm> commentsMidTermAll = new();
        List<ACDReportCommentMidTerm> commentsMidTerm = new();        
        List<ScoreRanking> scoreRanking = new();

        ACDReportCommentMidTerm midTermComment = new();
        ACDReportCommentMidTerm selectedItemMidTermComment = null;

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
            Layout.currentPage = "Mid-Term Comments Entry";
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            schTerm = await localStorageService.ReadEncryptedItemAsync<string>("schTerm");
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
            gradeList = (await _generalGradesListService.GetAllOfflineAsync()).ToList();
            juniorMockGradeList = (await _juniorMockGradesListService.GetAllOfflineAsync()).ToList();
            seniorMockGradeList = (await _seniorMockGradesListService.GetAllOfflineAsync()).ToList();
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
            await LoadGeneralGrades();
            await LoadJuniorMockGrades();
            await LoadSeniorMockGrades();
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

        async Task LoadMidTernComments()
        {
            if (IsOnline)
            {
                _processing = true;
                offlineprohressbarinfo = "Please wait generating comments templates(s) for offline use...";
                commentsMidTermAll = await _commentsService.GetAllAsync("AcademicsResultsComments/GetMidTermComments/0/" +
                                                                            termid + "/0/0");
                _processing = false;
            }
        }

        async Task GenerateOfflineTemplate()
        {
            stopwatchvalue = new TimeSpan();
            Task task1 = Task.Run(() => LoadMidTernComments());
            Task task2 = Task.Run(() => StartStopWatch());

            await Task.WhenAll(task1, task2);

            task1.Dispose();
            task2.Dispose();

            await Task.CompletedTask;
        }
        #endregion

        #region [Section - Comments Processing Operations]
        async Task MidTermMarksRanking()
        {
            scoreRanking = new List<ScoreRanking>();
            scoreRanking.Clear();

            if (IsOnline)
            {
                cognitiveMarks = await studentMarksCognitiveService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/1/" +
                                                                                   termid + "/" + schid + "/" + classid + "/0/0/0");
            }
            else
            {
                cognitiveMarks = cognitiveMarksAll.Where(m => m.EntryStatus_MidTerm == true && m.ClassID == classid).ToList();
            }

            var scoreList = cognitiveMarks.Where(u => (u.Mark_Mid + u.Mark_MidCBT) > 0)
                .GroupBy(s => s.STDID)
                .OrderByDescending(avg => avg.Average(u => (u.Mark_Mid + u.Mark_MidCBT)))
                .Select((avg, i) => new
                {
                    STDID = avg.Key,
                    SubjectCount = avg.Count(u => (u.Mark_Mid + u.Mark_MidCBT) > 0),
                    MarkObtained = avg.Sum(u => (u.Mark_Mid + u.Mark_MidCBT)),
                    MarkObtainable = avg.Count(u => (u.Mark_Mid + u.Mark_MidCBT) > 0) * 100,
                    AverageMark = avg.Average(u => (u.Mark_Mid + u.Mark_MidCBT)),
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

        async Task RunOnlineMidTermComment()
        {            
            IsShow = false;
            i = 0;
            j = 0;
            progressbarInfo = "Please wait loading Mid-Term Comments...";

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

                var _commentsMidTerm = await midTermCommentService.GetAllAsync("AcademicsResultsComments/GetMidTermComments/2/" +
                                                                            termid + "/0/" + item.STDID);

                if (_commentsMidTerm.Count() == 0)
                {
                    //Add Student Comment
                    midTermComment.TermID = termid;
                    midTermComment.SchSession = schoolSession;
                    midTermComment.ClassID = classid;
                    midTermComment.ClassTeacherID = classTeacherID;
                    midTermComment.STDID = item.STDID;
                    midTermComment.MarkObtained = item.MarkObtained;
                    midTermComment.MarkObtainable = item.MarkObtainable;
                    midTermComment.Comments_Teacher = string.Empty;
                    midTermComment.AVGPer = item.AverageMark; // pos.AVGPer;
                    midTermComment.Position = item.Position;
                    midTermComment.Grade = gradeLetter;

                    var response = await midTermCommentService.SaveAsync("AcademicsResultsComments/AddMidTermComment/", midTermComment);
                    midTermComment.CommentID = response.CommentID;
                    midTermComment.Id = response.CommentID;
                    await midTermCommentService.UpdateAsync("AcademicsResultsComments/UpdateMidTermComment/", 4, midTermComment);
                }
                else
                {
                    //Update Student Comment
                    midTermComment.CommentID = _commentsMidTerm.FirstOrDefault().CommentID;
                    midTermComment.ClassID = classid;
                    midTermComment.ClassTeacherID = classTeacherID;
                    midTermComment.MarkObtained = item.MarkObtained;
                    midTermComment.MarkObtainable = item.MarkObtainable;
                    midTermComment.Comments_Teacher = _commentsMidTerm.FirstOrDefault().Comments_Teacher;
                    midTermComment.AVGPer = item.AverageMark; // pos.AVGPer;
                    midTermComment.Position = item.Position;
                    midTermComment.Grade = gradeLetter;

                    await midTermCommentService.UpdateAsync("AcademicsResultsComments/UpdateMidTermComment/", 1, midTermComment);
                }
                StateHasChanged();
            }

            commentsMidTerm = await midTermCommentService.GetAllAsync("AcademicsResultsComments/GetMidTermComments/1/" +
                                                                            termid + "/" + classid + "/0");

            IsShow = true;
        }

        async Task RunOfflineMidTermComment()
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

                commentsMidTerm.Add(new ACDReportCommentMidTerm
                {
                    SN = sn++,
                    CommentID = commentsMidTermAll.SingleOrDefault(c => c.STDID == item.STDID).CommentID,
                    TermID = commentsMidTermAll.SingleOrDefault(c => c.STDID == item.STDID).TermID,
                    ClassID = commentsMidTermAll.SingleOrDefault(c => c.STDID == item.STDID).ClassID,
                    ClassTeacherID = commentsMidTermAll.SingleOrDefault(c => c.STDID == item.STDID).ClassTeacherID,
                    STDID = commentsMidTermAll.SingleOrDefault(c => c.STDID == item.STDID).STDID,
                    AdmissionNo = commentsMidTermAll.SingleOrDefault(c => c.STDID == item.STDID).AdmissionNo,
                    StudentName = commentsMidTermAll.SingleOrDefault(c => c.STDID == item.STDID).StudentName,
                    Comments_Teacher = commentsMidTermAll.SingleOrDefault(c => c.STDID == item.STDID).Comments_Teacher,
                    AVGPer = item.AverageMark, // pos.AVGPer;
                    Position = item.Position,
                    Grade = gradeLetter,
                    MarkObtained = item.MarkObtained,
                    MarkObtainable = item.SubjectCount,
                    Id = commentsMidTermAll.SingleOrDefault(c => c.STDID == item.STDID).Id
                });

                StateHasChanged();
            }

            IsShow = true;
            await Task.CompletedTask;
        }

        async Task RunMidTermComments()
        {
            isLoading = true;
            loadingmessage = "Please wait, computing grades...";
            if (IsOnline)
            {
                await MidTermMarksRanking();
                await RunOnlineMidTermComment();
            }
            else
            {
                cognitiveMarks = cognitiveMarksAll.Where(m => m.EntryStatus_MidTerm == true && m.ClassID == classid).ToList();
                await MidTermMarksRanking();
                commentsMidTermAll = (await _commentsService.GetAllOfflineAsync()).ToList();
                await RunOfflineMidTermComment();
            }
            isLoading = false;
        }

        void MidTermAutoComments()
        {
            foreach (var item in commentsMidTerm)
            {
                var _autoComments = gradeList.FirstOrDefault(g => g.LowerGrade <= item.AVGPer && g.HigherGrade >= item.AVGPer);

                if (_autoComments == null)
                {
                    autoCommentTeacher = string.Empty;
                }
                else
                {
                    autoCommentTeacher = _autoComments.TeachersComment;
                }

                commentsMidTerm.FirstOrDefault(s => s.STDID == item.STDID).Comments_Teacher = autoCommentTeacher;
            }
        }

        async Task ApplyAutoCommentsMidTerm()
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
                MidTermAutoComments();
            }
        }

        async Task UpdateMidTermComment()
        {
            midTermComment.CommentID = selectedItemMidTermComment.CommentID;
            midTermComment.TermID = selectedItemMidTermComment.TermID;
            midTermComment.ClassID = selectedItemMidTermComment.ClassID;
            midTermComment.ClassTeacherID = selectedItemMidTermComment.ClassTeacherID;
            midTermComment.STDID = selectedItemMidTermComment.STDID;
            midTermComment.AdmissionNo = selectedItemMidTermComment.AdmissionNo;
            midTermComment.StudentName = selectedItemMidTermComment.StudentName;
            midTermComment.Comments_Teacher = selectedItemMidTermComment.Comments_Teacher;
            midTermComment.AVGPer = selectedItemMidTermComment.AVGPer;
            midTermComment.Position = selectedItemMidTermComment.Position;
            midTermComment.Grade = selectedItemMidTermComment.Grade;
            midTermComment.MarkObtained = selectedItemMidTermComment.MarkObtained;
            midTermComment.MarkObtainable = selectedItemMidTermComment.MarkObtainable;
            midTermComment.Id = selectedItemMidTermComment.Id;

            await _commentsService.UpdateAsync("AcademicsResultsComments/UpdateMidTermComment/", 2, midTermComment);
            Snackbar.Add("Comment For " + selectedItemMidTermComment.StudentName + " Has Been Successfully Saved");
        }

        async Task SaveMidTermComments()
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
                    int maxValue = commentsMidTerm.Count();

                    foreach (var item in commentsMidTerm)
                    {
                        j++;
                        i = ((decimal)(j) / maxValue) * 100;

                        midTermComment.CommentID = item.CommentID;
                        midTermComment.TermID = item.TermID;
                        midTermComment.ClassID = item.ClassID;
                        midTermComment.ClassTeacherID = item.ClassTeacherID;
                        midTermComment.STDID = item.STDID;
                        midTermComment.AdmissionNo = item.AdmissionNo;
                        midTermComment.StudentName = item.StudentName;
                        midTermComment.Comments_Teacher = item.Comments_Teacher;
                        midTermComment.AVGPer = item.AVGPer;
                        midTermComment.Position = item.Position;
                        midTermComment.Grade = item.Grade;
                        midTermComment.MarkObtained = item.MarkObtained;
                        midTermComment.MarkObtainable = item.MarkObtainable;
                        midTermComment.Id = item.Id;
                        await _commentsService.UpdateAsync("AcademicsResultsComments/UpdateMidTermComment/", 2, midTermComment);
                        StateHasChanged();
                    }

                    IsShow = true;
                    await Swal.FireAsync("Save Comments", "Teacher's Comments For All The Student In The Selected Class Successfully Saved.", "success");
                }
            }
            else
            {
                await Swal.FireAsync("Mid-Term Comments", "Please select a class", "info");
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
            commentsMidTerm.Clear();
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
            commentsMidTerm.Clear();

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
            commentsMidTerm.Clear();

            if (!IsOnline)
            {
                cognitiveMarksAll = (await _cognitiveMarksService.GetAllOfflineAsync()).ToList();
                if (cognitiveMarksAll.Count == 0)
                {
                    await Swal.FireAsync("Mid-Term Comments", "Cogtive Marks Not Loaded for Offline Use. " +
                        "Please go Online to generate Mark Entry Template(s) for Cognitive Marks.", "error");
                }
                else
                {
                    await RunMidTermComments();
                }
            }
            else
            {
                await RunMidTermComments();
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
            APICallParameters.RequestUriUpdate = "AcademicsResultsComments/UpdateMidTermComment/";
        }

        void IDisposable.Dispose()
        {
            _networkStatusService.OnlineStatusChanged -= OnlineStatusChanged;
        }
    }
}
