using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Administration.Staff;
using WebAppAcademics.Shared.Models.Settings;

namespace WebAppAcademics.Client.Pages.Admin.Staff
{
    public partial class UpdateStaffInfo
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMEmployeeDepts> departmentService { get; set; }
        [Inject] IAPIServices<ADMEmployee> staffService { get; set; }
        [Inject] IAPIServices<SETSchSessions> schoolSessionsService { get; set; }


        [CascadingParameter]
        public MainLayout Layout { get; set; }

        int deptid { get; set; }
        string selectedDepartment { get; set; }
        string searchString { get; set; } = "";

        //Progess Bar Counter
        decimal i { get; set; } = 0;
        int j { get; set; } = 0;
        bool IsShow { get; set; } = true;
        string progressbarInfo { get; set; } = string.Empty;
        #endregion

        #region [Models Declaration]
        List<ADMEmployeeDepts> deptlist { get; set; }
        List<ADMEmployee> staffs { get; set; }
        List<SETSchSessions> academicTerms { get; set; }

        ADMEmployee selectedItem = null;
        ADMEmployee staff = new ADMEmployee();

        void InitializeModels()
        {
            deptlist = new List<ADMEmployeeDepts>();
            staffs = new List<ADMEmployee>();
            academicTerms = new List<SETSchSessions>();
        }

        #endregion

        protected override async Task OnInitializedAsync()
        {
            InitializeModels();
            Layout.currentPage = "Staff Info Batch Update";
            deptlist = await departmentService.GetAllAsync("AdminStaff/GetDepartments/1");
            academicTerms = await schoolSessionsService.GetAllAsync("Settings/GetAccademicSessions/1/0");
            await base.OnInitializedAsync();
        }

        #region [Section - List]
        async Task OnSelectedDeptChanged(IEnumerable<string> e)
        {
            selectedDepartment = e.ElementAt(0);
            deptid = deptlist.FirstOrDefault(d => d.EmployeeGroup == selectedDepartment).EmployeeGroupID;

            staffs.Clear();
            staffs = await staffService.GetAllAsync("AdminStaff/GetStaffs/2/1/" + deptid + "/0/0");
        }

        async Task UpdateEntry()
        {
            staff.StaffID = selectedItem.StaffID;
            string ttt = selectedItem.AcademicSession;

            staff.TermID = academicTerms.FirstOrDefault(t => t.AcademicSession == selectedItem.AcademicSession).TermID;
            staff.EmployeeID = selectedItem.EmployeeID;
            staff.Surname = selectedItem.Surname;
            staff.FirstName = selectedItem.FirstName;
            staff.MiddleName = selectedItem.MiddleName;
            staff.PhoneNos = selectedItem.PhoneNos;
            staff.Email = selectedItem.Email;

            await staffService.UpdateAsync("AdminStaff/UpdateStaff/", 8, staff);
            Snackbar.Add("Selected Staff Info (" + selectedItem.StaffName + ") Has Been Successfully Updated");
        }

        async Task SaveSelection()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Update Student Email Operation",
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
                progressbarInfo = "Please wait performing batch update of staff info...";

                int maxValue = staffs.Count();

                foreach (var item in staffs)
                {
                    j++;
                    i = ((decimal)(j) / maxValue) * 100;

                    staff.StaffID = item.StaffID;
                    staff.TermID = academicTerms.FirstOrDefault(t => t.AcademicSession == item.AcademicSession).TermID;
                    staff.EmployeeID = item.EmployeeID;
                    staff.Surname = item.Surname;
                    staff.FirstName = item.FirstName;
                    staff.MiddleName = item.MiddleName;
                    staff.PhoneNos = item.PhoneNos;
                    staff.Email = item.Email;

                    await staffService.UpdateAsync("AdminStaff/UpdateStaff/", 8, staff);

                    StateHasChanged();
                }

                IsShow = true;
                await Swal.FireAsync("Selected Staff", "Info Has Been Successfully Updated.", "success");
            }
        }

        private bool FilterFunc(ADMEmployee model)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (model.StaffNo.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.StaffName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
        #endregion

        #region [Section - Click Events]

        void GoBack()
        {
            navManager.NavigateTo("/staffs");
        }
        #endregion



    }
}
