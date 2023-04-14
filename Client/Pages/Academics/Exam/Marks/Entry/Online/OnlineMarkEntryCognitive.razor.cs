using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.CBT;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Academics.Subjects;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Administration.Students;
using WebAppAcademics.Shared.Models.Settings;

namespace WebAppAcademics.Client.Pages.Academics.Exam.Marks.Entry.Online
{
    public partial class OnlineMarkEntryCognitive
    {
        #region [Injection Declaration]
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<SETSchSessions> schoolSessionsService { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassList> classService { get; set; }
        [Inject] IAPIServices<ADMStudents> studentService { get; set; }
        [Inject] IAPIServices<ACDSbjAllocationTeachers> subjectAllocationTeacherService { get; set; }
        [Inject] IAPIServices<ACDSbjAllocationStudents> subjectAllocationStudentService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksCognitive> studentMarksCognitiveService { get; set; }
        [Inject] IAPIServices<ACDReportType> resultTypeService { get; set; }
        [Inject] IAPIServices<ACDSettingsMarks> markSettingService { get; set; }
        [Inject] IAPIServices<CBTExams> examService { get; set; }
        [Inject] IAPIServices<CBTStudentScores> studentCBTService { get; set; }


        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Student Cognitive Mark Entry";
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            schTerm = await localStorageService.ReadEncryptedItemAsync<string>("schTerm");
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

        bool MarkEntryValidationResult { get; set; } = false;
        #endregion

        #region [Models Declaration]
        List<SETSchSessions> sessions = new();
        List<ADMSchlList> schools = new();
        List<ADMSchClassList> classList = new();
        List<ADMStudents> students = new();
        List<ACDReportType> resultTypeList = new();
        List<ACDSettingsMarks> markSettingsList = new();
        List<ACDSbjAllocationTeachers> SelectedTeachersSubjects = new();
        List<ACDSbjAllocationStudents> studentSubjectsAllocation = new();
        List<ACDStudentsMarksCognitive> cognitiveMarks = new();
        List<ACDStudentsMarksCognitive> cognitiveMarksSTDID = new();
        List<CBTExams> exams = new();
        List<CBTStudentScores> studentCBTScores = new();

        List<string> noSubjectAllocationStudents = new();
        List<string> marksErrorLisitng = new();

        ACDSettingsOthers otherSetting = new();
        ACDStudentsMarksCognitive cognitiveMark = new();
        ACDStudentsMarksCognitive selectedItemCognitiveMark = null;

        #endregion

        #region [Load And Click Events]

        async Task LoadList()
        {
            //Cognitive Mark Entry
            sessions = await schoolSessionsService.GetAllAsync("Settings/GetAccademicSessions/1/0");
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            resultTypeList = await resultTypeService.GetAllAsync("AcademicsMarkSettings/GetResultTypeSettings/1");
            markSettingsList = await markSettingService.GetAllAsync("AcademicsMarkSettings/GetMarkSettings/1");
            maxTermID = sessions.FirstOrDefault(s => s.TermID == sessions.Max(t => t.TermID)).TermID;
            reportTypeID = resultTypeList.FirstOrDefault(r => r.SelectedExam == true).ReportTypeID;
            exams = await examService.GetAllAsync("AcademicsCBT/GetCBTExams/1/" + termid + "/0/0");
            UseCBTColumnTitle();

            SelectedExam();
        }

        async Task LoadSubjects()
        {
            if (roleid == 1) //Administrator
            {
                var _selectedTeachersSubject = await subjectAllocationTeacherService.GetAllAsync("AcademicsSubjects/GetTeacherAllocations/3/true/" +
                                                    termid + "/" + schid + "/" + classlistid + "/" + classid + "/0/0");
                SelectedTeachersSubjects = _selectedTeachersSubject.Where(s => s.SbjClassID == 1).ToList();
            }
            else
            {
                var _selectedTeachersSubject = await subjectAllocationTeacherService.GetAllAsync("AcademicsSubjects/GetTeacherAllocations/4/true/" +
                                                    termid + "/" + schid + "/" + classlistid + "/" + classid + "/0/" + staffid);
                SelectedTeachersSubjects = _selectedTeachersSubject.Where(s => s.SbjClassID == 1).ToList();

            }

            foreach (var item in SelectedTeachersSubjects)
            {
                SelectedTeachersSubjects.FirstOrDefault(c => c.SubjectID == item.SubjectID).Subject = item.Subject + " - " + item.SubjectTeacher;
            }
        }

        async Task LoadStudents(int id)
        {
            if (maxTermID == termid)
            {
                students = await studentService.GetAllAsync("AdminStudent/GetStudents/3/0/" + id + "/0/1");
            }
            else
            {
                if (reportTypeID == 1)
                {
                    cognitiveMarksSTDID = await studentMarksCognitiveService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/19/" +
                                        termid + "/" + schid + "/" + classid + "/0/0/0");

                }
                else if (reportTypeID == 2)
                {
                    cognitiveMarksSTDID = await studentMarksCognitiveService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/20/" +
                                        termid + "/" + schid + "/" + classid + "/0/0/0");
                }

                if (cognitiveMarksSTDID.Count == 0)
                {
                    students = await studentService.GetAllAsync("AdminStudent/GetStudents/3/0/" + id + "/0/1");
                }

                foreach (var item in cognitiveMarksSTDID)
                {
                    students.Add(new ADMStudents()
                    {
                        STDID = item.STDID,
                        StudentName = item.StudentName,
                    });
                }
            }
        }

