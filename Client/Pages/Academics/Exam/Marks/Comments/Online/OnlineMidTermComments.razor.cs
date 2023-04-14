using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Administration.School;

namespace WebAppAcademics.Client.Pages.Academics.Exam.Marks.Comments.Online
{
    public partial class OnlineMidTermComments
    {
        #region [Injection Declaration]
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassList> classService { get; set; }
        [Inject] IAPIServices<ACDSettingsGrade> gradeService { get; set; }
        [Inject] IAPIServices<ACDSettingsGradeMock> seniorMockGradeService { get; set; }
        [Inject] IAPIServices<ACDSettingsGradeOthers> juniorMockGradeService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksCognitive> studentMarksCognitiveService { get; set; }
        [Inject] IAPIServices<ACDReportCommentMidTerm> midTermCommentService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Mid-Term Comments Entry";
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            schTerm = await localStorageService.ReadEncryptedItemAsync<string>("schTerm");
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

        bool isLoading { get; set; } = true;
        string loadingmessage { get; set; } = "Waiting for your selection...";
        #endregion

        #region [Models Declaration]
        List<ADMSchlList> schools = new();
        List<ADMSchClassList> classList = new();
        List<ACDSettingsGrade> gradeList = new();
        List<ACDSettingsGradeMock> seniorMockGradeList = new();
        List<ACDSettingsGradeOthers> juniorMockGradeList = new();
        List<ACDSettingsGradeCheckPoint> checkpointGradeList = new();
        List<ACDSettingsGradeIGCSE> igcseGradeList = new();
        List<ACDStudentsMarksCognitive> cognitiveMarks = new();
        List<ACDReportCommentMidTerm> commentsMidTerm = new();
        List<ScoreRanking> scoreRanking = new();

        ACDReportCommentMidTerm midTermComment = new();
        ACDReportCommentMidTerm selectedItemMidTermComment = null;

        #endregion

        #region [Load / Click Events]
        async Task LoadDefaultList()
        {
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

            classid = 0;
            classTeacherID = 0;
            selectedClass = string.Empty;
            classList.Clear();
            await LoadClassList();

            cognitiveMarks.Clear();
            commentsMidTerm.Clear();
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

            await RunMidTermComments();
        }

        #endregion

        #region [Section - Mid Term Cooments]
        async Task MidTermMarksRanking()
        {
            //scoreRanking = new List<ScoreRanking>();
            scoreRanking.Clear();

            loadingmessage = "Please wait, computing grades...";
            await LoadCognitiveMarks(1);

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

        async Task RunMidTermComments()
        {
            await MidTermMarksRanking();

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

                    await midTermCommentService.SaveAsync("AcademicsResultsComments/AddMidTermComment/", midTermComment);
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

            IsShow = true;
            commentsMidTerm = await midTermCommentService.GetAllAsync("AcademicsResultsComments/GetMidTermComments/1/" +
                                                                            termid + "/" + classid + "/0");
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
            midTermComment.Comments_Teacher = selectedItemMidTermComment.Comments_Teacher;

            await midTermCommentService.UpdateAsync("AcademicsResultsComments/UpdateMidTermComment/", 2, midTermComment);
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
                        midTermComment.Comments_Teacher = item.Comments_Teacher;

                        await midTermCommentService.UpdateAsync("AcademicsResultsComments/UpdateMidTermComment/", 2, midTermComment);
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
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            selectedClass = string.Empty;
            classid = 0;
            classList.Clear();

            cognitiveMarks.Clear();
            commentsMidTerm.Clear();
        }

        #endregion
    }
}
