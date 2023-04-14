using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.CBT;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Academics.Subjects;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Shared.Models.Administration.Students;

namespace WebAppAcademics.Client.Pages.Academics.Exam.CBT
{
    public partial class Exams
    {

        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] IJSRuntime iJSRuntime { get; set; }
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassGroup> classGroupService { get; set; }
        [Inject] IAPIServices<ADMSchClassList> classService { get; set; }
        [Inject] IAPIServices<ACDReportType> reportTypeService { get; set; }
        [Inject] IAPIServices<CBTExamType> examTypeService { get; set; }
        [Inject] IAPIServices<CBTExams> examService { get; set; }
        [Inject] IAPIServices<ACDSbjAllocationTeachers> subjectAllocationTeacherService { get; set; }
        [Inject] IAPIServices<CBTStudentScores> studentCBTService { get; set; }
        [Inject] IAPIServices<CBTExamTakenFlags> examTakenService { get; set; }
        [Inject] IAPIServices<ADMStudents> studentService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "CBT Examinations";
            toolBarMenuId = 1;
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            schoolSession = await localStorageService.ReadEncryptedItemAsync<int>("schoolsession");
            academicSession = await localStorageService.ReadEncryptedItemAsync<string>("academicsession");
            staffid = await localStorageService.ReadEncryptedItemAsync<int>("staffid");
            roleid = await localStorageService.ReadEncryptedItemAsync<int>("roleid");
            await LoadDefaultList();
            await base.OnInitializedAsync();
        }


        #region [Section - CBT Exam List]
        #region [Models Declaration]
        //Exam List Models
        List<ADMSchlList> schools = new();
        List<ADMSchClassGroup> classGroups = new();
        List<ACDReportType> reportType = new();
        List<CBTExamType> examType = new();
        List<CBTExams> exams = new();
        #endregion

        #region [Variables Declaration]
        int toolBarMenuId { get; set; }
        int termid { get; set; }
        int schoolSession { get; set; }
        int staffid { get; set; }
        int roleid { get; set; }
        int schid { get; set; }
        int classlistid { get; set; }
        int reportTypeID { get; set; }
        int examTypeID { get; set; }

        string academicSession { get; set; }
        string selectedSchool { get; set; }
        string selectedClass { get; set; }
        string selectedReportType { get; set; }
        string selectedExamType { get; set; }

        string schoolCountDisplay { get; set; }
        string classCountDisplay { get; set; }
        string examTypeCountDisplay { get; set; }
        string reportTypeCountDisplay { get; set; }
        string searchString { get; set; } = string.Empty;


        bool isLoading { get; set; } = true;
        string loadingmessage { get; set; } = "Waiting for your selection...";

        #endregion

        async Task LoadDefaultList()
        {
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            reportType = await reportTypeService.GetAllAsync("AcademicsMarkSettings/GetResultTypeSettings/1");
            examType = await examTypeService.GetAllAsync("AcademicsCBT/GetCBTExamTypes/1");
            reportTypeID = reportType.FirstOrDefault(r => r.SelectedExam == true).ReportTypeID;

            loadingmessage = "Please wait, while loading...";
            if (roleid == 1) //Administrator
            {
                await LoadExams(1, termid, 0, 0);
                schoolCountDisplay = "Total Exam Count: " + exams.Count();
            }
            else
            {
                await LoadExams(4, termid, 0, staffid);
                schoolCountDisplay = "Total Exam Count: " + exams.Count();
            }
        }

        async Task LoadExams(int switchid, int _termid, int _classlistid, int _staffid)
        {
            isLoading = true;
            StateHasChanged();

            try
            {
                await Task.Delay(500);
                exams = await examService.GetAllAsync("AcademicsCBT/GetCBTExams/" +
                                                        switchid + "/" + _termid + "/" + _classlistid + "/" + _staffid);
            }
            finally
            {
                isLoading = false;
            }
        }

        int GetFilterID()
        {
            if (!String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClass)
                && String.IsNullOrWhiteSpace(selectedExamType) && String.IsNullOrWhiteSpace(selectedReportType))
            {
                //Filter By School
                return 1;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClass)
                && !String.IsNullOrWhiteSpace(selectedExamType) && String.IsNullOrWhiteSpace(selectedReportType))
            {
                //Filter By School And Exam Type
                return 2;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClass)
                && String.IsNullOrWhiteSpace(selectedExamType) && !String.IsNullOrWhiteSpace(selectedReportType))
            {
                //Filter By School And Report Type
                return 3;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClass)
                && !String.IsNullOrWhiteSpace(selectedExamType) && !String.IsNullOrWhiteSpace(selectedReportType))
            {
                //Filter By School, Exam Type And Report Type
                return 4;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClass)
                && String.IsNullOrWhiteSpace(selectedExamType) && String.IsNullOrWhiteSpace(selectedReportType))
            {
                //Filter By Class
                return 5;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClass)
                && !String.IsNullOrWhiteSpace(selectedExamType) && String.IsNullOrWhiteSpace(selectedReportType))
            {
                //Filter By Class And Exam Type
                return 6;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClass)
                && String.IsNullOrWhiteSpace(selectedExamType) && !String.IsNullOrWhiteSpace(selectedReportType))
            {
                //Filter By Class And Report Type
                return 7;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClass)
                && !String.IsNullOrWhiteSpace(selectedExamType) && !String.IsNullOrWhiteSpace(selectedReportType))
            {
                //Filter By Class, Exam Type And Report Type
                return 8;
            }
            else if (String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClass)
                && !String.IsNullOrWhiteSpace(selectedExamType) && String.IsNullOrWhiteSpace(selectedReportType))
            {
                //Filter By Exam Type
                return 9;
            }
            else if (String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClass)
                && String.IsNullOrWhiteSpace(selectedExamType) && !String.IsNullOrWhiteSpace(selectedReportType))
            {
                //Filter By Report Type
                return 10;
            }

            return 0;
        }

        void ExamFilters()
        {
            schoolCountDisplay = string.Empty;
            classCountDisplay = string.Empty;
            examTypeCountDisplay = string.Empty;
            reportTypeCountDisplay = string.Empty;

            switch (GetFilterID())
            {
                case 1: //Filter By School
                    exams = exams.Where(ex => ex.SchID == schid).ToList();
                    schoolCountDisplay = "Total Exam Count For " + selectedSchool + " School: " + exams.Where(ex => ex.SchID == schid).Count();
                    classCountDisplay = string.Empty;
                    examTypeCountDisplay = string.Empty;
                    reportTypeCountDisplay = string.Empty;
                    break;
                case 2: //Filter By School And Exam Type
                    exams = exams.Where(ex => ex.SchID == schid && ex.ExamTypeID == examTypeID).ToList();
                    schoolCountDisplay = "Total Exam Count For " + selectedSchool + " School: " + exams.Where(ex => ex.SchID == schid).Count();
                    classCountDisplay = string.Empty;
                    examTypeCountDisplay = "Total Exam Count For " + selectedExamType + ": " + exams.Where(ex => ex.SchID == schid
                                                                                                    && ex.ExamTypeID == examTypeID).Count();
                    reportTypeCountDisplay = string.Empty;
                    break;
                case 3: //Filter By School And Report Type
                    exams = exams.Where(ex => ex.SchID == schid && ex.ReportTypeID == reportTypeID).ToList();
                    schoolCountDisplay = "Total Exam Count For " + selectedSchool + " School: " + exams.Where(ex => ex.SchID == schid).Count();
                    classCountDisplay = string.Empty;
                    examTypeCountDisplay = string.Empty;
                    reportTypeCountDisplay = "Total Exam Count For " + selectedReportType + ": " + exams.Where(ex => ex.SchID == schid
                                                                                                             && ex.ReportTypeID == reportTypeID).Count();
                    break;
                case 4:  //Filter By School, Exam Type And Report Type
                    exams = exams.Where(ex => ex.SchID == schid && ex.ExamTypeID == examTypeID && ex.ReportTypeID == reportTypeID).ToList();
                    schoolCountDisplay = "Total Exam Count For " + selectedSchool + " School: " + exams.Where(ex => ex.SchID == schid
                                                                                                    && ex.ExamTypeID == examTypeID
                                                                                                    && ex.ReportTypeID == reportTypeID).Count();
                    classCountDisplay = string.Empty;
                    examTypeCountDisplay = "Total Exam Count For " + selectedExamType + ": " + exams.Where(ex => ex.SchID == schid
                                                                                                    && ex.ExamTypeID == examTypeID).Count();
                    reportTypeCountDisplay = "Total Exam Count For " + selectedReportType + ": " + exams.Where(ex => ex.SchID == schid
                                                                                                    && ex.ReportTypeID == reportTypeID).Count();
                    break;
                case 5: //Filter By Class
                    exams = exams.Where(ex => ex.SchID == schid && ex.ClassListID == classlistid).ToList();
                    schoolCountDisplay = "Total Exam Count For " + selectedSchool + " School: " + exams.Where(ex => ex.SchID == schid).Count();
                    classCountDisplay = "Total Exam Count For " + selectedClass + ": " + exams.Where(ex => ex.SchID == schid
                                                                                            && ex.ClassListID == classlistid).Count();
                    examTypeCountDisplay = string.Empty;
                    reportTypeCountDisplay = string.Empty;
                    break;
                case 6: //Filter By Class And Exam Type
                    exams = exams.Where(ex => ex.SchID == schid && ex.ClassListID == classlistid && ex.ExamTypeID == examTypeID).ToList();
                    schoolCountDisplay = "Total Exam Count For " + selectedSchool + " School: " + exams.Where(ex => ex.SchID == schid).Count();
                    classCountDisplay = "Total Exam Count For " + selectedClass + ": " + exams.Where(ex => ex.SchID == schid
                                                                                            && ex.ClassListID == classlistid).Count();
                    examTypeCountDisplay = "Total Exam Count For " + selectedExamType + ": " + exams.Where(ex => ex.SchID == schid
                                                                                            && ex.ClassListID == classlistid
                                                                                            && ex.ExamTypeID == examTypeID).Count();
                    reportTypeCountDisplay = string.Empty;
                    break;
                case 7: //Filter By Class And Report Type
                    exams = exams.Where(ex => ex.SchID == schid && ex.ClassListID == classlistid && ex.ReportTypeID == reportTypeID).ToList();
                    schoolCountDisplay = "Total Exam Count For " + selectedSchool + " School: " + exams.Where(ex => ex.SchID == schid).Count();
                    classCountDisplay = "Total Exam Count For " + selectedClass + ": " + exams.Where(ex => ex.SchID == schid
                                                                                            && ex.ClassListID == classlistid).Count();
                    examTypeCountDisplay = string.Empty;
                    reportTypeCountDisplay = "Total Exam Count For " + selectedReportType + ": " + exams.Where(ex => ex.SchID == schid
                                                                                            && ex.ClassListID == classlistid
                                                                                            && ex.ReportTypeID == reportTypeID).Count();
                    break;
                case 8: //Filter By Class, Exam Type And Report Type
                    exams = exams.Where(ex => ex.SchID == schid && ex.ClassListID == classlistid && ex.ExamTypeID == examTypeID
                            && ex.ReportTypeID == reportTypeID).ToList();
                    schoolCountDisplay = "Total Exam Count For " + selectedSchool + " School: " + exams.Where(ex => ex.SchID == schid).Count();
                    classCountDisplay = "Total Exam Count For " + selectedClass + ": " + exams.Where(ex => ex.SchID == schid
                                                                                            && ex.ClassListID == classlistid && ex.ExamTypeID == examTypeID
                                                                                            && ex.ReportTypeID == reportTypeID).Count();
                    examTypeCountDisplay = "Total Exam Count For " + selectedExamType + ": " + exams.Where(ex => ex.SchID == schid
                                                                                            && ex.ClassListID == classlistid
                                                                                            && ex.ExamTypeID == examTypeID).Count();
                    reportTypeCountDisplay = "Total Exam Count For " + selectedReportType + ": " + exams.Where(ex => ex.SchID == schid
                                                                                            && ex.ClassListID == classlistid
                                                                                            && ex.ReportTypeID == reportTypeID).Count();
                    break;
                case 9: //Filter By Exam Type
                    exams = exams.Where(ex => ex.ExamTypeID == examTypeID).ToList();
                    schoolCountDisplay = string.Empty;
                    classCountDisplay = string.Empty;
                    examTypeCountDisplay = "Total Exam Count For " + selectedExamType + ": " + exams.Where(ex => ex.ExamTypeID == examTypeID).Count();
                    reportTypeCountDisplay = string.Empty;
                    break;
                case 10: //Filter By Report Type
                    exams = exams.Where(ex => ex.ReportTypeID == reportTypeID).ToList();
                    schoolCountDisplay = string.Empty;
                    classCountDisplay = string.Empty;
                    examTypeCountDisplay = string.Empty;
                    reportTypeCountDisplay = "Total Exam Count For " + selectedReportType + ": " + exams.Where(ex => ex.ReportTypeID == reportTypeID).Count();
                    break;
                default: //All
                    schoolCountDisplay = "Total Exam Count: " + exams.Count();
                    break;
            }
        }

        async Task OnSchoolChanged(IEnumerable<string> e)
        {
            selectedSchool = e.ElementAt(0);
            schid = schools.FirstOrDefault(s => s.School == selectedSchool).SchID;

            classlistid = 0;
            selectedClass = string.Empty;
            classGroups.Clear();
            classGroups = await classGroupService.GetAllAsync("AdminSchool/GetClassGroups/1/" + schid);

            loadingmessage = "Please wait, while loading...";
            if (roleid == 1) //Administrator
            {
                await LoadExams(1, termid, 0, 0);
                //schoolCountDisplay = "Total Exam Count: " + exams.Count();
            }
            else
            {
                await LoadExams(4, termid, 0, staffid);
                //schoolCountDisplay = "Total Exam Count: " + exams.Count();
            }
            ExamFilters();
        }

        async Task OnClassChanged(IEnumerable<string> e)
        {
            selectedClass = e.ElementAt(0);
            classlistid = classGroups.FirstOrDefault(c => c.ClassName == selectedClass).ClassListID;

            loadingmessage = "Please wait, while loading...";
            if (roleid == 1) //Administrator
            {
                await LoadExams(1, termid, 0, 0);
                //schoolCountDisplay = "Total Exam Count: " + exams.Count();
            }
            else
            {
                await LoadExams(4, termid, 0, staffid);
                //schoolCountDisplay = "Total Exam Count: " + exams.Count();
            }
            ExamFilters();
        }

        async Task OnExamTypeChanged(IEnumerable<string> e)
        {
            selectedExamType = e.ElementAt(0);
            examTypeID = examType.FirstOrDefault(e => e.ExamType == selectedExamType).ExamTypeID;

            loadingmessage = "Please wait, while loading...";
            if (roleid == 1) //Administrator
            {
                await LoadExams(1, termid, 0, 0);
                //schoolCountDisplay = "Total Exam Count: " + exams.Count();
            }
            else
            {
                await LoadExams(4, termid, 0, staffid);
                //schoolCountDisplay = "Total Exam Count: " + exams.Count();
            }
            ExamFilters();
        }

        async Task OnReportTypeChanged(IEnumerable<string> e)
        {
            selectedReportType = e.ElementAt(0);
            reportTypeID = reportType.FirstOrDefault(e => e.ReportType == selectedReportType).ReportTypeID;

            loadingmessage = "Please wait, while loading...";
            if (roleid == 1) //Administrator
            {
                await LoadExams(1, termid, 0, 0);
                //schoolCountDisplay = "Total Exam Count: " + exams.Count();
            }
            else
            {
                await LoadExams(4, termid, 0, staffid);
                //schoolCountDisplay = "Total Exam Count: " + exams.Count();
            }
            ExamFilters();
        }

        bool FilterFunc(CBTExams model)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (model.ExamName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        async Task RefreshExamList()
        {
            selectedSchool = string.Empty;
            schid = 0;
            schools.Clear();

            selectedClass = string.Empty;
            classlistid = 0;
            classGroups.Clear();

            selectedExamType = string.Empty;
            examTypeID = 0;

            selectedReportType = string.Empty;
            reportTypeID = 0;

            schoolCountDisplay = string.Empty;
            classCountDisplay = string.Empty;
            examTypeCountDisplay = string.Empty;
            reportTypeCountDisplay = string.Empty;

            searchString = string.Empty;

            exams.Clear();
            await LoadDefaultList();
        }


        async Task ExportExamList()
        {
            byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            int rowNum = 4;
            int sn = 0;
            double rowCount = 0;

            using (var package = new ExcelPackage())
            {
                var _classList = (from x in exams
                            select new
                            {
                                x.ClassListID,
                                x.ClassName,
                            }).Distinct().OrderBy(x => x.ClassListID).ToList(); 

                foreach (var item in _classList)
                {
                    var workSheet = package.Workbook.Worksheets.Add(item.ClassName);

                    #region Header Row

                    workSheet.Cells[1, 1].Value = "CBT Exam List";
                    workSheet.Cells[1, 1].Style.Font.Size = 16;
                    workSheet.Cells[1, 1].Style.Font.Bold = true;

                    workSheet.Cells[2, 1].Value = academicSession;
                    workSheet.Cells[2, 1].Style.Font.Size = 14;
                    workSheet.Cells[2, 1].Style.Font.Bold = true;

                    workSheet.Cells[3, 1].Value = item.ClassName;
                    workSheet.Cells[3, 1].Style.Font.Size = 14;
                    workSheet.Cells[3, 1].Style.Font.Bold = true;

                    workSheet.Cells[5, 1].Value = "S/N";
                    workSheet.Cells[5, 1].Style.Font.Size = 12;
                    workSheet.Cells[5, 1].Style.Font.Bold = true;

                    workSheet.Cells[5, 2].Value = "Exam Date";
                    workSheet.Cells[5, 2].Style.Font.Size = 12;
                    workSheet.Cells[5, 2].Style.Font.Bold = true;

                    workSheet.Cells[5, 3].Value = "Exam Code";
                    workSheet.Cells[5, 3].Style.Font.Size = 12;
                    workSheet.Cells[5, 3].Style.Font.Bold = true;

                    workSheet.Cells[5, 4].Value = "Exam Name";
                    workSheet.Cells[5, 4].Style.Font.Size = 12;
                    workSheet.Cells[5, 4].Style.Font.Bold = true;

                    workSheet.Cells[5, 5].Value = "Teacher";
                    workSheet.Cells[5, 5].Style.Font.Size = 12;
                    workSheet.Cells[5, 5].Style.Font.Bold = true;

                    workSheet.Cells[5, 6].Value = "Exam Time (mins)";
                    workSheet.Cells[5, 6].Style.Font.Size = 12;
                    workSheet.Cells[5, 6].Style.Font.Bold = true;
                    #endregion

                    var _exams = exams.Where(ex => ex.ClassName == item.ClassName).OrderBy(x => x.SubjectID).ToList();
                    rowCount = 0;
                    rowNum = 4;
                    sn = 0;

                    foreach (var exam in _exams)
                    {
                        rowNum++;
                        sn++;

                        workSheet.Cells[rowNum + 1, 1].Value = sn;
                        workSheet.Cells[rowNum + 1, 1].Style.Font.Size = 12;
                        workSheet.Cells[rowNum + 1, 1].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                        workSheet.Cells[rowNum + 1, 2].Value = exam.ExamDate?.ToString("dd-MMM-yyyy");
                        workSheet.Cells[rowNum + 1, 2].Style.Font.Size = 12;
                        workSheet.Cells[rowNum + 1, 2].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                        workSheet.Cells[rowNum + 1, 3].Value = exam.ExamCode;
                        workSheet.Cells[rowNum + 1, 3].Style.Font.Size = 12;
                        workSheet.Cells[rowNum + 1, 3].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                        workSheet.Cells[rowNum + 1, 4].Value = exam.Subject;
                        workSheet.Cells[rowNum + 1, 4].Style.Font.Size = 12;
                        workSheet.Cells[rowNum + 1, 4].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                        workSheet.Cells[rowNum + 1, 5].Value = exam.SubjectTeacher;
                        workSheet.Cells[rowNum + 1, 5].Style.Font.Size = 12;
                        workSheet.Cells[rowNum + 1, 5].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                        workSheet.Cells[rowNum + 1, 6].Value = exam.ExamTime;
                        workSheet.Cells[rowNum + 1, 6].Style.Font.Size = 12;
                        workSheet.Cells[rowNum + 1, 6].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    }

                    workSheet.Column(1).Width = 5;
                    workSheet.Column(2).Width = 15;
                    workSheet.Column(3).Width = 15;
                    workSheet.Column(4).Width = 50;
                    workSheet.Column(5).Width = 20;
                    workSheet.Column(6).Width = 10;

                    rowCount = _exams.Count() + 5;

                    workSheet.View.FreezePanes(6, 1);

                    workSheet.Cells["A5:F" + rowCount].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A5:F" + rowCount].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A5:F" + rowCount].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A5:F" + rowCount].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    workSheet.Cells["A5:A" + rowCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    workSheet.Cells["C5:F" + rowCount].Style.WrapText = true;
                    workSheet.Cells["F5:F" + rowCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //workSheet.Cells["A6:F50"].Sort(x => x.SortBy.Column(3, eSortOrder.Ascending));

                    workSheet.PrinterSettings.PaperSize = ePaperSize.A4;
                    workSheet.PrinterSettings.Orientation = eOrientation.Landscape;
                    workSheet.PrinterSettings.LeftMargin = Convert.ToDecimal(0.511811023622047);
                    workSheet.PrinterSettings.RightMargin = Convert.ToDecimal(0.511811023622047);
                    workSheet.Names.AddFormula("_xlnm.Print_Titles", $"'{workSheet.Name}'!$A:$F,'{workSheet.Name}'!$1:$5");
                }

                fileContents = package.GetAsByteArray();
            }

            await iJSRuntime.InvokeAsync<Exams>(
              "saveAsFile",
                academicSession + "_" + "_CBT_Exams.xlsx",
              Convert.ToBase64String(fileContents)
              );
        }

        async Task ExportExamsClick()
        {
            if (!String.IsNullOrWhiteSpace(selectedReportType))
            {
                await ExportExamList();
            }
            else
            {
                await Swal.FireAsync("Export Exam List",
                    "Please select CBT Exam For.", "error");
            }
        }

        #endregion

        #region [Section - Exam Details]
        #region [Models Declaration]
        List<ACDSubjects> subjects = new();
        List<ACDSbjAllocationTeachers> teacherSubjectsAllocation = new();
        CBTExams exam = new();

        #endregion

        #region [Variables Declaration]
        int _schid { get; set; }
        int _classlistid { get; set; }
        int _reportTypeID { get; set; }
        int _examTypeID { get; set; }
        int _subjectid { get; set; }
        int _subjectTeacherID { get; set; }
        int examid { get; set; }

        string _selectedSchool { get; set; }
        string _selectedClass { get; set; }
        string _selectedReportType { get; set; }
        string _selectedExamType { get; set; }
        string _selectedSubject { get; set; }
        string pagetitle { get; set; } = "Create A New Exam";

        bool defaultExam { get; set; } = false;
        #endregion

        async Task LoadSubjects()
        {
            if (roleid == 1) //Administrator
            {
                teacherSubjectsAllocation = await subjectAllocationTeacherService.GetAllAsync(
                    "AcademicsSubjects/GetTeacherAllocations/2/true/" + termid + "/" + _schid + "/" + _classlistid + "/0/0/0");
            }
            else
            {
                teacherSubjectsAllocation = await subjectAllocationTeacherService.GetAllAsync(
                    "AcademicsSubjects/GetTeacherAllocations/6/true/" + termid + "/" + _schid + "/" + _classlistid + "/0/0/" + staffid);
            }

            subjects.Clear();
            foreach (var subject in teacherSubjectsAllocation.Where(s => s.SbjClassID == 1))
            {
                subjects.Add(new ACDSubjects()
                {
                    SubjectID = subject.SubjectID,
                    Subject = subject.Subject + " - " + subject.SubjectTeacher,
                    SbjMergeID = subject.StaffID,
                });
            }

            subjects = subjects.GroupBy(s => new { s.SubjectID, s.Subject, s.SbjMergeID }).Select(group => group.FirstOrDefault()).ToList();
        }

        void DefaultExamCheckBoxChanged(bool value)
        {
            defaultExam = value;
            exam.ExamDefault = value;
        }

        void OnSelectedExamTypeChanged(IEnumerable<string> e)
        {
            _selectedExamType = e.ElementAt(0);
            _examTypeID = examType.FirstOrDefault(e => e.ExamType == _selectedExamType).ExamTypeID;
            exam.ExamTypeID = _examTypeID;
        }

        void OnSelectedReportTypeChanged(IEnumerable<string> e)
        {
            _selectedReportType = e.ElementAt(0);
            _reportTypeID = reportType.FirstOrDefault(e => e.ReportType == _selectedReportType).ReportTypeID;
            exam.ReportTypeID = _reportTypeID;
        }

        async Task OnSelectedSchoolChanged(IEnumerable<string> e)
        {
            _selectedSchool = e.ElementAt(0);
            _schid = schools.FirstOrDefault(s => s.School == _selectedSchool).SchID;
            exam.SchID = _schid;

            _classlistid = 0;
            _selectedClass = string.Empty;
            classGroups.Clear();
            classGroups = await classGroupService.GetAllAsync("AdminSchool/GetClassGroups/1/" + _schid);

            exam.ClassName = string.Empty;

            if (examid == 0)
            {
                exam.Subject = string.Empty;
                subjects.Clear();
            }
        }

        async Task OnSelectedClassChanged(IEnumerable<string> e)
        {
            _selectedClass = e.ElementAt(0);
            _classlistid = classGroups.FirstOrDefault(c => c.ClassName == _selectedClass).ClassListID;
            exam.ClassListID = _classlistid;

            if (examid == 0)
            {
                subjects.Clear();
                await LoadSubjects();
            }
        }

        void OnSelectedSubjectChanged(IEnumerable<string> e)
        {
            _selectedSubject = e.ElementAt(0);
            _subjectid = subjects.FirstOrDefault(c => c.Subject == _selectedSubject).SubjectID;

            if (roleid == 1) //Administrator
            {
                _subjectTeacherID = subjects.FirstOrDefault(c => c.Subject == _selectedSubject).SbjMergeID;
            }
            else
            {
                _subjectTeacherID = staffid;
            }

            exam.SubjectID = _subjectid;
            exam.StaffID = _subjectTeacherID;
        }

        async Task RetrieveExamDetails(int _examid)
        {
            exam = await examService.GetByIdAsync("AcademicsCBT/GetCBTExam/", _examid);

            examid = _examid;
            defaultExam = exam.ExamDefault;
            _subjectTeacherID = exam.StaffID;

            classGroups.Clear();
            classGroups = await classGroupService.GetAllAsync("AdminSchool/GetClassGroups/1/" + exam.SchID);
        }

        bool EntryValidation()
        {
            bool ExamExist = exams.Where(ex => ex.SchID == _schid && ex.ClassListID == _classlistid && ex.ExamTypeID == _examTypeID
                                            && ex.ReportTypeID == _reportTypeID && ex.SubjectID == _subjectid).Any();

            if (ExamExist)
            {
                return true;
            }

            return false;
        }

        async Task SubmitValidForm()
        {
            if (examid == 0 && EntryValidation())
            {
                await Swal.FireAsync("Duplicate Exam Headings.",
                    "Exam Has Been Set For Selected Subject. Please, Proceed to Set Questions For The Exam.", "error");
            }
            else
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Exam Details Create/Update Operation",
                    Text = "Do You Want To Continue With This Operation?",
                    Icon = SweetAlertIcon.Warning,
                    ShowCancelButton = true,
                    ConfirmButtonText = "Yes, Contnue!",
                    CancelButtonText = "No"
                });

                if (result.IsConfirmed)
                {
                    if (examid == 0)
                    {
                        exam.TermID = termid;
                        await examService.SaveAsync("AcademicsCBT/AddCBTExam/", exam);
                        await Swal.FireAsync("A New Exam", "Has Been Successfully Created For " + exam.Subject + ".",
                            "success");
                    }
                    else
                    {
                        await examService.UpdateAsync("AcademicsCBT/UpdateCBTExam/", 1, exam);
                        await Swal.FireAsync("Exam Details", "Has Been Successfully Updated For " + exam.Subject + ".",
                            "success");
                    }

                    toolBarMenuId = 1;
                    await RefreshExamList();
                }
            }
        }

        async Task InvalidEntries()
        {
            await Swal.FireAsync("Please Check Your Entries.", "One or More Entries has error.", "error");
        }

        #endregion

        #region [Section - CBT Exam Monitor]
        #region [Models Declaration]
        //Exam List Models
        List<ADMSchlList> schoolsMonitor = new();
        List<ADMSchClassGroup> classGroupsMonitor = new();
        List<ADMSchClassList> classListMonitor = new();
        List<CBTExams> examSubjectsMonitor = new();
        List<CBTStudentScores> studentCBTScoresMonitor = new();

        enum CBTScoreViewType { Batch = 1, Single = 2 }
        enum CBTScoreType { Active = 1, Cancelled = 2 }
        #endregion

        #region [Variables Declaration]
        int schidMonitor { get; set; }
        int classidMonitor { get; set; }
        int classlistidMonitor { get; set; }
        int examidMonitor { get; set; }

        string selectedSchoolMonitor { get; set; }
        string selectedClassGroupMonitor { get; set; }
        string selectedClassMonitor { get; set; }
        string selectedSubjectMonitor { get; set; }

        TimeSpan stopwatchvalue = new TimeSpan();
        bool is_stopwatchrunning = false;
        bool _timerVisible { get; set; } = true;
        #endregion

        async Task OnSelectedSchoolMonitorChanged(IEnumerable<string> e)
        {
            selectedSchoolMonitor = e.ElementAt(0);
            schidMonitor = schoolsMonitor.FirstOrDefault(s => s.School == selectedSchoolMonitor).SchID;

            classlistidMonitor = 0;
            selectedClassGroupMonitor = string.Empty;
            classGroupsMonitor.Clear();
            classGroupsMonitor = await classGroupService.GetAllAsync("AdminSchool/GetClassGroups/1/" + schidMonitor);

            classidMonitor = 0;
            selectedClassMonitor = string.Empty;
            classListMonitor.Clear();

            examidMonitor = 0;
            selectedSubjectMonitor = string.Empty;
            examSubjectsMonitor.Clear();

            studentCBTScoresMonitor.Clear();
        }

        async Task OnSelectedClassGroupMonitorChanged(IEnumerable<string> e)
        {
            selectedClassGroupMonitor = e.ElementAt(0);
            classlistidMonitor = classGroupsMonitor.FirstOrDefault(c => c.ClassName == selectedClassGroupMonitor).ClassListID;

            classidMonitor = 0;
            selectedClassMonitor = string.Empty;
            classListMonitor.Clear();
            classListMonitor = await classService.GetAllAsync("AdminSchool/GetClassList/2/" + "0/" + classlistidMonitor);

            examidMonitor = 0;
            selectedSubjectMonitor = string.Empty;
            examSubjectsMonitor.Clear();
            examSubjectsMonitor = await examService.GetAllAsync("AcademicsCBT/GetCBTExams/5/" +
                                                                   termid + "/" + classlistidMonitor + "/" + reportTypeID);

            studentCBTScoresMonitor.Clear();
        }

        async Task OnSelectedClassMonitorChanged(IEnumerable<string> e)
        {
            selectedClassMonitor = e.ElementAt(0);
            classidMonitor = classListMonitor.FirstOrDefault(c => c.ClassName == selectedClassMonitor).ClassID;

            examidMonitor = 0;
            selectedSubjectMonitor = string.Empty;
            examSubjectsMonitor.Clear();
            examSubjectsMonitor = await examService.GetAllAsync("AcademicsCBT/GetCBTExams/5/" +
                                                                    termid + "/" + classlistidMonitor + "/" + reportTypeID);

            studentCBTScoresMonitor.Clear();
        }

        async Task OnSelectedSubjectViewChanged(IEnumerable<string> e)
        {
            selectedSubjectMonitor = e.ElementAt(0);
            examidMonitor = examSubjectsMonitor.FirstOrDefault(sbj => sbj.Subject == selectedSubjectMonitor).ExamID;

            studentCBTScoresMonitor.Clear();
            await LoadStudentScores();
        }

        async Task LoadStudentScores()
        {
            int _sn = 1;
            var _studentScores = await studentCBTService.GetAllAsync("AcademicsCBT/GetCBTStudentScores/1/" + examidMonitor + "/0/" + true);

            if (!String.IsNullOrWhiteSpace(selectedClassMonitor))
            {
                var _studentScoresSortByScore = _studentScores.Where(s => s.ClassID == classidMonitor).OrderByDescending(x => x.ScorePercentage).ToList();

                foreach (var item in _studentScoresSortByScore)
                {
                    studentCBTScoresMonitor.Add(new CBTStudentScores()
                    {
                        StudentScoreID = item.StudentScoreID,
                        ExamID = item.ExamID,
                        STDID = item.STDID,
                        ClassName = item.ClassName,
                        SN = _sn++,
                        AdmissionNo = item.AdmissionNo,
                        StudentName = item.StudentName,
                        NQuestions = item.NQuestions,
                        NUnAnsQuestions = item.NUnAnsQuestions,
                        NCorrectAns = item.NCorrectAns,
                        TimeAllocated = item.TimeAllocated,
                        TimeUsed = item.TimeUsed,
                        ScorePercentage = item.ScorePercentage,
                    });
                }
            }
            else
            {
                var _studentScoresSortByScore = _studentScores.OrderByDescending(x => x.ScorePercentage).ToList();

                foreach (var item in _studentScoresSortByScore)
                {
                    studentCBTScoresMonitor.Add(new CBTStudentScores()
                    {
                        StudentScoreID = item.StudentScoreID,
                        ExamID = item.ExamID,
                        STDID = item.STDID,
                        ClassName = item.ClassName,
                        SN = _sn++,
                        AdmissionNo = item.AdmissionNo,
                        StudentName = item.StudentName,
                        NQuestions = item.NQuestions,
                        NUnAnsQuestions = item.NUnAnsQuestions,
                        NCorrectAns = item.NCorrectAns,
                        TimeAllocated = item.TimeAllocated,
                        TimeUsed = item.TimeUsed,
                        ScorePercentage = item.ScorePercentage,
                    });
                }
            }
        }

        async Task StopWatch()
        {
            is_stopwatchrunning = true;
            while (is_stopwatchrunning)
            {
                await Task.Delay(1000);
                if (is_stopwatchrunning)
                {
                    stopwatchvalue = stopwatchvalue.Add(new TimeSpan(0, 0, 1));
                    studentCBTScoresMonitor.Clear();
                    await LoadStudentScores();
                    StateHasChanged();
                }
            }
        }

        async Task ViewSelectedStudentScore(int _examid, int _stdid)
        {
            navManager.NavigateTo("/obtexamstudentresult/" + _examid + "/" + _stdid, true);
            //await localStorageService.SaveItemEncryptedAsync("isview", true);
            //string url = "/obtexamstudentresult/" + _examid + "/" + _stdid;
            //await iJSRuntime.InvokeAsync<object>("open", url, "_blank");
            await Task.CompletedTask;
        }

        async Task RefreshCBTMonitor()
        {
            selectedSchoolMonitor = string.Empty;
            schoolsMonitor.Clear();
            schoolsMonitor = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            classlistidMonitor = 0;
            selectedClassGroupMonitor = String.Empty;
            classGroupsMonitor.Clear();
            classidMonitor = 0;
            classlistidMonitor = 0;
            selectedClassMonitor = string.Empty;
            classListMonitor.Clear();
            examidMonitor = 0;
            selectedSubjectMonitor = string.Empty;
            examSubjectsMonitor.Clear();
            studentCBTScoresMonitor.Clear();
            stopwatchvalue = stopwatchvalue.Add(new TimeSpan(0, 0, 0));
            is_stopwatchrunning = false;
        }

        #endregion

        #region [Section - Student CBT Scores]
        async Task ExportCBTResultsToExcel()
        {
            byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            int rowNum = 4;
            int sn = 0;

            using (var package = new ExcelPackage())
            {
                var workSheet = package.Workbook.Worksheets.Add("CBTResult");

                #region Header Row

                workSheet.Cells[1, 1].Value = "Student CBT Result";
                workSheet.Cells[1, 1].Style.Font.Size = 16;
                workSheet.Cells[1, 1].Style.Font.Bold = true;

                if (!String.IsNullOrWhiteSpace(selectedClassMonitor))
                {
                    workSheet.Cells[2, 1].Value = selectedClassMonitor;
                }
                else
                {
                    workSheet.Cells[2, 1].Value = selectedClassGroupMonitor;
                }
                workSheet.Cells[2, 1].Style.Font.Size = 14;
                workSheet.Cells[2, 1].Style.Font.Bold = true;

                workSheet.Cells[3, 1].Value = selectedSubjectMonitor;
                workSheet.Cells[3, 1].Style.Font.Size = 14;
                workSheet.Cells[3, 1].Style.Font.Bold = true;

                workSheet.Cells[5, 1].Value = "S/N";
                workSheet.Cells[5, 1].Style.Font.Size = 12;
                workSheet.Cells[5, 1].Style.Font.Bold = true;

                workSheet.Cells[5, 2].Value = "Admission No";
                workSheet.Cells[5, 2].Style.Font.Size = 12;
                workSheet.Cells[5, 2].Style.Font.Bold = true;

                workSheet.Cells[5, 3].Value = "Student Name";
                workSheet.Cells[5, 3].Style.Font.Size = 12;
                workSheet.Cells[5, 3].Style.Font.Bold = true;

                workSheet.Cells[5, 4].Value = "Total Questions";
                workSheet.Cells[5, 4].Style.Font.Size = 12;
                workSheet.Cells[5, 4].Style.Font.Bold = true;

                workSheet.Cells[5, 5].Value = "Questions Answered";
                workSheet.Cells[5, 5].Style.Font.Size = 12;
                workSheet.Cells[5, 5].Style.Font.Bold = true;

                workSheet.Cells[5, 6].Value = "Correct Answers";
                workSheet.Cells[5, 6].Style.Font.Size = 12;
                workSheet.Cells[5, 6].Style.Font.Bold = true;

                workSheet.Cells[5, 7].Value = "Time Allocated";
                workSheet.Cells[5, 7].Style.Font.Size = 12;
                workSheet.Cells[5, 7].Style.Font.Bold = true;

                workSheet.Cells[5, 8].Value = "Time Used";
                workSheet.Cells[5, 8].Style.Font.Size = 12;
                workSheet.Cells[5, 8].Style.Font.Bold = true;

                workSheet.Cells[5, 9].Value = "Score (%)";
                workSheet.Cells[5, 9].Style.Font.Size = 12;
                workSheet.Cells[5, 9].Style.Font.Bold = true;

                workSheet.Cells[5, 10].Value = "Class";
                workSheet.Cells[5, 10].Style.Font.Size = 12;
                workSheet.Cells[5, 10].Style.Font.Bold = true;
                #endregion

                foreach (var item in studentCBTScoresMonitor)
                {
                    rowNum++;
                    sn++;

                    var _studentScores = await studentCBTService.GetAllAsync("AcademicsCBT/GetCBTStudentScores/2/" +
                                                   examidMonitor + "/" + item.STDID + "/" + true);

                    workSheet.Cells[rowNum + 1, 1].Value = sn;
                    workSheet.Cells[rowNum + 1, 1].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 1].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 2].Value = _studentScores.FirstOrDefault().AdmissionNo;
                    workSheet.Cells[rowNum + 1, 2].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 2].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 3].Value = _studentScores.FirstOrDefault().StudentName;
                    workSheet.Cells[rowNum + 1, 3].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 3].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 4].Value = _studentScores.FirstOrDefault().NQuestions;
                    workSheet.Cells[rowNum + 1, 4].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 4].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 5].Value = _studentScores.FirstOrDefault().NQuestions - _studentScores.FirstOrDefault().NUnAnsQuestions;
                    workSheet.Cells[rowNum + 1, 5].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 5].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 6].Value = _studentScores.FirstOrDefault().NCorrectAns;
                    workSheet.Cells[rowNum + 1, 6].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 6].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 7].Value = _studentScores.FirstOrDefault().TimeAllocated;
                    workSheet.Cells[rowNum + 1, 7].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 7].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 8].Value = _studentScores.FirstOrDefault().TimeUsed;
                    workSheet.Cells[rowNum + 1, 8].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 8].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 9].Value = _studentScores.FirstOrDefault().ScorePercentage;
                    workSheet.Cells[rowNum + 1, 9].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 9].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[rowNum + 1, 10].Value = _studentScores.FirstOrDefault().ClassName;
                    workSheet.Cells[rowNum + 1, 10].Style.Font.Size = 12;
                    workSheet.Cells[rowNum + 1, 10].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                }

                double col1 = 5;
                workSheet.Column(1).Width = col1;
                workSheet.Column(2).Width = 15;
                workSheet.Column(3).Width = 35;
                workSheet.Column(4).Width = 12;
                workSheet.Column(5).Width = 11;
                workSheet.Column(6).Width = 9;
                workSheet.Column(7).Width = 10;
                workSheet.Column(8).Width = 9;
                workSheet.Column(9).Width = 8;
                workSheet.Column(10).Width = 17;

                double rowCount = studentCBTScoresMonitor.Count() + 5;

                workSheet.View.FreezePanes(6, 1);

                workSheet.Cells["A5:J" + rowCount].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells["A5:J" + rowCount].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                workSheet.Cells["A5:J" + rowCount].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells["A5:J" + rowCount].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                workSheet.Cells["A5:A" + rowCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells["D5:I" + rowCount].Style.WrapText = true;
                workSheet.Cells["D5:I" + rowCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.PrinterSettings.PaperSize = ePaperSize.A4;
                workSheet.PrinterSettings.Orientation = eOrientation.Landscape;
                workSheet.PrinterSettings.LeftMargin = Convert.ToDecimal(0.511811023622047);
                workSheet.PrinterSettings.RightMargin = Convert.ToDecimal(0.511811023622047);
                workSheet.Names.AddFormula("_xlnm.Print_Titles", $"'{workSheet.Name}'!$A:$J,'{workSheet.Name}'!$1:$5");

                fileContents = package.GetAsByteArray();
            }

            if (!String.IsNullOrWhiteSpace(selectedClassMonitor))
            {
                await iJSRuntime.InvokeAsync<Exams>(
               "saveAsFile",
               "StudentCBTResult_" + selectedClassMonitor + "_" + selectedSubjectMonitor + ".xlsx",
               Convert.ToBase64String(fileContents)
               );
            }
            else
            {
                await iJSRuntime.InvokeAsync<Exams>(
               "saveAsFile",
               "StudentCBTResult_" + selectedClassGroupMonitor + "_" + selectedSubjectMonitor + ".xlsx",
               Convert.ToBase64String(fileContents)
               );
            }
        }

        async Task ExportCBTResults()
        {
            if (String.IsNullOrWhiteSpace(selectedSubjectMonitor))
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Student CBT Result",
                    Icon = "info",
                    Text = "Please Select A Subject For Student CBT Result Export!"
                });
            }
            else
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Student CBT Result Export Operation",
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
                        await ExportCBTResultsToExcel();
                    }
                }
            }
        }
        #endregion

        #region [Section - CBT Exam List For Default Exams]
        CBTExams selectedExam = null;


        async Task SetDefaultExam()
        {
            exam.ExamID = selectedExam.ExamID;
            exam.ExamCode = selectedExam.ExamCode;
            exam.ExamName = selectedExam.ExamName;
            exam.PassingPercentage = selectedExam.PassingPercentage;
            exam.ExamTime = selectedExam.ExamTime;
            exam.ExamDefault = selectedExam.ExamDefault;

            await examService.UpdateAsync("AcademicsCBT/UpdateCBTExam/", 4, exam);
            //Snackbar.Add("Current Result Title Has Been Successfully Updated");
        }

        #endregion

        #region [Section - Release Exams]
        List<ADMSchClassList> classList = new();
        List<ADMStudents> students = new();
        List<CBTExamTakenFlags> examTakenList = new();
        CBTExamTakenFlags examTaken = new();
        CBTExamTakenFlags selectedSubject = null;


        int classid { get; set; }
        int stdid { get; set; }
        string selectSchoolExamTaken { get; set; }
        string selectedClassExamTaken { get; set; }
        string slectedStudentExamTaken { get; set; }

        async Task LoadClassList(int switchid, int schid, int classlistid)
        {
            classList = await classService.GetAllAsync("AdminSchool/GetClassList/" + switchid + "/" + schid + "/" + classlistid);
        }

        async Task OnSchoolChangedExamTaken(IEnumerable<string> e)
        {
            selectSchoolExamTaken = e.ElementAt(0);
            schid = schools.FirstOrDefault(s => s.School == selectSchoolExamTaken).SchID;

            selectedClassExamTaken = string.Empty;
            classList.Clear();
            await LoadClassList(1, schid, 0);

            slectedStudentExamTaken = string.Empty;
            students.Clear();

            examTakenList.Clear();
        }

        async Task OnClassChangedExamTaken(IEnumerable<string> e)
        {
            selectedClassExamTaken = e.ElementAt(0);
            classid = classList.FirstOrDefault(s => s.ClassName == selectedClassExamTaken).ClassID;
            students = await studentService.GetAllAsync("AdminStudent/GetStudents/3/0/" + classid + "/0/1");

            examTakenList = await examTakenService.GetAllAsync("AcademicsCBT/GetExamTakenFlags/1/" + termid + "/" + classid + "/0");
        }

        async Task OnStudentChangedExamTaken(IEnumerable<string> e)
        {
            slectedStudentExamTaken = e.ElementAt(0);
            stdid = students.FirstOrDefault(s => s.StudentNameWithNo == slectedStudentExamTaken).STDID;

            examTakenList = await examTakenService.GetAllAsync("AcademicsCBT/GetExamTakenFlags/2/" + termid + "/0/" + stdid);
        }

        async Task ReleaseSelectedSubject()
        {
            examTaken.FlagID = selectedSubject.FlagID;
            examTaken.Flag = selectedSubject.Flag;

            await examTakenService.UpdateAsync("AcademicsCBT/UpdateExamTakenFlag/", 1, examTaken);
        }

        #endregion

        #region [Section - Click Events]
        async Task CBTExamList()
        {
            toolBarMenuId = 1;
            await RefreshExamList();
        }

        void CBTExamDetails()
        {
            toolBarMenuId = 2;
            examid = 0;
            pagetitle = "Create A New Exam";
            exam = new CBTExams();
            classGroups.Clear();
            subjects.Clear();
            defaultExam = false;
        }

        async Task EditCBTExamDetails(int _examid)
        {
            toolBarMenuId = 2;
            exam.ExamID = _examid;
            pagetitle = "Edit Exam Details";
            await RetrieveExamDetails(_examid);
        }

        async Task ExamQuestion(int examid, string _examname)
        {
            await localStorageService.SaveItemEncryptedAsync("examname", _examname);
            navManager.NavigateTo("/academics/examsobjquestions/" + examid);
        }

        async Task CBTMonitor()
        {
            toolBarMenuId = 3;
            await RefreshCBTMonitor();
            //schoolsMonitor = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
        }

        async Task DefaultExamsToBeTaken()
        {
            toolBarMenuId = 4;
            await RefreshExamList();
        }

        async Task Cancel()
        {
            toolBarMenuId = 1;
            await RefreshExamList();
        }

        async Task ReleaseExams()
        {
            toolBarMenuId = 5;
            selectSchoolExamTaken = string.Empty;
            schid = 0;
            schools.Clear();
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            selectedClassExamTaken = string.Empty;
            classList.Clear();
            slectedStudentExamTaken = string.Empty;
            students.Clear();
            examTakenList.Clear();
        }

        #endregion




    }
}
