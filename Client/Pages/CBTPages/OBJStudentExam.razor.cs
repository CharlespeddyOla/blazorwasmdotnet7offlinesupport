using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.CBT;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Administration.Students;

namespace WebAppAcademics.Client.Pages.CBTPages
{
    public partial class OBJStudentExam
    {
        #region [Inject Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] ISessionStorageService sessionStorageService { get; set; }
        [Inject] IAPIServices<CBTExams> examService { get; set; }
        [Inject] IAPIServices<CBTQuestions> questionsService { get; set; }
        [Inject] IAPIServices<CBTAnswers> answersService { get; set; }
        [Inject] IAPIServices<CBTStudentAnswers> studentAnswersService { get; set; }
        [Inject] IAPIServices<CBTStudentScores> studentScoresService { get; set; }
        [Inject] IAPIServices<ADMStudents> studentService { get; set; }
        [Inject] IDialogService DialogService { get; set; }
        [Inject] IAPIServices<ACDFlags> flagsService { get; set; }
        [Inject] IAPIServices<CBTExamTakenFlags> examTakenService { get; set; }

        #endregion

        #region [Variables Declaration]  
        [Parameter] public int examid { get; set; }
        [Parameter] public int stdid { get; set; }

        int termid { get; set; }
        double examTime { get; set; }

        string academicSession { get; set; }

        //Photo Declaration Section        
        string imgSrc { get; set; } = "";
        Utilities utilities = new Utilities();
        #endregion

        #region [Models Declaration]
        List<CBTQuestions> Questions = new();
        List<CBTAnswers> Answers = new();
        List<CBTStudentAnswers> studentAnswers = new();
        List<CBTSelectedAnswer> selectedAnswers = new();
        List<CBTSelectedAnswer> SelectedAnswersReview = new();
        List<CBTStudentScores> studentScores = new();
        List<ACDFlags> flagList = new();
        List<CBTExamTakenFlags> examTakenList = new();
        CBTExamTakenFlags examTaken = new();

        CBTExams examDetails = new();
        CBTStudentAnswers studentAnswer = new();
        CBTStudentScores studentScore = new();
        ADMStudents student = new();

        OBJExamInstructions examInstutcion = new();

        #endregion

        protected override async Task OnInitializedAsync()
        {
            termid = await sessionStorageService.ReadEncryptedItemAsync<int>("termid");
            Questions = new List<CBTQuestions>();
            Answers = new List<CBTAnswers>();
            academicSession = await sessionStorageService.ReadEncryptedItemAsync<string>("academicSession");
            examTime = await sessionStorageService.ReadEncryptedItemAsync<double>("examTime");
            flagList = await flagsService.GetAllAsync("AcademicsCBT/GetFlags/1");
            await LoadDefaultList();

            await StopWatch();
            await base.OnInitializedAsync();
        }

        #region [Section - Default Values]

        async Task LoadDefaultList()
        {
            //Exam Details
            examDetails = await examService.GetByIdAsync("AcademicsCBT/GetCBTExam/", examid);

            //Questions
            Questions = await questionsService.GetAllAsync("AcademicsCBT/GetCBTExamQuestions/1/" + examid);
            TotalQuestions = Questions.Count();

            //Answers
            Answers = await answersService.GetAllAsync("AcademicsCBT/GetCBTExamAnswers/1/" + examid);
            UpdateDisplay();
        }
        #endregion

        #region [Section - Exam Timer]
        bool _timerVisible { get; set; } = true;
        TimeSpan TimeAllocated { get; set; }
        TimeSpan TimeLeft { get; set; }
        TimeSpan stopwatchvalue = new TimeSpan();
        bool is_stopwatchrunning { get; set; } = false;

        async Task StopWatch()
        {
            is_stopwatchrunning = true;
            while (is_stopwatchrunning)
            {
                await Task.Delay(1000);
                if (is_stopwatchrunning)
                {
                    stopwatchvalue = stopwatchvalue.Add(new TimeSpan(0, 0, 1));
                    if (stopwatchvalue == TimeSpan.FromSeconds(examDetails.ExamTime * 60))
                    {
                        is_stopwatchrunning = false;
                        await MarkStudentCBTOBJExam();
                        await RegisterStudentCBTOBJScore();
                        await SaveExamTaken();
                        LockCBT();
                        navManager.NavigateTo("/cbt/obtexamstudentresult/" + examid + "/" + stdid);
                    }
                    StateHasChanged();
                }
            }
        }

        #endregion

