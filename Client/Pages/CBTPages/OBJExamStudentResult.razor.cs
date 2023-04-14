using Microsoft.AspNetCore.Components;
using WebAppAcademics.Shared.Models.Academics.CBT;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Client.Pages.CBTPages
{
    public partial class OBJExamStudentResult
    {
        #region [Inject Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] ISessionStorageService sessionStorageService { get; set; }
        [Inject] IAPIServices<CBTQuestions> questionsService { get; set; }
        [Inject] IAPIServices<CBTAnswers> answersService { get; set; }
        [Inject] IAPIServices<CBTStudentAnswers> studentAnswersService { get; set; }
        [Inject] IAPIServices<CBTStudentScores> studentScoresService { get; set; }
        [Inject] IAPIServices<ACDFlags> flagsService { get; set; }
        #endregion

        #region [Variable Declaration]  
        [Parameter] public int examid { get; set; }
        [Parameter] public int stdid { get; set; }

        int selectedAnsID { get; set; }
        int questionsAnswered { get; set; }
        bool IsShow { get; set; } = true;

        //Photo Declaration Section        
        string imgSrcphoto { get; set; } = "";
        string imgSrc { get; set; } = "";
        Utilities utilities = new Utilities();

        string academicSession { get; set; }
        #endregion

        #region [Models Declaration]
        List<CBTQuestions> Questions { get; set; }
        List<CBTAnswers> Answers { get; set; }
        List<CBTStudentAnswers> studentAnswers { get; set; }
        List<CBTStudentScores> studentScores { get; set; }
        List<ACDFlags> flagList = new();
        CBTStudentScores studentScore { get; set; }

        void InitializeModels()
        {
            Questions = new List<CBTQuestions>();
            Answers = new List<CBTAnswers>();
            studentAnswers = new List<CBTStudentAnswers>();
            studentScores = new List<CBTStudentScores>();
            studentScore = new CBTStudentScores();
        }

        #endregion

        protected override async Task OnInitializedAsync()
        {
            InitializeModels();
            await LoadDefaultList();

            TimeLeft = await sessionStorageService.ReadEncryptedItemAsync<TimeSpan>("timeleft");
            IsView = await sessionStorageService.ReadEncryptedItemAsync<bool>("isview");
            academicSession = await sessionStorageService.ReadEncryptedItemAsync<string>("academicSession");
            flagList = await flagsService.GetAllAsync("AcademicsCBT/GetFlags/1");
            await StopWatch();
            await base.OnInitializedAsync();
        }


        #region [Section - Default Values]
        async Task LoadDefaultList()
        {
            //Questions
            Questions = await questionsService.GetAllAsync("AcademicsCBT/GetCBTExamQuestions/1/" + examid);

            //Answers
            Answers = await answersService.GetAllAsync("AcademicsCBT/GetCBTExamAnswers/1/" + examid);

            //Student Answers
            studentAnswers = await studentAnswersService.GetAllAsync("AcademicsCBT/GetCBTStudentAnswers/1/" + examid + "/" + stdid + "/" + true);

            studentScores = await studentScoresService.GetAllAsync("AcademicsCBT/GetCBTStudentScores/2/" + examid + "/" + stdid + "/true");
            studentScore = studentScores.FirstOrDefault();
            questionsAnswered = studentScore.NQuestions - studentScore.NUnAnsQuestions;
        }

        #endregion

        #region [Timer Operation]
        TimeSpan stopwatchvalue = new TimeSpan();
        bool is_stopwatchrunning = false;
        TimeSpan TimeLeft { get; set; }
        bool IsView { get; set; } = false;

        async Task StopWatch()
        {
            is_stopwatchrunning = true;
            while (is_stopwatchrunning)
            {
                await Task.Delay(1000);
                if (is_stopwatchrunning)
                {
                    stopwatchvalue = stopwatchvalue.Add(new TimeSpan(0, 0, 1));
                    if (IsView)
                    {
                        IsShow = false;
                    }
                    else
                    {
                        if (studentScore.TimeAllocated == studentScore.TimeUsed)
                        {
                            is_stopwatchrunning = false;
                            IsShow = false;
                        }

                        if (stopwatchvalue == TimeLeft)
                        {
                            IsShow = false;
                        }
                    }

                    StateHasChanged();
                }
            }
        }


        #endregion

        void Close()
        {
            bool _lockCBT = flagList.Where(l => l.FlagID == 1 && l.Flag == true).Any();

            if (_lockCBT)
            {
                navManager.NavigateTo("/cbt");
            }
            else
            {
                navManager.NavigateTo("/cbtobjexaminstructions", true);
            }                
        }

    }
}
