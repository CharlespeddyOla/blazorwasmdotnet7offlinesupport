using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Academics.Subjects;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Administration.Students;
using WebAppAcademics.Shared.Models.Settings;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Client.Pages.Admin.School;

namespace WebAppAcademics.Client.Pages.Academics.Subjects
{
    public partial class SubjectsAllocationToStudent
    {
        #region [Injection Declaration]
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] IJSRuntime iJSRuntime { get; set; }      
        [Inject] SweetAlertService Swal { get; set; }     
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassGroup> classGroupService { get; set; }
        [Inject] IAPIServices<ADMSchClassList> classService { get; set; }
        [Inject] IAPIServices<ADMStudents> studentService { get; set; }
        [Inject] IAPIServices<ACDSbjClassification> subjectClassificationService { get; set; }
        [Inject] IAPIServices<ACDSubjects> subjectService { get; set; }
        [Inject] IAPIServices<ACDSbjAllocationStudents> subjectAllocationStudentService { get; set; }
        [Inject] IAPIServices<SETStatusType> statusTypeService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksCognitive> studentMarksCognitiveService { get; set; }


        [CascadingParameter]
        public MainLayout Layout { get; set; }

        int toolBarMenuId { get; set; }
        int termid { get; set; }
        int schoolSession { get; set; }
       
        #endregion

        void InitializeModels()
        {            
            selectedStudents = new HashSet<string>();
            selectedSubjects = new HashSet<string>();
        }

        async Task LoadDefaultList()
        {
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            allClassList = await classService.GetAllAsync("AdminSchool/GetClassList/0/0/0");
            statusType = await statusTypeService.GetAllAsync("Settings/GetStatusTypes/1");

            schoolList = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            sbjclasslist = await subjectClassificationService.GetAllAsync("AcademicsSubjects/GetSubjectsClassifications/1");
        }

        protected override async Task OnInitializedAsync()
        {
            InitializeModels();

            Layout.currentPage = "Subjects Allocation To Students";
            toolBarMenuId = 1;                        
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            schoolSession = await localStorageService.ReadEncryptedItemAsync<int>("schoolsession");

            await LoadDefaultList();
            //await LoadList();
            await base.OnInitializedAsync();
        }

        #region [Section - Subject Allocation List]

        #region [Sub Section - Model Declaration]
        List<ADMSchlList> schools = new();
        List<ADMSchClassList> allClassList = new();
        List<ADMSchClassGroup> classGroups = new();
        List<ADMSchClassList> classList = new();
        List<ADMStudents> students = new();
        List<SETStatusType> statusType = new();
        List<ACDSbjAllocationStudents> subjectAllocationsAll = new();
        List<ACDSbjAllocationStudents> subjectAllocations = new();

        ACDSbjAllocationStudents subjectAllocation = new();
        ACDSbjAllocationStudents selectedItem = null;

        ACDStudentsMarksCognitive cognitiveMark = new();
        #endregion

        #region [Sub Section - Variable Declaration]
        int schid { get; set; }
        int classlistid { get; set; }
        int classid { get; set; }
        int stdid { get; set; }
        int statustypeid { get; set; } = 1;

        int schoolCount { get; set; }
        int classGroupCount { get; set; }
        int classCount { get; set; }

        string selectedSchool { get; set; }
        string selectedClassGroup { get; set; }
        string selectedClass { get; set; }
        string selectedStudent { get; set; }
        string selectedStatusType { get; set; } = "Active";

        string schoolCountDisplay { get; set; }
        string classGroupCountDisplay { get; set; }
        string classCountDisplay { get; set; }
        string studentCountDisplay { get; set; }
        string statusTypeCountDisplay { get; set; }

        string searchString { get; set; } = string.Empty;

        #endregion

