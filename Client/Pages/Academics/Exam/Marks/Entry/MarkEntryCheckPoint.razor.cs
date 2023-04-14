using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Academics.Subjects;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Administration.Students;
using WebAppAcademics.Shared.Models.Settings;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Client.OfflineRepo.Academics.Subjects;
using WebAppAcademics.Client.OfflineRepo.Admin.School;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.OfflineRepo.Admin.Student;
using WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Entry;
using WebAppAcademics.Client.OfflineRepo.Settings;

namespace WebAppAcademics.Client.Pages.Academics.Exam.Marks.Entry
{
    public partial class MarkEntryCheckPoint
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
        [Inject] StudentDBSyncRepo _studentService { get; set; }
        [Inject] MarkSettingsDBSyncRepo _markSettingsService { get; set; }
        [Inject] CognitiveDBSyncRepo _cognitiveMarksService { get; set; }
        [Inject] SessionsDBSyncRepo _sessionsService { get; set; }


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
        int classid { get; set; }
        int classlistid { get; set; }
        int classTeacherID { get; set; }
        int schoolPrincipalID { get; set; }
        int subjectid { get; set; }

        string selectedSchool { get; set; }
        string selectedClass { get; set; }
        string selectedSubject { get; set; }
        string columnTitle { get; set; }

        decimal i { get; set; } = 0;
        int j { get; set; } = 0;
        bool IsShow { get; set; } = true;
        string progressbarInfo { get; set; } = string.Empty;
        string offlineprohressbarinfo { get; set; } = string.Empty;

        bool IsCheckPointClass { get; set; }
        bool IsIGCSEClass { get; set; }
        bool MarkEntryValidationResult { get; set; } = false;
        #endregion

        #region [Models Declaration]
        List<SETSchSessions> sessions = new();
        List<ADMSchlList> schools = new();
        List<ADMSchClassList> classListAll = new();
        List<ADMSchClassList> classList = new();
        List<ADMStudents> studentsAll = new();
        List<ADMStudents> students = new();
        List<ACDSettingsMarks> markSettingsList = new();
        List<ACDStudentsMarksCognitive> cognitiveMarks = new();
        List<ACDStudentsMarksCognitive> cognitiveMarksSTDID = new();
        List<ACDSbjAllocationTeachers> teacherSubjectAllocation = new();
        List<ACDSbjAllocationTeachers> SelectedTeachersSubjects = new();
        List<ACDSbjAllocationStudents> studentSubjectsAllocation = new();

        List<string> noSubjectAllocationStudents = new();
        List<string> marksErrorLisitng = new();

