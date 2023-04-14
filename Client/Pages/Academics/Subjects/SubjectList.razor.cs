using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.Subjects;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Settings;

namespace WebAppAcademics.Client.Pages.Academics.Subjects
{
    public partial class SubjectList
    {
        #region [Injection Declaration]
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ACDSbjDept> subjectDepartmentService { get; set; }
        [Inject] IAPIServices<ACDSbjClassification> subjectClassificationService { get; set; }
        [Inject] IAPIServices<ACDSubjects> subjectService { get; set; }
        [Inject] IAPIServices<SETStatusType> statusTypeService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }

        int toolBarMenuId { get; set; }
        #endregion

        #region [Variables Declaration]
        int schid { get; set; }
        int sbjdeptid { get; set; }
        int sbjclassid { get; set; }
        int statustypeid { get; set; } = 1;
        int schoolCount { get; set; }
        int sbjdeptCount { get; set; }
        int statusTypeCount { get; set; }

        string selectedSchool { get; set; }
        string selectedSubjectDept { get; set; }
        string selectedSubjectClass { get; set; }
        string selectedStatusType { get; set; } = "Active";
        string schoolCountDisplay { get; set; }
        string subjectDeptCountDisplay { get; set; }
        string subjectClassCountDisplay { get; set; }
        string statusTypeCountDisplay { get; set; }
        string searchString { get; set; } = "";

        bool disableSaveButton { get; set; } = true;
        string editFormId { get; set; } = "editformid";
        #endregion

        #region [Models Declaration]
        List<ADMSchlList> schools = new();
        List<ACDSbjDept> sbjdepts = new();
        List<ACDSbjClassification> sbjclasslist = new();
        List<SETStatusType> statusType = new();
        List<ACDSubjects> subjects = new();
        List<ACDSubjects> subjectCount = new();

