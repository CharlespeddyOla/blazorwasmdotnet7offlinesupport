using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Administration.Staff;
using WebAppAcademics.Shared.Models.Administration.Students;
using WebAppAcademics.Shared.Models.Settings;
using System.Collections;
using WebAppAcademics.Client.Extensions;
using static MudBlazor.CategoryTypes;
using System.Diagnostics;
using static MudBlazor.Defaults;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Academics.Subjects;

namespace WebAppAcademics.Client.Pages.Admin.Student
{
    public partial class Students
    {
        #region [Injection Declaration]
        [Inject] IJSRuntime iJSRuntime { get; set; }
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassList> classService { get; set; }
        [Inject] IAPIServices<ADMStudentType> studentTypeService { get; set; }
        [Inject] IAPIServices<SETStatusType> statusTypeService { get; set; }
        [Inject] IAPIServices<ADMStudents> studentService { get; set; }
        [Inject] IAPIServices<ADMSchParents> parentService { get; set; }
        [Inject] IAPIServices<SETGender> genderService { get; set; }
        [Inject] IAPIServices<ADMSchClassDiscipline> disciplineService { get; set; }
        [Inject] IAPIServices<SETCountries> countryService { get; set; }
        [Inject] IAPIServices<SETStates> stateService { get; set; }
        [Inject] IAPIServices<SETLGA> lgaService { get; set; }
        [Inject] IAPIServices<SETReligion> religionService { get; set; }
        [Inject] IAPIServices<ADMEmployee> staffService { get; set; }
        [Inject] IAPIServices<ADMSchClub> clubService { get; set; }
        [Inject] IAPIServices<ADMSchClubRole> roleService { get; set; }
        [Inject] IAPIServices<SETPayeeType> payeeTypeService { get; set; }
        [Inject] IAPIServices<SETMedical> medicalInfoService { get; set; }
        [Inject] IAPIServices<ADMStudentMEDHistory> mdecialHistoryService { get; set; }
        [Inject] IAPIServices<ADMSchEducationInstitute> previousSchoolService { get; set; }
        [Inject] IAPIServices<ADMStudentExit> exitTypeService { get; set; }
        [Inject] IAPIServices<ACDSbjAllocationStudents> subjectAllocationStudentService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksCognitive> studentMarksCognitiveService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksAssessment> studentOtherMarksService { get; set; }
        [Inject] IAPIServices<ACDReportCommentsTerminal> termEndCommentService { get; set; }
        [Inject] IAPIServices<ACDReportCommentMidTerm> midTermCommentService { get; set; }
        [Inject] IAPIServices<ACDReportCommentCheckPointIGCSE> checkpointigcseCommentService { get; set; }


        [CascadingParameter]
        public MainLayout Layout { get; set; }

        int toolBarMenuId { get; set; }
        bool isLoading { get; set; } = true;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Student List";
            toolBarMenuId = 1;
            await LoadDefautList();
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            schoolSession = await localStorageService.ReadEncryptedItemAsync<int>("schoolsession");
            schinfoid = await localStorageService.ReadEncryptedItemAsync<int>("schinfoid");
            await base.OnInitializedAsync();
        }

        #region [Section - Student List]

        #region [Variables Declaration]
        int termid { get; set; }
        int schoolSession { get; set; }
        int switchidCount { get; set; }
        int switchidList { get; set; }
        int schid { get; set; }
        int classid { get; set; }
        int studenttypeid { get; set; }
        int statustypeid { get; set; } = 1;
        int schoolCount { get; set; }
        int classCount { get; set; }
        int statusTypeCount { get; set; }
        int totalStudentCount { get; set; }

        string selectedSchool { get; set; }
        string selectedClass { get; set; }
        string selectedStudentType { get; set; }
        string selectedStatusType { get; set; } = "Active";
        string schoolCountDisplay { get; set; }
        string classCountDisplay { get; set; }
        string studentTypeCountDisplay { get; set; }
        string statusTypeCountDisplay { get; set; }
        string searchString { get; set; } = string.Empty;
        #endregion

        #region [Models Declaration]
        List<ADMSchlList> schools = new();
        List<ADMSchClassList> classList = new();
        List<ADMStudents> students = new();
        List<ADMStudents> studentsCount = new();
        List<ADMStudentType> studentType = new();
        List<SETStatusType> statusType = new();
        List<ADMSchClassList> allClassList = new();

        ADMStudents student = new();
        #endregion

        async Task LoadDefautList()
        {
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            studentType = await studentTypeService.GetAllAsync("AdminStudent/GetStudentType/1");
            statusType = await statusTypeService.GetAllAsync("Settings/GetStatusTypes/1");
            allClassList = await classService.GetAllAsync("AdminSchool/GetClassList/0/0/0");
            switchidCount = 4; switchidList = 1; schid = 0; classid = 0; studenttypeid = 0; statustypeid = 1;
            await StudentKount();
            await LoadList();
            statusTypeCountDisplay = "Total " + selectedStatusType + " Student Count: " + totalStudentCount;
        }

