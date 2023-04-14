using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Administration.Staff;

namespace WebAppAcademics.Client.Pages.Admin.Staff
{
    public partial class Departments
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMEmployeeDepts> departmentService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int toolBarMenuId { get; set; }
        int deptid { get; set; }

        // Set default page title and button text
        string pagetitle = "Create a new Department";
        string buttontitle = "Save";

        bool disableSaveButton { get; set; } = true;
        string editFormId { get; set; } = "editformid";
        #endregion

        #region [Models Declaration]
        List<ADMEmployeeDepts> deptlist = new();
        ADMEmployeeDepts dept = new();

        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Departments";
            toolBarMenuId = 1;
            deptlist = await departmentService.GetAllAsync("AdminStaff/GetDepartments/1");
            await base.OnInitializedAsync();
        }

        #region [Section - List]

        #endregion

        #region [Section - Details]
        async Task RetrieveDepartmets(int _deptid)
        {
            dept = await departmentService.GetByIdAsync("AdminStaff/GetDepartment/", _deptid);
            deptid = _deptid;
            // Change page title and button text since this is an edit.
            pagetitle = dept.EmployeeGroup;
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
                if (deptid == 0)
                {
                    var response = await departmentService.SaveAsync("AdminStaff/AddDepartment/", dept);
                    dept.EmployeeGroupID= response.EmployeeGroupID;
                    dept.Id = response.EmployeeGroupID;
                    await departmentService.UpdateAsync("AdminStaff/UpdateDepartment/", 2, dept);
                    await Swal.FireAsync("New Department", "Has Been Successfully Saved.", "success");
                }
                else
                {
                    dept.EmployeeGroupID = deptid;
                    await departmentService.UpdateAsync("AdminStaff/UpdateDepartment/", 1, dept);
                    await Swal.FireAsync("Selected Department", "Has Been Successfully Updated.", "success");
                }

                await DeptListEvent();
            }
        }

        async Task InvalidEntries()
        {
            await Swal.FireAsync("Please Check Your Entries.", "One or More Entries has error.", "error");
        }


        #endregion

        #region [Section - Click Events]
        async Task DeptListEvent()
        {
            toolBarMenuId = 1;
            buttontitle = "Save";
            disableSaveButton = true;
            deptlist.Clear();
            deptlist = await departmentService.GetAllAsync("AdminStaff/GetDepartments/1");
        }

        void CreateNewDepartment()
        {
            toolBarMenuId = 2;
            buttontitle = "Save";
            disableSaveButton = false;
            deptid = 0;
            pagetitle = "Create a new Department";
            dept = new ADMEmployeeDepts();
        }

        async Task UpdateDeptDetails(int _deptid)
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            dept = new ADMEmployeeDepts();
            await RetrieveDepartmets(_deptid);
        }

        void GoBack()
        {
            navManager.NavigateTo("/staffs");
        }
        #endregion
    }
}
