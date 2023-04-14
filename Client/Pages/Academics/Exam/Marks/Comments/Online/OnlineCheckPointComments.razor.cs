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
    public partial class OnlineCheckPointComments
    {
        #region [Injection Declaration]        
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassList> classService { get; set; }
        [Inject] IAPIServices<ACDSettingsGradeCheckPoint> checkpointGradeService { get; set; }
        [Inject] IAPIServices<ACDSettingsGradeIGCSE> igcseGradeService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksCognitive> studentMarksCognitiveService { get; set; }
        [Inject] IAPIServices<ACDReportCommentCheckPointIGCSE> checkpointigcseCommentService { get; set; }


        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Check Point / IGCSE Comments Entry";
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

        bool isLoading { get; set; } = true;
        string loadingmessage { get; set; } = "Waiting for your selection...";

        #endregion

        #region [Models Declaration]
        List<ADMSchlList> schools = new();
        List<ADMSchClassList> classList = new();
        List<ACDSettingsGradeCheckPoint> checkpointGradeList = new();
        List<ACDSettingsGradeIGCSE> igcseGradeList = new();
        List<ACDStudentsMarksCognitive> cognitiveMarks = new();
        List<ScoreRanking> scoreRanking = new();
        List<ACDReportCommentCheckPointIGCSE> commentsCheckPointIGCSE = new();

        ACDReportCommentCheckPointIGCSE checkpoinigcseComment = new();
        ACDReportCommentCheckPointIGCSE selectedCheckPointIGCSEComment = null;

        #endregion

        #region [Load / Click Events]
        async Task LoadDefaultList()
        {
            checkpointGradeList = await checkpointGradeService.GetAllAsync("AcademicsMarkSettings/GetCheckPointGrades/1");
            igcseGradeList = await igcseGradeService.GetAllAsync("AcademicsMarkSettings/GetIGCSEGrades/1");

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
            commentsCheckPointIGCSE.Clear();
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

            await RunCheckPointIGCSEComments();
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

        #endregion

        #region [Check Point / IGCSE Comments]

        async Task CheckPointIGCSEScoreRanking()
        {
            scoreRanking = new List<ScoreRanking>();
            scoreRanking.Clear();

            loadingmessage = "Please wait, computing grades...";
            await LoadCognitiveMarks(3);

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

        async Task RunCheckPointIGCSEComments()
        {
            await CheckPointIGCSEScoreRanking();

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

                        await checkpointigcseCommentService.SaveAsync("AcademicsResultsComments/AddCheckPointIGCSEComment/", checkpoinigcseComment);
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

            IsShow = true;
            commentsCheckPointIGCSE = await checkpointigcseCommentService.GetAllAsync(
                "AcademicsResultsComments/GetCheckPointIGCSEComments/1/" + termid + "/" + classid + "/0");
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
            checkpoinigcseComment.Comments = selectedCheckPointIGCSEComment.Comments;

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
                        checkpoinigcseComment.Comments = item.Comments;

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
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            selectedClass = string.Empty;
            classid = 0;
            classList.Clear();

            cognitiveMarks.Clear();
            commentsCheckPointIGCSE.Clear();
        }

        #endregion
    }
}
