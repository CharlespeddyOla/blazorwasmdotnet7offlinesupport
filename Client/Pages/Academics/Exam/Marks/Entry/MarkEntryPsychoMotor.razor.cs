using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Entry;
using WebAppAcademics.Client.OfflineRepo.Academics.Subjects;
using WebAppAcademics.Client.OfflineRepo.Admin.School;
using WebAppAcademics.Client.OfflineRepo.Admin.Student;
using WebAppAcademics.Client.OfflineRepo.Settings;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Academics.Subjects;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Administration.Students;
using WebAppAcademics.Shared.Models.Settings;
using static MudBlazor.CategoryTypes;
using static System.Net.Mime.MediaTypeNames;

namespace WebAppAcademics.Client.Pages.Academics.Exam.Marks.Entry
{
    public partial class MarkEntryPsychoMotor
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
        [Inject] StudentDBSyncRepo _studentService { get; set; }
        [Inject] RatingDBSyncRepo _ratingService { get; set; }
        [Inject] RatingOptionsDBSyncRepo _ratingOptionsService { get; set; }
        [Inject] RatingTextDBSyncRepo _ratingTextService { get; set; }
        [Inject] AssessmentDBSyncRepo _assessmentService { get; set; }
        [Inject] SubjectClassificationDBSyncRepo _subjectClassificationService { get; set; }

        [Inject] IAPIServices<ACDStudentsMarksAssessment> studentOtherMarksService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int termid { get; set; }
        int maxTermID { get; set; }
        int schoolSession { get; set; }
        int roleid { get; set; }
        int staffid { get; set; }
        string academicSession { get; set; }
        int _schid { get; set; }
        int _classid { get; set; }
        int _classlistid { get; set; }
        int _sbjclassid { get; set; }
        int _classTeacherID { get; set; }
        int _selectedStudentID { get; set; }
        int _selectedStudentSN { get; set; }
        int _ratingID { get; set; }
        int ratingOptionID { get; set; }
        int ratingTextID { get; set; }

        string _selectedSchool { get; set; }
        string _selectedClass { get; set; }
        string _selectedClassTeacher { get; set; }
        string _selectedSubjectClass { get; set; }
        string _selectedStudentName { get; set; }

        bool RatingValueCheck { get; set; }

        int selectedRowNumber { get; set; } = -1;
        MudTable<ADMStudents> mudTable;
        int _selSTDID { get; set; }
        int _selSN { get; set; }
        string _selStudentName { get; set; }
        string progressbarInfo { get; set; } = string.Empty;
        string offlineprohressbarinfo { get; set; } = string.Empty;
        #endregion

