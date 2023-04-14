using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.CBT;

namespace WebAppAcademics.Client.Pages.CBTPages
{
    public partial class OBJStudentExamReview
    {
        #region [Inject Declaration]
        [Inject] ISessionStorageService sessionStorageService { get; set; }
        [Inject] IAPIServices<CBTQuestions> questionsService { get; set; }
        [Inject] IAPIServices<CBTAnswers> answersService { get; set; }

        #endregion

        #region [Variables Declaration]
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        void Submit() => MudDialog.Close(DialogResult.Ok(true));
        void Cancel() => MudDialog.Cancel();

        [Parameter] public List<CBTSelectedAnswer> _selectedAnsers { get; set; } = new List<CBTSelectedAnswer>();
        int examid { get; set; }
        int selectedAnsID { get; set; }
        string imgSrc { get; set; } = "";
        #endregion

        #region [Models Declaration]
        List<CBTQuestions> Questions { get; set; }
        List<CBTAnswers> Answers { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Questions = new List<CBTQuestions>();
            Answers = new List<CBTAnswers>();

            examid = await sessionStorageService.ReadEncryptedItemAsync<int>("examid");
            await LoadDefaultList();
            await base.OnInitializedAsync();
        }

        async Task LoadDefaultList()
        {
            Questions = await questionsService.GetAllAsync("AcademicsCBT/GetCBTExamQuestions/1/" + examid);
            Answers = await answersService.GetAllAsync("AcademicsCBT/GetCBTExamAnswers/1/" + examid);
        }

    }
}