        List<ACDStudentsMarksCognitive> cognitiveMarksAll = new();
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
                if (schools.Count > 0)
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
            Layout.currentPage = "Student Check Point / IGCSE Marks Entry";
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            schoolSession = await localStorageService.ReadEncryptedItemAsync<int>("schoolsession");
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
            markSettingsList = (await _markSettingsService.GetAllOfflineAsync()).ToList();
            classListAll = (await _classListService.GetAllOfflineAsync()).ToList();
            studentsAll = (await _studentService.GetAllOfflineAsync()).ToList();
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
            //SetRequestURL();
            await Task.CompletedTask;
        }
               
        async Task LoadSubjectsAllocated()
        {
            if (IsOnline)
            {
                if (roleid == 1) //Administrator
                {
                    teacherSubjectAllocation = await _subjectAllocationService.GetAllAsync("AcademicsSubjects/GetTeacherAllocations/14/true/" +
                                                                                            termid + "/0/0/0/0/0");
                }
                else
                {
                    teacherSubjectAllocation = await _subjectAllocationService.GetAllAsync("AcademicsSubjects/GetTeacherAllocations/15/true/" +
                                                    termid + "/0/0/0/0/" + staffid);
                }
            }
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

        string SelectedEntryType()
        {
            string result = string.Empty;
            columnTitle = "Mark";

            if (IsCheckPointClass)
            {
                columnTitle = "CheckPoint Mark";
                result = "Check Point Mark Entry";
            }
            else if (IsIGCSEClass)
            {
                columnTitle = "IGCSE Mark";
                result = "IGSCE Mark Entry";
            }

            return result;
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
                    if (!studentSubjectsAllocation.Where(s => s.STDID == _student.STDID && s.SubjectID == subjectid).Any())
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

                    if (studentSubjectsAllocation.Where(s => s.STDID == _student.STDID && s.SubjectID == subjectid).Any())
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

                foreach (var item in _markList)
                {
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
                }
            }

            IsShow = true;
        }

        bool CheckPointIGCSEUseMarkType(decimal _mark)
        {
            if (_mark > 0)
            {
                return true;
            }

            return false;
        }

        async Task UpdateIGSCE()
        {
            MarkEntryValidationResult = false;

            if (selectedItemCognitiveMark.Mark_ICGC > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 7).Mark))
            {
                await Swal.FireAsync("Invalid IGCSE Mark", selectedItemCognitiveMark.SN + ". " +
                                        selectedItemCognitiveMark.StudentName + ": " + selectedItemCognitiveMark.Mark_ICGC + " > " +
                                        markSettingsList.FirstOrDefault(m => m.MarkID == 7).Mark + ".", "error");
                MarkEntryValidationResult = true;
            }

            if (!MarkEntryValidationResult)
            {
                bool _entryStatus = CheckPointIGCSEUseMarkType(selectedItemCognitiveMark.Mark_ICGC);

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
                cognitiveMark.EntryStatus_ICGCS = _entryStatus;
                await _cognitiveMarksService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 3, cognitiveMark);
            }
        }

        async Task SaveIGSCE()
        {
            if (cognitiveMarks.Count() > 0)
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Save Students Marks",
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
                        marksErrorLisitng.Clear();
                        int k1 = 0;
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

                            if (mark.Mark_ICGC > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 7).Mark))
                            {
                                k1++;
                            }

                            if (k1 > 0)
                            {
                                k++;
                                k1 = 0;
                                marksErrorLisitng.Add(mark.SN + ". " + mark.StudentName);
                            }
                            else
                            {
                                bool _entryStatus = CheckPointIGCSEUseMarkType(mark.Mark_ICGC);
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
                                cognitiveMark.EntryStatus_MidTerm = mark.EntryStatus_MidTerm;
                                cognitiveMark.EntryStatus_TermEnd= mark.EntryStatus_TermEnd;
                                cognitiveMark.EntryStatus_ICGCS = _entryStatus;
                                await _cognitiveMarksService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 3, cognitiveMark);
                            }

                            StateHasChanged();
                        }

                        IsShow = true;

                        if (k != 0)
                        {
                            string studentNames = string.Join("\n", marksErrorLisitng.ToArray());

                            SweetAlertResult resulterror = await Swal.FireAsync(new SweetAlertOptions
                            {
                                Title = "The Following IGSCE Student Marks Were Not Saved Because Of Invalid Mark Entry:",
                                Width = "500",
                                Icon = "info",
                                Html = "<pre class='format-pre' style='color: white;'>" + studentNames + "</pre>"
                            });
                        }
                        else
                        {
                            await Swal.FireAsync("CheckPoint / IGCSE. Operation Completed Successfully", "Student Marks Succesfully Saved.", "success");
                        }
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

        async Task LoadAllList()
        {
            _processing = true;
            offlineprohressbarinfo = "Please wait while synchronizing Offline Data with Online Data...";
            await Task.Delay(2000);
            await LoadSchools();
             await LoadSessions();
            await LoadClassList();
            await LoadAllStudents();
            await LoadMarkSettings();
            await LoadSubjectsAllocated();
            await LoadStudentsSubjectAllocated();
            await LoadDefaultList();
            //await _cognitiveMarksService.SyncLocalToServer();
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
                    await Swal.FireAsync("Offline Mark Entry", "Offline Mark Entry Template(s) Successfully Generating.", "success");
                }
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

        #endregion


        //void SetRequestURL()
        //{
        //    APICallParameters.Id = 0;
        //    APICallParameters.RequestUriUpdate = "AcademicsMarks/UpdateCognitiveMark/";
        //}

        void IDisposable.Dispose()
        {
            _networkStatusService.OnlineStatusChanged -= OnlineStatusChanged;
        }
    }
}