        #region [Models Declaration]
        List<SETSchSessions> sessions = new();
        List<ADMSchlList> schools = new();
        List<ADMSchClassList> classListAll = new();
        List<ADMSchClassList> classList = new();
        List<ACDSbjClassification> sbjclasslist = new();
        List<ACDSbjClassification> sbjclasslistAll = new();
        List<ADMStudents> studentsAll = new();
        List<ADMStudents> students = new();
        List<ACDSbjAllocationTeachers> teacherSubjectAllocation = new();
        List<ACDSbjAllocationTeachers> classTeacherSubjectsAllocation = new();
        List<ACDSbjAllocationTeachers> distinctclassTeachers = new();
        List<ACDSbjAllocationStudents> studentSubjectsAllocation = new();
        List<ACDSettingsRatingOptions> ratingOptionsList = new();
        List<ACDSettingsRatingText> ratingTextList = new();
        List<ACDSettingsRating> ratingList = new();
        List<ACDStudentsMarksAssessment> otherMarksAll = new();
        List<ACDStudentsMarksAssessment> otherMarks = new();
        List<ACDStudentsMarksAssessment> otherMarksSTDID = new();
        List<string> marksErrorLisitng = new();
        ACDStudentsMarksAssessment otherMark = new();
        ACDStudentsMarksAssessment selectedItemOtherMark = null;
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
            Layout.currentPage = "Student PsychoMotor And Other Accessment Marks Entry";
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
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
                    await OnlineListProcessing();
                }
            }
        }

        #region [Section - Load Offline Data]
        async Task LoadDefaultList()
        {
            maxTermID = sessions.FirstOrDefault(s => s.TermID == sessions.Max(t => t.TermID)).TermID;
            ratingOptionID = ratingOptionsList.FirstOrDefault(o => o.UsedOption == true).OptionID;
            ratingTextID = ratingTextList.FirstOrDefault(t => t.UsedText == true).TextID;
            SetRequestURL();
            await Task.CompletedTask;
        }

        async Task LoadOffLineList()
        {
            offlineprohressbarinfo = string.Empty;
            _processing = true;
            offlineprohressbarinfo = "Please wait loading offline data...";
            sessions = (await _sessionsService.GetAllOfflineAsync()).ToList();
            classListAll = (await _classListService.GetAllOfflineAsync()).ToList();
            studentsAll = (await _studentService.GetAllOfflineAsync()).ToList();
            sbjclasslistAll = (await _subjectClassificationService.GetAllOfflineAsync()).ToList();
            ratingList = (await _ratingService.GetAllOfflineAsync()).ToList();
            ratingOptionsList = (await _ratingOptionsService.GetAllOfflineAsync()).ToList();
            ratingTextList = (await _ratingTextService.GetAllOfflineAsync()).ToList();
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


        #endregion

        #region [Section - Load Online Data]

        async Task LoadAllOnlineList()
        {
            _processing = true;
            offlineprohressbarinfo = "Please wait while synchronizing Offline Data with Online Data...";
            await Task.Delay(2000);
            await LoadSessions();
            await LoadSchools();
            await LoadClassList();
            await LoadSubjectClassificationList();
            await LoadAllStudents();
            await LoadRatingSettings();
            await LoadRatingOptions();
            await LoadRatingText();
            await LoadTeachersSubjectsAllocated();
            await LoadStudentsSubjectAllocated();
            await LoadDefaultList();
            await _assessmentService.SyncLocalToServer();
            _processing = false;

            await Swal.FireAsync("Data Synchronization", "Synchronization Completed Succesfully.", "success");
        }

        async Task OnlineListProcessing()
        {
            stopwatchvalue = new TimeSpan();
            Task task1 = Task.Run(() => LoadAllOnlineList());
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

        async Task LoadSubjectClassificationList()
        {
            if (IsOnline)
            {
                var list = await _subjectClassificationService.GetAllAsync("AcademicsSubjects/GetSubjectsClassifications/2");
                if (list != null)
                {
                    sbjclasslistAll = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task LoadRatingSettings()
        {
            if (IsOnline)
            {
                var list = await _ratingService.GetAllAsync("AcademicsMarkSettings/GeRatingSettings/1");
                if (list != null)
                {
                    ratingList = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task LoadRatingOptions()
        {
            if (IsOnline)
            {
                var list = await _ratingOptionsService.GetAllAsync("AcademicsMarkSettings/GeRatingOptionSettings/1");
                if (list != null)
                {
                    ratingOptionsList = list.ToList();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task LoadRatingText()
        {
            if (IsOnline)
            {
                var list = await _ratingTextService.GetAllAsync("AcademicsMarkSettings/GeRatingTextSettings/1");
                if (list != null)
                {
                    ratingTextList = list.ToList();
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

        async Task LoadTeachersSubjectsAllocated()
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

        async Task LoadAssessmentMarks()
        {
            if (IsOnline)
            {
                _processing = true;
                offlineprohressbarinfo = "Please wait generating mark entry templates(s) for offline use...";
                otherMarksAll = await _assessmentService.GetAllAsync("AcademicsMarks/GetOtherMarks/11/" + termid + "/0/0/0/0/0/0");               
                _processing = false;
            }
        }

        async Task GenerateOfflineTemplate()
        {
            stopwatchvalue = new TimeSpan();
            Task task1 = Task.Run(() => LoadAssessmentMarks());
            Task task2 = Task.Run(() => StartStopWatch());

            await Task.WhenAll(task1, task2);

            task1.Dispose();
            task2.Dispose();

            await Task.CompletedTask;
        }

        #endregion

        #region [Section - Marks Processing Operations]

        bool IsCurrentTermResults()
        {
            bool result = false;

            if (maxTermID == termid)
            {
                result = true;
            }

            return result;
        }

        async Task LoadStudents(int id)
        {
            if (maxTermID == termid)
            {
                var _students = studentsAll.Where(s => s.StatusTypeID == 1 && s.ClassID == id).ToList();
                int sn = 1;

                foreach (var item in _students)
                {
                    students.Add(new ADMStudents()
                    {
                        SN = sn++,
                        STDID = item.STDID,
                        AdmissionNo = item.AdmissionNo,
                        StudentName = item.StudentName
                    });
                }
            }
            else
            {
                var _students = studentSubjectsAllocation.Where(s => s.SbjClassID == _sbjclassid && s.ClassID == id)
                                .GroupBy(x => new {x.STDID, x.StudentNo, x.StudentName})
                                .Select(x => x.First())
                                .ToList();
                int sn = 1;

                foreach (var item in _students)
                {
                    students.Add(new ADMStudents()
                    {
                        SN = sn++,
                        STDID = item.STDID,
                        AdmissionNo = studentsAll.SingleOrDefault(s => s.STDID == item.STDID).AdmissionNo,
                        StudentName = studentsAll.SingleOrDefault(s => s.STDID == item.STDID).StudentName
                    });
                }
            }

            await Task.CompletedTask;
        }

        async Task LoadStudentOtherMarks(int _stdid)
        {
            otherMarks.Clear();
            if (IsOnline)
            {
                var _selectedStudentSubjects = studentSubjectsAllocation.Where(s => s.SbjClassID == _sbjclassid &&
                                                                                s.STDID == _stdid).ToList();
                var _otherMarks = await studentOtherMarksService.GetAllAsync("AcademicsMarks/GetOtherMarks/3/" + termid + "/" + _schid + "/" +
                                                                        _classid + "/" + _sbjclassid + "/0/" + _stdid + "/0");

                foreach (var item in _selectedStudentSubjects)
                {
                    bool StudentMarkExist = _otherMarks.Where(m => m.SubjectID == item.SubjectID).Any();
                    if (!StudentMarkExist) 
                    {
                        otherMark.TermID = termid;
                        otherMark.SchSession = schoolSession;
                        otherMark.SchID = _schid;
                        otherMark.ClassID = _classid;
                        otherMark.StaffID = _classTeacherID;
                        otherMark.STDID = _stdid;
                        otherMark.SubjectID = item.SubjectID;
                        otherMark.SbjClassID= item.SbjClassID;
                        otherMark.Rating = 0;
                        otherMark.OptionID = ratingOptionID;
                        otherMark.TextID = ratingTextID;
                        otherMark.RatingID = 0;
                        otherMark.SbjSelection = true;
                        var response = await studentOtherMarksService.SaveAsync("AcademicsMarks/AddOtherMark/", otherMark);
                        otherMark.StudentMarkID = response.StudentMarkID;
                        otherMark.Id = response.StudentMarkID;
                        await studentOtherMarksService.UpdateAsync("AcademicsMarks/UpdateOtherMark/", 4, otherMark);
                    }
                }

                otherMarks = await studentOtherMarksService.GetAllAsync("AcademicsMarks/GetOtherMarks/3/" + termid + "/" + _schid + "/" +
                                                                       _classid + "/" + _sbjclassid + "/0/" + _stdid + "/0");
            }
            else
            {
                otherMarksAll = (await _assessmentService.GetAllOfflineAsync()).ToList();
                var _markList = otherMarksAll.Where(m => m.SbjClassID == _sbjclassid && m.STDID == _stdid).ToList();

                int sn = 1;

                foreach (var item in _markList)
                {
                    otherMarks.Add(new ACDStudentsMarksAssessment
                    {
                        StudentMarkID = item.StudentMarkID,
                        Id = item.Id,
                        SN = sn++,
                        TermID = item.TermID,
                        SchID = item.SchID,
                        ClassID = item.ClassID,
                        StaffID = item.StaffID,
                        STDID = item.STDID,
                        SubjectID = item.SubjectID,
                        SubjectCode = item.SubjectCode,
                        Subject = item.Subject,
                        SbjClassID= item.SbjClassID,
                        Rating = item.Rating,
                        OptionID = item.OptionID,
                        TextID = item.TextID,
                        RatingID = item.RatingID 
                    });
                }
            }
        }

        async Task<bool> IsSubjectAllocatedToStudent(int _stdid)
        {
            var _selectedStudentSubjects = studentSubjectsAllocation.Where(s => s.SbjClassID == _sbjclassid &&
                                                                                s.STDID == _stdid).ToList();

            if (_selectedStudentSubjects.Count() > 0)
            {
                return true;
            }

            await Task.CompletedTask;
            return false;
        }

        async Task<bool> IsSubjectsAllocatedToClassTeacher()
        {
            var _selectedClassTeacherSubject = teacherSubjectAllocation.Where(s => s.SchID == _schid && 
                                                                    s.SbjClassID == _sbjclassid && 
                                                                    s.StaffID_ClassTeacher == _classTeacherID).ToList();   
            if (_selectedClassTeacherSubject.Count() > 0)
            {
                return true;
            }

            await Task.CompletedTask;
            return false;
        }

        async Task CommittedItemChanges(ACDStudentsMarksAssessment item)
        {
            int _mark = Convert.ToInt32(Math.Floor(item.Rating));
            _ratingID = 0;
            RatingValueCheck = false;
            if (ratingOptionID == 1)
            {
                int _ratingMAXValue = ratingList.FirstOrDefault(r => r.Rating == ratingList.Max(val => val.Rating)).Rating;
                int _ratingMINValue = ratingList.FirstOrDefault(r => r.Rating == ratingList.Min(val => val.Rating)).Rating;

                if (_mark < _ratingMINValue || _mark > _ratingMAXValue)
                {
                    await Swal.FireAsync("Invalid Rating Value", "Your Rating Must Be Between (" + _ratingMINValue + " - " +
                                            _ratingMAXValue + ")", "error");
                    RatingValueCheck = true;
                }
                else
                {
                    _ratingID = ratingList.FirstOrDefault(val => val.Rating == _mark).RatingID;
                    RatingValueCheck = false;
                }
            }
            else if (ratingOptionID == 2)
            {
                int _ratingMAXValue = ratingList.FirstOrDefault(r => r.Rating == ratingList.Max(val => val.Rating)).HighScore;
                int _ratingMINValue = ratingList.FirstOrDefault(r => r.Rating == ratingList.Min(val => val.Rating)).LowScore;
                var _rating = ratingList.FirstOrDefault(val => val.LowScore <= _mark && val.HighScore >= _mark);

                if (_rating == null)
                {
                    await Swal.FireAsync("Invalid Rating Value", "Your Rating Must Be Between (" + _ratingMINValue + " - " +
                                            _ratingMAXValue + ")", "error");
                    RatingValueCheck = true;
                }
                else
                {
                    _ratingID = _rating.RatingID;
                    RatingValueCheck = false;
                }
            }

            if (!RatingValueCheck)
            {
                otherMark.StudentMarkID = item.StudentMarkID;
                otherMark.Id = item.Id;
                otherMark.TermID = item.TermID;
                otherMark.STDID = item.STDID;
                otherMark.SchID = item.SchID;
                otherMark.ClassID = item.ClassID;
                otherMark.StaffID = item.StaffID;
                otherMark.SubjectID = item.SubjectID;
                otherMark.SubjectCode = item.SubjectCode;
                otherMark.Subject = item.Subject;
                otherMark.SbjClassID = item.SbjClassID;
                otherMark.Rating = item.Rating;
                otherMark.OptionID = ratingOptionID;
                otherMark.TextID = ratingTextID;
                otherMark.RatingID = _ratingID;

                await _assessmentService.UpdateAsync("AcademicsMarks/UpdateOtherMark/", 1, otherMark);
            }
        }

        async Task UpdateOtherMarks()
        {
            int _mark = Convert.ToInt32(Math.Floor(selectedItemOtherMark.Rating));
            _ratingID = 0;
            RatingValueCheck = false;

            if (ratingOptionID == 1)
            {
                int _ratingMAXValue = ratingList.FirstOrDefault(r => r.Rating == ratingList.Max(val => val.Rating)).Rating;
                int _ratingMINValue = ratingList.FirstOrDefault(r => r.Rating == ratingList.Min(val => val.Rating)).Rating;

                if (_mark < _ratingMINValue || _mark > _ratingMAXValue)
                {
                    await Swal.FireAsync("Invalid Rating Value", "Your Rating Must Be Between (" + _ratingMINValue + " - " +
                                            _ratingMAXValue + ")", "error");
                    RatingValueCheck = true;
                }
                else
                {
                    _ratingID = ratingList.FirstOrDefault(val => val.Rating == _mark).RatingID;
                    RatingValueCheck = false;
                }
            }
            else if (ratingOptionID == 2)
            {
                int _ratingMAXValue = ratingList.FirstOrDefault(r => r.Rating == ratingList.Max(val => val.Rating)).HighScore;
                int _ratingMINValue = ratingList.FirstOrDefault(r => r.Rating == ratingList.Min(val => val.Rating)).LowScore;
                var _rating = ratingList.FirstOrDefault(val => val.LowScore <= _mark && val.HighScore >= _mark);

                if (_rating == null)
                {
                    await Swal.FireAsync("Invalid Rating Value", "Your Rating Must Be Between (" + _ratingMINValue + " - " +
                                            _ratingMAXValue + ")", "error");
                    RatingValueCheck = true;
                }
                else
                {
                    _ratingID = _rating.RatingID;
                    RatingValueCheck = false;
                }
            }

            if (!RatingValueCheck)
            {
                otherMark.StudentMarkID = selectedItemOtherMark.StudentMarkID;
                otherMark.Id = selectedItemOtherMark.Id;
                otherMark.TermID = selectedItemOtherMark.TermID;
                otherMark.STDID = selectedItemOtherMark.STDID;
                otherMark.SchID = selectedItemOtherMark.SchID;
                otherMark.ClassID = selectedItemOtherMark.ClassID;
                otherMark.StaffID = selectedItemOtherMark.StaffID;
                otherMark.SubjectID = selectedItemOtherMark.SubjectID;
                otherMark.SubjectCode = selectedItemOtherMark.SubjectCode;
                otherMark.Subject = selectedItemOtherMark.Subject;
                otherMark.SbjClassID = selectedItemOtherMark.SbjClassID;
                otherMark.Rating = selectedItemOtherMark.Rating;
                otherMark.OptionID = ratingOptionID;
                otherMark.TextID = ratingTextID;
                otherMark.RatingID = _ratingID;

                await _assessmentService.UpdateAsync("AcademicsMarks/UpdateOtherMark/", 1, otherMark);
            }
        }

        async Task SavePsychoMarkEntries()
        {
            marksErrorLisitng.Clear();
            int k1 = 0;
            int k = 0;

            foreach (var item in otherMarks)
            {
                int _mark = Convert.ToInt32(Math.Floor(item.Rating));
                _ratingID = 0;

                if (ratingOptionID == 1)
                {
                    int _ratingMAXValue = ratingList.FirstOrDefault(r => r.Rating == ratingList.Max(val => val.Rating)).Rating;
                    int _ratingMINValue = ratingList.FirstOrDefault(r => r.Rating == ratingList.Min(val => val.Rating)).Rating;

                    if (_mark < _ratingMINValue || _mark > _ratingMAXValue)
                    {
                        k1++;
                    }
                    else
                    {
                        _ratingID = ratingList.FirstOrDefault(val => val.Rating == _mark).RatingID;
                    }
                }
                else if (ratingOptionID == 2)
                {
                    int _ratingMAXValue = ratingList.FirstOrDefault(r => r.Rating == ratingList.Max(val => val.Rating)).HighScore;
                    int _ratingMINValue = ratingList.FirstOrDefault(r => r.Rating == ratingList.Min(val => val.Rating)).LowScore;
                    var _rating = ratingList.FirstOrDefault(val => val.LowScore <= _mark && val.HighScore >= _mark);

                    if (_rating == null)
                    {
                        k1++;
                    }
                    else
                    {
                        _ratingID = _rating.RatingID;
                    }
                }

                if (k1 > 0)
                {
                    k++;
                    k1 = 0;
                    marksErrorLisitng.Add(item.Subject);
                }
                else
                {
                    otherMark.StudentMarkID = item.StudentMarkID;
                    otherMark.Id = item.Id;
                    otherMark.TermID = item.TermID;
                    otherMark.STDID = item.STDID;
                    otherMark.SchID = item.SchID;
                    otherMark.ClassID = item.ClassID;
                    otherMark.StaffID = item.StaffID;
                    otherMark.SubjectID = item.SubjectID;
                    otherMark.SubjectCode = item.SubjectCode;
                    otherMark.Subject = item.Subject;
                    otherMark.SbjClassID = item.SbjClassID;
                    otherMark.Rating = item.Rating;
                    otherMark.OptionID = ratingOptionID;
                    otherMark.TextID = ratingTextID;
                    otherMark.RatingID = _ratingID;

                    await _assessmentService.UpdateAsync("AcademicsMarks/UpdateOtherMark/", 1, otherMark);
                }
            }

            if (k != 0)
            {
                string studentSubjects = string.Join("\n", marksErrorLisitng.ToArray());

                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = _selSN + ". " + _selStudentName + " - " +
                    " Marks For The Following Subject(s) Were Not Saved Because Of Invalid Mark Entry:",
                    Width = "500",
                    Icon = "info",
                    Html = "<pre class='format-pre' style='color: white;'>" + studentSubjects + "</pre>"
                });
            }
            else
            {
                await Swal.FireAsync("Psychomotor & Other Assessment. Operation Completed Successfully", "Student Marks Succesfully Saved.", "success");
            }
        }

        async Task SaveOtherMarkEntries()
        {
            if (otherMarks.Count() > 0)
            {
                if (_selSTDID > 0)
                {
                    SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                    {
                        Title = "Save " + _selectedSubjectClass + " Students Marks",
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
                            await SavePsychoMarkEntries();
                        }
                    }
                }
            }
            else
            {
                await Swal.FireAsync("Cannot Save", "No Student Has Been Selected", "error");
            }
        }

        async Task RefreshOtherMarks()
        {
            _schid = 0;
            _selectedSchool = string.Empty;
            schools.Clear();
            await LoadSchools();

            _classid = 0;
            _classTeacherID = 0;
            _selectedClass = string.Empty;
            classList.Clear();
            await LoadClassList();

            _sbjclassid = 0;
            _selectedSubjectClass = string.Empty;
            sbjclasslist.Clear();
            _selectedStudentID = 0;
            _selectedStudentSN = 0;
            _selectedStudentName = string.Empty;

            if (!IsOnline)
            {
                schools = (await _schoolService.GetAllOfflineAsync()).ToList();
                classListAll = (await _classListService.GetAllOfflineAsync()).ToList();
            }

            students.Clear();
            otherMarks.Clear();

            _selectedClassTeacher = string.Empty;
            _selStudentName = string.Empty;
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

        #endregion

        #region [Section - Click Events]

        async Task OnSchoolChanged(IEnumerable<string> e)
        {
            _selectedSchool = e.ElementAt(0);
            _schid = schools.FirstOrDefault(s => s.School == _selectedSchool).SchID;

            _classid = 0;
            _classlistid = 0;
            _selectedClass = string.Empty;
            classList.Clear();

            if (roleid == 1)
            {
                classList = classListAll.Where(c => c.SchID == _schid).ToList();
            }
            else
            {
                classList = classListAll.Where(c => c.SchID == _schid && c.StaffID == staffid).ToList();
            }

            _classTeacherID = 0;
            _selectedClassTeacher = string.Empty;
            _selectedSubjectClass = string.Empty;
            sbjclasslist.Clear();
            students.Clear();

            _selSTDID = 0;
            _selSN = 0;
            _selStudentName = string.Empty;

            await Task.CompletedTask;
        }

        async Task OnClassChanged(IEnumerable<string> e)
        {
            _selectedClass = e.ElementAt(0);
            _classid = classList.FirstOrDefault(c => c.ClassName == _selectedClass).ClassID;
            _classlistid = classList.FirstOrDefault(c => c.ClassName == _selectedClass).ClassListID;

            _classTeacherID = 0;
            _selectedClassTeacher = string.Empty;

            if (!IsCurrentTermResults())
            {
                var _classTeacherPreviousTerm = teacherSubjectAllocation.Where(s => s.ClassID == _classid).ToList();

                if (_classTeacherPreviousTerm.Count == 0)
                {
                    _classTeacherID = classList.FirstOrDefault(c => c.ClassName == _selectedClass).StaffID;
                    _selectedClassTeacher = classList.FirstOrDefault(c => c.ClassName == _selectedClass).ClassTeacher;
                }
                else
                {
                    _classTeacherID = teacherSubjectAllocation.Where(s => s.ClassID == _classid)
                                                          .Distinct()
                                                          .Select(x => x.StaffID_ClassTeacher).First();
                    _selectedClassTeacher = teacherSubjectAllocation.Where(s => s.ClassID == _classid)
                                                               .Distinct()
                                                               .Select(x => x.ClassTeacher).First();
                }              
            }
            else
            {
                _classTeacherID = classList.FirstOrDefault(c => c.ClassName == _selectedClass).StaffID;
                _selectedClassTeacher = classList.FirstOrDefault(c => c.ClassName == _selectedClass).ClassTeacher;
            }

            _sbjclassid = 0;
            _selectedSubjectClass = string.Empty;
            //sbjclasslist.Clear();
            sbjclasslist = sbjclasslistAll;

            students.Clear();
            _selSTDID = 0;
            _selSN = 0;
            _selStudentName = string.Empty;

            await Task.CompletedTask;
        }

        async Task OnClassificationChanged(IEnumerable<string> e)
        {
            _selectedSubjectClass = e.ElementAt(0);
            _sbjclassid = sbjclasslist.FirstOrDefault(s => s.SbjClassification == _selectedSubjectClass).SbjClassID;

            students.Clear();
            otherMarks.Clear();

            if (await IsSubjectsAllocatedToClassTeacher())
            {
                await LoadStudents(_classid);
            }
            else
            {
                await Swal.FireAsync("PsychoMotor And Others Not Allocated", "PsychoMotor And Others Has Not Been Allocated To The " +
                    "Class Teacher - " + _selectedClassTeacher, "success");
            }

            _selSTDID = 0;
            _selSN = 0;
            _selStudentName = string.Empty;
        }

        async Task RowClickEvent(TableRowClickEventArgs<ADMStudents> tableRowClickEventArgs)
        {
            _selSTDID = tableRowClickEventArgs.Item.STDID;
            _selSN = tableRowClickEventArgs.Item.SN;
            _selStudentName = tableRowClickEventArgs.Item.StudentName;

            if (await IsSubjectAllocatedToStudent(_selSTDID))
            {
                await LoadStudentOtherMarks(_selSTDID);
            }
            else
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Student Assessment Allocation",
                    Width = "500",
                    Icon = "info",
                    Text = _selectedSubjectClass + " Has Not Been Allocated To " + _selStudentName
                });
            }
        }

        string SelectedRowClassFunc(ADMStudents element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                selectedRowNumber = -1;
                return string.Empty;
            }
            else if (mudTable.SelectedItem != null && mudTable.SelectedItem.Equals(element))
            {
                selectedRowNumber = rowNumber;
                return "selected";
            }
            else
            {
                return string.Empty;
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

        void SetRequestURL()
        {
            APICallParameters.Id = 1;
            APICallParameters.RequestUriUpdate = "AcademicsMarks/UpdateOtherMark/";
        }

        void IDisposable.Dispose()
        {
            _networkStatusService.OnlineStatusChanged -= OnlineStatusChanged;
        }
    }
}
