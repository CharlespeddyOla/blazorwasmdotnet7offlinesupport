using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.CBT;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Academics.Subjects;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Administration.Staff;
using WebAppAcademics.Shared.Models.Settings;
using WebAppAcademics.Client.Extensions;
using System.Diagnostics;
using WebAppAcademics.Client.Pages.Admin.Staff;
using static MudBlazor.CategoryTypes;
using WebAppAcademics.Client.Pages.Admin.School;

namespace WebAppAcademics.Client.Pages.Academics.Subjects
{
    public partial class SubjectsAllocationToTeacher
    {
        #region [Injection Declaration]
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] IJSRuntime iJSRuntime { get; set; }
        [Inject] SweetAlertService Swal { get; set; }        
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassList> classService { get; set; }
        [Inject] IAPIServices<ADMSchClassGroup> classGroupService { get; set; }
        [Inject] IAPIServices<ADMEmployee> staffService { get; set; }
        [Inject] IAPIServices<ACDSbjClassification> subjectClassificationService { get; set; }
        [Inject] IAPIServices<ACDSubjects> subjectService { get; set; }
        [Inject] IAPIServices<ACDSbjAllocationTeachers> subjectAllocationTeacherService { get; set; }
        [Inject] IAPIServices<SETStatusType> statusTypeService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksCognitive> studentMarksCognitiveService { get; set; }
        [Inject] IAPIServices<CBTExams> examService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }

        int toolBarMenuId { get; set; }
        int termid { get; set; }
        int schoolSession { get; set; }
        int calendarid { get; set; }
        #endregion

        void InitializeModels()
        {            
            _selectedClasses = new HashSet<string>();
            _selectedSubjects = new HashSet<string>();
            _teachersList = new List<string>();
            teachersFilter = new List<string>();
            messageListing = new List<string>();
        }

        async Task LoadDefaultList()
        {
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            allClassList = await classService.GetAllAsync("AdminSchool/GetClassList/0/0/0");
            staffs = await staffService.GetAllAsync("AdminStaff/GetStaffs/1/1/0/0/0");
            teachersFilter = staffs.Select(t => t.StaffNameWithNo).ToList();
            statusType = await statusTypeService.GetAllAsync("Settings/GetStatusTypes/1");

            _schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            _staffs = await staffService.GetAllAsync("AdminStaff/GetStaffs/1/1/0/0/0");
            _teachersList = _staffs.Select(t => t.StaffNameWithNo).ToList();
            _subjectsClassifications = await subjectClassificationService.GetAllAsync("AcademicsSubjects/GetSubjectsClassifications/1");
        }

        protected override async Task OnInitializedAsync()
        {
            InitializeModels();
            Layout.currentPage = "Subjects Allocation To Teachers";
            toolBarMenuId = 1;
            
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            schoolSession = await localStorageService.ReadEncryptedItemAsync<int>("schoolsession");
            calendarid = await localStorageService.ReadEncryptedItemAsync<int>("calendarid");

            await LoadDefaultList();
            await base.OnInitializedAsync();
        }

        #region [Section - Subject Allocation List]

        #region [Sub Section - Model Declaration]
        List<ADMSchlList> schools = new();
        List<ADMSchClassList> allClassList = new();
        List<ADMSchClassGroup> classGroups = new();
        List<ADMSchClassList> classList = new();
        List<ADMEmployee> staffs = new();
        List<SETStatusType> statusType = new();
        List<ACDSbjAllocationTeachers> subjectAllocationsAll = new();
        List<ACDSbjAllocationTeachers> subjectAllocations = new();

        ACDSbjAllocationTeachers selectedItem = null;
        ACDSbjAllocationTeachers subjectAllocation = new();
        List<string> teachersFilter { get; set; }

        ACDStudentsMarksCognitive cognitiveMark = new();
        CBTExams exam = new();
        #endregion

        #region [Sub Section - Variable Declaration]
        int schid { get; set; }
        int classlistid { get; set; }
        int classid { get; set; }
        int staffid { get; set; }
        int statustypeid { get; set; } = 1;
        int schoolCount { get; set; }
        int classGroupCount { get; set; }
        int classCount { get; set; }
       
        string selectedSchool { get; set; }
        string selectedclassGroup { get; set; }
        string selectedClass { get; set; }
        string selectedTeacher { get; set; }
        string selectedStatusType { get; set; } = "Active";

        string schoolCountDisplay { get; set; }
        string classGroupCountDisplay { get; set; }
        string classCountDisplay { get; set; }
        string teacherCountDisplay { get; set; }
        string statusTypeCountDisplay { get; set; }

        string searchString { get; set; } = string.Empty;
        string value1 { get; set; }
        #endregion

        async Task LoadAllTeachersAllocation()
        {
            _processing = true;
            timerDisplay = "Please wait, loading teachers allocated subjects...";
            var _subjectAllocationsAll = await subjectAllocationTeacherService.GetAllAsync(
                "AcademicsSubjects/GetTeacherAllocations/0/true/" + termid + "/0/0/0/0/0");
            int _sn = 1;
            subjectAllocationsAll.Clear();
            foreach (var item in _subjectAllocationsAll)
            {
                subjectAllocationsAll.Add(new ACDSbjAllocationTeachers
                {
                    SbjAllocID = item.SbjAllocID,
                    TermID= item.TermID,
                    SN = _sn++,
                    StaffID = item.StaffID,
                    SubjectID = item.SubjectID,
                    SchID = item.SchID,
                    ClassID = item.ClassID,
                    ClassListID = item.ClassListID,
                    SbjSelection = item.SbjSelection,
                    SubjectCode = item.SubjectCode,
                    Subject = item.Subject,
                    SbjClassID = item.SbjClassID,
                    SubjectTeacher = item.SubjectTeacher,
                    School = item.School,
                    ClassGroupName = allClassList.SingleOrDefault(c => c.ClassID == item.ClassID).ClassGroupName,
                    ClassName = allClassList.SingleOrDefault(c => c.ClassID == item.ClassID).ClassName,
                });
            }
            _processing = false;

            await Swal.FireAsync("Subject Allocation", "Subjects Successfully Loaded.", "success");
        }

        async Task SerializeList()
        {
            int sn = 1;
            foreach (var item in subjectAllocationsAll)
            {
                subjectAllocationsAll.FirstOrDefault(s => s.SbjAllocID == item.SbjAllocID).SN = sn++;
            }

            await Task.CompletedTask;
        }

        async Task LoadList()
        {
            stopwatchvalue = new TimeSpan();
            Task task1 = Task.Run(() => LoadAllTeachersAllocation());
            Task task2 = Task.Run(() => StartStopWatch());

            await Task.WhenAll(task1, task2);

            task1.Dispose();
            task2.Dispose();

            await Task.CompletedTask;
        }

        async Task StartProcess()
        {
            await RefreshList();
            await LoadList();
        }

        bool SbjSelection()
        {
            if (selectedStatusType == "Active")
            {
                return true;
            }

            return false;
        }

        int GetFilterID()
        {
            if (!String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedclassGroup) && String.IsNullOrWhiteSpace(selectedClass)
                && String.IsNullOrWhiteSpace(selectedTeacher)) //School By school
            {
                return 1;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedclassGroup) && String.IsNullOrWhiteSpace(selectedClass)
                && String.IsNullOrWhiteSpace(selectedTeacher)) //Filter By School And Class Group
            {
                return 2;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedclassGroup) && !String.IsNullOrWhiteSpace(selectedClass)
                && String.IsNullOrWhiteSpace(selectedTeacher)) //Filter By School, Class Group And Class
            {
                return 3;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedclassGroup) && !String.IsNullOrWhiteSpace(selectedClass)
                && !String.IsNullOrWhiteSpace(selectedTeacher)) //Filter By School, Class Group, Class And Teacher
            {
                return 4;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedclassGroup) && String.IsNullOrWhiteSpace(selectedClass)
                && !String.IsNullOrWhiteSpace(selectedTeacher)) //Filter By School And Teacher
            {
                return 5;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedclassGroup) && String.IsNullOrWhiteSpace(selectedClass)
                && !String.IsNullOrWhiteSpace(selectedTeacher)) //Filter By School, Class Group And Teacher
            {
                return 6;
            }
            else if (String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedclassGroup) && String.IsNullOrWhiteSpace(selectedClass)
                && !String.IsNullOrWhiteSpace(selectedTeacher)) //Filter Teacher
            {
                return 7;
            }
            else if (String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedclassGroup) && String.IsNullOrWhiteSpace(selectedClass)
               && String.IsNullOrWhiteSpace(selectedTeacher) && !String.IsNullOrWhiteSpace(selectedStatusType)) //Filter By Allocated/Not-Allocated Subjects
            {
                return 9;
            }

            return 0;
        }

        async Task SubjectAllocationFilter()
        {
            schoolCountDisplay = string.Empty;
            classGroupCountDisplay = string.Empty;
            classCountDisplay = string.Empty;
            teacherCountDisplay = string.Empty;

            switch (GetFilterID())
            {
                case 1: //School By school
                    subjectAllocations = subjectAllocationsAll.Where(s => s.SbjSelection == SbjSelection() && s.SchID == schid).ToList();

                    schoolCountDisplay = "Allocation Count For " + selectedSchool + ": " + subjectAllocations.Count();
                    classGroupCountDisplay = string.Empty;
                    classCountDisplay = string.Empty;
                    teacherCountDisplay = string.Empty;
                    break;
                case 2: //Filter By School And Class Group
                    subjectAllocations = subjectAllocationsAll.Where(s => s.SbjSelection == SbjSelection() && 
                                                                    s.SchID == schid && s.ClassListID == classlistid).ToList();


                    schoolCountDisplay = string.Empty;
                    classGroupCountDisplay = "Allocation Count For " + selectedclassGroup + ": " + subjectAllocations.Count();
                    classCountDisplay = string.Empty;
                    teacherCountDisplay = string.Empty;
                    break;
                case 3: //Filter By School, Class Group And Class
                    subjectAllocations = subjectAllocationsAll.Where(s => s.SbjSelection == SbjSelection() &&
                                                                    s.SchID == schid && s.ClassListID == classlistid &&
                                                                    s.ClassID == classid).ToList();
                    //await SerializeList();
                    schoolCountDisplay = string.Empty;
                    classGroupCountDisplay = string.Empty;
                    classCountDisplay = "Allocation Count For " + selectedclassGroup + ": " + subjectAllocations.Count();
                    teacherCountDisplay = string.Empty;
                    break;
                case 4: //Filter By School, Class Group, Class And Teacher
                    subjectAllocations = subjectAllocationsAll.Where(s => s.SbjSelection == SbjSelection() &&
                                                                    s.SchID == schid && s.ClassListID == classlistid &&
                                                                    s.ClassID == classid && s.StaffID == staffid).ToList();

                    schoolCountDisplay = string.Empty;
                    classGroupCountDisplay = string.Empty;
                    classCountDisplay = string.Empty;
                    teacherCountDisplay = "Allocation Count For " + selectedTeacher + ": " + subjectAllocations.Count();
                    break;
                case 5: //Filter By School And Teacher
                    subjectAllocations = subjectAllocationsAll.Where(s => s.SbjSelection == SbjSelection() &&
                                                                    s.SchID == schid && s.StaffID == staffid).ToList();

                    schoolCountDisplay = string.Empty;
                    classGroupCountDisplay = string.Empty;
                    classCountDisplay = string.Empty;
                    teacherCountDisplay = "Allocation Count For " + selectedclassGroup + ": " + subjectAllocations.Count();
                    break;
                case 6: //Filter By School, Class Group And Teacher
                    subjectAllocations = subjectAllocationsAll.Where(s => s.SbjSelection == SbjSelection() &&
                                                                    s.SchID == schid && s.ClassListID == classlistid &&
                                                                    s.StaffID == staffid).ToList();

                    schoolCountDisplay = string.Empty;
                    classGroupCountDisplay = string.Empty;
                    classCountDisplay = string.Empty;
                    teacherCountDisplay = "Allocation Count For " + selectedclassGroup + ": " + subjectAllocations.Count();
                    break;
                case 7: //Filter By Teacher
                    subjectAllocations = subjectAllocationsAll.Where(s => s.SbjSelection == SbjSelection() &&
                                                                     s.StaffID == staffid).ToList();

                    schoolCountDisplay = string.Empty;
                    classGroupCountDisplay = string.Empty;
                    classCountDisplay = string.Empty;
                    teacherCountDisplay = "Allocation Count For " + selectedSchool + ": " + subjectAllocations.Count();
                    break;
                case 9: //Filter By Allocated/Not-Allocated Subjects
                    subjectAllocations = subjectAllocationsAll.Where(s => s.SbjSelection == SbjSelection()).ToList();

                    schoolCountDisplay = string.Empty;
                    classGroupCountDisplay = string.Empty;
                    classCountDisplay = string.Empty;
                    teacherCountDisplay = string.Empty;
                    if (statustypeid == 1)
                    {
                        statusTypeCountDisplay = "Allocated Subject Count: " + subjectAllocations.Count();
                    }
                    else
                    {
                        statusTypeCountDisplay = "Not Allocated Subject Count: " + subjectAllocations.Count();
                    }
                    break;
            }

            await Task.CompletedTask;
        }

        async Task OnSelectedSchoolChanged(IEnumerable<string> e)
        {
            selectedSchool = e.ElementAt(0);
            schid = schools.FirstOrDefault(s => s.School == selectedSchool).SchID;

            selectedclassGroup = string.Empty;
            classGroups.Clear();
            classGroups = await classGroupService.GetAllAsync("AdminSchool/GetClassGroups/1/" + schid);

            selectedClass = string.Empty;
            classList.Clear();
            classList = await classService.GetAllAsync("AdminSchool/GetClassList/2/0/0");

            await SubjectAllocationFilter();
        }

        async Task OnSelectedClassGroupChanged(IEnumerable<string> e)
        {
            selectedclassGroup = e.ElementAt(0);
            classlistid = classGroups.FirstOrDefault(s => s.SchClass == selectedclassGroup).ClassListID;

            selectedClass = string.Empty;
            classList.Clear();
            classList = await classService.GetAllAsync("AdminSchool/GetClassList/2/0/" + classlistid);

            await SubjectAllocationFilter();
        }

        async Task OnSelectedClassChanged(IEnumerable<string> e)
        {
            selectedClass = e.ElementAt(0);
            classid = classList.FirstOrDefault(s => s.ClassName == selectedClass).ClassID;

            await SubjectAllocationFilter();
        }

        async Task OnSelectedTeacherChanged(IEnumerable<string> e)
        {
            selectedTeacher = e.ElementAt(0);
            staffid = staffs.FirstOrDefault(s => s.StaffNameWithNo == selectedTeacher).StaffID;

            await SubjectAllocationFilter();
        }

        async Task OnSelectedStatusTypeChanged(IEnumerable<string> e)
        {
            selectedStatusType = e.ElementAt(0);
            statustypeid = statusType.FirstOrDefault(s => s.StatusType == selectedStatusType).StatusTypeID;

            await SubjectAllocationFilter();
        }

        async Task RefreshList()
        {
            selectedSchool = string.Empty;
            schid = 0;
            schools.Clear();
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            selectedclassGroup = string.Empty;
            classlistid = 0;
            classGroups.Clear();
            classGroups = await classGroupService.GetAllAsync("AdminSchool/GetClassGroups/1/" + schid);

            selectedClass = string.Empty;
            classid = 0;
            classList.Clear();
            classList = await classService.GetAllAsync("AdminSchool/GetClassList/2/0/0");

            selectedTeacher = string.Empty;
            staffid = 0;
            staffs.Clear();
            staffs = await staffService.GetAllAsync("AdminStaff/GetStaffs/1/1/0/0/0");
            teachersFilter = staffs.Select(t => t.StaffNameWithNo).ToList();

            searchString = string.Empty;

            subjectAllocations.Clear();
            schoolCountDisplay = string.Empty;
            classGroupCountDisplay = string.Empty;
            classCountDisplay = string.Empty;
            teacherCountDisplay = string.Empty;

            selectedStatusType = string.Empty;
            statusType.Clear();
            statustypeid = 1;
            selectedStatusType = "Active";
            statusType = await statusTypeService.GetAllAsync("Settings/GetStatusTypes/1");
            statusTypeCountDisplay = string.Empty;
        }

        private bool FilterFunc(ACDSbjAllocationTeachers model)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (model.Subject.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.SubjectTeacher.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.School.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.ClassGroupName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.ClassName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        async Task UpdateEntry()
        {
            subjectAllocation.SbjAllocID = selectedItem.SbjAllocID;
            var _selectedStaff = staffs.FirstOrDefault(t => t.StaffInitialsWithNo == selectedItem.SubjectTeacher);
            if (_selectedStaff != null)
            {
                subjectAllocation.StaffID = staffs.FirstOrDefault(t => t.StaffInitialsWithNo == selectedItem.SubjectTeacher).StaffID;

                subjectAllocation.SbjSelection = selectedItem.SbjSelection;
                await subjectAllocationTeacherService.UpdateAsync("AcademicsSubjects/UpdateTeacherAllocation/", 1, subjectAllocation);

                cognitiveMark.TermID = selectedItem.TermID;
                cognitiveMark.SchID = selectedItem.SchID; 
                cognitiveMark.ClassID = selectedItem.ClassID; 
                cognitiveMark.SubjectID = selectedItem.SubjectID;
                cognitiveMark.StaffID = subjectAllocation.StaffID;
                await studentMarksCognitiveService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 9, cognitiveMark);

                exam.TermID = termid;
                exam.SchID = selectedItem.SchID;
                exam.SubjectID = selectedItem.SubjectID;
                exam.StaffID = selectedItem.StaffID;
                await examService.UpdateAsync("AcademicsCBT/UpdateCBTExam/", 3, exam);

                Snackbar.Add("Selected Subject Class (" + selectedItem.SubjectTeacher + ") Has Been Updated");
            }
        }

        #endregion

        #region [Section - Subjects Allocation Details]

        #region [Sub Section - Model Declaration]
        List<ADMSchlList> _schools = new();
        List<ADMSchClassList> _classList = new();
        List<ADMEmployee> _staffs = new();
        List<ACDSbjClassification> _subjectsClassifications = new();
        List<ACDSubjects> _subjects = new();
        List<ACDSbjAllocationTeachers> _subjectsAllocations = new();

        IEnumerable<string> _selectedClasses { get; set; }
        IEnumerable<string> _selectedSubjects { get; set; }
        List<string> _teachersList { get; set; }
        List<string> messageListing { get; set; }
        #endregion

        #region [Sub Section - Variable Declaration]
        int staffidallocation { get; set; }
        int schidallocation { get; set; }
        int classidallocation { get; set; }
        int classlistidallocation { get; set; }
        int classTeacherID { get; set; }
        int sbjclassid { get; set; }

        decimal i { get; set; } = 0;
        int j { get; set; } = 0;
        bool IsShow { get; set; } = true;
        string progressbarInfo { get; set; } = string.Empty;

        string selectedStaffAllocation { get; set; }
        string selectedSchoolAllocation { get; set; }
        string selectedClassAllocation { get; set; } = "Nothing selected";
        string selectedSubjectClassification { get; set; }
        string selectedSubject { get; set; } = "Nothing selected";
        string selectedClassTeacherAllocation { get; set; }
        string selectedClassTeacher { get; set; }

        bool disableSaveButton { get; set; } = true;

        #endregion

        async Task LoadSubjects(int switchid, int schid, int sbjdeptid, int sbjclassid, bool subjectstatus)
        {
            _subjects = await subjectService.GetAllAsync("AcademicsSubjects/GetSubjects/" + switchid + "/" + schid + "/" + sbjdeptid + "/" + sbjclassid + "/" + subjectstatus);
        }

        bool ValidateSelections()
        {
            if (sbjclassid != 0)
            {
                if (sbjclassid == 1)
                {
                    if (!String.IsNullOrWhiteSpace(selectedSchoolAllocation) && !String.IsNullOrWhiteSpace(selectedStaffAllocation) &&
                            !String.IsNullOrWhiteSpace(selectedClassAllocation) && !String.IsNullOrWhiteSpace(selectedSubject))
                    {
                        return true;
                    }
                }
                else if (sbjclassid > 1)
                {
                    if (!String.IsNullOrWhiteSpace(selectedSchoolAllocation) && !String.IsNullOrWhiteSpace(selectedClassTeacher) &&
                            !String.IsNullOrWhiteSpace(selectedClassTeacherAllocation) && !String.IsNullOrWhiteSpace(selectedSubject))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        async Task OnSchoolChanged(IEnumerable<string> e)
        {
            selectedSchoolAllocation = e.ElementAt(0);
            schidallocation = _schools.FirstOrDefault(s => s.School == selectedSchoolAllocation).SchID;

            classidallocation = 0;
            _selectedClasses = new HashSet<string>();
            selectedClassTeacherAllocation = string.Empty;
            _classList.Clear();
            _classList = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + schidallocation + "/0");
        }

        async Task OnClassChanged(IEnumerable<string> e)
        {
            selectedClassTeacherAllocation = e.ElementAt(0);
            classidallocation = _classList.FirstOrDefault(s => s.ClassName == selectedClassTeacherAllocation).ClassID;
            classlistidallocation = _classList.FirstOrDefault(s => s.ClassID == classidallocation).ClassListID;

            bool IsClassTeacherActive = _staffs.Where(s => s.StaffID == _classList.FirstOrDefault(s => s.ClassID == classidallocation).StaffID).Any();

            if (IsClassTeacherActive)
            {
                classTeacherID = _classList.FirstOrDefault(s => s.ClassID == classidallocation).StaffID;
                selectedClassTeacher = _classList.FirstOrDefault(s => s.ClassID == classidallocation).ClassTeacher;
            }
            else
            {
                string InActiveClassTeacher = _classList.FirstOrDefault(s => s.ClassID == classidallocation).ClassTeacher;
                await Swal.FireAsync("Class Teacher Subject Allocation", InActiveClassTeacher + " Is No Longer An Active Staff of The School. " +
                                        "Please, Assign a New ClassTeacher To " + selectedClassTeacherAllocation + ".", "error");
            }
        }

        async Task OnSubjectClassChanged(IEnumerable<string> e)
        {
            selectedSubjectClassification = e.ElementAt(0);
            sbjclassid = _subjectsClassifications.FirstOrDefault(s => s.SbjClassification == selectedSubjectClassification).SbjClassID;

            selectedSubject = string.Empty;
            _selectedSubjects = new HashSet<string>();
            _subjects.Clear();
            await LoadSubjects(6, schidallocation, 0, sbjclassid, true);
        }

        async Task SubjectAllocation()
        {
            messageListing.Clear();
            int sn = 1;

            foreach (var item in _classList)
            {
                bool IsClassSelected = _selectedClasses.Where(c => c.Contains(item.ClassName)).Any();
                foreach (var sbj in _subjects)
                {
                    bool IsSubjectSelected = _selectedSubjects.Where(s => s.Equals(sbj.Subject)).Any();                    

                    if (IsClassSelected && IsSubjectSelected)
                    {
                        var getSubjectBySchoolAndClass = 
                            await subjectAllocationTeacherService.GetAllAsync("AcademicsSubjects/GetTeacherAllocations/10/true/" + 
                                            termid + "/" + schidallocation + "/0/" + item.ClassID + "/" + sbj.SubjectID + "/0");

                        bool IsSubjectExist = getSubjectBySchoolAndClass.Any();

                        if (!IsSubjectExist)
                        {
                            //Add New Allocation
                            subjectAllocation.TermID = termid;
                            subjectAllocation.SchSession = schoolSession;
                            subjectAllocation.StaffID = staffidallocation;
                            subjectAllocation.SbjSelection = true;
                            subjectAllocation.SubjectID = sbj.SubjectID;
                            subjectAllocation.SchID = schidallocation;
                            subjectAllocation.ClassID = item.ClassID;
                            subjectAllocation.ClassListID = item.ClassListID;
                            subjectAllocation.StaffID_ClassTeacher = _classList.FirstOrDefault(s => s.ClassID == item.ClassID).StaffID;
                            subjectAllocation.StaffID_Principal = _classList.FirstOrDefault(s => s.ClassID == item.ClassID).PrincipalID;

                            var response = await subjectAllocationTeacherService.SaveAsync("AcademicsSubjects/AddTeacherAllocation/", subjectAllocation);
                            subjectAllocation.SbjAllocID = response.SbjAllocID;
                            subjectAllocation.Id = response.SbjAllocID;
                            await subjectAllocationTeacherService.UpdateAsync("AcademicsSubjects/UpdateTeacherAllocation/", 5, subjectAllocation);
                        }
                        else
                        {
                            string _subjectTeacher = getSubjectBySchoolAndClass.FirstOrDefault().SubjectTeacher;                            
                            messageListing.Add(sn++ + ". " + _subjectTeacher + " has been allocated to " + sbj.Subject + " for " + 
                                                item.ClassName);
                        }
                    }
                }
            }
        }

        async Task SubjectAllocationPsychomotor()
        {
            messageListing.Clear();
            int sn = 1;

            foreach (var sbj in _subjects)
            {
                var getSubjectBySchoolAndClass =
                           await subjectAllocationTeacherService.GetAllAsync("AcademicsSubjects/GetTeacherAllocations/10/true/" +
                                           termid + "/" + schidallocation + "/0/" + classidallocation + "/" + sbj.SubjectID + "/0");

                bool IsSubjectExist = getSubjectBySchoolAndClass.Any();

                if (!IsSubjectExist)
                {
                    //Add New Allocation
                    subjectAllocation.TermID = termid;
                    subjectAllocation.SchSession = schoolSession;
                    subjectAllocation.StaffID = classTeacherID;
                    subjectAllocation.SbjSelection = true;
                    subjectAllocation.SubjectID = sbj.SubjectID;
                    subjectAllocation.SchID = schidallocation;
                    subjectAllocation.ClassID = classidallocation;
                    subjectAllocation.ClassListID = classlistidallocation;
                    subjectAllocation.StaffID_ClassTeacher = _classList.FirstOrDefault(s => s.ClassID == classidallocation).StaffID;
                    subjectAllocation.StaffID_Principal = _classList.FirstOrDefault(s => s.ClassID == classidallocation).PrincipalID;

                    var response = await subjectAllocationTeacherService.SaveAsync("AcademicsSubjects/AddTeacherAllocation/", subjectAllocation);
                    subjectAllocation.SbjAllocID = response.SbjAllocID;
                    subjectAllocation.Id = response.SbjAllocID;
                    await subjectAllocationTeacherService.UpdateAsync("AcademicsSubjects/UpdateTeacherAllocation/", 5, subjectAllocation);
                }
                else
                {
                    messageListing.Add(sn++ + ". " + sbj.Subject);
                }
            }
        }

        async Task Save()
        {
            if (ValidateSelections())
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Teachers Subjects Allocation  Operation",
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
                        if (sbjclassid == 1)
                        {
                            staffidallocation = _staffs.FirstOrDefault(s => s.StaffNameWithNo == selectedStaffAllocation).StaffID;
                            await SubjectAllocation();

                            if (messageListing.Count() > 0)
                            {
                                string _message = string.Join("\n", messageListing.ToArray());

                                SweetAlertResult messageResult = await Swal.FireAsync(new SweetAlertOptions
                                {
                                    Title = "Selected Subject(s) for the Selected Class has already been allocated!",
                                    Width = "500",
                                    Icon = "info",                                    
                                    Html = "<pre class='format-pre' style='color: white;'>" + _message + "</pre>"
                                });
                            }

                            await Swal.FireAsync("Subject Allocation", "Subject For Selected Class Has Been Allocated To " +
                                            "The Selected Teacher (" + selectedStaffAllocation + "). " +
                                            "Please, confirm your allocation below.", "success");
                        }
                        else if (sbjclassid > 1)
                        {
                            await SubjectAllocationPsychomotor();

                            if (messageListing.Count() > 0)
                            {
                                string _message = string.Join("\n", messageListing.ToArray());

                                SweetAlertResult messageResult = await Swal.FireAsync(new SweetAlertOptions
                                {
                                    Title = selectedClassTeacher + " has already been allocated to " + selectedClassTeacherAllocation + " for the following Subject(s):",
                                    Width = "500",
                                    Icon = "info",
                                    Html = "<pre class='format-pre' style='color: white;'>" + _message + "</pre>"
                                });
                            }
                                
                            await Swal.FireAsync("Subject Allocation", "Subject For Selected Class Has Been Allocated To " +
                                            "The Selected Class Teacher (" + selectedClassTeacherAllocation + "). Please, confirm your allocation below.", "success");
                        }

                        await RefreshListAllocation();
                    }
                }
            }
            else
            {
                await Swal.FireAsync("Subject Allocation", "Cannot Allocate Subjects Because of Incomplete Selections. " +
                                        "Please, Check Your Selections.", "error");
            }
        }

        async Task RefreshListAllocation()
        {
            staffidallocation = 0;
            selectedStaffAllocation = string.Empty;
            _staffs.Clear();
            _staffs = await staffService.GetAllAsync("AdminStaff/GetStaffs/1/1/0/0/0");

            schidallocation = 0;
            selectedSchoolAllocation = string.Empty;
            _schools.Clear();
            _schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            _selectedClasses = new HashSet<string>();
            classidallocation = 0;
            selectedClassTeacherAllocation = string.Empty;
            selectedClassTeacher = string.Empty;
            _classList.Clear();
            _classList = await classService.GetAllAsync("AdminSchool/GetClassList/1/0/0");

            sbjclassid = 0;
            selectedSubjectClassification = string.Empty;
            _subjectsClassifications.Clear();
            _subjectsClassifications = await subjectClassificationService.GetAllAsync("AcademicsSubjects/GetSubjectsClassifications/1");

            _selectedSubjects = new HashSet<string>();
            _subjects.Clear();
            await LoadSubjects(6, 0, 0, 0, false);
        }

        async Task ImportPreviousTermAllocations()
        {
            IsShow = false;
            i = 0;
            j = 0;
            progressbarInfo = "Please wait importing previous term subject allocations...";

            int _lasttermid = termid - 1;
            var lastTermSubjectAllocations = 
                await subjectAllocationTeacherService.GetAllAsync("AcademicsSubjects/GetTeacherAllocations/13/true/" + _lasttermid + "/0/0/0/0/0");

            int maxValue = lastTermSubjectAllocations.Count;

            foreach (var item in lastTermSubjectAllocations)
            {
                j++;
                i = ((decimal)(j) / maxValue) * 100;

                subjectAllocation.TermID = termid;
                subjectAllocation.SchSession = schoolSession;
                subjectAllocation.StaffID = item.StaffID;
                subjectAllocation.SbjSelection = true;
                subjectAllocation.SubjectID = item.SubjectID;
                subjectAllocation.SchID = item.SchID;
                subjectAllocation.ClassID = item.ClassID;
                subjectAllocation.ClassListID = item.ClassListID;
                subjectAllocation.StaffID_ClassTeacher = item.StaffID_ClassTeacher;
                subjectAllocation.StaffID_Principal = item.StaffID_Principal;

                var response = await subjectAllocationTeacherService.SaveAsync("AcademicsSubjects/AddTeacherAllocation/", subjectAllocation);
                subjectAllocation.SbjAllocID = response.SbjAllocID;
                subjectAllocation.Id = response.SbjAllocID;
                await subjectAllocationTeacherService.UpdateAsync("AcademicsSubjects/UpdateTeacherAllocation/", 5, subjectAllocation);
                StateHasChanged();
            }

            IsShow = true;
            await Swal.FireAsync("Import Previous Term Subject Allocation", "Operation Completed Successfully!", "success");

        }

        async Task StartImport()
        {
            if (calendarid > 1)
            {
                var currentAllocations = 
                    await subjectAllocationTeacherService.GetAllAsync("AcademicsSubjects/GetTeacherAllocations/13/true/" + termid + "/0/0/0/0/0");


                if (currentAllocations.Where(a => a.SbjSelection == true).Count() == 0)
                {
                    SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                    {
                        Title = "Import Previous Term Subject Allocation Operation",
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
                            await ImportPreviousTermAllocations();
                        }
                    }
                }
                else
                {
                    await Swal.FireAsync("Import Previous Term Subject Allocations",
                        "Import of Subject Allocations Has Already Been Performmed.", "error");
                }
            }
            else
            {
                string academicYear = schoolSession.ToString().Substring(0, 4) + "/" + schoolSession.ToString().Substring(4, 4);
                await Swal.FireAsync("Import Previous Term Subject Allocation", "You Can Only Use This In Second And Third Term To " +
                                    "Import Subject Allocations From Previous Term. Please, Create New Teachers Subjects Allocations " +
                                    "For This New Session, " + academicYear + ".", "info");
            }
        }

        #endregion

        #region [Section - Click Events]
        async Task TeachersAllocationEvent()
        {
            toolBarMenuId = 1;
            disableSaveButton = true;
            await RefreshList();
        }

        async Task AllocateSubjectsToTeachers()
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            await RefreshListAllocation();
        }

        #endregion

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

    }
}
