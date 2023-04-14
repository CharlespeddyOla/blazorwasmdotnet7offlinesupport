using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Client.OfflineRepo.Academics.Exam.CBT;
using WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Entry;
using WebAppAcademics.Client.OfflineRepo.Academics.Subjects;
using WebAppAcademics.Client.OfflineRepo.Admin.School;
using WebAppAcademics.Client.OfflineRepo.Admin.Student;
using WebAppAcademics.Client.OfflineRepo.Settings;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.CBT;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Academics.Subjects;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Administration.Students;
using WebAppAcademics.Shared.Models.Settings;
using static MudBlazor.CategoryTypes;

namespace WebAppAcademics.Client.Pages.Academics.Exam.Marks.Entry
{
    public partial class MarkEntryCognitive
    {
        #region [Injection Declaration]

        bool Initialized = false;
        bool IsOnline = true;

        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] INetworkStatus _networkStatusService { get; set; }
        [Inject] SchoolDBSyncRepo _schoolService { get; set; }
        [Inject] ClassListDBSyncRepo _classListService { get; set; }
        [Inject] SbjAllocTeacherDBSyncRepo _subjectAllocationService { get; set; }
        [Inject] SbjAllocStudentDBSyncRepo _subjectAllocationStudentService { get; set; }
        [Inject] SessionsDBSyncRepo _sessionsService { get; set; }
        [Inject] ResultTypeDBSyncRepo _resultTypeService { get; set; }
        [Inject] MarkSettingsDBSyncRepo _markSettingsService { get; set; }
        [Inject] CBTExamsDBSyncRepo _cbtExamService { get; set; }
        [Inject] StudentDBSyncRepo _studentService { get; set; }
        [Inject] CognitiveDBSyncRepo _cognitiveMarksService { get; set; }
        [Inject] SubjectsDBSyncRepo _subjectService { get; set; }
        [Inject] CBTScoresDBSyncRepo _cbtScoresService { get; set; }

        [Inject] IAPIServices<ACDStudentsMarksCognitive> studentMarksCognitiveService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int termid { get; set; }
        int maxTermID { get; set; }
        int schoolSession { get; set; }
        int roleid { get; set; }
        int staffid { get; set; }
        int schid { get; set; }
        string schTerm { get; set; }
        int classid { get; set; }
        int classlistid { get; set; }
        int classTeacherID { get; set; }
        int schoolPrincipalID { get; set; }
        int subjectid { get; set; }
        int reportTypeID { get; set; }

        string selectedSchool { get; set; }
        string selectedClass { get; set; }
        string selectedSubject { get; set; }
        string headerCBTColumn { get; set; }
        string selectedExamTitle { get; set; }

        bool DisableMidTermColumn { get; set; }
        bool DisableExamColumns { get; set; }
        bool DisableCA1Column { get; set; }
        bool DisableCA2Column { get; set; }
        bool DisableCA3Column { get; set; }
        bool DisableCA4Column { get; set; }

        string academicSession { get; set; }

        decimal i { get; set; } = 0;
        int j { get; set; } = 0;
        bool IsShow { get; set; } = true;
        string progressbarInfo { get; set; } = string.Empty;
        string offlineprohressbarinfo { get; set; } = string.Empty;
        bool MarkEntryValidationResult { get; set; } = false;
        #endregion

        #region [Models Declaration]
        List<SETSchSessions> sessions = new();
        List<ADMSchlList> schools = new();
        List<ADMSchClassList> classListAll = new();
        List<ADMSchClassList> classList = new();
        List<ADMStudents> studentsAll = new();
        List<ADMStudents> students = new();
        List<ACDSubjects> subjects = new();
        List<ACDReportType> resultTypeList = new();
        List<ACDSettingsMarks> markSettingsList = new();
        List<ACDSbjAllocationTeachers> teacherSubjectAllocation = new();
        List<ACDSbjAllocationTeachers> SelectedTeachersSubjects = new();
        List<ACDSbjAllocationStudents> studentSubjectsAllocation = new();
        List<ACDStudentsMarksCognitive> cognitiveMarksAll = new();
        List<ACDStudentsMarksCognitive> cognitiveMarks = new();
        List<CBTExams> exams = new();
        List<CBTStudentScores> studentCBTScores = new();

        List<string> noSubjectAllocationStudents = new();
        List<string> marksErrorLisitng = new();

        ACDSettingsOthers otherSetting = new();
        ACDStudentsMarksCognitive cognitiveMark = new();
        ACDStudentsMarksCognitive selectedItemCognitiveMark = null;

        #endregion

        protected async void OnlineStatusChanged(object sender, OnlineStatusEventArgs args)
        {            
            IsOnline = args.IsOnline;
            if (args.IsOnline == false)
            {
                // reload from IndexedDB
                schools = (await _schoolService.GetAllOfflineAsync()).ToList();
                if (schools.Count> 0) 
                {
                    await OfflineListProcessing();
                }
                else
                {
                    await Swal.FireAsync("No Offline Data", 
                        "No Mark Entry Data. Please, go Online To Generate Mark Entry Templates.", "error");
                }
            }
            else
            {
                if (Initialized)
                    // reload from API
                    await Task.CompletedTask;
                else
                    Initialized = true;                    
            }
            await InvokeAsync(StateHasChanged);
        }

        protected override async Task OnInitializedAsync()
        {
            _networkStatusService.OnlineStatusChanged += OnlineStatusChanged;
            Layout.currentPage = "Student Cognitive Mark Entry";
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            schTerm = await localStorageService.ReadEncryptedItemAsync<string>("schTerm");
            schoolSession = await localStorageService.ReadEncryptedItemAsync<int>("schoolsession");
            academicSession = await localStorageService.ReadEncryptedItemAsync<string>("academicsession");
            roleid = await localStorageService.ReadEncryptedItemAsync<int>("roleid");
            staffid = await localStorageService.ReadEncryptedItemAsync<int>("staffid");
            
            await base.OnInitializedAsync();
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            // execute conditionally for loading data, otherwise this will load
            // every time the page refreshes
            if (firstRender)
            {
                // Do work to load page data and set properties
                if (IsOnline)
                {
                    await ProcessList();
                }
            }
        }

        #region [Section - Database Sync Operation]