        async Task LoadAllStudentsAllocation()
        {           
            _processing = true;
            timerDisplay = "Please wait, loading students allocated subjects...";
            var _subjectAllocationsAll =
                await subjectAllocationStudentService.GetAllAsync("AcademicsSubjects/GetStudentAllocations/0/" + schoolSession + "/false/0/0/0/0/0");
            int _sn = 1;
            subjectAllocationsAll.Clear();
            foreach (var item in _subjectAllocationsAll)
            {
                subjectAllocationsAll.Add(new ACDSbjAllocationStudents
                {
                    SbjAllocID = item.SbjAllocID,
                    SchSession = item.SchSession, 
                    SN = _sn++,
                    STDID = item.STDID,
                    SubjectID = item.SubjectID,
                    SchID = item.SchID,
                    ClassID = item.ClassID,
                    ClassListID = item.ClassListID,
                    SbjSelection = item.SbjSelection,
                    SubjectCode = item.SubjectCode,
                    Subject = item.Subject,
                    StudentName = item.StudentName,
                    SchClass = allClassList.SingleOrDefault(c => c.ClassID == item.ClassID).SchClass,
                    ClassName = allClassList.SingleOrDefault(c => c.ClassID == item.ClassID).ClassName,
                });
            }            
            _processing = false;

            await Swal.FireAsync("Subject Allocation", "Subjects Successfully Loaded.", "success");
        }

