using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Administration.Staff;
using WebAppAcademics.Shared.Models.Settings;

namespace WebAppAcademics.Client.Pages.Settings.Users
{
    public partial class UserStaff
    {
        #region [Injection Declaration]
        [Inject] IJSRuntime iJSRuntime { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMEmployeeDepts> deptService { get; set; }
        [Inject] IAPIServices<ADMEmployee> staffService { get; set; }
        [Inject] IAPIServices<SETRole> rolesService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int deptID { get; set; }

        string selectedDeptment { get; set; }
        string searchString { get; set; } = "";

        decimal x { get; set; } = 0;
        int y { get; set; } = 0;
        bool IsShowProgress { get; set; } = true;
        string progressbarInfo { get; set; } = string.Empty;
        #endregion

        #region [Model Declaration]
        List<ADMEmployeeDepts> departments = new();
        List<ADMEmployee> staffs = new();
        List<SETRole> roles = new();

        ADMEmployee staff = new();
        ADMEmployee selectedItem = null;
        PINGenerator generatePIN = new();

        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Users Management - Staff";
            departments = await deptService.GetAllAsync("AdminStaff/GetDepartments/0");
            roles = await rolesService.GetAllAsync("Settings/GetRoles/1");
            await base.OnInitializedAsync();
        }


        #region [Staff Access Roles]
        async Task OnSelectedDepartmentChanged(IEnumerable<string> e)
        {
            selectedDeptment = e.ElementAt(0);
            deptID = departments.FirstOrDefault(d => d.EmployeeGroup == selectedDeptment).EmployeeGroupID;

            staffs = await staffService.GetAllAsync("AdminStaff/GetStaffs/2/1/" + deptID + "/0/0");
        }

        async Task UpdateEntry()
        {
            staff.StaffID = selectedItem.StaffID;
            staff.RoleID = roles.FirstOrDefault(s => s.RoleDesc == selectedItem.RoleDesc).RoleID;
            staff.Email = selectedItem.Email;
            staff.ResetPassword = selectedItem.ResetPassword;

            await staffService.UpdateAsync("AdminStaff/UpdateStaff/", 9, staff);

            Snackbar.Add("Selected Row Entries Updated Successfully.");
        }

        bool FilterFunc(ADMEmployee model)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (model.StaffName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.StaffNo.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        void ExportToExcel()
        {
            byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            int rowNum = 4;
            int sn = 0;

            using (var package = new ExcelPackage())
            {
                var workSheet = package.Workbook.Worksheets.Add("StaffList");

                #region Header Row
                //Cells[Row, Column]

                workSheet.Cells[1, 1].Value = "Staff List";
                workSheet.Cells[1, 1].Style.Font.Size = 16;
                workSheet.Cells[1, 1].Style.Font.Bold = true;
                //workSheet.Cells[1, 1].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                workSheet.Cells[2, 1].Value = selectedDeptment;
                workSheet.Cells[2, 1].Style.Font.Size = 14;
                workSheet.Cells[2, 1].Style.Font.Bold = true;
                //workSheet.Cells[2, 1].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                workSheet.Cells[5, 1].Value = "S/N";
                workSheet.Cells[5, 1].Style.Font.Size = 12;
                workSheet.Cells[5, 1].Style.Font.Bold = true;
                //workSheet.Cells[5, 1].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                workSheet.Cells[5, 2].Value = "Staff No";
                workSheet.Cells[5, 2].Style.Font.Size = 12;
                workSheet.Cells[5, 2].Style.Font.Bold = true;
                //workSheet.Cells[5, 2].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                workSheet.Cells[5, 3].Value = "Staff Name";
                workSheet.Cells[5, 3].Style.Font.Size = 12;
                workSheet.Cells[5, 3].Style.Font.Bold = true;
                //workSheet.Cells[5, 3].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                workSheet.Cells[5, 4].Value = "Emal";
                workSheet.Cells[5, 4].Style.Font.Size = 12;
                workSheet.Cells[5, 4].Style.Font.Bold = true;

                workSheet.Cells[5, 5].Value = "Role";
                workSheet.Cells[5, 5].Style.Font.Size = 12;
                workSheet.Cells[5, 5].Style.Font.Bold = true;
                //workSheet.Cells[5, 4].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                workSheet.Cells[5, 6].Value = "PINs";
                workSheet.Cells[5, 6].Style.Font.Size = 12;
                workSheet.Cells[5, 6].Style.Font.Bold = true;
                #endregion

                foreach (var item in staffs)
                {
                    rowNum++;
                    sn++;

                    workSheet.Cells[rowNum + 1, 1].Value = sn;
                    workSheet.Cells[rowNum + 1, 1].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 1].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 2].Value = item.StaffNo;
                    workSheet.Cells[rowNum + 1, 2].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 2].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 3].Value = item.StaffName;
                    workSheet.Cells[rowNum + 1, 3].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 3].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 4].Value = item.Email;
                    workSheet.Cells[rowNum + 1, 4].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 4].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 5].Value = item.RoleDesc;
                    workSheet.Cells[rowNum + 1, 5].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 5].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 6].Value = item.StaffPIN;
                    workSheet.Cells[rowNum + 1, 6].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 6].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                }

                //double col1 = 5;
                workSheet.Column(1).Width = 5;
                workSheet.Column(2).Width = 14;
                workSheet.Column(3).Width = 35;
                workSheet.Column(4).Width = 35;
                workSheet.Column(5).Width = 17;
                workSheet.Column(6).Width = 11;

                double rowCount = staffs.Count() + 5;

                workSheet.View.FreezePanes(6, 1);

                workSheet.Cells["A5:F" + rowCount].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells["A5:F" + rowCount].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                workSheet.Cells["A5:F" + rowCount].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells["A5:F" + rowCount].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                workSheet.Cells["A5:A" + rowCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                fileContents = package.GetAsByteArray();
            }

            iJSRuntime.InvokeAsync<UserStaff>(
               "saveAsFile",
               "StaffList_" + selectedDeptment + ".xlsx",
               Convert.ToBase64String(fileContents)
               );
        }

        async Task ExportStaffList()
        {
            if (String.IsNullOrWhiteSpace(selectedDeptment))
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Staff Access Roles Management",
                    Icon = "info",
                    Text = "Please Select A Department For Staff Access Roles Export!"
                });
            }
            else
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Staff Access Roles Export Operation",
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
                        ExportToExcel();
                    }
                }
            }
        }

        void Refresh()
        {
            selectedDeptment = string.Empty;
            deptID = 0;
            staffs.Clear();
        }

        async Task GenerateStaffPINs()
        {
            if (String.IsNullOrWhiteSpace(selectedDeptment))
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Staff Access Roles Management",
                    Icon = "info",
                    Text = "Please Select A Department For Staff Access Roles Export!"
                });
            }
            else
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Staff PINs Generation Operation",
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
                        IsShowProgress = false;
                        x = 0;
                        y = 0;
                        int maxValue = staffs.Count;

                        progressbarInfo = "Please wait, generating staff PINs...";

                        foreach (var item in staffs)
                        {
                            y++;
                            x = ((decimal)(y) / maxValue) * 100;

                            staff.StaffID = item.StaffID;
                            staff.StaffPIN = generatePIN.Generate(6);
                            await staffService.UpdateAsync("AdminStaff/UpdateStaff/", 2, staff);

                            StateHasChanged();
                        }

                        staffs.Clear();
                        staffs = await staffService.GetAllAsync("AdminStaff/GetStaffs/2/1/" + deptID + "/0/0");
                        IsShowProgress = true;
                    }
                }
            }
        }

        #endregion


    }
}
