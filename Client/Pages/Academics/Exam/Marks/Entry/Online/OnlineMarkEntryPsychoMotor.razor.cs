using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Academics.Subjects;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Administration.Students;
using WebAppAcademics.Shared.Models.Settings;

namespace WebAppAcademics.Client.Pages.Academics.Exam.Marks.Entry.Online
{
    public partial class OnlineMarkEntryPsychoMotor
    {

        #region [Injection Declaration]
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<SETSchSessions> schoolSessionsService { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassList> classService { get; set; }
        [Inject] IAPIServices<ADMStudents> studentService { get; set; }
        [Inject] IAPIServices<ACDSbjClassification> subjectClassificationService { get; set; }
        [Inject] IAPIServices<ACDSbjAllocationTeachers> subjectAllocationTeacherService { get; set; }
        [Inject] IAPIServices<ACDSbjAllocationStudents> subjectAllocationStudentService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksAssessment> studentOtherMarksService { get; set; }
        [Inject] IAPIServices<ACDSettingsRatingOptions> ratingOptionsService { get; set; }
        [Inject] IAPIServices<ACDSettingsRatingText> ratingTextService { get; set; }
        [Inject] IAPIServices<ACDSettingsRating> ratingService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Student PsychoMotor And Other Accessment Marks Entry";
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            schoolSession = await localStorageService.ReadEncryptedItemAsync<int>("schoolsession");
            academicSession = await localStorageService.ReadEncryptedItemAsync<string>("academicsession");
            roleid = await localStorageService.ReadEncryptedItemAsync<int>("roleid");
            staffid = await localStorageService.ReadEncryptedItemAsync<int>("staffid");
            await LoadList();
            await base.OnInitializedAsync();
        }

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

        #endregion

        #region [Models Declaration]
        List<SETSchSessions> sessions = new();
        List<ADMSchlList> _schools = new();
        List<ADMSchClassList> _classList = new();
        List<ACDSbjClassification> _sbjclasslist = new();
        List<ADMStudents> students = new();
        List<ACDSbjAllocationTeachers> classTeacherSubjectsAllocation = new();
        List<ACDSbjAllocationTeachers> distinctclassTeachers = new();
        List<ACDSbjAllocationStudents> studentSubjectsAllocation = new();
        List<ACDSettingsRatingOptions> ratingOptionsList = new();
        List<ACDSettingsRatingText> ratingTextList = new();
        List<ACDSettingsRating> ratingList = new();
        List<ACDStudentsMarksAssessment> otherMarks = new();
        List<ACDStudentsMarksAssessment> otherMarksSTDID = new();
        List<string> marksErrorLisitng = new();
        ACDStudentsMarksAssessment otherMark = new();
        ACDStudentsMarksAssessment selectedItemOtherMark = null;
        #endregion

        #region [Load And Click Events]

        async Task LoadList()
        {
            //PsychoMotor And Others
            sessions = await schoolSessionsService.GetAllAsync("Settings/GetAccademicSessions/1/0");
            _schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            ratingList = await ratingService.GetAllAsync("AcademicsMarkSettings/GeRatingSettings/1");
            ratingOptionsList = await ratingOptionsService.GetAllAsync("AcademicsMarkSettings/GeRatingOptionSettings/1");
            ratingTextList = await ratingTextService.GetAllAsync("AcademicsMarkSettings/GeRatingTextSettings/1");

            ratingOptionID = ratingOptionsList.FirstOrDefault(o => o.UsedOption == true).OptionID;
            ratingTextID = ratingTextList.FirstOrDefault(t => t.UsedText == true).TextID;
            maxTermID = sessions.FirstOrDefault(s => s.TermID == sessions.Max(t => t.TermID)).TermID;
            distinctclassTeachers = await subjectAllocationTeacherService.GetAllAsync("AcademicsSubjects/GetTeacherAllocations/12/true/" + termid + "/0/0/0/0/0");

        }

