using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Settings;

namespace WebAppAcademics.Client.Pages.Academics.Exam.Marks.Comments.Online
{
    public partial class OnlineTermEndComments
    {
        #region [Injection Declaration]
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<SETSchSessions> schoolSessionsService { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassList> classService { get; set; }
        [Inject] IAPIServices<ACDSettingsGrade> gradeService { get; set; }
        [Inject] IAPIServices<ACDSettingsGradeMock> seniorMockGradeService { get; set; }
        [Inject] IAPIServices<ACDSettingsGradeOthers> juniorMockGradeService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksCognitive> studentMarksCognitiveService { get; set; }
        [Inject] IAPIServices<ACDReportCommentsTerminal> termEndCommentService { get; set; }


        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "End of Term Comments Entry";
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            schTerm = await localStorageService.ReadEncryptedItemAsync<string>("schTerm");
            schoolSession = await localStorageService.ReadEncryptedItemAsync<int>("schoolsession");
            staffid = await localStorageService.ReadEncryptedItemAsync<int>("staffid");
            roleid = await localStorageService.ReadEncryptedItemAsync<int>("roleid");
            await LoadDefaultList();
            await base.OnInitializedAsync();
        }

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

        bool isLoading { get; set; } = true;
        string loadingmessage { get; set; } = "Waiting for your selection...";

        DialogOptions dialogOptions = new() { FullWidth = true };
        bool visible { get; set; }
        void Submit() => visible = false;

        #endregion

        #region [Models Declaration]
        List<SETSchSessions> sessions = new();
        List<ADMSchlList> schools = new();
        List<ADMSchClassList> classList = new();
        List<ACDSettingsGrade> gradeList = new();
        List<ACDSettingsGradeMock> seniorMockGradeList = new();
        List<ACDSettingsGradeOthers> juniorMockGradeList = new();
        List<ACDSettingsGradeCheckPoint> checkpointGradeList = new();
        List<ACDSettingsGradeIGCSE> igcseGradeList = new();
        List<ACDStudentsMarksCognitive> cognitiveMarks = new();
        List<ScoreRanking> scoreRanking = new();
        List<ACDReportCommentsTerminal> commentsTermEnd = new();

        ACDReportCommentsTerminal termEndComment = new();
        ACDReportCommentsTerminal selectedTermEndComment = null;

        #endregion


        #region [Common Load / Click Events]
        async Task LoadDefaultList()
        {
            sessions = await schoolSessionsService.GetAllAsync("Settings/GetAccademicSessions/1/0");
            gradeList = await gradeService.GetAllAsync("AcademicsMarkSettings/GetGrades/1");
            seniorMockGradeList = await seniorMockGradeService.GetAllAsync("AcademicsMarkSettings/GetMockGrades/1");
            juniorMockGradeList = await juniorMockGradeService.GetAllAsync("AcademicsMarkSettings/GeOtherGradeSettings/1");

            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
        }

        async Task LoadClassList()
        {
            if (roleid == 1) //Administrator
            {
                classList = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + schid + "/0");
            }
            else
            {
                var _classlist = await classService.GetAllAsync("AdminSchool/GetClassList/0/" + schid + "/0");
                classList = _classlist.Where(c => c.StaffID == staffid).ToList();
            }
        }

        async Task LoadCognitiveMarks(int switchid)
        {
            isLoading = true;
            StateHasChanged();

            try
            {
                await Task.Delay(500);
                cognitiveMarks = await studentMarksCognitiveService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/" + switchid + "/" +
                                                                                    termid + "/" + schid + "/" + classid + "/0/0/0");
            }
            finally
            {
                isLoading = false;
            }
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
            commentsTermEnd.Clear();
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

            await RunTermEndComments();
        }

        #endregion


        #region [End of Term Comments]
        async Task TermEndScoreRanking()
        {
            scoreRanking = new List<ScoreRanking>();
            scoreRanking.Clear();

            loadingmessage = "Please wait, computing grades...";
            await LoadCognitiveMarks(2);

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

        async Task RunTermEndComments()
        {
            await TermEndScoreRanking();

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

                    await termEndCommentService.SaveAsync("AcademicsResultsComments/AddTermEndComment/", termEndComment);
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

            IsShow = true;
            commentsTermEnd = await termEndCommentService.GetAllAsync("AcademicsResultsComments/GetTermEndComments/1/" +
                                                                          termid + "/" + classid + "/0");
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

        async Task UpdateTermEndComments()
        {
            termEndComment.CommentID = selectedTermEndComment.CommentID;
            termEndComment.Comments_Teacher = selectedTermEndComment.Comments_Teacher;
            termEndComment.Comments_Principal = selectedTermEndComment.Comments_Principal;
            termEndComment.DaysAbsent = selectedTermEndComment.DaysAbsent;

            await termEndCommentService.UpdateAsync("AcademicsResultsComments/UpdateTermEndComment/", 2, termEndComment);
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
                        termEndComment.Comments_Teacher = item.Comments_Teacher;
                        termEndComment.Comments_Principal = item.Comments_Principal;
                        termEndComment.DaysAbsent = item.DaysAbsent;

                        await termEndCommentService.UpdateAsync("AcademicsResultsComments/UpdateTermEndComment/", 2, termEndComment);
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
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            selectedClass = string.Empty;
            classid = 0;
            classList.Clear();

            cognitiveMarks.Clear();
            commentsTermEnd.Clear();
        }

        #endregion
    }
}
