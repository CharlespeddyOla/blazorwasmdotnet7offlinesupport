using Microsoft.AspNetCore.Components;
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
    public partial class OnlineMarkEntryCheckPoint
    {
        #region [Injection Declaration]
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<SETSchSessions> schoolSessionsService { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassList> classService { get; set; }
        [Inject] IAPIServices<ADMStudents> studentService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksCognitive> studentMarksCognitiveService { get; set; }
        [Inject] IAPIServices<ACDSettingsMarks> markSettingService { get; set; }
        [Inject] IAPIServices<ACDSbjAllocationTeachers> subjectAllocationTeacherService { get; set; }
        [Inject] IAPIServices<ACDSbjAllocationStudents> subjectAllocationStudentService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Student Check Point / IGCSE Marks Entry";
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            schoolSession = await localStorageService.ReadEncryptedItemAsync<int>("schoolsession");
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

        bool IsCheckPointClass { get; set; }
        bool IsIGCSEClass { get; set; }
        bool MarkEntryValidationResult { get; set; } = false;
        #endregion

        #region [Models Declaration]
        List<SETSchSessions> sessions = new();
        List<ADMSchlList> schools = new();
        List<ADMSchClassList> classList = new();
        List<ADMStudents> students = new();
        List<ACDSettingsMarks> markSettingsList = new();
        List<ACDStudentsMarksCognitive> cognitiveMarks = new();
        List<ACDStudentsMarksCognitive> cognitiveMarksSTDID = new();
        List<ACDSbjAllocationTeachers> SelectedTeachersSubjects = new();
        List<ACDSbjAllocationStudents> studentSubjectsAllocation = new();
        List<string> noSubjectAllocationStudents = new();
        List<string> marksErrorLisitng = new();
        ACDStudentsMarksCognitive cognitiveMark = new();
        ACDStudentsMarksCognitive selectedItemCognitiveMark = null;

        #endregion

        #region [Load And Click Events]
        async Task LoadList()
        {
            sessions = await schoolSessionsService.GetAllAsync("Settings/GetAccademicSessions/1/0");
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            markSettingsList = await markSettingService.GetAllAsync("AcademicsMarkSettings/GetMarkSettings/1");
            maxTermID = sessions.FirstOrDefault(s => s.TermID == sessions.Max(t => t.TermID)).TermID;
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

        async Task LoadStudents(int id)
        {
            if (maxTermID == termid)
            {
                students = await studentService.GetAllAsync("AdminStudent/GetStudents/3/0/" + id + "/0/1");
            }
            else
            {
                //students = await studentService.GetAllAsync("AdminStudent/GetStudents/7/0/" + id + "/0/0");
                cognitiveMarksSTDID = await studentMarksCognitiveService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/21/" +
                                        termid + "/" + schid + "/" + classid + "/0/0/0");

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
            IsCheckPointClass = classList.FirstOrDefault(c => c.ClassID == classid).CheckPointClass;
            IsIGCSEClass = classList.FirstOrDefault(c => c.ClassID == classid).IGCSEClass;

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

        #region [Section - CheckPoint And IGSCE Mark Entry ]

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
                cognitiveMark.Mark_ICGC = selectedItemCognitiveMark.Mark_ICGC;
                cognitiveMark.GradeID_ICGC = 0;
                cognitiveMark.EntryStatus_ICGCS = _entryStatus;

                await studentMarksCognitiveService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 3, cognitiveMark);
            }
        }

        async Task SaveIGSCE()
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
                    cognitiveMark.Mark_ICGC = mark.Mark_ICGC;
                    cognitiveMark.GradeID_ICGC = 0;
                    cognitiveMark.EntryStatus_ICGCS = _entryStatus;

                    await studentMarksCognitiveService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 3, cognitiveMark);
                }

                StateHasChanged();
            }

            IsShow = true;

            if (k != 0)
            {
                string studentNames = string.Join("\n", marksErrorLisitng.ToArray());

                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
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

    }
}
