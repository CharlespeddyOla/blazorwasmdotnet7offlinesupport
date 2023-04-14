using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Administration.Staff;

namespace WebAppAcademics.Client.Pages.Admin.Staff
{
    public partial class JobTypes
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMEmployeeJobType> jobTypeService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int toolBarMenuId { get; set; }
        int jobtypeid { get; set; }

        // Set default page title and button text
        string pagetitle = "Create a new Job Type";
        string buttontitle = "Save";

        bool disableSaveButton { get; set; } = true;
        string editFormId { get; set; } = "editformid";
        #endregion

        #region [Models Declaration]
        List<ADMEmployeeJobType> jobtypelist = new();
        ADMEmployeeJobType jobtype = new();

        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Job Type";
            toolBarMenuId = 1;
            jobtypelist = await jobTypeService.GetAllAsync("AdminStaff/GetJobTypes/1");
            await base.OnInitializedAsync();
        }

        #region [Section - List]

        #endregion

        #region [Section - Details]
        async Task RetrieveJobType(int _jobtypeid)
        {
            jobtype = await jobTypeService.GetByIdAsync("AdminStaff/GetJobType/", _jobtypeid);
            jobtypeid = _jobtypeid;
            // Change page title and button text since this is an edit.
            pagetitle = jobtype.JobType;
            buttontitle = "Update";
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
                if (jobtypeid == 0)
                {
                    var response = await jobTypeService.SaveAsync("AdminStaff/AddJobType/", jobtype);
                    jobtype.JobTypeID= response.JobTypeID;
                    jobtype.Id = response.JobTypeID;
                    await jobTypeService.UpdateAsync("AdminStaff/UpdateJobType/", 2, jobtype);
                    await Swal.FireAsync("New Job Type", "Has Been Successfully Saved.", "success");
                }
                else
                {
                    jobtype.JobTypeID = jobtypeid;
                    await jobTypeService.UpdateAsync("AdminStaff/UpdateJobType/", 1, jobtype);
                    await Swal.FireAsync("Selected Job Type", "Has Been Successfully Updated.", "success");
                }

                await JobTypeEvent();
            }
        }

        async Task InvalidEntries()
        {
            await Swal.FireAsync("Please Check Your Entries.", "One or More Entries has error.", "error");
        }
        #endregion

        #region [Section - Click Events]
        async Task JobTypeEvent()
        {
            toolBarMenuId = 1;
            buttontitle = "Save";
            disableSaveButton = true;
            jobtypelist.Clear();
            jobtypelist = await jobTypeService.GetAllAsync("AdminStaff/GetJobTypes/1");
        }

        void CreateNewJobType()
        {
            toolBarMenuId = 2;
            buttontitle = "Save";
            disableSaveButton = false;
            jobtypeid = 0;
            pagetitle = "Create a new Job Type";
            jobtype = new ADMEmployeeJobType();
        }

        async Task UpdateJobType(int _jobtypeid)
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            jobtype = new ADMEmployeeJobType();
            await RetrieveJobType(_jobtypeid);
        }

        void GoBack()
        {
            navManager.NavigateTo("/staffs");
        }
        #endregion
    }
}