        async Task LoadList()
        {
            stopwatchvalue = new TimeSpan();
            Task task1 = Task.Run(() => LoadAllStudentsAllocation());
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
            if (String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClassGroup) &&
                String.IsNullOrWhiteSpace(selectedClass) && String.IsNullOrWhiteSpace(selectedStudent))
            {
                //Filter By Allocated/Not-Allocated Subjects
                return 1;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClassGroup) &&
                String.IsNullOrWhiteSpace(selectedClass) && String.IsNullOrWhiteSpace(selectedStudent))
            {
                //Filter By S Allocated/Not-Allocated Subjects And School
                return 2;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClassGroup) &&
                String.IsNullOrWhiteSpace(selectedClass) && String.IsNullOrWhiteSpace(selectedStudent))
            {
                //Filter By  Allocated/Not-Allocated Subjects, School And ClassGroup
                return 3;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClassGroup) &&
                !String.IsNullOrWhiteSpace(selectedClass) && String.IsNullOrWhiteSpace(selectedStudent))
            {
                //Filter By Allocated/Not-Allocated Subjects, School, ClassGroup And ClassList 
                return 4;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClassGroup) &&
                !String.IsNullOrWhiteSpace(selectedClass) && !String.IsNullOrWhiteSpace(selectedStudent))
            {
                //Filter By Allocated/Not-Allocated Subjects, School, Class Group, Class List And Student
                return 5;
            }

            return 0;
        }

        async Task StudentSubjectAllocationFilter()
        {
            schoolCountDisplay = string.Empty;
            classGroupCountDisplay = string.Empty;
            classCountDisplay = string.Empty;
            studentCountDisplay = string.Empty;

            switch (GetFilterID())
            {
                case 1://Filter By Allocated/Not-Allocated Subjects
                    subjectAllocations = subjectAllocationsAll.Where(s => s.SbjSelection == SbjSelection()).ToList();

                    schoolCountDisplay = string.Empty;
                    classGroupCountDisplay = string.Empty;
                    classCountDisplay = string.Empty;
                    studentCountDisplay = string.Empty;
                    if (statustypeid == 1)
                    {
                        statusTypeCountDisplay = "Allocated Subject Count: " + subjectAllocations.Count();
                    }
                    else
                    {
                        statusTypeCountDisplay = "Not Allocated Subject Count: " + subjectAllocations.Count();
                    }
                    break;
                case 2: //Filter By Allocated/Not-Allocated Subjects And School
                    subjectAllocations = subjectAllocationsAll.Where(s => s.SchID == schid && s.SbjSelection == SbjSelection()).ToList();

                    schoolCountDisplay = "Allocation Count For " + selectedSchool + ": " + subjectAllocations.Count();
                    classGroupCountDisplay = string.Empty;
                    classCountDisplay = string.Empty;
                    studentCountDisplay = string.Empty;
                    break;
                case 3: //Filter By  Allocated/Not-Allocated Subjects, School And ClassGroup
                    subjectAllocations = subjectAllocationsAll.Where(s => s.SchID == schid && s.ClassListID == classlistid && 
                                                                        s.SbjSelection == SbjSelection()).ToList();

                    classGroupCountDisplay = "Allocation Count For " + selectedClassGroup + ": " + subjectAllocations.Count();
                    classCountDisplay = string.Empty;
                    studentCountDisplay = string.Empty;
                    break;
                case 4: //Filter By Allocated/Not-Allocated Subjects, School, ClassGroup And ClassList 
                    subjectAllocations = subjectAllocationsAll.Where(s => s.SchID == schid && s.ClassListID == classlistid &&
                                                                        s.ClassID == classid && s.SbjSelection == SbjSelection()).ToList();

                    classCountDisplay = "Allocation Count For " + selectedClass + ": " + subjectAllocations.Count();
                    studentCountDisplay = string.Empty;
                    break;
                case 5: //Filter By Allocated/Not-Allocated Subjects, School, Class Group, Class List And Student
                    subjectAllocations = subjectAllocationsAll.Where(s => s.SchID == schid && s.ClassListID == classlistid &&
                                                                        s.ClassID == classid && s.STDID == stdid && 
                                                                        s.SbjSelection == SbjSelection()).ToList();

                    if (statustypeid == 1)
                    {
                        studentCountDisplay = "Subject Allocated: " + subjectAllocations.Count();
                    }
                    else
                    {
                        studentCountDisplay = "Subject Not Allocated: " + subjectAllocations.Count();
                    }
                    break;
            }

            await Task.CompletedTask;
        }

        async Task OnSelectedSchoolChanged(IEnumerable<string> e)
        {
            selectedSchool = e.ElementAt(0);
            schid = schools.FirstOrDefault(s => s.School == selectedSchool).SchID;

            selectedClassGroup = string.Empty;
            classGroups.Clear();
            var _classGroups = allClassList.Where(a => a.SchID == schid)
                        .GroupBy(x => new { x.ClassListID, x.SchClass })
                        .Select(x => x.First())
                        .ToList();
            foreach(var item in _classGroups)
            {
                classGroups.Add(new ADMSchClassGroup
                {
                    ClassListID = item.ClassListID,
                    SchClass = item.SchClass,
                });
            }

            selectedClass = string.Empty;
            classList.Clear();
            selectedStudent = string.Empty;
            students.Clear();
            await StudentSubjectAllocationFilter();
        }

        async Task OnSelectedClassGroupChanged(IEnumerable<string> e)
        {
            selectedClassGroup = e.ElementAt(0);
            classlistid = classGroups.FirstOrDefault(s => s.SchClass == selectedClassGroup).ClassListID;

            selectedClass = string.Empty;
            classList.Clear();
            classList = await classService.GetAllAsync("AdminSchool/GetClassList/2/0/" + classlistid);
            classList = allClassList.Where(c => c.ClassListID == classlistid)
                        .GroupBy(x => new { x.ClassID, x.ClassName })
                        .Select(x => x.First())
                        .ToList();


            selectedStudent = string.Empty;
            students.Clear();
            await StudentSubjectAllocationFilter();
        }

        async Task OnSelectedClassChanged(IEnumerable<string> e)
        {
            selectedClass = e.ElementAt(0);
            classid = classList.FirstOrDefault(s => s.ClassName == selectedClass).ClassID;

            students.Clear();
            var distinctStudents = subjectAllocationsAll.Where(c => c.ClassID == classid)
                                   .GroupBy(x => new { x.STDID, x.StudentName })
                                   .Select(x => x.First())
                                   .ToList();
            foreach (var item in distinctStudents)
            {
                students.Add(new ADMStudents
                {
                    STDID = item.STDID,
                    StudentName = item.StudentName,
                });
            }
            await StudentSubjectAllocationFilter();
        }

        async Task OnSelectedStudentChanged(IEnumerable<string> e)
        {
            selectedStudent = e.ElementAt(0);
            stdid = students.FirstOrDefault(s => s.StudentName == selectedStudent).STDID;

            await StudentSubjectAllocationFilter();
        }

        async void OnSelectedStatusTypeChanged(IEnumerable<string> e)
        {
            selectedStatusType = e.ElementAt(0);
            statustypeid = statusType.FirstOrDefault(s => s.StatusType == selectedStatusType).StatusTypeID;

            //await RefreshList();
            await Task.CompletedTask;
        }

        async Task UpdateEntry()
        {
            subjectAllocation.SbjAllocID = selectedItem.SbjAllocID;
            subjectAllocation.SbjSelection = selectedItem.SbjSelection;
            await subjectAllocationStudentService.UpdateAsync("AcademicsSubjects/UpdateStudentAllocation/", 1, subjectAllocation);

            //Update Cognitive Mark Entry
            cognitiveMark.TermID = termid;
            cognitiveMark.ClassID = selectedItem.ClassID; // subjectAllocations.FirstOrDefault(s => s.STDID == selectedItem.STDID).ClassID;
            cognitiveMark.SubjectID = selectedItem.SubjectID; // subjectAllocations.FirstOrDefault(s => s.STDID == selectedItem.STDID).SubjectID;
            cognitiveMark.STDID = selectedItem.STDID;
            cognitiveMark.SbjSelection = selectedItem.SbjSelection;
            await studentMarksCognitiveService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 8, cognitiveMark);

            Snackbar.Add("Selected Subject For " + selectedItem.StudentName + " Has Been Updated");
        }

        private bool FilterFunc(ACDSbjAllocationStudents model)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (model.SbjClassification.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.SbjDept.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.Subject.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.StudentName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.School.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.SchClass.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.ClassName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        async Task RefreshList()
        {
            selectedSchool = string.Empty;
            schools.Clear();
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            selectedClassGroup = string.Empty;
            classGroups.Clear();

            selectedClass = string.Empty;
            classList.Clear();

            selectedStudent = string.Empty;
            students.Clear();
            
            searchString = string.Empty;

            subjectAllocations.Clear();
            schoolCountDisplay = string.Empty;
            classGroupCountDisplay = string.Empty;
            classCountDisplay = string.Empty;
            studentCountDisplay = string.Empty;

            selectedStatusType = string.Empty;
            statusType.Clear();
            statustypeid = 1;
            selectedStatusType = "Active";
            statusType = await statusTypeService.GetAllAsync("Settings/GetStatusTypes/1");
            statusTypeCountDisplay = string.Empty;            
        }


        #endregion

        #region [Section - Subjects Allocation Details]

        #region [Sub Section - Model Declaration]
        List<ADMSchlList> schoolList = new();
        List<ADMSchClassGroup> grouplist = new();
        List<ADMSchClassList> _classList = new();
        List<ADMStudents> studentlist = new();
        List<ACDSbjClassification> sbjclasslist = new();
        List<ACDSubjects> subjectlist = new();

        IEnumerable<string> selectedStudents { get; set; }
        IEnumerable<string> selectedSubjects { get; set; }
        #endregion

        #region [Sub Section - Variable Declaration]
        int _schid { get; set; }
        int _classlistid { get; set; }
        int _classid { get; set; }
        int _sbjclassid { get; set; }

        string _slectedSchool { get; set; }
        string _selectedClassGroup { get; set; }
        string _selectedClass { get; set; }
        string _selectedSubjectClass { get; set; }
        string _selectedSubject { get; set; }
        string _selectedStudent { get; set; }
        string _ErrorMessage { get; set; }

        decimal i { get; set; } = 0;
        int j { get; set; } = 0;
        bool IsShow { get; set; } = true;
        string progressbarInfo { get; set; } = string.Empty;

        bool disableSaveButton { get; set; } = true;
        #endregion

        async Task LoadSubjects(int switchid, int schid, int sbjdeptid, int sbjclassid, bool subjectstatus)
        {
            subjectlist = await subjectService.GetAllAsync("AcademicsSubjects/GetSubjects/" + switchid + "/" + schid + "/" + sbjdeptid + "/" + sbjclassid + "/" + subjectstatus);
        
            
        }

        async Task OnSchoolChanged(IEnumerable<string> e)
        {
            _slectedSchool = e.ElementAt(0);
            _schid = schoolList.FirstOrDefault(s => s.School == _slectedSchool).SchID;

            _classlistid = 0;
            _selectedClassGroup = string.Empty;
            grouplist.Clear();
            grouplist = await classGroupService.GetAllAsync("AdminSchool/GetClassGroups/1/" + _schid);

            _classid = 0;
            _selectedClass = string.Empty;
            _classList.Clear();
            _classList = await classService.GetAllAsync("AdminSchool/GetClassList/2/0/0");

            selectedStudents = new HashSet<string>();
            studentlist.Clear();
            studentlist = await studentService.GetAllAsync("AdminStudent/GetStudents/3/0/0/0/1");

            _selectedSubjectClass = string.Empty;
            sbjclasslist.Clear();
            sbjclasslist = await subjectClassificationService.GetAllAsync("AcademicsSubjects/GetSubjectsClassifications/1");

            selectedSubjects = new HashSet<string>();
            subjectlist.Clear();
            await LoadSubjects(6, 0, 0, 0, false);
        }

        async Task OnClassGroupChanged(IEnumerable<string> e)
        {
            _selectedClassGroup = e.ElementAt(0);
            _classlistid = grouplist.FirstOrDefault(s => s.SchClass == _selectedClassGroup).ClassListID;

            _classid = 0;
            _selectedClass = string.Empty;
            _classList.Clear();
            _classList = await classService.GetAllAsync("AdminSchool/GetClassList/2/0/" + _classlistid);

            selectedStudents = new HashSet<string>();
            studentlist.Clear();
            studentlist = await studentService.GetAllAsync("AdminStudent/GetStudents/3/0/0/0/1");

            _selectedSubjectClass = string.Empty;
            sbjclasslist.Clear();
            sbjclasslist = await subjectClassificationService.GetAllAsync("AcademicsSubjects/GetSubjectsClassifications/1");

            selectedSubjects = new HashSet<string>();
            subjectlist.Clear();
            await LoadSubjects(6, 0, 0, 0, false);
        }

        async Task OnClassChanged(IEnumerable<string> e)
        {
            _selectedClass = e.ElementAt(0);
            _classid = _classList.FirstOrDefault(s => s.ClassName == _selectedClass).ClassID;

            studentlist = await studentService.GetAllAsync("AdminStudent/GetStudents/3/0/" + _classid + "/0/1");

            _selectedSubjectClass = string.Empty;
            sbjclasslist.Clear();
            sbjclasslist = await subjectClassificationService.GetAllAsync("AcademicsSubjects/GetSubjectsClassifications/1");

            selectedSubjects = new HashSet<string>();
            subjectlist.Clear();
            await LoadSubjects(6, 0, 0, 0, false);
        }

        async Task OnSubjectClassChanged(IEnumerable<string> e)
        {
            _selectedSubjectClass = e.ElementAt(0);
            _sbjclassid = sbjclasslist.FirstOrDefault(s => s.SbjClassification == _selectedSubjectClass).SbjClassID;

            selectedSubjects = new HashSet<string>();
            subjectlist.Clear();
            await LoadSubjects(6, _schid, 0, _sbjclassid, true);
        }

        async Task _RefreshList()
        {
            _slectedSchool = string.Empty;
            _schid = 0;
            schoolList.Clear();
            schoolList = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            _classlistid = 0;
            _selectedClassGroup = string.Empty;
            grouplist.Clear();
            grouplist = await classGroupService.GetAllAsync("AdminSchool/GetClassGroups/1/" + _schid);

            _classid = 0;
            _selectedClass = string.Empty;
            _classList.Clear();
            _classList = await classService.GetAllAsync("AdminSchool/GetClassList/2/0/0");

            selectedStudents = new HashSet<string>();
            studentlist.Clear();
            studentlist = await studentService.GetAllAsync("AdminStudent/GetStudents/3/0/0/0/1");

            _selectedSubjectClass = string.Empty;
            sbjclasslist.Clear();
            sbjclasslist = await subjectClassificationService.GetAllAsync("AcademicsSubjects/GetSubjectsClassifications/1");

            selectedSubjects = new HashSet<string>();
            subjectlist.Clear();
            await LoadSubjects(6, 0, 0, 0, false);
        }

        bool ValidateSelections()
        {            
            if (!String.IsNullOrWhiteSpace(_slectedSchool) && !String.IsNullOrWhiteSpace(_selectedClassGroup)
                && !String.IsNullOrWhiteSpace(_selectedClass) && !String.IsNullOrWhiteSpace(_selectedStudent)
                && !String.IsNullOrWhiteSpace(_selectedSubjectClass) && !String.IsNullOrWhiteSpace(_selectedSubject))
            {
                return true;                
            }
            else
            {
                _ErrorMessage = "Cannot Allocate Subjects Because of Incomplete Selections. Please, Check Your Selections.";
                return false;
            }
        }

        async Task StartSubjectAllocation()
        {
            IsShow = false;
            i = 0;
            j = 0;
            progressbarInfo = "Please wait allocating subjects to students...";

            int maxValue = selectedStudents.Count();

            foreach (var _student in selectedStudents)
            {
                j++;
                i = ((decimal)(j) / maxValue) * 100;

                int _stdid = studentlist.FirstOrDefault(s => s.StudentNameWithNo == _student).STDID;

                foreach (var _subject in selectedSubjects)
                {
                    int _subjectID = subjectlist.FirstOrDefault(s => s.Subject == _subject).SubjectID;

                    var getSubjectByStudent = await
                        subjectAllocationStudentService.GetAllAsync("AcademicsSubjects/GetStudentAllocations/7/" +
                                        schoolSession + "/true/" + _schid + "/0/" + _classid + "/" + _subjectID + "/" + _stdid);
                    bool studentExist = getSubjectByStudent.Any();

                    if (!studentExist)
                    {
                        subjectAllocation.TermID = termid;
                        subjectAllocation.SchSession = schoolSession;
                        subjectAllocation.STDID = _stdid;
                        subjectAllocation.SbjSelection = true;
                        subjectAllocation.SubjectID = _subjectID;
                        subjectAllocation.SchID = _schid;
                        subjectAllocation.ClassID = _classid;
                        subjectAllocation.ClassListID = _classlistid;
                        var response = await subjectAllocationStudentService.SaveAsync("AcademicsSubjects/AddStudentAllocation/", subjectAllocation);
                        subjectAllocation.SbjAllocID = response.SbjAllocID;
                        subjectAllocation.Id = response.SbjAllocID;
                        await subjectAllocationStudentService.UpdateAsync("AcademicsSubjects/UpdateStudentAllocation/", 3, subjectAllocation);
                    }                   
                }

                StateHasChanged();
            }

            IsShow = true;
        }

        async Task Save()
        {
            if (ValidateSelections())
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Students Subjects Allocation  Operation",
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
                        await StartSubjectAllocation();
                        await _RefreshList();
                        await Swal.FireAsync("Subject Allocation", "Subjects Has Been Allocated To All The Students In " + _selectedClass + " Class. " +
                                                "Please, confirm your allocation below.", "success");
                    }
                }
            }
            else
            {
                await Swal.FireAsync("Subject Allocation", _ErrorMessage, "error");
            }
        }


        #endregion

        #region [Section - Click Events]
        async Task StudentsAllocationEvent()
        {
            toolBarMenuId = 1;
            disableSaveButton = true;
            await RefreshList();
        }

        async Task AllocateSubjectsToStudents()
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            await _RefreshList();
            await Task.CompletedTask;
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