        async Task LoadList()
        {
            stopwatchvalue = new TimeSpan();
            Task task1 = Task.Run(() => LoadStudents());
            Task task2 = Task.Run(() => StartStopWatch());

            await Task.WhenAll(task1, task2);

            task1.Dispose();
            task2.Dispose();

            await Task.CompletedTask;
        }

        async Task StudentKount()
        {
            totalStudentCount = 0;
            totalStudentCount = await studentService.CountAsync("AdminStudent/GetCount/" + switchidCount + "/" + schid +
                                                                "/" + classid + "/" + studenttypeid + "/", statustypeid);            
        }

        async Task LoadStudents()
        {
            students.Clear();
            _processing = true;
            timerDisplay = "Please wait, loading list...";
            //await StudentKount();
            students = await studentService.GetAllAsync("AdminStudent/GetStudents/" + switchidList + "/" + schid +
                                                      "/" + classid + "/" + studenttypeid + "/" + statustypeid);
            _processing = false;
        }

        async Task<bool> StatusTypeFilter()
        {
            schoolCountDisplay = string.Empty;
            classCountDisplay = string.Empty;
            studentTypeCountDisplay = string.Empty;
            
            if (String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClass) && String.IsNullOrWhiteSpace(selectedStudentType))
            {
                switchidCount = 4; switchidList = 1;
                await StudentKount();
                await LoadList();
                
                schoolCountDisplay = string.Empty;
                classCountDisplay = string.Empty;
                statusTypeCountDisplay = "Total " + selectedStatusType + " Student Count: " + totalStudentCount;

                return true;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClass) && String.IsNullOrWhiteSpace(selectedStudentType))
            {
                switchidCount = 4; await StudentKount(); statusTypeCount = totalStudentCount;
                switchidCount = 2; switchidList = 2; await StudentKount(); await LoadList();

                schoolCountDisplay = selectedStatusType + " Student Count For " + selectedSchool + ": " + totalStudentCount;
                classCountDisplay = string.Empty;
                statusTypeCountDisplay = "Total " + selectedStatusType + " Student Count: " + statusTypeCount;

                return true;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClass) && String.IsNullOrWhiteSpace(selectedStudentType))
            {
                switchidCount = 4; await StudentKount(); statusTypeCount = totalStudentCount;
                switchidCount = 2; await StudentKount(); schoolCount = totalStudentCount;
                switchidCount = 1; switchidList = 3; await StudentKount(); await LoadList();

                schoolCountDisplay = selectedStatusType + " Student Count For " + selectedSchool + ": " + schoolCount;
                classCountDisplay = selectedStatusType + " Student Count For " + selectedClass + ": " + totalStudentCount;
                statusTypeCountDisplay = "Total " + selectedStatusType + " Student Count: " + statusTypeCount;

                return true;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClass) && !String.IsNullOrWhiteSpace(selectedStudentType))
            {
                switchidCount = 4; await StudentKount(); statusTypeCount = totalStudentCount;
                switchidCount = 2; await StudentKount(); schoolCount = totalStudentCount;
                switchidCount = 1; await StudentKount(); classCount = totalStudentCount;
                switchidCount = 5; switchidList = 5; await StudentKount(); await LoadList();

                schoolCountDisplay = selectedStatusType + " Student Count For " + selectedSchool + ": " + schoolCount;
                classCountDisplay = selectedStatusType + " Student Count For " + selectedClass + ": " + classCount;
                studentTypeCountDisplay = selectedStatusType + " " + selectedStudentType + " Student Count For " + selectedClass + ": " + totalStudentCount;
                statusTypeCountDisplay = "Total " + selectedStatusType + " Student Count: " + statusTypeCount;

                return true;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClass) && !String.IsNullOrWhiteSpace(selectedStudentType))
            {
                switchidCount = 4; await StudentKount(); statusTypeCount = totalStudentCount;
                switchidCount = 2; await StudentKount(); schoolCount = totalStudentCount;
                switchidCount = 6; switchidList = 4; await StudentKount(); await LoadList();

                schoolCountDisplay = selectedStatusType + " Student Count For " + selectedSchool + ": " + schoolCount;
                classCountDisplay = string.Empty;
                studentTypeCountDisplay = selectedStatusType + " " + selectedStudentType + " Student Count For " + selectedSchool + ": " + totalStudentCount;
                statusTypeCountDisplay = "Total " + selectedStatusType + " Student Count: " + statusTypeCount;

                return true;
            }
            else if (String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClass) && !String.IsNullOrWhiteSpace(selectedStudentType))
            {
                switchidCount = 4; await StudentKount(); statusTypeCount = totalStudentCount;
                switchidCount = 7; switchidList = 6; await StudentKount(); await LoadList();

                schoolCountDisplay = string.Empty;
                classCountDisplay = string.Empty;
                studentTypeCountDisplay = selectedStatusType + " " + selectedStudentType + " Student Count: " + totalStudentCount;
                statusTypeCountDisplay = "Total " + selectedStatusType + " Student Count: " + statusTypeCount;

                return true;
            }

            return false;
        }

        async Task<bool> SchoolFilter()
        {
            schoolCountDisplay = string.Empty;
            classCountDisplay = string.Empty;
            studentTypeCountDisplay = string.Empty;
            statusTypeCountDisplay = string.Empty;

            if (!String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClass) && String.IsNullOrWhiteSpace(selectedStudentType))
            {
                switchidCount = 4; await StudentKount(); statusTypeCount = totalStudentCount;
                switchidCount = 2; switchidList = 2; await StudentKount(); await LoadList();

                schoolCountDisplay = selectedStatusType + " Student Count For " + selectedSchool + ": " + totalStudentCount;
                classCountDisplay = string.Empty;
                statusTypeCountDisplay = "Total " + selectedStatusType + " Student Count: " + statusTypeCount;

                return true;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClass) && !String.IsNullOrWhiteSpace(selectedStudentType))
            {
                switchidCount = 4; await StudentKount(); statusTypeCount = totalStudentCount;
                switchidCount = 2; await StudentKount(); schoolCount = totalStudentCount;
                switchidCount = 6; switchidList = 4; await StudentKount(); await LoadList();

                schoolCountDisplay = selectedStatusType + " Student Count For " + selectedSchool + ": " + schoolCount;
                classCountDisplay = string.Empty;
                studentTypeCountDisplay = selectedStatusType + " " + selectedStudentType + " Student Count For " + selectedSchool + ": " + totalStudentCount;
                statusTypeCountDisplay = "Total " + selectedStatusType + " Student Count: " + statusTypeCount;

                return true;
            }

            return false;
        }

        async Task<bool> ClassFilter()
        {
            schoolCountDisplay = string.Empty;
            classCountDisplay = string.Empty;
            studentTypeCountDisplay = string.Empty;
            statusTypeCountDisplay = string.Empty;

            if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClass) && String.IsNullOrWhiteSpace(selectedStudentType))
            {
                switchidCount = 4; await StudentKount(); statusTypeCount = totalStudentCount;
                switchidCount = 2; await StudentKount(); schoolCount = totalStudentCount;
                switchidCount = 1; switchidList = 3; await StudentKount(); await LoadList();

                schoolCountDisplay = selectedStatusType + " Student Count For " + selectedSchool + ": " + schoolCount;
                classCountDisplay = selectedStatusType + " Student Count For " + selectedClass + ": " + totalStudentCount;
                statusTypeCountDisplay = "Total " + selectedStatusType + " Student Count: " + statusTypeCount;

                return true;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClass) && !String.IsNullOrWhiteSpace(selectedStudentType))
            {
                switchidCount = 4; await StudentKount(); statusTypeCount = totalStudentCount;
                switchidCount = 2; await StudentKount(); schoolCount = totalStudentCount;
                switchidCount = 1; await StudentKount(); classCount = totalStudentCount;
                switchidCount = 5; switchidList = 5; await StudentKount(); await LoadList();

                schoolCountDisplay = selectedStatusType + " Student Count For " + selectedSchool + ": " + schoolCount;
                classCountDisplay = selectedStatusType + " Student Count For " + selectedClass + ": " + classCount;
                studentTypeCountDisplay = selectedStatusType + " " + selectedStudentType + " Student Count For " + selectedClass + ": " + totalStudentCount;
                statusTypeCountDisplay = "Total " + selectedStatusType + " Student Count: " + statusTypeCount;

                return true;
            }

            return false;
        }

        async Task OnSelectedSchoolChanged(IEnumerable<string> e)
        {
            selectedSchool = e.ElementAt(0);
            schid = schools.FirstOrDefault(s => s.School == selectedSchool).SchID;

            selectedClass = string.Empty;
            classList.Clear();
            classList = allClassList.Where(c => c.SchID == schid).ToList();

            if (await SchoolFilter()) { }
        }

        async Task OnSelectedClassChanged(IEnumerable<string> e)
        {
            selectedClass = e.ElementAt(0);
            classid = classList.FirstOrDefault(s => s.ClassName == selectedClass).ClassID;

            if (await ClassFilter()) { }
        }

        async Task OnSelectedStudentTypeChanged(IEnumerable<string> e)
        {
            selectedStudentType = e.ElementAt(0);
            studenttypeid = studentType.FirstOrDefault(s => s.StudentType == selectedStudentType).StudentTypeID;

            selectedSchool = string.Empty;
            schools.Clear();
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            selectedClass = string.Empty;
            classList.Clear();
           
            selectedStatusType = string.Empty;
            statusType.Clear();
            statustypeid = 1;
            selectedStatusType = "Active";
            statusType = await statusTypeService.GetAllAsync("Settings/GetStatusTypes/1");

            schoolCountDisplay = string.Empty;
            classCountDisplay = string.Empty;
            studentTypeCountDisplay = string.Empty;
            students.Clear();
        }

        async Task OnSelectedStatusTypeChanged(IEnumerable<string> e)
        {
            selectedStatusType = e.ElementAt(0);
            statustypeid = statusType.FirstOrDefault(s => s.StatusType == selectedStatusType).StatusTypeID;

            if (await StatusTypeFilter()) { }
        }

        async Task RefreshList()
        {
            selectedSchool = string.Empty;
            schools.Clear();
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            selectedClass = string.Empty;
            classList.Clear();
           
            selectedStudentType = string.Empty;
            studentType.Clear();
            studentType = await studentTypeService.GetAllAsync("AdminStudent/GetStudentType/1");

            selectedStatusType = string.Empty;
            statusType.Clear();
            statustypeid = 1;
            selectedStatusType = "Active";
            statusType = await statusTypeService.GetAllAsync("Settings/GetStatusTypes/1");

            students.Clear();
            schoolCountDisplay = string.Empty;
            classCountDisplay = string.Empty;
            studentTypeCountDisplay = string.Empty;
            statusTypeCountDisplay = "Total " + selectedStatusType + " Student Count: " + students.Count();

            searchString = string.Empty;
            await LoadDefautList();
        }

        private bool FilterFunc(ADMStudents model)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (model.AdmissionNo.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.StudentName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.School.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.ClassName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.StudentType.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private async void GenerateExcel()
        {
            allClassList = await classService.GetAllAsync("AdminSchool/GetClassList/0/0/0");

            byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            int rowNum = 2;
            int sn = 0;
            double rowCount = 0;

            using (var package = new ExcelPackage())
            {
                foreach (var classname in allClassList.OrderBy(c => c.ClassListID))
                {                   
                    var workSheet = package.Workbook.Worksheets.Add(classname.ClassGroupName + classname.CATCode);

                    #region Header Row
                    workSheet.Cells[1, 1].Value = classname.ClassName;
                    workSheet.Cells[1, 1].Style.Font.Size = 16;
                    workSheet.Cells[1, 1].Style.Font.Bold = true;

                    workSheet.Cells[3, 1].Value = "S/N";
                    workSheet.Cells[3, 1].Style.Font.Size = 12;
                    workSheet.Cells[3, 1].Style.Font.Bold = true;

                    workSheet.Cells[3, 2].Value = "Admission No";
                    workSheet.Cells[3, 2].Style.Font.Size = 12;
                    workSheet.Cells[3, 2].Style.Font.Bold = true;

                    workSheet.Cells[3, 3].Value = "Student Name";
                    workSheet.Cells[3, 3].Style.Font.Size = 12;
                    workSheet.Cells[3, 3].Style.Font.Bold = true;
                    #endregion

                    rowCount = 0;
                    rowNum = 2;
                    sn = 0;

                    var _students = students.Where(c => c.ClassID == classname.ClassID).OrderBy(x => x.StudentName).ToList();

                    foreach (var item in _students)
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
                    }

                    workSheet.Column(1).Width = 5;
                    workSheet.Column(2).Width = 15;
                    workSheet.Column(3).Width = 50;

                    rowCount = _students.Count() + 3;

                    workSheet.View.FreezePanes(3, 1);

                    workSheet.Cells["A3:C" + rowCount].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A3:C" + rowCount].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A3:C" + rowCount].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A3:C" + rowCount].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    workSheet.Cells["A3:A" + rowCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                }



                fileContents = package.GetAsByteArray();

            }

            await iJSRuntime.InvokeAsync<Students>(
                "saveAsFile",
                "Student_List.xlsx",
                Convert.ToBase64String(fileContents)
                );
        }

        async Task DeleteStudent(int stdid)
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Student Deletion Operation",
                Text = "Do You Want To Continue With This Operation? This Operation Cannot Be Undo.",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                student.STDID = stdid;
                student.DeleteName = true;
                await studentService.UpdateAsync("AdminStudent/UpdateStudent/", 2, student);
                await Swal.FireAsync("Student Deletion Operation", "Selected Student Has Been Successfully Deleted.", "success");
                await RefreshList();
            }
        }

        async Task DisplayAll()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "This Operation Might Take A Longer Time To Display All Active Students",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                students = null;
                students = await studentService.GetAllAsync("AdminStudent/GetStudents/1/0/0/0/1");
            }                
        }

        #region [Section - Progress Bar: Timer]
        private bool _processing = false;
        TimeSpan stopwatchvalue = new();
        bool Is_stopwatchrunning { get; set; } = false;
        string timerDisplay { get; set; }
        //int val = 0;    

        async Task StartStopWatch()
        {
            Is_stopwatchrunning = true;

            while (Is_stopwatchrunning)
            {
                await Task.Delay(1000);
                if (Is_stopwatchrunning)
                {
                    await InvokeAsync(() =>
                    {
                        stopwatchvalue = stopwatchvalue.Add(new TimeSpan(0, 0, 1));
                        //val = (int)stopwatchvalue.TotalHours + (int)stopwatchvalue.TotalMinutes + (int)stopwatchvalue.TotalSeconds;
                        if (_processing == false)
                        {
                            Is_stopwatchrunning = false;
                            StateHasChanged();
                        }

                        StateHasChanged();
                    });
                }
            }
        }

        #endregion


        #endregion

        #region [Section - Student Details]

        #region [Variables Declaration]
        int stdid { get; set; }
        int newstdid { get; set; }

        // Set default page title and button text
        string pagetitle { get; set; } = "Add a new Student";
        string buttontitle = "Save";

        //Photo Declaration Section
        string imgSrc { get; set; } = "";
        IBrowserFile file { get; set; } = null;
        byte[] _fileBytes { get; set; } = null;
        Utilities utilities = new Utilities();
        long maxFileSize { get; set; } = 1024 * 1024 * 15;

        int schinfoid { get; set; }
        int payeetypeid { get; set; } = 2;

        string _selectedSchool { get; set; }
        string _selectedClass { get; set; }
        string _selectedStudentType { get; set; }
        string selectedGender { get; set; }
        string selectedDiscipline { get; set; }
        string selectedExitType { get; set; }        
        string selectedRegion { get; set; }
        string selectedPayeeType { get; set; }
        string selectedParent { get; set; }
        string selectedClub { get; set; }
        string selectedClubRole { get; set; }
        string selectedCountry { get; set; }
        string selectedState { get; set; }
        string selectedLGA { get; set; }
        string selectedPreviousSchool { get; set; }
        string studentStatus { get; set; } = "Active";

        bool studentStatuschecked { get; set; } = true;
        bool disableControl { get; set; } = true;
        bool fixed_header { get; set; } = true;

        bool disableSaveButton { get; set; } = true;
        string editFormId { get; set; } = "editformid";
        #endregion

        #region [Models Declaration]
        List<ADMSchlList> _schools = new();
        List<ADMSchClassList> _classList = new();
        List<ADMStudentType> _studentType = new();
        List<ADMStudents> _students = new();
        List<SETGender> gender = new();
        List<ADMSchClassDiscipline> desciplines = new();
        List<SETCountries> country = new();
        List<SETStates> states = new();
        List<SETLGA> lgas = new();
        List<SETReligion> religion = new();
        List<ADMSchParents> parents = new();
        List<ADMEmployee> staffs = new();
        List<ADMSchClub> club = new();
        List<ADMSchClubRole> clubrole = new();
        List<ADMStudentExit> exittype = new();
        List<SETMedical> medicalinfo = new();
        List<ADMStudentMEDHistory> medicalhistory = new();
        List<SETPayeeType> payeetype = new();
        List<ADMSchEducationInstitute> prevschools = new();
        ArrayList ArrayStudentMedicalHistory = new();

        ADMStudentMEDHistory studentmedicalinfo = new();
        ACDSbjAllocationStudents subjectAllocation = new();
        ACDStudentsMarksCognitive cognitiveMark = new();
        ACDStudentsMarksAssessment otherMark = new();
        ACDReportCommentsTerminal termEndComment = new();
        ACDReportCommentMidTerm midTermComment = new();
        ACDReportCommentCheckPointIGCSE checkpoinigcseComment = new();
        #endregion

        #region [Load Events]
        async Task LoadPayeeType(int switchid)
        {
            payeetype = await payeeTypeService.GetAllAsync("Settings/GetPayeeTypes/" + switchid);
        }

        async Task LoadClassList(int switchid, int schid)
        {
            _classList = await classService.GetAllAsync("AdminSchool/GetClassList/" + switchid + "/" + schid + "/0");
        }

        async Task LoadStates(int countryid)
        {
            states = await stateService.GetAllAsync("Settings/GetStates/1/" + countryid);
        }

        async Task LoadLGA(int stateid)
        {
            lgas = await lgaService.GetAllAsync("Settings/GetLGAs/1/" + stateid);
        }

        async Task LoadParents(int switchid)
        {
            parents = await parentService.GetAllAsync("AdminStudent/GetParents/1/" + switchid);
        }

        async Task LoadStaffs()
        {
            staffs = await staffService.GetAllAsync("AdminStaff/GetStaffs/8/1/0/0/0");
        }

        async Task LoadListForDetails()
        {
            _schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            gender = await genderService.GetAllAsync("Settings/GetGenderList/1");
            _studentType = await studentTypeService.GetAllAsync("AdminStudent/GetStudentType/1");
            desciplines = await disciplineService.GetAllAsync("AdminSchool/GetDisciplines/1");
            country = await countryService.GetAllAsync("Settings/GetCountries/1");
            religion = await religionService.GetAllAsync("Settings/GetReligionList/1");
            club = await clubService.GetAllAsync("AdminStudent/GetClubs/1");
            clubrole = await roleService.GetAllAsync("AdminStudent/GetClubRoles/1");
            exittype = await exitTypeService.GetAllAsync("AdminStudent/GetExitTypeList/1");
            await LoadPayeeType(1);
            medicalinfo = await medicalInfoService.GetAllAsync("Settings/GetMedicalInfoList/1");
            prevschools = await previousSchoolService.GetAllAsync("AdminSchool/GetPreviousSchools/1");
            //students = await studentService.GetAllAsync("Students/GetStudents/1/0/0/0/1");
        }

        void GenerateStudentMedicalHistoryTemplate()
        {
            ArrayStudentMedicalHistory.Clear();

            foreach (var item in medicalinfo)
            {
                ArrayStudentMedicalHistory.Add(new
                    StudentMedicalHistory(0, schinfoid, 0, item.MEDID, item.MEDName, false, string.Empty));
            }

            foreach (StudentMedicalHistory studentmedhistory in ArrayStudentMedicalHistory)
            {
                medicalhistory.Add(new ADMStudentMEDHistory()
                {
                    MEDHistoryID = 0,
                    SchInfoID = studentmedhistory.SchInfoID,
                    STDID = 0,
                    MEDID = studentmedhistory.MEDID,
                    MEDName = studentmedhistory.MEDName,
                    MEDValue = false,
                    MEDTextValue = string.Empty,
                });
            }
        }

        async Task RetrieveStudent(int _stdid)
        {
            disableControl = false;
            student = await studentService.GetByIdAsync("AdminStudent/GetStudent/", _stdid);
            stdid = _stdid;

            pagetitle = student.StudentName;
            buttontitle = "Update";

            if (student.StatusTypeID == 1)
            {
                studentStatuschecked = true;
            }
            else
            {
                studentStatuschecked = false;
            }

            if (student.studentPhoto != null)
            {
                student.studentPhoto = utilities.GetImage(Convert.ToBase64String(student.studentPhoto));
                student.ImageUrl = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(student.studentPhoto));
            }
            imgSrc = student.ImageUrl;

            medicalhistory = await mdecialHistoryService.GetAllAsync("AdminStudent/GetMedicalHistoryList/1/" + stdid);
            if (medicalhistory.Count() == 0)
            {
                GenerateStudentMedicalHistoryTemplate();
            }

            await LoadClassList(1, student.SchID);
            if (student.PayeeTypeID == 2)
            {
                await LoadParents(1);
            }
            else if (student.PayeeTypeID == 3)
            {
                await LoadStaffs();
            }
            await LoadStates(student.CountryID);
            await LoadLGA(student.StateID);
        }

        #endregion

        #region [OnChange Events]

        async Task OnSchoolChanged(IEnumerable<string> value)
        {
            _selectedSchool = value.ElementAt(0);
            student.School = _selectedSchool;
            student.SchID = _schools.FirstOrDefault(s => s.School == _selectedSchool).SchID;
            student.ClassName = string.Empty;

            _classList = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + student.SchID + "/0");
            //await LoadClassList(1, student.SchID, 0);
        }

        async Task OnClassChanged(IEnumerable<string> value)
        {
            _selectedClass = value.ElementAt(0);
            student.ClassName = _selectedClass;
            student.ClassID = _classList.FirstOrDefault(s => s.ClassName == _selectedClass).ClassID;

            var classdetails = await classService.GetByIdAsync("AdminSchool/GetClass/", student.ClassID);
            student.ClassListID = classdetails.ClassListID;
            student.ClassTeacherID = classdetails.StaffID;
        }

        void OnDisciplineChanged(IEnumerable<string> value)
        {
            selectedDiscipline = value.ElementAt(0);
            student.Discipline = selectedDiscipline;
            student.DisciplineID = desciplines.FirstOrDefault(s => s.Discipline == selectedDiscipline).DisciplineID;
        }

        void OnGenderChange(IEnumerable<string> value)
        {
            selectedGender = value.ElementAt(0);
            student.Gender = selectedGender;
            student.GenderID = gender.FirstOrDefault(s => s.Gender == selectedGender).GenderID;
        }

        void OnStudentTypeChange(IEnumerable<string> value)
        {
            _selectedStudentType = value.ElementAt(0);
            student.StudentType = _selectedStudentType;
            student.StudentTypeID = studentType.FirstOrDefault(s => s.StudentType == _selectedStudentType).StudentTypeID;
        }

        void OnReligionChanged(IEnumerable<string> value)
        {
            selectedRegion = value.ElementAt(0);
            student.Religion = selectedRegion;
            student.ReligionID = religion.FirstOrDefault(s => s.Religion == selectedRegion).ReligionID;
        }
        
        async Task OnPayeeTypeChanged(IEnumerable<string> value)
        {
            selectedPayeeType = value.ElementAt(0);
            student.PayeeType = selectedPayeeType;
            student.PayeeTypeID = payeetype.FirstOrDefault(s => s.PayeeType == selectedPayeeType).PayeeTypeID;
            payeetypeid = student.PayeeTypeID;
            
            if (student.PayeeTypeID == 2)
            {
                parents = await parentService.GetAllAsync("AdminStudent/GetParents/1/1");
            }
            else if (student.PayeeTypeID == 3)
            {
                staffs = await staffService.GetAllAsync("AdminStaff/GetStaffs/8/1/0/0/0");
            }
        }

        void OnParentChanged(IEnumerable<string> value)
        {
            selectedParent = value.ElementAt(0);
            
            if (payeetypeid == 2)
            {
                student.ParentName = selectedParent;
                student.PayeeID = parents.FirstOrDefault(s => s.ParentName == selectedParent).ParentID;
                student.StaffID = 0;
            }
            else if (payeetypeid == 3)
            {
                student.ParentName = selectedParent;
                student.StaffID = staffs.FirstOrDefault(s => s.ParentName == selectedParent).StaffID;
                student.PayeeID = 0;
            }
        }

        void OnClubChanged(IEnumerable<string> value)
        {
            selectedClub = value.ElementAt(0);
            student.ClubID = club.FirstOrDefault(s => s.ClubName == selectedClub).ClubID;
            student.ClubName = selectedClub;
        }

        void OnRoleChanged(IEnumerable<string> value)
        {
            selectedClubRole = value.ElementAt(0);
            student.RoleID = clubrole.FirstOrDefault(s => s.RoleName == selectedClubRole).RoleID; ;
            student.RoleName = selectedClubRole;
        }

        async Task OnCountryChanged(IEnumerable<string> value)
        {
            selectedCountry = value.ElementAt(0);
            student.Country = selectedCountry;
            student.CountryID = country.FirstOrDefault(s => s.Country == selectedCountry).CountryID;
            student.State = string.Empty;

            states = await stateService.GetAllAsync("Settings/GetStates/1/" + student.CountryID);
            //await LoadStates(student.CountryID);
        }

        async Task OnStateChanged(IEnumerable<string> value)
        {
            selectedState = value.ElementAt(0);
            student.State = selectedState;
            student.StateID = states.FirstOrDefault(s => s.State == selectedState).StateID;
            student.LGA = string.Empty;

            lgas = await lgaService.GetAllAsync("Settings/GetLGAs/1/" + student.StateID);
            //await LoadLGA(student.StateID);
        }

        void OnLGAChanged(IEnumerable<string> value)
        {
            selectedLGA = value.ElementAt(0);

            if (selectedLGA != "none")
            {
                student.LGA = selectedLGA;
                student.LGAID = lgas.FirstOrDefault(s => s.LGA == selectedLGA).LGAID;
            }
        }

        void OnPreviousSchoolChange(IEnumerable<string> value)
        {
            selectedPreviousSchool = value.ElementAt(0);
            student.EDUInstitute = selectedPreviousSchool;
            student.EDUID = prevschools.FirstOrDefault(s => s.EDUInstitute == selectedPreviousSchool).EDUID;
        }

        void OnExitTypeChanged(IEnumerable<string> value)
        {
            selectedExitType = value.ElementAt(0);
            student.ExitType = selectedExitType;
            student.ExitID = exittype.FirstOrDefault(s => s.ExitType == selectedExitType).ExitID;
        }

        #endregion

        #region [Photo Processing]

        async Task UploadFiles(InputFileChangeEventArgs e)
        {
            try
            {
                file = e.File;
                using var ms = new MemoryStream();
                var stream = file.OpenReadStream(maxFileSize);

                await stream.CopyToAsync(ms);
                _fileBytes = ms.ToArray();

                var photo = utilities.GetImage(Convert.ToBase64String(_fileBytes));
                imgSrc = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(photo), 50, 50);

                //TODO upload the files to the server
            }
            catch (Exception ex)
            {
                var msg = file.Name + ex.Message;
            }
        }


        #endregion

        #region [Save Events]

        async Task SaveMedicalInfo()
        {
            if (stdid != 0)
            {
                var medHistory = await mdecialHistoryService.GetAllAsync("AdminStudent/GetMedicalHistoryList/1/" + stdid); 

                if (medHistory.Count() > 0)
                {
                    foreach (var item in medicalhistory)
                    {
                        studentmedicalinfo.MEDHistoryID = item.MEDHistoryID;
                        studentmedicalinfo.MEDValue = item.MEDValue;
                        studentmedicalinfo.MEDTextValue = item.MEDTextValue;
                        await mdecialHistoryService.UpdateAsync("AdminStudent/UpdateMedicalHistory/", 1, studentmedicalinfo);
                    }
                }
                else
                {
                    foreach (var item in medicalhistory)
                    {
                        studentmedicalinfo.SchInfoID = schinfoid;
                        studentmedicalinfo.STDID = stdid;
                        studentmedicalinfo.MEDID = item.MEDID;
                        studentmedicalinfo.MEDValue = item.MEDValue;
                        studentmedicalinfo.MEDTextValue = item.MEDTextValue;
                        await mdecialHistoryService.SaveAsync("AdminStudent/AddMedicalHistory/", studentmedicalinfo);
                    }
                }
            }
            else
            {
                foreach (var item in medicalhistory)
                {
                    studentmedicalinfo.SchInfoID = schinfoid;
                    studentmedicalinfo.STDID = newstdid;
                    studentmedicalinfo.MEDID = item.MEDID;
                    studentmedicalinfo.MEDValue = item.MEDValue;
                    studentmedicalinfo.MEDTextValue = item.MEDTextValue;
                    await mdecialHistoryService.SaveAsync("AdminStudent/AddMedicalHistory/", studentmedicalinfo);
                }
            }
        }
               
        async Task SubmitValidForm()
        {
            if (_fileBytes != null)
            {
                student.photoStatus = 1;
                student.studentPhoto = _fileBytes;
            }

            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Student Details Save/Update Operation",
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
                    if (student.STDID == 0)
                    {
                        if (student.StudentID > 0)
                        {
                            student.SchInfoID = schinfoid;
                            student.TermID = termid;
                            student.EDUID = 1;

                            // Insert if STDID is zero.
                            var response = await studentService.SaveAsync("AdminStudent/AddStudent/", student);
                            newstdid = response.STDID;
                            student.STDID = newstdid;
                            student.Id = newstdid;
                            await studentService.UpdateAsync("AdminStudent/UpdateStudent/", 17, student);
                            await SaveMedicalInfo();
                            await Swal.FireAsync("The New Student Details", "Has Been Successfully Saved.", "success");
                        }
                    }
                    else
                    {
                        if (studentStatuschecked == true)
                        {
                            student.StatusTypeID = 1;
                        }
                        else if (studentStatuschecked == false)
                        {
                            student.StatusTypeID = 2;
                        }

                        await studentService.UpdateAsync("AdminStudent/UpdateStudent/", 1, student);
                        await SaveMedicalInfo();

                        ////Update Subjects Allocated When Student Class Change
                        //subjectAllocation.STDID = stdid;
                        //subjectAllocation.SchID = student.SchID;
                        //subjectAllocation.ClassID = student.ClassID;
                        //subjectAllocation.ClassListID = student.ClassListID;
                        //subjectAllocation.SchSession = schoolSession;
                        //await subjectAllocationStudentService.UpdateAsync("AcademicsSubjects/UpdateStudentAllocation/", 2, subjectAllocation);

                        ////Update Comments Entry When Class Teacher Change
                        //termEndComment.STDID = stdid;
                        //termEndComment.ClassID = student.ClassID;
                        //termEndComment.ClassTeacherID = student.ClassTeacherID;                        
                        //await termEndCommentService.UpdateAsync("AcademicsResultsComments/UpdateTermEndComment/", 5, termEndComment);

                        //midTermComment.STDID = stdid;
                        //midTermComment.ClassID = student.ClassID;
                        //midTermComment.ClassTeacherID = student.ClassTeacherID;
                        //await midTermCommentService.UpdateAsync("AcademicsResultsComments/UpdateMidTermComment/", 5, midTermComment);

                        //checkpoinigcseComment.STDID = stdid;
                        //checkpoinigcseComment.ClassID = student.ClassID;
                        //checkpoinigcseComment.ClassTeacherID = student.ClassTeacherID;
                        //await checkpointigcseCommentService.UpdateAsync("AcademicsResultsComments/UpdateCheckPointIGCSEComment/", 5, checkpoinigcseComment);


                        await Swal.FireAsync("Selected Student Details", "Has Been Successfully Updated.", "success");
                    }

                    await StudentListEvent();
                }
            }
        }

        async Task InvalidEntries()
        {
            await Swal.FireAsync("Please Check Your Entries.", "One or More Entries has error.", "error");
        }


        #endregion


        #endregion

        #region [Section - Click Events]
        async Task StudentListEvent()
        {
            toolBarMenuId = 1;
            buttontitle = "Save";
            disableSaveButton = true;
            await RefreshList();
        }

        async Task AddNewStudentEvent()
        {
            toolBarMenuId = 2;
            buttontitle = "Save";
            disableSaveButton = false;
            stdid = 0;
            student = new ADMStudents();
            await LoadListForDetails();
            student.StatusTypeID = 1;
            student.StudentID = students.Count() + 1;
            GenerateStudentMedicalHistoryTemplate();
        }

        async Task UpdateStudentEvent(int _stdid)
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            student = new ADMStudents();
            await LoadListForDetails();
            await RetrieveStudent(_stdid);
        }

        #endregion

    }
}
