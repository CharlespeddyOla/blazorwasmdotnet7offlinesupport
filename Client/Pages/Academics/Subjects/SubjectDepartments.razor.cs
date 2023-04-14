using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.Subjects;

namespace WebAppAcademics.Client.Pages.Academics.Subjects
{
    public partial class SubjectDepartments
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ACDSbjDept> subjectDepartmentService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int toolBarMenuId { get; set; }
        int Id { get; set; }

        // Set default page title and button text
        string pagetitle = "Create a new Subject Department";

        bool disableSaveButton { get; set; } = true;
        string editFormId { get; set; } = "editformid";
        #endregion

        #region [Models Declaration]
        List<ACDSbjDept> deptlist = new();
        ACDSbjDept details = new();

        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Subject Deparment List";
            toolBarMenuId = 1;
            deptlist = await subjectDepartmentService.GetAllAsync("AcademicsSubjects/GetDepartments/1");
            await base.OnInitializedAsync();
        }

        #region [Section - List]

        #endregion

        #region [Section - Details]
        async Task RetrieveDepartment(int _id)
        {
            details = await subjectDepartmentService.GetByIdAsync("AcademicsSubjects/GetDepartment/", _id);
            Id = _id;
            // Change page title and button text since this is an edit.
            pagetitle = details.SbjDept;
        }

        private async Task SubmitValidForm()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Department Save/Update Operation",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                if (Id == 0)
                {
                    var response = await subjectDepartmentService.SaveAsync("AcademicsSubjects/AddDepartment/", details);
                    details.SbjDeptID= response.SbjDeptID;
                    details.Id = response.SbjDeptID;
                    await subjectDepartmentService.UpdateAsync("AcademicsSubjects/UpdateDepartment/", 2, details);
                    await Swal.FireAsync("New Department", "Has Been Successfully Saved.", "success");
                }
                else
                {
                    details.SbjDeptID = Id;
                    await subjectDepartmentService.UpdateAsync("AcademicsSubjects/UpdateDepartment/", 1, details);
                    await Swal.FireAsync("Selected Department", "Has Been Successfully Updated.", "success");
                }

                await DepartmentEvent();
            }
        }

        async Task InvalidEntries()
        {
            await Swal.FireAsync("Please Check Your Entries.", "One or More Entries has error.", "error");
        }

        #endregion

        #region [Section - Click Events]
        async Task DepartmentEvent()
        {
            toolBarMenuId = 1;
            disableSaveButton = true;
            pagetitle = "Create a new Subject Department";
            deptlist.Clear();
            deptlist = await subjectDepartmentService.GetAllAsync("AcademicsSubjects/GetDepartments/1");
        }

        void CreateNewSubjectDepartment()
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            Id = 0;
            details = new ACDSbjDept();
        }

        async Task UpdateSubjectDepartment(int _id)
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            details = new ACDSbjDept();
            await RetrieveDepartment(_id);
        }

        void GoBack()
        {
            navManager.NavigateTo("/subjectlist");
        }
        #endregion

    }
}
