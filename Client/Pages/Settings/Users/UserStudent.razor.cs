using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.CBT;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Administration.Students;

namespace WebAppAcademics.Client.Pages.Settings.Users
{
    public partial class UserStudent
    {
        #region [Injection Declaration]
        [Inject] IJSRuntime iJSRuntime { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassList> classService { get; set; }
        [Inject] IAPIServices<ADMStudents> studentService { get; set; }
        [Inject] IAPIServices<CBTConnectionInfo> cbtConnectionInfoService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Users Management - Student";
            menuId = 1;
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            cbtconnectionInfo = await cbtConnectionInfoService.GetAllAsync("AcademicsCBT/GetConnectionInfo/1");
            await base.OnInitializedAsync();
        }


        #region [Variables Declaration]
        int schid { get; set; }
        int classid { get; set; }
        int menuId { get; set; }

        string selectedSchool { get; set; }
        string selectedClass { get; set; }

        protected bool _lockcbt { get; set; }
        string cbtLockState { get; set; } = "Lock CBT Exam";

        DialogOptions dialogOptions = new() { FullWidth = true };
        bool visible { get; set; }
        void Submit() => visible = false;

        decimal x { get; set; } = 0;
        int y { get; set; } = 0;
        bool IsShowProgress { get; set; } = true;
        string progressbarInfo { get; set; } = string.Empty;
        #endregion

        #region [Model Declaration]
        List<ADMSchlList> schools = new();
        List<ADMSchClassList> classList = new();
        List<ADMStudents> students = new();
        List<ADMStudents> studentsAll = new();
        List<List<string>> excelArrayPINs = new();
        List<CBTConnectionInfo> cbtconnectionInfo = new();

        ADMStudents student = new();
        ADMStudents selectedStudent { get; set; } = null;
        CBTConnectionInfo selectedConnectionInfo = null;
        CBTConnectionInfo cbtConnInfo = new();

        PINGenerator generatePIN = new();

        #endregion

        #region [Student List]
        async Task OnSelectedSchoolChanged(IEnumerable<string> e)
        {
            selectedSchool = e.ElementAt(0);
            schid = schools.FirstOrDefault(s => s.School == selectedSchool).SchID;

            selectedClass = string.Empty;
            classid = 0;
            classList.Clear();
            classList = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + schid + "/0");

            students.Clear();
        }

        async Task OnSelectedClassChanged(IEnumerable<string> e)
        {
            selectedClass = e.ElementAt(0);
            classid = classList.FirstOrDefault(s => s.ClassName == selectedClass).ClassID;

            students.Clear();
            students = await studentService.GetAllAsync("AdminStudent/GetStudents/3/0/" + classid + "/0/1");
        }

        async Task UpdateStudentEntry()
        {
            student.STDID = selectedStudent.STDID;
            student.ParentPin = selectedStudent.ParentPin;
            student.StudentPin = selectedStudent.StudentPin;
            student.CBTLock = selectedStudent.CBTLock;

            await studentService.UpdateAsync("AdminStudent/UpdateStudent/", 13, student);
            Snackbar.Add("Selected Row Entries Updated Successfully.");
        }

        async Task Refresh()
        {
            menuId = 1;
            selectedSchool = string.Empty;
            schid = 0;
            schools.Clear();
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            selectedClass = string.Empty;
            classid = 0;
            classList.Clear();

            students.Clear();
        }