        async Task LoadStudents(int id)
        {
            if (maxTermID == termid)
            {
                students = await studentService.GetAllAsync("AdminStudent/GetStudents/3/0/" + id + "/0/1");
            }
            else
            {
                //students = await studentService.GetAllAsync("AdminStudent/GetStudents/7/0/" + id + "/0/0");

                otherMarksSTDID = await studentOtherMarksService.GetAllAsync("AcademicsMarks/GetOtherMarks/10/" + termid + "/" + _schid + "/" +
                                                                        _classid + "/0/0/0/0");

                if (otherMarksSTDID.Count == 0)
                {
                    students = await studentService.GetAllAsync("AdminStudent/GetStudents/3/0/" + id + "/0/1");
                }

                foreach (var item in otherMarksSTDID)
                {
                    students.Add(new ADMStudents()
                    {
                        STDID = item.STDID,
                        AdmissionNo = item.AdmissionNo,
                        StudentName = item.StudentName,
                    });
                }
            }
        }

        bool IsCurrentTermResults()
        {
            bool result = false;

            if (maxTermID == termid)
            {
                result = true;
            }

            return result;
        }

        async Task LoadStudentOtherMarks(int _stdid)
        {
            studentSubjectsAllocation.Clear();
            otherMarks.Clear();
            studentSubjectsAllocation = await subjectAllocationStudentService.GetAllAsync("AcademicsSubjects/GetStudentAllocations/9/" +
                                                        schoolSession + "/true/" + _schid + "/0/" + _sbjclassid + "/0/" + _stdid);

            otherMarks = await studentOtherMarksService.GetAllAsync("AcademicsMarks/GetOtherMarks/3/" + termid + "/" + _schid + "/" +
                                                                        _classid + "/" + _sbjclassid + "/0/" + _stdid + "/0");

            foreach (var subject in studentSubjectsAllocation)
            {
                bool StudentMarkExist = otherMarks.Where(m => m.SubjectID == subject.SubjectID).Any();

                if (!StudentMarkExist)
                {
                    otherMark.TermID = termid;
                    otherMark.SchSession = schoolSession;
                    otherMark.SchID = _schid;
                    otherMark.ClassID = _classid;
                    otherMark.StaffID = _classTeacherID;
                    otherMark.STDID = _stdid;
                    otherMark.SubjectID = subject.SubjectID;
                    otherMark.Rating = 0;
                    otherMark.OptionID = ratingOptionID;
                    otherMark.TextID = ratingTextID;
                    otherMark.RatingID = 0;
                    otherMark.SbjSelection = true;
                    await studentOtherMarksService.SaveAsync("AcademicsMarks/AddOtherMark/", otherMark);
                }
            }

            otherMarks.Clear();
            otherMarks = await studentOtherMarksService.GetAllAsync("AcademicsMarks/GetOtherMarks/3/" + termid + "/" + _schid + "/" +
                                                                       _classid + "/" + _sbjclassid + "/0/" + _stdid + "/0");
        }

        async Task<bool> IsSubjectAllocatedToStudent(int _stdid)
        {
            studentSubjectsAllocation = await subjectAllocationStudentService.GetAllAsync("AcademicsSubjects/GetStudentAllocations/9/" +
                                                       schoolSession + "/true/" + _schid + "/0/" + _sbjclassid + "/0/" + _stdid);

            if (studentSubjectsAllocation.Count() > 0)
            {
                return true;
            }

            return false;
        }

        async Task<bool> IsSubjectsAllocatedToClassTeacher()
        {
            classTeacherSubjectsAllocation = await subjectAllocationTeacherService.GetAllAsync(
                "AcademicsSubjects/GetTeacherAllocations/11/true/" + termid + "/" + _schid + "/0/" + _sbjclassid + "/0/" + _classTeacherID);

            if (classTeacherSubjectsAllocation.Count() > 0)
            {
                return true;
            }

            return false;
        }