        async Task LoadStudentsMarks()
        {
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

                studentSubjectsAllocation = await subjectAllocationStudentService.GetAllAsync("AcademicsSubjects/GetStudentAllocations/7/" +
                            schoolSession + "/true/" + schid + "/" + classlistid + "/" + classid + "/" + subjectid + "/" + _student.STDID);

                if (studentSubjectsAllocation.Count() > 0 && studentSubjectsAllocation.Where(s => s.SbjSelection == true).Any())
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
                        await studentMarksCognitiveService.SaveAsync("AcademicsMarks/AddCognitiveMark/", cognitiveMark);
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

            IsShow = true;
        }

        async Task<bool> ValidateSelections()
        {
            noSubjectAllocationStudents.Clear();

            foreach (var _student in students)
            {
                studentSubjectsAllocation = await subjectAllocationStudentService.GetAllAsync("AcademicsSubjects/GetStudentAllocations/8/" +
                            schoolSession + "/true/" + schid + "/" + classlistid + "/" + classid + "/" + subjectid + "/" + _student.STDID);

                if (studentSubjectsAllocation.Count() == 0 || studentSubjectsAllocation.Where(a => a.SbjSelection == false).Any())
                {
                    if (_student.StudentName != null)
                    {
                        noSubjectAllocationStudents.Add(_student.StudentName);
                    }
                }
            }

            if (noSubjectAllocationStudents.Count() > 0)
            {
                return true;
            }

            return false;
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
            classList = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + schid + "/0");

            subjectid = 0;
            selectedSubject = string.Empty;
            SelectedTeachersSubjects.Clear();