        ACDSubjects selectedItem = null;
        ACDSubjects subject = new();

        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Subjects";
            toolBarMenuId = 1;
            subjects = null;
            subjects = await subjectService.GetAllAsync("AcademicsSubjects/GetSubjects/1/0/0/0/true");
            await LoadList();
            await base.OnInitializedAsync();
        }

        #region [Section - List]
        bool SubjectStatus()
        {
            bool result = false;

            if (statustypeid == 1)
            {
                result = true;
            }

            return result;
        }

        async Task LoadList()
        {
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            sbjdepts = await subjectDepartmentService.GetAllAsync("AcademicsSubjects/GetDepartments/1");
            sbjclasslist = await subjectClassificationService.GetAllAsync("AcademicsSubjects/GetSubjectsClassifications/1");
            statusType = await statusTypeService.GetAllAsync("Settings/GetStatusTypes/1");
                      
            subjectCount = await subjectService.GetAllAsync("AcademicsSubjects/GetSubjects/0/0/0/0/false");
            schoolCountDisplay = string.Empty;
            subjectDeptCountDisplay = string.Empty;
            subjectClassCountDisplay = string.Empty;
            statusTypeCountDisplay = "Total " + selectedStatusType + " Subject Count: " + subjects.Count();
        }

        async Task LoadSubjects(int switchid, int schid, int sbjdeptid, int sbjclassid, bool subjectstatus)
        {
            subjects = await subjectService.GetAllAsync("AcademicsSubjects/GetSubjects/" + switchid + "/" + schid + "/" + sbjdeptid + "/" + sbjclassid + "/" + subjectstatus);
        }

        int GetFilterID()
        {
            if (String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedSubjectDept) && String.IsNullOrWhiteSpace(selectedSubjectClass))
            {
                //Filter By Subject Status
                return 1;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedSubjectDept) && String.IsNullOrWhiteSpace(selectedSubjectClass))
            {
                //Filter By School And Subject Status
                return 2;
            }
            else if (String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedSubjectDept) && String.IsNullOrWhiteSpace(selectedSubjectClass))
            {
                //Filter By Department And Subject Status
                return 3;
            }
            else if (String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedSubjectDept) && !String.IsNullOrWhiteSpace(selectedSubjectClass))
            {
                //Filter By Classification And Subject Status
                return 4;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedSubjectDept) && String.IsNullOrWhiteSpace(selectedSubjectClass))
            {
                //Filter By School, Department And Subject Status
                return 5;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedSubjectDept) && !String.IsNullOrWhiteSpace(selectedSubjectClass))
            {
                //Filter By School, Classification And Subject Status
                return 6;
            }
            else if (String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedSubjectDept) && !String.IsNullOrWhiteSpace(selectedSubjectClass))
            {
                //Filter By Department, Classification And Subject Status
                return 7;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedSubjectDept) && !String.IsNullOrWhiteSpace(selectedSubjectClass))
            {
                //Filter By School, Department, Classification And Subject Status
                return 8;
            }

            return 0;
        }

        async Task SubjectFilters()
        {
            schoolCountDisplay = string.Empty;
            subjectDeptCountDisplay = string.Empty;
            subjectClassCountDisplay = string.Empty;
            statusTypeCountDisplay = string.Empty;

            switch (GetFilterID())
            {
                case 1://Filter By Subject Status
                    await LoadSubjects(1, schid, sbjdeptid, sbjclassid, SubjectStatus());

                    schoolCountDisplay = string.Empty;
                    subjectDeptCountDisplay = string.Empty;
                    subjectClassCountDisplay = string.Empty;
                    statusTypeCountDisplay = "Total " + selectedStatusType + " Subject Count: " + subjects.Count();
                    break;
                case 2: //Filter By School And Subject Status
                    statusTypeCount = subjectCount.Where(s => s.SubjectStatus == SubjectStatus()).Count();

                    await LoadSubjects(2, schid, sbjdeptid, sbjclassid, SubjectStatus());
                    schoolCountDisplay = selectedStatusType + " Subject Count For " + selectedSchool + ": " + subjects.Count();
                    subjectDeptCountDisplay = string.Empty;
                    subjectClassCountDisplay = string.Empty;
                    statusTypeCountDisplay = "Total " + selectedStatusType + " Subject Count: " + statusTypeCount;
                    break;
                case 3://Filter By Department And Subject Status
                    statusTypeCount = subjectCount.Where(s => s.SubjectStatus == SubjectStatus()).Count();

                    await LoadSubjects(3, schid, sbjdeptid, sbjclassid, SubjectStatus());
                    schoolCountDisplay = string.Empty;
                    subjectDeptCountDisplay = selectedStatusType + " Subject Count For " + selectedSubjectDept + ": " + subjects.Count();
                    subjectClassCountDisplay = string.Empty;
                    statusTypeCountDisplay = "Total " + selectedStatusType + " Subject Count: " + statusTypeCount;
                    break;
                case 4://Filter By Classification And Subject Status
                    statusTypeCount = subjectCount.Where(s => s.SubjectStatus == SubjectStatus()).Count();

                    await LoadSubjects(4, schid, sbjdeptid, sbjclassid, SubjectStatus());
                    schoolCountDisplay = string.Empty;
                    subjectDeptCountDisplay = string.Empty;
                    subjectClassCountDisplay = selectedStatusType + " Subject Count For " + selectedSubjectClass + ": " + subjects.Count();
                    statusTypeCountDisplay = "Total " + selectedStatusType + " Subject Count: " + statusTypeCount;
                    break;
                case 5: //Filter By School, Department And Subject Status
                    statusTypeCount = subjectCount.Where(s => s.SubjectStatus == SubjectStatus()).Count();
                    schoolCount = subjectCount.Where(s => s.SubjectStatus == SubjectStatus() && s.SchID == schid).Count();

                    await LoadSubjects(5, schid, sbjdeptid, sbjclassid, SubjectStatus());
                    schoolCountDisplay = selectedStatusType + " Subject Count For " + selectedSchool + ": " + schoolCount;
                    subjectDeptCountDisplay = selectedStatusType + " Subject Count For " + selectedSubjectDept + ": " + subjects.Count();
                    subjectClassCountDisplay = string.Empty;
                    statusTypeCountDisplay = "Total " + selectedStatusType + " Subject Count: " + statusTypeCount;
                    break;
                case 6://Filter By School, Classification And Subject Status
                    statusTypeCount = subjectCount.Where(s => s.SubjectStatus == SubjectStatus()).Count();
                    schoolCount = subjectCount.Where(s => s.SubjectStatus == SubjectStatus() && s.SchID == schid).Count();

                    await LoadSubjects(6, schid, sbjdeptid, sbjclassid, SubjectStatus());
                    schoolCountDisplay = selectedStatusType + " Subject Count For " + selectedSchool + ": " + schoolCount;
                    subjectDeptCountDisplay = string.Empty;
                    subjectClassCountDisplay = selectedStatusType + " Subject Count For " + selectedSubjectClass + ": " + subjects.Count();
                    statusTypeCountDisplay = "Total " + selectedStatusType + " Subject Count: " + statusTypeCount;
                    break;
                case 7://Filter By Department, Classification And Subject Status
                    statusTypeCount = subjectCount.Where(s => s.SubjectStatus == SubjectStatus()).Count();
                    sbjdeptCount = subjectCount.Where(s => s.SubjectStatus == SubjectStatus() && s.SchID == schid && s.SbjDeptID == sbjdeptid).Count();

                    await LoadSubjects(7, schid, sbjdeptid, sbjclassid, SubjectStatus());
                    schoolCountDisplay = string.Empty;
                    subjectDeptCountDisplay = selectedStatusType + " Subject Count For " + selectedSubjectDept + ": " + sbjdeptCount;
                    subjectClassCountDisplay = selectedStatusType + " Subject Count For " + selectedSubjectClass + ": " + subjects.Count();
                    statusTypeCountDisplay = "Total " + selectedStatusType + " Subject Count: " + statusTypeCount;
                    break;
                case 8://Filter By School, Department, Classification And Subject Status
                    statusTypeCount = subjectCount.Where(s => s.SubjectStatus == SubjectStatus()).Count();
                    schoolCount = subjectCount.Where(s => s.SubjectStatus == SubjectStatus() && s.SchID == schid).Count();
                    sbjdeptCount = subjectCount.Where(s => s.SubjectStatus == SubjectStatus() && s.SchID == schid && s.SbjDeptID == sbjdeptid).Count();

                    await LoadSubjects(8, schid, sbjdeptid, sbjclassid, SubjectStatus());
                    schoolCountDisplay = selectedStatusType + " Subject Count For " + selectedSchool + ": " + schoolCount;
                    subjectDeptCountDisplay = selectedStatusType + " Subject Count For " + selectedSubjectDept + ": " + sbjdeptCount;
                    subjectClassCountDisplay = selectedStatusType + " Subject Count For " + selectedSubjectClass + ": " + subjects.Count();
                    statusTypeCountDisplay = "Total " + selectedStatusType + " Subject Count: " + statusTypeCount;
                    break;
            }
        }

        async Task OnSelectedSchoolChanged(IEnumerable<string> e)
        {
            selectedSchool = e.ElementAt(0);
            schid = schools.FirstOrDefault(s => s.School == selectedSchool).SchID;

            await SubjectFilters();
        }

        async Task OnSelectedDepartmentChanged(IEnumerable<string> e)
        {
            selectedSubjectDept = e.ElementAt(0);
            sbjdeptid = sbjdepts.FirstOrDefault(s => s.SbjDept == selectedSubjectDept).SbjDeptID;

            await SubjectFilters();
        }

        async Task OnSelectedClassificationChanged(IEnumerable<string> e)
        {
            selectedSubjectClass = e.ElementAt(0);
            sbjclassid = sbjclasslist.FirstOrDefault(s => s.SbjClassification == selectedSubjectClass).SbjClassID;

            await SubjectFilters();
        }

        async Task OnSelectedStatusTypeChanged(IEnumerable<string> e)
        {
            selectedStatusType = e.ElementAt(0);
            statustypeid = statusType.FirstOrDefault(s => s.StatusType == selectedStatusType).StatusTypeID;

            await SubjectFilters();
        }

        async Task UpdateEntry()
        {
            subject.SubjectID = selectedItem.SubjectID;
            subject.SchID = schools.FirstOrDefault(s => s.School == selectedItem.School).SchID;
            subject.SbjDeptID = sbjdepts.FirstOrDefault(s => s.SbjDept == selectedItem.SubjectDepartment).SbjDeptID; ;
            subject.SbjClassID = sbjclasslist.FirstOrDefault(s => s.SbjClassification == selectedItem.SubjectClassification).SbjClassID;
            subject.SubjectCode = selectedItem.SubjectCode;
            subject.Subject = selectedItem.Subject;
            subject.SubjectStatus = selectedItem.SubjectStatus;
            subject.SortID = selectedItem.SortID;

            await subjectService.UpdateAsync("AcademicsSubjects/UpdateSubject/", 1, subject);

            Snackbar.Add("Selected Row Entries Updated Successfully.");
        }

        async Task RefreshList()
        {
            selectedSchool = string.Empty;
            schools.Clear();
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            selectedSubjectDept = string.Empty;
            sbjdepts.Clear();
            sbjdepts = await subjectDepartmentService.GetAllAsync("AcademicsSubjects/GetDepartments/1");

            selectedSubjectClass = string.Empty;
            sbjclasslist.Clear();
            sbjclasslist = await subjectClassificationService.GetAllAsync("AcademicsSubjects/GetSubjectsClassifications/1");
            
            selectedStatusType = string.Empty;
            statusType.Clear();
            statustypeid = 1;
            selectedStatusType = "Active";
            statusType = await statusTypeService.GetAllAsync("Settings/GetStatusTypes/1");

            subjects.Clear();
            await LoadSubjects(1, 0, 0, 0, true);
            schoolCountDisplay = string.Empty;
            subjectDeptCountDisplay = string.Empty;
            subjectClassCountDisplay = string.Empty;
            statusTypeCountDisplay = "Total " + selectedStatusType + " Student Count: " + subjects.Count();

            searchString = string.Empty;
        }

        private bool FilterFunc(ACDSubjects model)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (model.Subject.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.SubjectCode.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        #endregion

        #region [Section - Details]
        void OnSchoolChanged(IEnumerable<string> value)
        {
            string selectedValue = value.ElementAt(0);
            subject.SchID = schools.FirstOrDefault(s => s.School == selectedValue).SchID;
            subject.School = selectedValue;
        }

        void OnSubjectDepartmentChanged(IEnumerable<string> value)
        {
            string selectedValue = value.ElementAt(0);
            subject.SbjDeptID = sbjdepts.FirstOrDefault(s => s.SbjDept == selectedValue).SbjDeptID;
            subject.SubjectDepartment = selectedValue;
        }

        void OnSubjectClassChanged(IEnumerable<string> value)
        {
            string selectedValue = value.ElementAt(0);
            subject.SbjClassID = sbjclasslist.FirstOrDefault(s => s.SbjClassification == selectedValue).SbjClassID;
            subject.SubjectClassification = selectedValue;
        }

        async Task SubmitValidForm()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Subject Save Operation",
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
                    subject.SubjectStatus = true;
                    var response = await subjectService.SaveAsync("AcademicsSubjects/AddSubject/", subject);
                    subject.SubjectID= response.SubjectID;
                    subject.Id = response.SubjectID;
                    await subjectService.UpdateAsync("AcademicsSubjects/UpdateSubject/", 3, subject);
                    await Swal.FireAsync("New Subject", "Has Been Successfully Created.", "success");
                    await SubjectListEvent();
                }
            }
        }

        async Task InvalidEntries()
        {
            await Swal.FireAsync("Please Check Your Entries.", "One or More Entries has error.", "error");
        }

        #endregion

        #region [Section - Click Events]
        async Task SubjectListEvent()
        {
            toolBarMenuId = 1;
            disableSaveButton = true;
            await RefreshList();
        }

        void AddNewSubject()
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            subject = new ACDSubjects();
        }

        #endregion
    }
}