        async Task LockCBT(bool value)
        {
            _lockcbt = value;

            if (_lockcbt)
            {
                if (classid > 0)
                {
                    cbtLockState = "UnLock CBT Exam";
                    foreach (var item in students)
                    {
                        student.STDID = item.STDID;
                        student.CBTLock = true;
                        await studentService.UpdateAsync("AdminStudent/UpdateStudent/", 9, student);
                    }

                    students = await studentService.GetAllAsync("AdminStudent/GetStudents/3/0/" + classid + "/0/1");
                }
                else
                {
                    SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                    {
                        Title = "Lock CBT Examination",
                        Text = "Please Select A Class.",
                        Icon = SweetAlertIcon.Warning
                    });
                    _lockcbt = false;
                }
            }
            else
            {
                cbtLockState = "Lock CBT Exam";
                foreach (var item in students)
                {
                    student.STDID = item.STDID;
                    student.CBTLock = false;
                    await studentService.UpdateAsync("AdminStudent/UpdateStudent/", 9, student);
                }
                students = await studentService.GetAllAsync("AdminStudent/GetStudents/3/0/" + classid + "/0/1");
            }
        }


        #endregion

        #region [Generate Student PINs]
        async Task GenerateStudentPIN(string _password)
        {
            studentsAll.Clear();
            studentsAll = await studentService.GetAllAsync("AdminStudent/GetStudents/1/0/0/0/1");
            IsShowProgress = false;
            x = 0;
            y = 0;
            int maxValue = studentsAll.Count;
            student.Password = _password;
            progressbarInfo = "Please wait, generating student CBT PINs And Setting CBT Password...";
            foreach (var item in studentsAll)
            {
                y++;
                x = ((decimal)(y) / maxValue) * 100;

                student.STDID = item.STDID;
                student.StudentPin = generatePIN.Generate(6);
                await studentService.UpdateAsync("AdminStudent/UpdateStudent/", 8, student);

                StateHasChanged();
            }

            IsShowProgress = true;
            await Swal.FireAsync("Student CBT PIN And CBT Password Management", "Operation Completed Successfully.", "success");
        }

        async Task StartStudentPINsGenerator()
        {
            menuId = 1;
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Student CBT PIN And CBT Password Management Operation",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                visible = true;
            }
        }

        #endregion

        #region [Export / Print Student PIN]

        void ExportToExcelStudentPINs()
        {
            byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            int rowNum = 4;
            int sn = 0;

            using (var package = new ExcelPackage())
            {
                var workSheet = package.Workbook.Worksheets.Add("StudentsPINs");

                #region Header Row
              
                workSheet.Cells[1, 1].Value = "Student CBT Login PIN List";
                workSheet.Cells[1, 1].Style.Font.Size = 16;
                workSheet.Cells[1, 1].Style.Font.Bold = true;

                workSheet.Cells[2, 1].Value = selectedClass;
                workSheet.Cells[2, 1].Style.Font.Size = 14;
                workSheet.Cells[2, 1].Style.Font.Bold = true;

                workSheet.Cells[5, 1].Value = "S/N";
                workSheet.Cells[5, 1].Style.Font.Size = 12;
                workSheet.Cells[5, 1].Style.Font.Bold = true;

                workSheet.Cells[5, 2].Value = "Admission No";
                workSheet.Cells[5, 2].Style.Font.Size = 12;
                workSheet.Cells[5, 2].Style.Font.Bold = true;

                workSheet.Cells[5, 3].Value = "Student Name";
                workSheet.Cells[5, 3].Style.Font.Size = 12;
                workSheet.Cells[5, 3].Style.Font.Bold = true;

                workSheet.Cells[5, 4].Value = "Student PIN";
                workSheet.Cells[5, 4].Style.Font.Size = 12;
                workSheet.Cells[5, 4].Style.Font.Bold = true;
                #endregion

                foreach (var item in students)
                {
                    rowNum++;
                    sn++;

                    workSheet.Cells[rowNum + 1, 1].Value = sn;
                    workSheet.Cells[rowNum + 1, 1].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 1].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 2].Value = item.AdmissionNo;
                    workSheet.Cells[rowNum + 1, 2].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 2].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 3].Value = item.StudentName;
                    workSheet.Cells[rowNum + 1, 3].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 3].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 4].Value = item.StudentPin;
                    workSheet.Cells[rowNum + 1, 4].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 4].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                }

                workSheet.Column(1).Width = 5;
                workSheet.Column(2).Width = 15;
                workSheet.Column(3).Width = 47;
                workSheet.Column(4).Width = 15;

                double rowCount = students.Count() + 5;

                workSheet.View.FreezePanes(6, 1);

                workSheet.Cells["A5:D" + rowCount].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells["A5:D" + rowCount].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                workSheet.Cells["A5:D" + rowCount].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells["A5:D" + rowCount].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                workSheet.Cells["A5:A" + rowCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                fileContents = package.GetAsByteArray();
            }

            iJSRuntime.InvokeAsync<UserStudent>(
                "saveAsFile",
                "StudentPINs_" + selectedClass + ".xlsx",
                Convert.ToBase64String(fileContents)
                );
        }

        async Task ExportStudentPINs()
        {
            menuId = 1;
            if (String.IsNullOrWhiteSpace(selectedClass))
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Student PIN List",
                    Icon = "info",
                    Text = "Please Select A Class For Student PINS Export!"
                });
            }
            else
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Student PIN List Export Operation",
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
                        ExportToExcelStudentPINs();
                    }
                }
            }
        }

        void ExportToExcelStudentPINsDistribution()
        {
            byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            int kount = 0;
            excelArrayPINs.Clear();

            string wifi = "Wi-Fi Password";
            string wifipassword = cbtconnectionInfo.FirstOrDefault(conn => conn.ConnectionID == 1).ConnectionValue;
            string url = cbtconnectionInfo.FirstOrDefault(conn => conn.ConnectionID == 2).ConnectionValue; 
            string cbtpassword = cbtconnectionInfo.FirstOrDefault(conn => conn.ConnectionID == 3).ConnectionValue; 

            using (var package = new ExcelPackage())
            {
                var workSheet = package.Workbook.Worksheets.Add("StudentsPINs");

                foreach (var item in students)
                {
                    excelArrayPINs.Add(new List<string> { item.AdmissionNo, item.StudentInitials, item.StudentPin });
                }

                int row = 0;
                //j - Row; i - Column
                kount = excelArrayPINs.Count();
                double k = (double)kount / 2;
                int maxValue = Convert.ToInt32(Math.Ceiling(k));

                int name_pointer = 1;
                int wifi_pointer = 2;
                int wifipassword_pointer = 3;
                int url_pointer = 4;
                int urladdress_pointer = 5;
                int cbtpinheader_pointer = 6;
                int pin_poiter = 7;
                int cbtpasswordheader_pointer = 8;
                int cbtpassword_pointer = 9;

                for (int j = 1; j <= maxValue; j++)
                {
                    for (int i = 1; i < 3; i++)
                    {
                        if (row < kount)
                        {
                            workSheet.Cells[name_pointer, i].Value = excelArrayPINs[row][1];
                            workSheet.Cells[name_pointer, i].Style.Font.Size = 12;
                            workSheet.Cells[name_pointer, i].Style.Font.Bold = true;
                            workSheet.Cells[name_pointer, i].Style.Font.UnderLine = true;
                            workSheet.Cells[name_pointer, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[wifi_pointer, i].Value = wifi;
                            workSheet.Cells[wifi_pointer, i].Style.Font.Size = 12;
                            workSheet.Cells[wifi_pointer, i].Style.Font.Bold = true;
                            workSheet.Cells[wifi_pointer, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[wifipassword_pointer, i].Value = wifipassword;
                            workSheet.Cells[wifipassword_pointer, i].Style.Font.Size = 12;
                            workSheet.Cells[wifipassword_pointer, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[url_pointer, i].Value = "URL";
                            workSheet.Cells[url_pointer, i].Style.Font.Size = 12;
                            workSheet.Cells[url_pointer, i].Style.Font.Bold = true;
                            workSheet.Cells[url_pointer, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[urladdress_pointer, i].Value = url;
                            workSheet.Cells[urladdress_pointer, i].Style.Font.Size = 12;
                            workSheet.Cells[urladdress_pointer, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[cbtpinheader_pointer, i].Value = "CBT PIN";
                            workSheet.Cells[cbtpinheader_pointer, i].Style.Font.Size = 12;
                            workSheet.Cells[cbtpinheader_pointer, i].Style.Font.Bold = true;
                            workSheet.Cells[cbtpinheader_pointer, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[pin_poiter, i].Value = excelArrayPINs[row][2];
                            workSheet.Cells[pin_poiter, i].Style.Font.Size = 12;
                            workSheet.Cells[pin_poiter, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[cbtpasswordheader_pointer, i].Value = "CBT Password";
                            workSheet.Cells[cbtpasswordheader_pointer, i].Style.Font.Size = 12;
                            workSheet.Cells[cbtpasswordheader_pointer, i].Style.Font.Bold = true;
                            workSheet.Cells[cbtpasswordheader_pointer, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[cbtpassword_pointer, i].Value = cbtpassword;
                            workSheet.Cells[cbtpassword_pointer, i].Style.Font.Size = 12;
                            workSheet.Cells[cbtpassword_pointer, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            row = row + 1;
                        }
                    }

                    name_pointer = name_pointer + 11;
                    wifi_pointer = wifi_pointer + 11;
                    wifipassword_pointer = wifipassword_pointer + 11;
                    url_pointer = url_pointer + 11;
                    urladdress_pointer = urladdress_pointer + 11;
                    cbtpinheader_pointer = cbtpinheader_pointer + 11;
                    pin_poiter = pin_poiter + 11;
                    cbtpasswordheader_pointer = cbtpasswordheader_pointer + 11;
                    cbtpassword_pointer = cbtpassword_pointer + 11;
                }

                workSheet.Column(1).Width = 44;
                workSheet.Column(2).Width = 44;

                fileContents = package.GetAsByteArray();
            }

            iJSRuntime.InvokeAsync<UserStudent>(
               "saveAsFile",
               "StudentPINsPrintFormat_" + selectedClass + ".xlsx",
               Convert.ToBase64String(fileContents)
               );
        }

        async Task ExportStudentPINsPrint()
        {
            menuId = 1;
            if (String.IsNullOrWhiteSpace(selectedClass))
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Student PIN - Distribution",
                    Icon = "info",
                    Text = "Please Select A Class For Student PINS Distribution!"
                });
            }
            else
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Student PIN List Export Operation",
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
                        ExportToExcelStudentPINsDistribution();
                    }
                }
            }
        }


        #endregion

        #region [Generate Parent PINs]
        async Task GenerateParentPIN()
        {
            menuId = 1;
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Parent PIN Management Operation",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                studentsAll.Clear();
                studentsAll = await studentService.GetAllAsync("AdminStudent/GetStudents/1/0/0/0/1");
                progressbarInfo = "Please wait, generating Parent PINs...";

                IsShowProgress = false;
                x = 0;
                y = 0;
                int maxValue = studentsAll.Count();

                foreach (var item in studentsAll)
                {
                    y++;
                    x = ((decimal)(y) / maxValue) * 100;

                    student.STDID = item.STDID;
                    student.ParentPin = generatePIN.Generate(6);
                    await studentService.UpdateAsync("AdminStudent/UpdateStudent/", 10, student);

                    StateHasChanged();
                }

                IsShowProgress = true;
                await Swal.FireAsync("Parent PINs Generator", "Operation Completed Successfully.", "success");
            }
        }
        #endregion

        #region [Export / Print Parents PIN]
        void ExportToExcelParentPINs()
        {
            byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            int rowNum = 4;
            int sn = 0;

            using (var package = new ExcelPackage())
            {
                var workSheet = package.Workbook.Worksheets.Add("ParentPINs");

                #region Header Row

                workSheet.Cells[1, 1].Value = "Parent PIN List";
                workSheet.Cells[1, 1].Style.Font.Size = 16;
                workSheet.Cells[1, 1].Style.Font.Bold = true;

                workSheet.Cells[2, 1].Value = selectedClass;
                workSheet.Cells[2, 1].Style.Font.Size = 14;
                workSheet.Cells[2, 1].Style.Font.Bold = true;

                workSheet.Cells[5, 1].Value = "S/N";
                workSheet.Cells[5, 1].Style.Font.Size = 12;
                workSheet.Cells[5, 1].Style.Font.Bold = true;

                workSheet.Cells[5, 2].Value = "Admission No";
                workSheet.Cells[5, 2].Style.Font.Size = 12;
                workSheet.Cells[5, 2].Style.Font.Bold = true;

                workSheet.Cells[5, 3].Value = "Student Name";
                workSheet.Cells[5, 3].Style.Font.Size = 12;
                workSheet.Cells[5, 3].Style.Font.Bold = true;

                workSheet.Cells[5, 4].Value = "Parent PIN";
                workSheet.Cells[5, 4].Style.Font.Size = 12;
                workSheet.Cells[5, 4].Style.Font.Bold = true;
                #endregion

                foreach (var item in students)
                {
                    rowNum++;
                    sn++;

                    workSheet.Cells[rowNum + 1, 1].Value = sn;
                    workSheet.Cells[rowNum + 1, 1].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 1].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 2].Value = item.AdmissionNo;
                    workSheet.Cells[rowNum + 1, 2].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 2].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 3].Value = item.StudentName;
                    workSheet.Cells[rowNum + 1, 3].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 3].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 4].Value = item.ParentPin;
                    workSheet.Cells[rowNum + 1, 4].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 4].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                }

                double col1 = 5;
                workSheet.Column(1).Width = col1;
                workSheet.Column(2).Width = 15;
                workSheet.Column(3).Width = 47;
                workSheet.Column(4).Width = 15;

                double rowCount = students.Count() + 5;

                workSheet.View.FreezePanes(6, 1);

                workSheet.Cells["A5:D" + rowCount].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells["A5:D" + rowCount].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                workSheet.Cells["A5:D" + rowCount].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells["A5:D" + rowCount].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                workSheet.Cells["A5:A" + rowCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                fileContents = package.GetAsByteArray();
            }

            iJSRuntime.InvokeAsync<UserStudent>(
                "saveAsFile",
                "ParentPINs_" + selectedClass + ".xlsx",
                Convert.ToBase64String(fileContents)
                );
        }

        async Task ExportParentPINs()
        {
            menuId = 1;
            if (String.IsNullOrWhiteSpace(selectedClass))
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Parent PIN List",
                    Icon = "info",
                    Text = "Please Select A Class For Parent PINS Export!"
                });
            }
            else
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Parent PIN List Export Operation",
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
                        ExportToExcelParentPINs();
                    }
                }
            }
        }

        void ExportToExcelParentPINsDistribution()
        {
            byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            int kount = 0;
            excelArrayPINs.Clear();

            string url = cbtconnectionInfo.FirstOrDefault(conn => conn.ConnectionID == 4).ConnectionValue;

            using (var package = new ExcelPackage())
            {
                var workSheet = package.Workbook.Worksheets.Add("ParentPINs");

                foreach (var item in students)
                {
                    excelArrayPINs.Add(new List<string> { item.AdmissionNo, item.StudentInitials, item.ParentPin });
                }

                int row = 0;               
                kount = excelArrayPINs.Count();
                double k = (double)kount / 2;
                int maxValue = Convert.ToInt32(Math.Ceiling(k));

                int name_pointer = 1;
                int url_pointer = 2;
                int urladdress_pointer = 3;
                int cbtpinheader_pointer = 4;
                int pin_poiter = 5;

                for (int j = 1; j <= maxValue; j++)
                {
                    for (int i = 1; i < 3; i++)
                    {
                        if (row < kount)
                        {
                            workSheet.Cells[name_pointer, i].Value = excelArrayPINs[row][1];
                            workSheet.Cells[name_pointer, i].Style.Font.Size = 12;
                            workSheet.Cells[name_pointer, i].Style.Font.Bold = true;
                            workSheet.Cells[name_pointer, i].Style.Font.UnderLine = true;
                            workSheet.Cells[name_pointer, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[url_pointer, i].Value = "Wed URL";
                            workSheet.Cells[url_pointer, i].Style.Font.Size = 12;
                            workSheet.Cells[url_pointer, i].Style.Font.Bold = true;
                            workSheet.Cells[url_pointer, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[urladdress_pointer, i].Value = url;
                            workSheet.Cells[urladdress_pointer, i].Style.Font.Size = 12;
                            workSheet.Cells[urladdress_pointer, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[cbtpinheader_pointer, i].Value = "Parent PIN";
                            workSheet.Cells[cbtpinheader_pointer, i].Style.Font.Size = 12;
                            workSheet.Cells[cbtpinheader_pointer, i].Style.Font.Bold = true;
                            workSheet.Cells[cbtpinheader_pointer, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            workSheet.Cells[pin_poiter, i].Value = excelArrayPINs[row][2];
                            workSheet.Cells[pin_poiter, i].Style.Font.Size = 12;
                            workSheet.Cells[pin_poiter, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            row = row + 1;
                        }
                    }

                    name_pointer = name_pointer + 11;
                    url_pointer = url_pointer + 11;
                    urladdress_pointer = urladdress_pointer + 11;
                    cbtpinheader_pointer = cbtpinheader_pointer + 11;
                    pin_poiter = pin_poiter + 11;
                }

                workSheet.Column(1).Width = 44;
                workSheet.Column(2).Width = 44;

                fileContents = package.GetAsByteArray();
            }

            iJSRuntime.InvokeAsync<UserStudent>(
               "saveAsFile",
               "ParentPINsPrintFormat_" + selectedClass + ".xlsx",
               Convert.ToBase64String(fileContents)
               );
        }

        async Task ExportParentPINsPrint()
        {
            menuId = 1;
            if (String.IsNullOrWhiteSpace(selectedClass))
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Parent PIN - Distribution",
                    Icon = "info",
                    Text = "Please Select A Class For Parent PINS Distribution!"
                });
            }
            else
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Parent PIN List Export Operation",
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
                        ExportToExcelParentPINsDistribution();
                    }
                }
            }
        }


        #endregion

        #region [Student CBT / Parent Access Information]
        void AccessInfo()
        {
            menuId = 2;
        }

        async Task UpdateConnectionInfo()
        {
            cbtConnInfo.ConnectionID = selectedConnectionInfo.ConnectionID;
            cbtConnInfo.ConnectionValue = selectedConnectionInfo.ConnectionValue;

            await cbtConnectionInfoService.UpdateAsync("AcademicsCBT/UpdateConnectionInfo/", 1, cbtConnInfo);

            Snackbar.Add("Selected Row Entry Updated Successfully.");
        }

        #endregion
    }
}