        async Task OnSchoolChanged(IEnumerable<string> e)
        {
            _selectedSchool = e.ElementAt(0);
            _schid = _schools.FirstOrDefault(s => s.School == _selectedSchool).SchID;

            _classid = 0;
            _classlistid = 0;
            _selectedClass = string.Empty;
            _classList.Clear();

            if (roleid == 1)
            {
                _classList = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + _schid + "/0");
            }
            else if (roleid == 10)
            {
                var teacherClasses = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + _schid + "/0");
                _classList = teacherClasses.Where(c => c.StaffID == staffid).ToList();
            }

            _selectedClassTeacher = string.Empty;
            _selectedSubjectClass = string.Empty;
            _sbjclasslist.Clear();
            students.Clear();

            _selSTDID = 0;
            _selSN = 0;
            _selStudentName = string.Empty;
        }

        async Task OnClassChanged(IEnumerable<string> e)
        {
            _selectedClass = e.ElementAt(0);
            _classid = _classList.FirstOrDefault(c => c.ClassName == _selectedClass).ClassID;
            _classlistid = _classList.FirstOrDefault(c => c.ClassName == _selectedClass).ClassListID;

            if (!IsCurrentTermResults())
            {
                _classTeacherID = distinctclassTeachers.FirstOrDefault(t => t.ClassID == _classid).StaffID_ClassTeacher;
                _selectedClassTeacher = distinctclassTeachers.FirstOrDefault(t => t.ClassID == _classid).ClassTeacher;
            }
            else
            {
                _classTeacherID = _classList.FirstOrDefault(c => c.ClassName == _selectedClass).StaffID;
                _selectedClassTeacher = _classList.FirstOrDefault(c => c.ClassName == _selectedClass).ClassTeacher;
            }

            _sbjclassid = 0;
            _selectedSubjectClass = string.Empty;
            _sbjclasslist.Clear();
            _sbjclasslist = await subjectClassificationService.GetAllAsync("AcademicsSubjects/GetSubjectsClassifications/2");

            students.Clear();
            _selSTDID = 0;
            _selSN = 0;
            _selStudentName = string.Empty;
        }

        async Task OnClassificationChanged(IEnumerable<string> e)
        {
            _selectedSubjectClass = e.ElementAt(0);
            _sbjclassid = _sbjclasslist.FirstOrDefault(s => s.SbjClassification == _selectedSubjectClass).SbjClassID;

            students.Clear();
            otherMarks.Clear();
            studentSubjectsAllocation.Clear();

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
                otherMark.STDID = selectedItemOtherMark.STDID;
                otherMark.Rating = selectedItemOtherMark.Rating;
                otherMark.OptionID = ratingOptionID;
                otherMark.TextID = ratingTextID;
                otherMark.RatingID = _ratingID;

                await studentOtherMarksService.UpdateAsync("AcademicsMarks/UpdateOtherMark/", 1, otherMark);
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
                    otherMark.STDID = item.STDID;
                    otherMark.Rating = item.Rating;
                    otherMark.OptionID = ratingOptionID;
                    otherMark.TextID = ratingTextID;
                    otherMark.RatingID = _ratingID;

                    await studentOtherMarksService.UpdateAsync("AcademicsMarks/UpdateOtherMark/", 1, otherMark);
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
            _schools.Clear();
            _schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            _classid = 0;
            _classTeacherID = 0;
            _selectedClass = string.Empty;
            _classList.Clear();
            _classList = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + _schid + "/0");

            _sbjclassid = 0;
            _selectedSubjectClass = string.Empty;
            _sbjclasslist.Clear();
            _selectedStudentID = 0;
            _selectedStudentSN = 0;
            _selectedStudentName = string.Empty;

            students.Clear();
            otherMarks.Clear();

            _selectedClassTeacher = string.Empty;
            _selStudentName = string.Empty;
        }

        #endregion

        #region [Click Events]


        #endregion

    }
}