            students.Clear();
            cognitiveMarks.Clear();
        }

        async Task OnSelectedClassChanged(IEnumerable<string> e)
        {
            selectedClass = e.ElementAt(0);
            classid = classList.FirstOrDefault(c => c.ClassName == selectedClass).ClassID;
            classlistid = classList.FirstOrDefault(c => c.ClassName == selectedClass).ClassListID;
            classTeacherID = classList.FirstOrDefault(c => c.ClassName == selectedClass).StaffID;
            //IsFinalYearClass = classList.FirstOrDefault(c => c.ClassID == classid).FinalYearClass;

            subjectid = 0;
            selectedSubject = string.Empty;
            SelectedTeachersSubjects.Clear();
            await LoadSubjects();

            students.Clear();
            await LoadStudents(classid);
            cognitiveMarks.Clear();
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
            studentSubjectsAllocation.Clear();
            await LoadStudentsMarks();
        }

        bool UseCBTColumn()
        {
            if (reportTypeID == 2)
            {
                if (!otherSetting.BoolValue)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            return false;
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
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            classid = 0;
            classTeacherID = 0;
            selectedClass = string.Empty;
            classList.Clear();
            classList = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + schid + "/0");

            subjectid = 0;
            selectedSubject = string.Empty;
            SelectedTeachersSubjects.Clear();

            students.Clear();
            cognitiveMarks.Clear();
        }
        #endregion

        #region [Mid-Term Exam]

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
                        studentCBTScores = await studentCBTService.GetAllAsync("AcademicsCBT/GetCBTStudentScores/1/" + _examID + "/0/" + true);

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
                cognitiveMark.Mark_MidCBT = selectedItemCognitiveMark.Mark_MidCBT;
                cognitiveMark.Mark_Mid = selectedItemCognitiveMark.Mark_Mid;
                cognitiveMark.GradeID_Mid = 0;
                cognitiveMark.EntryStatus_MidTerm = _entryStatus;

                await studentMarksCognitiveService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 1, cognitiveMark);
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
                    cognitiveMark.Mark_MidCBT = mark.Mark_MidCBT;
                    cognitiveMark.Mark_Mid = mark.Mark_Mid;
                    cognitiveMark.GradeID_Mid = 0;
                    cognitiveMark.EntryStatus_MidTerm = _entryStatus;

                    await studentMarksCognitiveService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 1, cognitiveMark);
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
                await Swal.FireAsync("Mid-Term Exam. Operation Completed Successfully", "Student Marks Succesfully Saved.", "success");
            }
        }


        #endregion

        #region [Term End Exam]

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
                        studentCBTScores =
                            await studentCBTService.GetAllAsync("AcademicsCBT/GetCBTStudentScores/1/" + _examID + "/0/true");
                        // [Route("GetCBTStudentScores/{id}/{examid}/{stdid}/{cbttouse}")]
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
                        studentCBTScores =
                            await studentCBTService.GetAllAsync("AcademicsCBT/GetCBTStudentScores/1/" + _examID + "/0/true");
                        // [Route("GetCBTStudentScores/{id}/{examid}/{stdid}/{cbttouse}")]
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
                    if (_exam > 0)
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
                cognitiveMark.Mark_CA1 = selectedItemCognitiveMark.Mark_CA1;
                cognitiveMark.Mark_CA2 = selectedItemCognitiveMark.Mark_CA2;
                cognitiveMark.Mark_CA3 = selectedItemCognitiveMark.Mark_CA3;
                cognitiveMark.Mark_CBT = selectedItemCognitiveMark.Mark_CBT;
                cognitiveMark.Mark_Exam = selectedItemCognitiveMark.Mark_Exam;
                cognitiveMark.GradeID = 0;
                cognitiveMark.EntryStatus_TermEnd = _entryStatus;

                await studentMarksCognitiveService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 2, cognitiveMark);
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
                    cognitiveMark.Mark_CA1 = item.Mark_CA1;
                    cognitiveMark.Mark_CA2 = item.Mark_CA2;
                    cognitiveMark.Mark_CA3 = item.Mark_CA3;
                    cognitiveMark.Mark_CBT = item.Mark_CBT;
                    cognitiveMark.Mark_Exam = item.Mark_Exam;
                    cognitiveMark.GradeID = 0;
                    cognitiveMark.EntryStatus_TermEnd = _entryStatus;

                    await studentMarksCognitiveService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 2, cognitiveMark);
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
                await Swal.FireAsync("End of Term. Operation Completed Successfully", "Student Marks Succesfully Saved.", "success");
            }
        }


        #endregion

        #region [Click Events]
        async Task CognitiveMarkEvent()
        {
            await RefreshCogvitive();
            SelectedExam();
        }

        #endregion

    }
}