        async Task LoadOffLineList()
        {
            offlineprohressbarinfo = string.Empty;
            _processing = true;
            offlineprohressbarinfo = "Please wait loading offline data...";
            sessions = (await _sessionsService.GetAllOfflineAsync()).ToList();
            resultTypeList = (await _resultTypeService.GetAllOfflineAsync()).ToList();
            markSettingsList = (await _markSettingsService.GetAllOfflineAsync()).ToList();
            classListAll = (await _classListService.GetAllOfflineAsync()).ToList();
            studentsAll = (await _studentService.GetAllOfflineAsync()).ToList();
            exams = (await _cbtExamService.GetAllOfflineAsync()).ToList();
            teacherSubjectAllocation = (await _subjectAllocationService.GetAllOfflineAsync()).ToList();
            studentSubjectsAllocation = (await _subjectAllocationStudentService.GetAllOfflineAsync()).ToList();
            await LoadDefaultList();
            _processing = false;

            await Swal.FireAsync("Offline Data", "Offline Data Succesfully Loaded.", "success");
        }

        async Task OfflineListProcessing()
        {
            stopwatchvalue = new TimeSpan();
            Task task1 = Task.Run(() => LoadOffLineList());
            Task task2 = Task.Run(() => StartStopWatch());

            await Task.WhenAll(task1, task2);

            task1.Dispose();
            task2.Dispose();

            await Task.CompletedTask;            
        }

