using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.Subjects;

namespace WebAppAcademics.Client.Pages.Academics.Subjects
{
    public partial class SubjectClassifications
    {

        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ACDSbjClassification> subjectClassificationService { get; set; }
        [Inject] IAPIServices<ACDSubjects> subjectService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int toolBarMenuId { get; set; }

        bool disableSaveButton { get; set; } = true;
        string editFormId { get; set; } = "editformid";
       
        #endregion

        #region [Models Declaration]
        List<ACDSbjClassification> sbjclasslist = new();
        List<ACDSubjects> subjects = new();

        ACDSbjClassification details = new();
        ACDSbjClassification selectedItem = null;

        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Subject Classifications";
            toolBarMenuId = 1;
            details.Remark = true;
            await LoadSubjectClassificationList();
            await base.OnInitializedAsync();
        }

        #region [Section - List]
        async Task LoadSubjectClassificationList()
        {
            sbjclasslist = await subjectClassificationService.GetAllAsync("AcademicsSubjects/GetSubjectsClassifications/0");
            subjects = await subjectService.GetAllAsync("AcademicsSubjects/GetSubjects/1/0/0/0/true");

            foreach (var item in sbjclasslist)
            {
                sbjclasslist.FirstOrDefault(c => c.SbjClassID == item.SbjClassID).SubjectCount = subjects.Where(s => s.SbjClassID == item.SbjClassID).Count();
            }
        }

        async Task UpdateEntry()
        {
            details.SbjClassID = selectedItem.SbjClassID;
            details.SbjClassification = selectedItem.SbjClassification;
            details.Remark = selectedItem.Remark;
            details.Status = selectedItem.Status;
            await subjectClassificationService.UpdateAsync("AcademicsSubjects/UpdateSubjectsClassification/", 1, details);

            Snackbar.Add("Selected Subject Class (" + selectedItem.SbjClassification + ") Has Been Updated");
        }
        #endregion

        #region [Section - Details]
        async Task SubmitValidForm()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Subject Classification Save Operation",
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
                    details.Status = true;
                    var response = await subjectClassificationService.SaveAsync("AcademicsSubjects/AddSubjectsClassification/", details);
                    details.SbjClassID = response.SbjClassID;
                    details.Id = response.SbjClassID;
                    await subjectClassificationService.UpdateAsync("AcademicsSubjects/UpdateSubjectsClassification/", 2, details);
                    await SubjectClassificationsEvent();
                }
            }
        }

        async Task InvalidEntries()
        {
            await Swal.FireAsync("Please Check Your Entries.", "One or More Entries has error.", "error");
        }
        #endregion

        #region [Section - Click Events]
        async Task SubjectClassificationsEvent()
        {
            toolBarMenuId = 1;
            disableSaveButton = true;
            await LoadSubjectClassificationList();
        }

        void CreateNewSubjectClassification()
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            details = new ACDSbjClassification();
        }

        void GoBack()
        {
            navManager.NavigateTo("/subjectlist");
        }
        #endregion

    }
}