        #region [Section - Questions Paging]
        [Parameter] public int ItemsPerPage { get; set; } = 1;
        private int TotalQuestions { get; set; }
        private int _QID { get; set; }
        private int _QNO { get; set; }
        private int _SelectedAnswerID { get; set; }
        private int CurrentPage { get; set; } = 1;
        private List<CBTQuestions> CurrentDisplay = new List<CBTQuestions>();

        void UpdateDisplay()
        {
            CurrentDisplay = Questions.Skip((CurrentPage - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
            _QID = Questions.FirstOrDefault(q => q.QNo == CurrentPage).QID;
            _QNO = Questions.FirstOrDefault(q => q.QNo == CurrentPage).QNo;
        }

        private bool AtLastPage()
        {
            return CurrentPage >= TotalPages();
        }

        private int TotalPages()
        {
            return Convert.ToInt32(Math.Ceiling(TotalQuestions / Convert.ToDecimal(ItemsPerPage)));
        }

        void MoveFirst()
        {
            CurrentPage = 1;
            UpdateDisplay();
        }

        void MoveNext()
        {
            CurrentPage++;
            UpdateDisplay();
        }

        void MoveBack()
        {
            CurrentPage--;
            UpdateDisplay();
        }

        void MoveLast()
        {
            CurrentPage = TotalPages();
            UpdateDisplay();
        }
        #endregion

        #region [Section - Student Score]
        int _ansID { get; set; } = 0;
        int _choiceID { get; set; } = 0;
        int newScoreID { get; set; }
        int questionsAnswered { get; set; }
        int questionsAnsweredCorrectly { get; set; }

      
        async Task MarkStudentCBTOBJExam()
        {
            studentAnswer.STDID = stdid;
            studentAnswer.ExamID = examid;
            studentAnswer.CBTToUse = false;
            await studentAnswersService.UpdateAsync("AcademicsCBT/UpdateCBTStudentAnswer/", 1, studentAnswer);

            foreach (var question in Questions)
            {
                studentAnswer.QTypeID = question.QTypeID;
                studentAnswer.QID = question.QID;
                studentAnswer.QNo = question.QNo;
                studentAnswer.QPoints = question.QPoints;
                studentAnswer.CBTToUse = true;

                if (selectedAnswers.OrderBy(q => q.QNo).Where(q => q.QID == question.QID).Any())
                {
                    _ansID = Answers.FirstOrDefault(a => a.QID == question.QID && a.CorrectAns == true).AnsID;
                    _choiceID = selectedAnswers.FirstOrDefault(c => c.QID == question.QID).AnsID;
                    studentAnswer.Answer = Answers.FirstOrDefault(a => a.QID == question.QID && a.AnsID == _choiceID).AnsLetter;
                    studentAnswer.AnsID = _ansID;
                    studentAnswer.QAnswered = true;
                    if (_ansID == _choiceID)
                    {
                        studentAnswer.Correct = true;
                        selectedAnswers.OrderBy(q => q.QNo).FirstOrDefault(c => c.QID == question.QID).Correct = true;
                    }
                    else
                    {
                        studentAnswer.Correct = false;
                        selectedAnswers.OrderBy(q => q.QNo).FirstOrDefault(c => c.QID == question.QID).Correct = false;
                    }
                    selectedAnswers.OrderBy(q => q.QNo).FirstOrDefault(c => c.QID == question.QID).QAnswered = true;
                }
                else
                {
                    studentAnswer.Answer = "N";
                    studentAnswer.QAnswered = false;
                    studentAnswer.Correct = false;
                    studentAnswer.AnsID = 0;
                }

                await studentAnswersService.SaveAsync("AcademicsCBT/AddCBTStudentAnswer/", studentAnswer);
            }

            var _questionsAnswered = selectedAnswers.Where(n => n.QAnswered == true).GroupBy(a => a.QID).Select(q => new
            {
                QID = q.Key,
                Count = q.Select(a => a.QNo).Distinct().Count()
            });

            questionsAnswered = _questionsAnswered.Count();

            var _questionsAnsweredCorrectly = selectedAnswers.Where(n => n.Correct == true).GroupBy(a => a.QID).Select(q => new
            {
                QID = q.Key,
                Count = q.Select(a => a.QNo).Distinct().Count()
            });

            questionsAnsweredCorrectly = _questionsAnsweredCorrectly.Count();
        }

        async Task RegisterStudentCBTOBJScore()
        {
            studentScore.ExamID = examid;
            studentScore.STDID = stdid;
            studentScore.ExamDate = Convert.ToDateTime(examDetails.ExamDate);
            studentScore.NQuestions = TotalQuestions;
            studentScore.NUnAnsQuestions = TotalQuestions - questionsAnswered;
            studentScore.NWrongAns = TotalQuestions - questionsAnsweredCorrectly;
            studentScore.NCorrectAns = questionsAnsweredCorrectly;
            double YourScore = (Convert.ToDouble(questionsAnsweredCorrectly) / Convert.ToDouble(TotalQuestions)) * 100;
            studentScore.ScorePercentage = YourScore;
            studentScore.QTimer = true;
            studentScore.ExamTimer = true;
            TimeAllocated = TimeSpan.FromSeconds(examDetails.ExamTime * 60);
            studentScore.TimeAllocated = string.Format("{0:00}:{1:00}:{2:00}", ((int)TimeAllocated.TotalHours), TimeAllocated.Minutes, TimeAllocated.Seconds);
            studentScore.TimeUsed = string.Format("{0:00}:{1:00}:{2:00}", ((int)stopwatchvalue.TotalHours), stopwatchvalue.Minutes, stopwatchvalue.Seconds);

            await sessionStorageService.SaveItemEncryptedAsync("isview", false);
            TimeLeft = TimeAllocated - stopwatchvalue;
            await sessionStorageService.SaveItemEncryptedAsync("timeleft", TimeLeft);

            studentScore.CBTToUse = false;
            await studentScoresService.UpdateAsync("AcademicsCBT/UpdateCBTStudentScore/", 1, studentScore);

            var response = await studentScoresService.SaveAsync("AcademicsCBT/AddCBTStudentScore/", studentScore);
            newScoreID = response.StudentScoreID;

            studentAnswer.STDID = stdid;
            studentAnswer.ExamID = examid;
            studentAnswer.CBTToUse = true;
            studentAnswer.StudentScoreID = newScoreID;
            await studentAnswersService.UpdateAsync("AcademicsCBT/UpdateCBTStudentAnswer/", 2, studentAnswer);
        }

        void LockCBT()
        {
            bool _lockCBT = flagList.Where(l => l.FlagID == 1 && l.Flag == true).Any();

            if (_lockCBT)
            {
                student.STDID = stdid;
                student.CBTLock = true;
                studentService.UpdateAsync("AdminStudent/UpdateStudent/", 9, student);
            }                
        }

        async Task SaveExamTaken()
        {
            examTakenList = await examTakenService.GetAllAsync("AcademicsCBT/Search/1/" + termid + "/" + stdid + "/" + examid + "/" + false);

            if (examTakenList.Count == 0)
            {
                examTaken.STDID = stdid;
                examTaken.ExamID = examid;
                examTaken.TermID = termid;
                examTaken.Flag = true;

                await examTakenService.SaveAsync("AcademicsCBT/AddExamTakenFlag/", examTaken);
            }
            else
            {
                int _flagid = examTakenList.Single().FlagID;
                examTaken.FlagID = examTakenList.Single().FlagID;
                examTaken.Flag = true;
                await examTakenService.UpdateAsync("AcademicsCBT/UpdateExamTakenFlag/", 1, examTaken);
            }
        }

        #endregion

        #region [Click Events]  
        void OnSelectedOptionChanged(int value)
        {
            _SelectedAnswerID = value;

            if (selectedAnswers.Where(q => q.QID == _QID).Any())
            {
                selectedAnswers.Remove(selectedAnswers.SingleOrDefault(q => q.QID == _QID));
            }

            selectedAnswers.Add(new CBTSelectedAnswer()
            {
                QID = _QID,
                QNo = _QNO,
                AnsID = _SelectedAnswerID,
                AnsImage = Answers.FirstOrDefault(ans => ans.AnsID == _SelectedAnswerID).AnsImage
            });
        }

        async Task SubmitExam()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Submit Your Final CBT Exam",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                is_stopwatchrunning = false;
                await MarkStudentCBTOBJExam();
                await RegisterStudentCBTOBJScore();
                await SaveExamTaken();
                LockCBT();
                navManager.NavigateTo("/objcbtscore/" + examid + "/" + stdid);
            }
        }

        async Task ShowExamReviewPage()
        {
            await sessionStorageService.SaveItemEncryptedAsync("examid", examid);

            var parameters = new DialogParameters();
            parameters.Add("_selectedAnsers", selectedAnswers);

           // var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            DialogService.Show<OBJStudentExamReview>("CBT Exam Review", parameters);
        }

        DialogOptions fullScreen = new DialogOptions() { FullScreen = true, CloseButton = true };

        void EnlargeImage(DialogOptions options, byte[] _qImage)
        {
            var parameters = new DialogParameters();
            parameters.Add("_QImage", _qImage);


            DialogService.Show<DialogEnlargeQuestionImage>("", parameters, options);
        }


        #endregion
    }
}