        async Task LoadSchools()
        {
            if (IsOnline)
            {
                var list = await _schoolService.GetAllAsync("AdminSchool/GetSchools/0");
                if (list != null)
                {
                    schools = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task LoadClassList()
        {
            if (IsOnline)
            {
                var list = await _classListService.GetAllAsync("AdminSchool/GetClassList/0/0/0");
                if (list != null)
                {
                    classListAll = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task LoadSessions()
        {
            if (IsOnline)
            {
                var list = await _sessionsService.GetAllAsync("Settings/GetAccademicSessions/1/0");
                if (list != null)
                {
                    sessions = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task LoadResultType()
        {
            if (IsOnline)
            {
                var list = await _resultTypeService.GetAllAsync("AcademicsMarkSettings/GetResultTypeSettings/1");
                if (list != null)
                {
                    resultTypeList = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }
               
        async Task LoadMarkSettings()
        {
            if (IsOnline)
            {
                var list = await _markSettingsService.GetAllAsync("AcademicsMarkSettings/GetMarkSettings/1");
                if (list != null)
                {
                    markSettingsList = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task LoadCBTExams()
        {
            if (IsOnline)
            {
                var list = await _cbtExamService.GetAllAsync("AcademicsCBT/GetCBTExams/1/" + termid + "/0/0");
                if (list != null)
                {
                    exams = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }
               
        async Task LoadAllSubjects()
        {
            if (IsOnline)
            {
                var list = await _subjectService.GetAllAsync("AcademicsSubjects/GetSubjects/9/0/0/0/true");
                if (list != null)
                {
                    subjects = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task LoadAllStudents()
        {
            if (IsOnline)
            {
                var list = await _studentService.GetAllAsync("AdminStudent/GetStudents/9/0/0/0/0");
                if (list != null)
                {
                    studentsAll = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task LoadSubjectsAllocated()
        {
            if (IsOnline)
            {
                if (roleid == 1) //Administrator
                {
                    teacherSubjectAllocation = await _subjectAllocationService.GetAllAsync(
                        "AcademicsSubjects/GetTeacherAllocations/14/true/" + termid + "/0/0/0/0/0");
                }
                else
                {
                    teacherSubjectAllocation = await _subjectAllocationService.GetAllAsync(
                        "AcademicsSubjects/GetTeacherAllocations/15/true/" + termid + "/0/0/0/0/" + staffid);
                }
            }
        }

        async Task LoadStudentsSubjectAllocated()
        {
            if (IsOnline)
            {
                var list = await _subjectAllocationStudentService.GetAllAsync("AcademicsSubjects/GetStudentAllocations/11/" +
                                                                                schoolSession + "/true/0/0/0/0/0");
                if (list != null)
                {
                    studentSubjectsAllocation = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }
        #endregion

        #region [Load Events]
        async Task LoadDefaultList()
        {
            maxTermID = sessions.FirstOrDefault(s => s.TermID == sessions.Max(t => t.TermID)).TermID;
            reportTypeID = resultTypeList.FirstOrDefault(r => r.SelectedExam == true).ReportTypeID;
            UseCBTColumnTitle();
            SelectedExam();
            SetRequestURL();
            await Task.CompletedTask;
        }

        async Task LoadCognitiveMarks()
        {
            if (IsOnline)
            {
                _processing = true;
                offlineprohressbarinfo = "Please wait generating mark entry templates(s) for offline use...";
                if (roleid == 1) //Administrator
                {
                    cognitiveMarksAll = await _cognitiveMarksService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/0/" + 
                                                                                    termid + "/0/0/0/0/0");
                }
                else
                {
                    cognitiveMarksAll = await _cognitiveMarksService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/23/" +
                                                                                    termid + "/0/0/0/" + staffid + "/0");
                }
                _processing = false;
            }
        }

        async Task LoadSubjects()
        {
            if (roleid == 1)
            {
                foreach (var item in teacherSubjectAllocation.Where(s => s.SbjClassID == 1 && s.SchID == schid && 
                                                                    s.ClassID == classid).ToList())
                {
                    SelectedTeachersSubjects.Add(new ACDSbjAllocationTeachers
                    {
                        SubjectID = item.SubjectID,
                        Subject = item.Subject + " - " + item.SubjectTeacher,
                        StaffID = item.StaffID,
                    });
                }
            }
            else
            {
                foreach (var item in teacherSubjectAllocation.Where(s => s.SbjClassID == 1 && s.SchID == schid && 
                                                                    s.ClassID == classid && s.StaffID == staffid).ToList())
                {
                    SelectedTeachersSubjects.Add(new ACDSbjAllocationTeachers
                    {
                        SubjectID = item.SubjectID,
                        Subject = item.Subject + " - " + item.SubjectTeacher,
                        StaffID = item.StaffID,
                    });
                }
            }

            await Task.CompletedTask;
        }

        async Task LoadStudents(int id)
        {
            if (maxTermID == termid)
            {
                students = studentsAll.Where(s => s.StatusTypeID == 1 && s.ClassID == id).ToList();
            }
            else
            {
               var distinctStudents = cognitiveMarksAll
                                    .GroupBy(x => x.STDID)
                                    .Select(x => x.First())
                                    .ToList();

                if (distinctStudents.Count == 0)
                {
                    students = studentsAll.Where(s => s.StatusTypeID == 1 && s.ClassID == id).ToList();
                }

                foreach (var item in distinctStudents.Where(s => s.ClassID == id).ToList())
                {
                    students.Add(new ADMStudents()
                    {
                        STDID = item.STDID
                    });
                }
            }

            await Task.CompletedTask;
        }

        async Task LoadStudentScores(int examid)
        {
            if (IsOnline)
            {
                var list = await _cbtScoresService.GetAllAsync("AcademicsCBT/GetCBTStudentScores/1/" + examid + "/0/" + true);
                if (list != null)
                {
                    studentCBTScores = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task OnSelectedSchoolChanged(IEnumerable<string> e)
        {
            selectedSchool = e.ElementAt(0);
            schid = schools.FirstOrDefault(s => s.School == selectedSchool).SchID;
            schoolPrincipalID = schools.FirstOrDefault(s => s.School == selectedSchool).StaffID;

            classid = 0;
            classTeacherID = 0;
            selectedClass = string.Empty;
            classList.Clear();
            classList = classListAll.Where(c => c.SchID == schid).ToList();

            subjectid = 0;
            selectedSubject = string.Empty;
            SelectedTeachersSubjects.Clear();

            students.Clear();
            cognitiveMarks.Clear();

            await Task.CompletedTask;
        }

        async Task OnSelectedClassChanged(IEnumerable<string> e)
        {
            selectedClass = e.ElementAt(0);
            classid = classList.FirstOrDefault(c => c.ClassName == selectedClass).ClassID;
            classlistid = classList.FirstOrDefault(c => c.ClassName == selectedClass).ClassListID;
            classTeacherID = classList.FirstOrDefault(c => c.ClassName == selectedClass).StaffID;

            subjectid = 0;
            selectedSubject = string.Empty;
            SelectedTeachersSubjects.Clear();
            await LoadSubjects();

            students.Clear();
            await LoadStudents(classid);
            cognitiveMarks.Clear();

            await Task.CompletedTask;
        }

        async Task OnSelectedSubjectChanged(IEnumerable<string> e)
        {
            selectedSubject = e.ElementAt(0);
            subjectid = SelectedTeachersSubjects.FirstOrDefault(sbj => sbj.Subject == selectedSubject).SubjectID;
            staffid = SelectedTeachersSubjects.FirstOrDefault(sbj => sbj.Subject == selectedSubject).StaffID;

            if (await ValidateSelections())
            {
                string studentNames = string.Join("\n", noSubjectAllocationStudents.ToArray());

                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Subject Not Allocated To The Following Students:",
                    Width = "500",
                    Icon = "info",
                    Html = "<pre class='format-pre' style='color: white;'>" + studentNames + "</pre>"
                });
            }

            cognitiveMarks.Clear();
            await LoadStudentsMarks();
        }

        void UseCBTColumnTitle()
        {
            if (otherSetting.BoolValue)
            {
                headerCBTColumn = "CA 4";
            }
            else
            {
                headerCBTColumn = "CBT";
            }
        }

        void SelectedExam()
        {
            switch (reportTypeID)
            {
                case 1: //Mid-Term Exam
                    selectedExamTitle = "Mid-Term Exam for " + schTerm + " Term";
                    break;
                case 2: //End of Term Exam
                    selectedExamTitle = "End of Term Exam for " + schTerm + " Term";
                    break;
            }

            DisableMidTermColumn = markSettingsList.FirstOrDefault(m => m.MarkID == 1).ApplyPassMark;
            DisableCA1Column = markSettingsList.FirstOrDefault(m => m.MarkID == 2).ApplyPassMark;
            DisableCA2Column = markSettingsList.FirstOrDefault(m => m.MarkID == 3).ApplyPassMark;
            DisableCA3Column = markSettingsList.FirstOrDefault(m => m.MarkID == 4).ApplyPassMark;
            DisableCA4Column = markSettingsList.FirstOrDefault(m => m.MarkID == 5).ApplyPassMark;
            DisableExamColumns = markSettingsList.FirstOrDefault(m => m.MarkID == 6).ApplyPassMark;
        }

        #endregion

        #region [Section - Marks Processing Operations]
        async Task<bool> ValidateSelections()
        {
            noSubjectAllocationStudents.Clear();

            if (maxTermID == termid)
            {
                foreach (var _student in students)
                {
                    if (!studentSubjectsAllocation.Any(s => s.STDID == _student.STDID && s.SubjectID == subjectid))
                    {

                        noSubjectAllocationStudents.Add(studentsAll.SingleOrDefault(s => s.STDID == _student.STDID).StudentName);
                    }
                }

                if (noSubjectAllocationStudents.Count() > 0)
                {
                    return true;
                }
            }
            
            await Task.CompletedTask;

            return false;
        }

        async Task LoadStudentsMarks()
        {
            if (IsOnline)
            {
                //var _studentMarks = cognitiveMarksAll.Where(m => m.SchID == schid && m.ClassID == classid && m.SubjectID == subjectid);
                var _studentMarks = await studentMarksCognitiveService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/7/" +
                                       termid + "/" + schid + "/" + classid + "/0/0/" + subjectid);

                IsShow = false;
                i = 0;
                j = 0;
                
                progressbarInfo = "Please wait loading student marks...";

                int maxValue = students.Count();

                foreach (var _student in students)
                {
                    j++;
                    i = ((decimal)(j) / maxValue) * 100;

                    if (studentSubjectsAllocation.Any(s => s.STDID == _student.STDID && s.SubjectID == subjectid))
                    {
                        bool StudentMarkExist = _studentMarks.Where(m => m.STDID == _student.STDID).Any();

                        if (!StudentMarkExist)
                        {
                            cognitiveMark.TermID = termid;
                            cognitiveMark.SchSession = schoolSession;
                            cognitiveMark.SchID = schid;
                            cognitiveMark.ClassID = classid;
                            cognitiveMark.StaffID = staffid;
                            cognitiveMark.ClassTeacherID = classTeacherID;
                            cognitiveMark.SchoolPrincipalID = schoolPrincipalID;
                            cognitiveMark.STDID = _student.STDID;
                            cognitiveMark.SubjectID = subjectid;
                            cognitiveMark.SbjSelection = true;
                            cognitiveMark.Mark_ICGC = 0;
                            cognitiveMark.Mark_MidCBT = 0;
                            cognitiveMark.Mark_Mid = 0;
                            cognitiveMark.Mark_CA1 = 0;
                            cognitiveMark.Mark_CA2 = 0;
                            cognitiveMark.Mark_CA3 = 0;
                            cognitiveMark.Mark_CBT = 0;
                            cognitiveMark.Mark_Exam = 0;
                            var response = await studentMarksCognitiveService.SaveAsync("AcademicsMarks/AddCognitiveMark/", cognitiveMark);
                            cognitiveMark.StudentMarkID = response.StudentMarkID;
                            cognitiveMark.Id = response.StudentMarkID;
                            await studentMarksCognitiveService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 13, cognitiveMark);
                        }
                        else
                        {
                            int ggg = _student.STDID;
                        }
                    }

                    StateHasChanged();
                }

                cognitiveMarks.Clear();
                cognitiveMarks = await studentMarksCognitiveService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/5/" +
                                        termid + "/" + schid + "/" + classid + "/0/" + staffid + "/" + subjectid);
            } 
            else
            {
                cognitiveMarks.Clear();
                int sn = 1;

                cognitiveMarksAll = (await _cognitiveMarksService.GetAllOfflineAsync()).ToList();
                var _markList = cognitiveMarksAll.Where(m => m.SchID == schid && m.ClassID == classid && m.SubjectID == subjectid).ToList();

                IsShow = false;
                i = 0;
                j = 0;

                progressbarInfo = "Please wait loading student marks...";

                int maxValue = _markList.Count();

                foreach (var item in _markList)
                {
                    j++;
                    i = ((decimal)(j) / maxValue) * 100;

                    cognitiveMarks.Add(new ACDStudentsMarksCognitive
                    {
                        StudentMarkID = item.StudentMarkID,
                        Id = item.Id,
                        SchID = item.SchID,
                        ClassID = item.ClassID,
                        SubjectID = item.SubjectID,
                        STDID = item.STDID,
                        EntryStatus_ICGCS = item.EntryStatus_ICGCS,
                        EntryStatus_MidTerm = item.EntryStatus_MidTerm,
                        EntryStatus_TermEnd = item.EntryStatus_TermEnd,
                        TermID = item.TermID,
                        SchSession = item.SchSession,
                        StaffID = item.StaffID,
                        ClassTeacherID = item.ClassTeacherID,
                        SchoolPrincipalID = item.SchoolPrincipalID,
                        SbjSelection = item.SbjSelection,
                        SN = sn++,
                        AdmissionNo = studentsAll.Single(s => s.STDID == item.STDID).AdmissionNo,
                        StudentName = studentsAll.Single(s => s.STDID == item.STDID).StudentName,
                        Mark_ICGC = item.Mark_ICGC,
                        Mark_MidCBT = item.Mark_MidCBT,
                        Mark_Mid = item.Mark_Mid,
                        Mark_CA1 = item.Mark_CA1,
                        Mark_CA2 = item.Mark_CA2,
                        Mark_CA3 = item.Mark_CA3,
                        Mark_CBT = item.Mark_CBT,
                        Mark_Exam = item.Mark_Exam,
                        Total_Exam = item.Total_Exam,                        
                    });

                    StateHasChanged();
                }
            }

            IsShow = true;
        }

        async Task RetrieveMarks()
        {
            if (cognitiveMarks.Count() > 0)
            {
                if (reportTypeID == 1)
                {
                    //Mid-Term CBT
                    await RetrieveMidTermCBT();
                }
                else if (reportTypeID == 2)
                {
                    //End of Term CBT
                    await RetrieveTermEndCBT();
                }
            }
        }

        async Task ConvertMarks()
        {
            if (cognitiveMarks.Count() > 0)
            {
                if (reportTypeID == 1)
                {
                    //Mid-Term CBT
                    await ConvertMidTermCBT();
                }
                else if (reportTypeID == 2)
                {
                    //End of Term CBT
                    await ConvertTermEndCBT();
                }
            }
        }

        async Task UpdateCognitiveMarks()
        {
            if (reportTypeID == 1)
            {
                await UpdateMidTermMarks();
            }
            else if (reportTypeID == 2)
            {
                await UpdateTermEndMarks();
            }
        }

        async Task SaveCognitiveMarks()
        {
            if (cognitiveMarks.Count() > 0)
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Save " + selectedExamTitle + " Students Marks",
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
                        if (reportTypeID == 1)
                        {
                            await SaveMidTermMarks();
                        }
                        else if (reportTypeID == 2)
                        {
                            await SaveTermEndMarks();
                        }

                        await Swal.FireAsync("Save Mark Entry", "Operation Completed Successfully.", "success");
                    }
                }
            }
            else
            {
                await Swal.FireAsync("Cannot Save", "Mark Entry List Is Empty", "error");
            }
        }

        async Task RefreshCogvitive()
        {
            schid = 0;
            schoolPrincipalID = 0;
            selectedSchool = string.Empty;
            schools.Clear();
            await LoadSchools();

            classid = 0;
            classTeacherID = 0;
            selectedClass = string.Empty;
            classList.Clear();
            await LoadClassList();
            
            if (!IsOnline)
            {
                schools = (await _schoolService.GetAllOfflineAsync()).ToList();
                classListAll = (await _classListService.GetAllOfflineAsync()).ToList();
            }
            
            subjectid = 0;
            selectedSubject = string.Empty;
            SelectedTeachersSubjects.Clear();

            students.Clear();
            cognitiveMarks.Clear();
        }

        #endregion

        #region [Section - Mid-Term Exam]
        async Task RetrieveMidTermCBT()
        {
            if (reportTypeID == 1)
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Retrieve Mid-Term CBT",
                    Text = "Do You Want To Continue?",
                    Icon = SweetAlertIcon.Warning,
                    ShowCancelButton = true,
                    ConfirmButtonText = "Yes, Contnue!",
                    CancelButtonText = "No"
                });

                if (result.IsConfirmed)
                {
                    var _examdetails = exams.FirstOrDefault(ex => ex.ReportTypeID == reportTypeID && ex.ClassListID == classlistid && ex.SubjectID == subjectid);

                    if (_examdetails != null)
                    {
                        int _examID = _examdetails.ExamID;
                        await LoadStudentScores(_examID);

                        foreach (var item in cognitiveMarks)
                        {
                            //int id = item.STDID;
                            //int markid = item.StudentMarkID;
                            bool studentCBTScoreExist = studentCBTScores.Where(s => s.STDID == item.STDID).Any();

                            if (studentCBTScoreExist)
                            {
                                double score = studentCBTScores.FirstOrDefault(s => s.STDID == item.STDID).ScorePercentage;
                                cognitiveMarks.FirstOrDefault(s => s.StudentMarkID == item.StudentMarkID).Mark_MidCBT = Convert.ToDecimal(score);
                            }

                        }
                    }
                }
            }
            else
            {
                await Swal.FireAsync("Mid-Term Exam CBT", "Currently Set Exam Is End of Term.", "info");
            }
        }

        async Task ConvertMidTermCBT()
        {
            if (reportTypeID == 1)
            {
                decimal _conversionFactor = markSettingsList.FirstOrDefault(m => m.MarkID == 8).PassMark;

                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Mid-Term CBT Conversion",
                    Text = "You Are About To Convert Mid-Term CBT Marks Using The Conversion Factor " + _conversionFactor +
                    ". Do You Want To Continue?",
                    Icon = SweetAlertIcon.Warning,
                    ShowCancelButton = true,
                    ConfirmButtonText = "Yes, Contnue!",
                    CancelButtonText = "No"
                });

                if (result.IsConfirmed)
                {
                    var _examdetails = exams.FirstOrDefault(ex => ex.ReportTypeID == reportTypeID && ex.ClassListID == classlistid && ex.SubjectID == subjectid);

                    if (_examdetails != null)
                    {
                        foreach (var item in cognitiveMarks)
                        {
                            cognitiveMarks.FirstOrDefault(s => s.StudentMarkID == item.StudentMarkID).Mark_MidCBT = (_conversionFactor * item.Mark_MidCBT) / 100;
                        }
                    }
                }
            }
            else
            {
                await Swal.FireAsync("Mid-Term Exam CBT", "Currently Set Exam Is End of Term.", "info");
            }
        }

        async Task ConvertMidTermExanMarks()
        {
            if (cognitiveMarks.Count() > 0)
            {
                if (reportTypeID == 2)
                {
                    decimal _conversionFactor = markSettingsList.FirstOrDefault(m => m.MarkID == 2).PassMark;

                    SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                    {
                        Title = "Mid-Term Exam Mark Conversion",
                        Text = "You Are About To Convert Mid-Term Exam Marks To CA 1 Using The Conversion Factor " + _conversionFactor +
                        ". Do You Want To Continue?",
                        Icon = SweetAlertIcon.Warning,
                        ShowCancelButton = true,
                        ConfirmButtonText = "Yes, Contnue!",
                        CancelButtonText = "No"
                    });

                    if (result.IsConfirmed)
                    {
                        foreach (var item in cognitiveMarks)
                        {
                            cognitiveMarks.FirstOrDefault(s => s.StudentMarkID == item.StudentMarkID).Mark_CA1 = (_conversionFactor * (item.Mark_MidCBT + item.Mark_Mid)) / 100;
                        }
                    }
                }
            }
        }

        int GetMidTermUseMarkTypeID()
        {
            bool _exam = markSettingsList.FirstOrDefault(m => m.MarkID == 1).ApplyCBT;
            bool _ca = markSettingsList.FirstOrDefault(m => m.MarkID == 8).ApplyCBT;

            if (_exam == true && _ca == false)
            {
                //Use ONLY Mid-Term Exam Mark For Results
                return 1;
            }
            else if (_exam == false && _ca == true)
            {
                //Use ONLY Mid-Term CA Mark For Results
                return 2;
            }
            else if (_exam == true && _ca == true)
            {
                //Use Both Mid-Term CA Mark And Mid-Term CBT mark For Results
                return 3;
            }

            return 0;
        }

        bool MidTermUseMarkType(decimal _ca, decimal _exam)
        {
            switch (GetMidTermUseMarkTypeID())
            {
                case 1: ////Use ONLY Mid-Term Exam Mark For Results
                    if (_exam > 0)
                    {
                        return true;
                    }
                    break;
                case 2://Use ONLY Mid-Term CA Mark For Results
                    if (_ca > 0)
                    {
                        return true;
                    }
                    break;
                case 3: //se Both Mid-Term CA Mark And Mid-Term CBT mark For Results
                    if (_exam > 0 && _ca > 0)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        async Task UpdateMidTermMarks()
        {
            MarkEntryValidationResult = false;

            if (selectedItemCognitiveMark.Mark_MidCBT > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 8).Mark))
            {
                await Swal.FireAsync("Invalid Mid-Term CA Mark", selectedItemCognitiveMark.SN + ". " +
                                        selectedItemCognitiveMark.StudentName + ": " + selectedItemCognitiveMark.Mark_MidCBT + " > " +
                                        markSettingsList.FirstOrDefault(m => m.MarkID == 8).Mark + ".", "error");
                MarkEntryValidationResult = true;
            }
            else if (selectedItemCognitiveMark.Mark_Mid > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 1).Mark))
            {
                await Swal.FireAsync("Invalid Mid-Term Exam Mark", selectedItemCognitiveMark.SN + ". " +
                                        selectedItemCognitiveMark.StudentName + ": " + selectedItemCognitiveMark.Mark_Mid + " > " +
                                        markSettingsList.FirstOrDefault(m => m.MarkID == 1).Mark + ".", "error");
                MarkEntryValidationResult = true;
            }

            if (!MarkEntryValidationResult)
            {
                bool _entryStatus = MidTermUseMarkType(selectedItemCognitiveMark.Mark_MidCBT, selectedItemCognitiveMark.Mark_Mid);

                cognitiveMark.StudentMarkID = selectedItemCognitiveMark.StudentMarkID;
                cognitiveMark.Id = selectedItemCognitiveMark.Id;
                cognitiveMark.TermID = selectedItemCognitiveMark.TermID;
                cognitiveMark.StaffID = selectedItemCognitiveMark.StaffID;
                cognitiveMark.ClassTeacherID = selectedItemCognitiveMark.ClassTeacherID;
                cognitiveMark.SbjSelection = selectedItemCognitiveMark.SbjSelection;    
                cognitiveMark.SchID = selectedItemCognitiveMark.SchID;
                cognitiveMark.ClassID = selectedItemCognitiveMark.ClassID;
                cognitiveMark.SubjectID = selectedItemCognitiveMark.SubjectID;
                cognitiveMark.STDID = selectedItemCognitiveMark.STDID;
                cognitiveMark.Mark_ICGC = selectedItemCognitiveMark.Mark_ICGC;
                cognitiveMark.Mark_MidCBT = selectedItemCognitiveMark.Mark_MidCBT;
                cognitiveMark.Mark_Mid = selectedItemCognitiveMark.Mark_Mid;
                cognitiveMark.Mark_CA1 = selectedItemCognitiveMark.Mark_CA1;
                cognitiveMark.Mark_CA2 = selectedItemCognitiveMark.Mark_CA2;
                cognitiveMark.Mark_CA3 = selectedItemCognitiveMark.Mark_CA3;
                cognitiveMark.Mark_CBT = selectedItemCognitiveMark.Mark_CBT;
                cognitiveMark.Mark_Exam = selectedItemCognitiveMark.Mark_Exam;
                cognitiveMark.EntryStatus_ICGCS = selectedItemCognitiveMark.EntryStatus_ICGCS;
                cognitiveMark.EntryStatus_MidTerm = _entryStatus;
                cognitiveMark.EntryStatus_TermEnd = selectedItemCognitiveMark.EntryStatus_TermEnd;
                                
                await _cognitiveMarksService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 1, cognitiveMark);
            }
            else
            {
                await Swal.FireAsync("Mid-Term Exam Marks Error",
                    "Please, Check Your Entry. You have Entered A Mark Greater Than The Maximum Mark Set.", "error");
            }
        }

        async Task SaveMidTermMarks()
        {
            marksErrorLisitng.Clear();
            int k1 = 0;
            int k2 = 0;
            int k = 0;

            IsShow = false;
            i = 0;
            j = 0;
            progressbarInfo = "Please wait saving student marks...";

            int maxValue = cognitiveMarks.Count();

            foreach (var mark in cognitiveMarks)
            {
                j++;
                i = ((decimal)(j) / maxValue) * 100;

                if (mark.Mark_MidCBT > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 8).Mark))
                {
                    k1++;
                }

                if (mark.Mark_Mid > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 1).Mark))
                {
                    k2++;
                }

                if (k1 > 0 || k2 > 0)
                {
                    k++;
                    k1 = 0;
                    k2 = 0;
                    marksErrorLisitng.Add(mark.SN + ". " + mark.StudentName);
                }
                else
                {
                    bool _entryStatus = MidTermUseMarkType(mark.Mark_MidCBT, mark.Mark_Mid);

                    cognitiveMark.StudentMarkID = mark.StudentMarkID;
                    cognitiveMark.Id = mark.Id;
                    cognitiveMark.TermID = mark.TermID;
                    cognitiveMark.StaffID = mark.StaffID;
                    cognitiveMark.ClassTeacherID = mark.ClassTeacherID;
                    cognitiveMark.SbjSelection = mark.SbjSelection;
                    cognitiveMark.SchID = mark.SchID;
                    cognitiveMark.ClassID = mark.ClassID;
                    cognitiveMark.SubjectID = mark.SubjectID;
                    cognitiveMark.STDID = mark.STDID;
                    cognitiveMark.Mark_ICGC = mark.Mark_ICGC;
                    cognitiveMark.Mark_MidCBT = mark.Mark_MidCBT;
                    cognitiveMark.Mark_Mid = mark.Mark_Mid;
                    cognitiveMark.Mark_CA1 = mark.Mark_CA1;
                    cognitiveMark.Mark_CA2 = mark.Mark_CA2;
                    cognitiveMark.Mark_CA3 = mark.Mark_CA3;
                    cognitiveMark.Mark_CBT = mark.Mark_CBT;
                    cognitiveMark.Mark_Exam = mark.Mark_Exam;
                    cognitiveMark.EntryStatus_ICGCS = mark.EntryStatus_ICGCS;
                    cognitiveMark.EntryStatus_MidTerm = _entryStatus;
                    cognitiveMark.EntryStatus_TermEnd= mark.EntryStatus_TermEnd;
                    await _cognitiveMarksService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 1, cognitiveMark);
                }

                StateHasChanged();
            }

            IsShow = true;

            if (k != 0)
            {
                string studentNames = string.Join("\n", marksErrorLisitng.ToArray());

                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "The Following Mid_Term Student Marks Were Not Saved Because Of Invalid Mark Entry:",
                    Width = "500",
                    Icon = "info",
                    Html = "<pre class='format-pre' style='color: white;'>" + studentNames + "</pre>"
                });
            }
            else
            {
                //await Swal.FireAsync("Mid-Term Exam. Operation Completed Successfully", "Student Marks Succesfully Saved.", "success");
            }
        }

        #endregion

        #region [Section - Term End Exam]
        async Task RetrieveTermEndCBT()
        {
            if (reportTypeID == 2)
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Retrieve End of Term CBT",
                    Text = "Click Yes To Use CBT For CA Marks Or No To Use CBT For Exam.",
                    Icon = SweetAlertIcon.Warning,
                    ShowCancelButton = true,
                    ConfirmButtonText = "Yes, Use CBT For CA!",
                    CancelButtonText = "No, Use CBT For Exam!"
                });

                if (result.IsConfirmed)
                {
                    var _examdetails = exams.FirstOrDefault(ex => ex.ReportTypeID == reportTypeID && ex.ClassListID == classlistid && ex.SubjectID == subjectid);

                    if (_examdetails != null)
                    {
                        int _examID = _examdetails.ExamID;
                        await LoadStudentScores(_examID);
                        
                        foreach (var item in cognitiveMarks)
                        {
                            bool studentCBTScoreExist = studentCBTScores.Where(s => s.STDID == item.STDID).Any();

                            if (studentCBTScoreExist)
                            {
                                double score = studentCBTScores.FirstOrDefault(s => s.STDID == item.STDID).ScorePercentage;
                                cognitiveMarks.FirstOrDefault(s => s.StudentMarkID == item.StudentMarkID).Mark_CBT = Convert.ToDecimal(score);
                            }
                        }
                    }
                }
                else
                {
                    var _examdetails = exams.FirstOrDefault(ex => ex.ReportTypeID == reportTypeID && ex.ClassListID == classlistid && ex.SubjectID == subjectid);

                    if (_examdetails != null)
                    {
                        int _examID = _examdetails.ExamID;
                        await LoadStudentScores(_examID);

                        foreach (var item in cognitiveMarks)
                        {
                            bool studentCBTScoreExist = studentCBTScores.Where(s => s.STDID == item.STDID).Any();

                            if (studentCBTScoreExist)
                            {
                                double score = studentCBTScores.FirstOrDefault(s => s.STDID == item.STDID).ScorePercentage;
                                cognitiveMarks.FirstOrDefault(s => s.StudentMarkID == item.StudentMarkID).Mark_Exam = Convert.ToDecimal(score);
                            }
                        }
                    }
                }
            }
            else
            {
                await Swal.FireAsync("End of Term Exam CBT", "Currently Set Exam IsEnd of Term Exam.", "info");
            }
        }

        async Task ConvertTermEndCBT()
        {
            if (reportTypeID == 2)
            {
                decimal _conversionFactor = markSettingsList.FirstOrDefault(m => m.MarkID == 5).PassMark;

                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "End of Term CBT Conversion",
                    Text = "You Are About To Convert End of Term CBT Marks Using The Conversion Factor " + _conversionFactor +
                    ". Do You Want To Continue?",
                    Icon = SweetAlertIcon.Warning,
                    ShowCancelButton = true,
                    ConfirmButtonText = "Yes, Contnue!",
                    CancelButtonText = "No"
                });

                if (result.IsConfirmed)
                {
                    var _examdetails = exams.FirstOrDefault(ex => ex.ReportTypeID == reportTypeID && ex.ClassListID == classlistid && ex.SubjectID == subjectid);

                    if (_examdetails != null)
                    {
                        foreach (var item in cognitiveMarks)
                        {
                            cognitiveMarks.FirstOrDefault(s => s.StudentMarkID == item.StudentMarkID).Mark_CBT = (_conversionFactor * item.Mark_CBT) / 100;
                        }
                    }
                }
            }
            else
            {
                await Swal.FireAsync("End of Term Exam CBT", "Currently Set Exam Is End of Term Exam.", "info");
            }
        }

        int GetTermEndUseMarkTypeID()
        {
            bool _exam = markSettingsList.FirstOrDefault(m => m.MarkID == 6).ApplyCBT;

            if (_exam == true)
            {
                //Use CA And End of Exam Mark For Results
                return 1;
            }
            else if (_exam == false)
            {
                //Use ONLY End of Term CA Mark For Results
                return 2;
            }

            return 0;
        }

        bool TermEndUseMarkType(decimal _ca, decimal _exam)
        {
            switch (GetTermEndUseMarkTypeID())
            {
                case 1: //Use CA And End of Exam Mark For Results
                    if (_ca + _exam > 0)
                    {
                        return true;
                    }
                    break;
                case 2://Use ONLY End of Term CA Mark For Results
                    if (_ca > 0)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        async Task UpdateTermEndMarks()
        {
            MarkEntryValidationResult = false;

            if (selectedItemCognitiveMark.Mark_CA1 > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 2).Mark))
            {
                await Swal.FireAsync("Invalid CA 1 Mark", selectedItemCognitiveMark.SN + ". " +
                                        selectedItemCognitiveMark.StudentName + ": " + selectedItemCognitiveMark.Mark_CA1 + " > " +
                                        markSettingsList.FirstOrDefault(m => m.MarkID == 2).Mark + ".", "error");
                MarkEntryValidationResult = true;
            }
            else if (selectedItemCognitiveMark.Mark_CA2 > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 3).Mark))
            {
                await Swal.FireAsync("Invalid CA 2 Mark", selectedItemCognitiveMark.SN + ". " +
                                        selectedItemCognitiveMark.StudentName + ": " + selectedItemCognitiveMark.Mark_CA2 + " > " +
                                        markSettingsList.FirstOrDefault(m => m.MarkID == 3).Mark + ".", "error");
                MarkEntryValidationResult = true;
            }
            else if (selectedItemCognitiveMark.Mark_CA3 > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 4).Mark))
            {
                await Swal.FireAsync("Invalid CA 3 Mark", selectedItemCognitiveMark.SN + ". " +
                                        selectedItemCognitiveMark.StudentName + ": " + selectedItemCognitiveMark.Mark_CA3 + " > " +
                                        markSettingsList.FirstOrDefault(m => m.MarkID == 4).Mark + ".", "error");
                MarkEntryValidationResult = true;
            }
            else if (otherSetting.BoolValue && selectedItemCognitiveMark.Mark_CBT > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 5).Mark))
            {
                await Swal.FireAsync("Invalid CA 4 Mark", selectedItemCognitiveMark.SN + ". " +
                                        selectedItemCognitiveMark.StudentName + ": " + selectedItemCognitiveMark.Mark_CBT + " > " +
                                        markSettingsList.FirstOrDefault(m => m.MarkID == 5).Mark + ".", "error");
                MarkEntryValidationResult = true;
            }
            else if (selectedItemCognitiveMark.Mark_Exam > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 6).Mark))
            {
                await Swal.FireAsync("Invalid Exam Mark", selectedItemCognitiveMark.SN + ". " +
                                        selectedItemCognitiveMark.StudentName + ": " + selectedItemCognitiveMark.Mark_Exam + " > " +
                                        markSettingsList.FirstOrDefault(m => m.MarkID == 6).Mark + ".", "error");
                MarkEntryValidationResult = true;
            }

            if (!MarkEntryValidationResult)
            {
                decimal _CA = selectedItemCognitiveMark.Mark_CA1 + selectedItemCognitiveMark.Mark_CA2 +
                                                selectedItemCognitiveMark.Mark_CA3 + selectedItemCognitiveMark.Mark_CBT;
                bool _entryStatus = TermEndUseMarkType(_CA, selectedItemCognitiveMark.Mark_Exam);

                cognitiveMark.StudentMarkID = selectedItemCognitiveMark.StudentMarkID;
                cognitiveMark.Id = selectedItemCognitiveMark.Id;
                cognitiveMark.TermID = selectedItemCognitiveMark.TermID;
                cognitiveMark.StaffID = selectedItemCognitiveMark.StaffID;
                cognitiveMark.ClassTeacherID = selectedItemCognitiveMark.ClassTeacherID;
                cognitiveMark.SbjSelection = selectedItemCognitiveMark.SbjSelection;
                cognitiveMark.SchID = selectedItemCognitiveMark.SchID;
                cognitiveMark.ClassID = selectedItemCognitiveMark.ClassID;
                cognitiveMark.SubjectID = selectedItemCognitiveMark.SubjectID;
                cognitiveMark.STDID = selectedItemCognitiveMark.STDID;
                cognitiveMark.Mark_ICGC = selectedItemCognitiveMark.Mark_ICGC;
                cognitiveMark.Mark_MidCBT = selectedItemCognitiveMark.Mark_MidCBT;
                cognitiveMark.Mark_Mid = selectedItemCognitiveMark.Mark_Mid;
                cognitiveMark.Mark_CA1 = selectedItemCognitiveMark.Mark_CA1;
                cognitiveMark.Mark_CA2 = selectedItemCognitiveMark.Mark_CA2;
                cognitiveMark.Mark_CA3 = selectedItemCognitiveMark.Mark_CA3;
                cognitiveMark.Mark_CBT = selectedItemCognitiveMark.Mark_CBT;
                cognitiveMark.Mark_Exam = selectedItemCognitiveMark.Mark_Exam;
                cognitiveMark.EntryStatus_ICGCS = selectedItemCognitiveMark.EntryStatus_ICGCS;
                cognitiveMark.EntryStatus_MidTerm = selectedItemCognitiveMark.EntryStatus_MidTerm;
                cognitiveMark.EntryStatus_TermEnd = _entryStatus;
                await _cognitiveMarksService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 2, cognitiveMark);
                cognitiveMarks.FirstOrDefault(m => m.StudentMarkID == selectedItemCognitiveMark.StudentMarkID).Total_Exam =
                   selectedItemCognitiveMark.Mark_CA1 + selectedItemCognitiveMark.Mark_CA2 + selectedItemCognitiveMark.Mark_CA3 + selectedItemCognitiveMark.Mark_CBT + selectedItemCognitiveMark.Mark_Exam;
            }
            else
            {
                await Swal.FireAsync("End of Term Exam Marks Error",
                    "Please, Check Your Entry. You have Entered A Mark Greater Than The Maximum Mark Set.", "error");
            }
        }

        async Task SaveTermEndMarks()
        {
            marksErrorLisitng.Clear();
            int k1 = 0; int k2 = 0; int k3 = 0; int k4 = 0; int k5 = 0; int k = 0;

            IsShow = false;
            i = 0;
            j = 0;
            progressbarInfo = "Please wait saving student marks...";

            int maxValue = cognitiveMarks.Count();

            foreach (var item in cognitiveMarks)
            {
                j++;
                i = ((decimal)(j) / maxValue) * 100;

                if (item.Mark_CA1 > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 2).Mark))
                {
                    k1++;
                }
                else if (item.Mark_CA2 > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 3).Mark))
                {
                    k2++;
                }
                else if (item.Mark_CA3 > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 4).Mark))
                {
                    k3++;
                }
                else if (otherSetting.BoolValue && item.Mark_CBT > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 5).Mark))
                {
                    k4++;
                }
                else if (item.Mark_Exam > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 6).Mark))
                {
                    k5++;
                }

                if (k1 > 0 || k2 > 0 || k3 > 0 || k4 > 0 || k5 > 0)
                {
                    k++; k1 = 0; k2 = 0; k3 = 0; k4 = 0; k5 = 0;
                    marksErrorLisitng.Add(item.SN + ". " + item.StudentName);
                }
                else
                {
                    decimal _CA = item.Mark_CA1 + item.Mark_CA2 + item.Mark_CA3 + item.Mark_CBT;
                    bool _entryStatus = TermEndUseMarkType(_CA, item.Mark_Exam);

                    cognitiveMark.StudentMarkID = item.StudentMarkID;
                    cognitiveMark.Id = item.Id;
                    cognitiveMark.TermID = item.TermID;
                    cognitiveMark.StaffID = item.StaffID;
                    cognitiveMark.ClassTeacherID = item.ClassTeacherID;
                    cognitiveMark.SbjSelection = item.SbjSelection;
                    cognitiveMark.SchID = item.SchID;
                    cognitiveMark.ClassID = item.ClassID;
                    cognitiveMark.SubjectID = item.SubjectID;
                    cognitiveMark.STDID = item.STDID;
                    cognitiveMark.Mark_ICGC = item.Mark_ICGC;
                    cognitiveMark.Mark_MidCBT = item.Mark_MidCBT;
                    cognitiveMark.Mark_Mid = item.Mark_Mid;
                    cognitiveMark.Mark_CA1 = item.Mark_CA1;
                    cognitiveMark.Mark_CA2 = item.Mark_CA2;
                    cognitiveMark.Mark_CA3 = item.Mark_CA3;
                    cognitiveMark.Mark_CBT = item.Mark_CBT;
                    cognitiveMark.Mark_Exam = item.Mark_Exam;
                    cognitiveMark.EntryStatus_ICGCS = item.EntryStatus_ICGCS;
                    cognitiveMark.EntryStatus_MidTerm = item.EntryStatus_MidTerm;
                    cognitiveMark.EntryStatus_TermEnd = _entryStatus; 
                    await _cognitiveMarksService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 2, cognitiveMark);
                    cognitiveMarks.FirstOrDefault(m => m.StudentMarkID == item.StudentMarkID).Total_Exam =
                    item.Mark_CA1 + item.Mark_CA2 + item.Mark_CA3 + item.Mark_CBT + item.Mark_Exam;
                }

                StateHasChanged();
            }

            IsShow = true;

            if (k != 0)
            {
                string studentNames = string.Join("\n", marksErrorLisitng.ToArray());

                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "The Following End of Term Student Marks Were Not Saved Because Of Invalid Mark Entry:",
                    Width = "500",
                    Icon = "info",
                    Html = "<pre class='format-pre' style='color: white;'>" + studentNames + "</pre>"
                });
            }
            else
            {
                //await Swal.FireAsync("End of Term. Operation Completed Successfully", "Student Marks Succesfully Saved.", "success");
            }
        }

        #endregion

        #region [Section - Progress Bar: Timer]
        private bool _processing = false;
        TimeSpan stopwatchvalue = new();
        bool Is_stopwatchrunning { get; set; } = false;
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

        async Task LoadAllList()
        {           
            _processing = true;
            offlineprohressbarinfo = "Please wait while synchronizing Offline Data with Online Data...";
            await Task.Delay(2000);
            await LoadSessions();
            await LoadResultType();
            await LoadSchools();
            await LoadClassList();
            await LoadAllStudents();
            await LoadMarkSettings();
            await LoadCBTExams();
            await LoadSubjectsAllocated();
            await LoadStudentsSubjectAllocated();
            await LoadDefaultList();
            await _cognitiveMarksService.SyncLocalToServer();
            _processing = false;

            await Swal.FireAsync("Data Synchronization", "Synchronization Completed Succesfully.", "success");
        }

        async Task ProcessList()
        {
            stopwatchvalue = new TimeSpan();
            Task task1 = Task.Run(() => LoadAllList());
            Task task2 = Task.Run(() => StartStopWatch());

            await Task.WhenAll(task1, task2);

            task1.Dispose();
            task2.Dispose();

            await Task.CompletedTask;
        }

        async Task GenerateOfflineTemplate()
        {
            stopwatchvalue = new TimeSpan();
            Task task1 = Task.Run(() => LoadCognitiveMarks());
            Task task2 = Task.Run(() => StartStopWatch());

            await Task.WhenAll(task1, task2);

            task1.Dispose();
            task2.Dispose();

            await Task.CompletedTask;
        }

        #endregion

        #region [Click Events]
        async Task CognitiveMarkEvent()
        {
            await RefreshCogvitive();
            SelectedExam();
        }

        async Task GenerateMarkTemplates()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Offline Mark Entry",
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
                    await GenerateOfflineTemplate();
                    await Swal.FireAsync("Offline Mark Entry", "Offline Mark Entry Template(s) Successfully Generated.", "success");
                }
            }                
        }

        void SetRequestURL()
        {
            APICallParameters.Id = 14;
            APICallParameters.RequestUriUpdate = "AcademicsMarks/UpdateCognitiveMark/";                    
        }

        #endregion

        void IDisposable.Dispose()
        {
            _networkStatusService.OnlineStatusChanged -= OnlineStatusChanged;
        }
    }
}
