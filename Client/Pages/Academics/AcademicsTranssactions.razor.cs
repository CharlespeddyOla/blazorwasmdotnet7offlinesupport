using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Academics.Subjects;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Administration.Staff;
using WebAppAcademics.Shared.Models.Administration.Students;
using WebAppAcademics.Shared.Models.Settings;
using WebAppAcademics.Client.Extensions;

namespace WebAppAcademics.Client.Pages.Academics
{
    public partial class AcademicsTranssactions
    {
        #region [Injection Declaration]
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<SETSchSessions> schoolSessionsService { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassList> classService { get; set; }
        [Inject] IAPIServices<ADMStudents> studentService { get; set; }
        [Inject] IAPIServices<ADMEmployee> staffService { get; set; }
        [Inject] IAPIServices<ACDSubjects> subjectService { get; set; }
        [Inject] IAPIServices<ACDSbjClassification> subjectClassificationService { get; set; }
        [Inject] IAPIServices<ACDSbjAllocationTeachers> subjectAllocationTeacherService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksCognitive> studentMarksCognitiveService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksAssessment> studentOtherMarksService { get; set; }
        [Inject] IAPIServices<ACDSettingsMarks> markSettingService { get; set; }
        [Inject] IAPIServices<ACDSettingsRatingOptions> ratingOptionsService { get; set; }
        [Inject] IAPIServices<ACDSettingsRatingText> ratingTextService { get; set; }
        [Inject] IAPIServices<ACDSettingsRating> ratingService { get; set; }


        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Academics Transactions";
            InitializeModels();           
            toolBarMenuId = 3;
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            academicSession = await localStorageService.ReadEncryptedItemAsync<string>("academicsession");
            await LoadDefaultList();          
            await base.OnInitializedAsync();
        }

        #region [Models Declaration]
        List<SETSchSessions> sessions { get; set; }
        List<ADMSchlList> schools { get; set; }
        List<ADMSchClassList> classList { get; set; }
        List<ADMStudents> students { get; set; }
        List<ADMEmployee> staffs { get; set; }
        List<ACDSubjects> subjects { get; set; }
        List<ACDSbjClassification> subjectsClassifications { get; set; }
        List<ACDSbjAllocationTeachers> subjectAllocations { get; set; }
        List<ACDStudentsMarksCognitive> cognitiveMarks { get; set; }      
        List<ACDSettingsMarks> markSettingsList { get; set; }

        ACDStudentsMarksCognitive cognitiveMark = new ACDStudentsMarksCognitive();
        ACDStudentsMarksCognitive selectedItemCognitiveMark = null;

        //Psycho, Others
        List<ACDStudentsMarksAssessment> otherMarks { get; set; }
        List<ACDSettingsRatingOptions> ratingOptionsList { get; set; }
        List<ACDSettingsRatingText> ratingTextList { get; set; }
        List<ACDSettingsRating> ratingList { get; set; }

        ACDStudentsMarksAssessment otherMark = new ACDStudentsMarksAssessment();
        ACDStudentsMarksAssessment selectedItemOtherMark = null;


        async void InitializeModels()
        {
            sessions = new List<SETSchSessions>();
            schools = new List<ADMSchlList>();
            classList = new List<ADMSchClassList>();
            students = new List<ADMStudents>();
            staffs = new List<ADMEmployee>();
            subjects = new List<ACDSubjects>();
            subjectsClassifications = new List<ACDSbjClassification>();
            subjectAllocations = new List<ACDSbjAllocationTeachers>();
            otherMarks = new List<ACDStudentsMarksAssessment>();           
            markSettingsList = new List<ACDSettingsMarks>();

            //Psycho, Others
            ratingOptionsList = new List<ACDSettingsRatingOptions>();
            ratingTextList = new List<ACDSettingsRatingText>();
            ratingList = new List<ACDSettingsRating>();
            

            await Task.Delay(500); // simulate loading
            cognitiveMarks = new List<ACDStudentsMarksCognitive>();

            await Task.Delay(500); // simulate loading
            otherMarks = new List<ACDStudentsMarksAssessment>();
        }
        #endregion

        #region [Variables Declaration]
        int toolBarMenuId { get; set; }
        int termid { get; set; }
        int maxTermID { get; set; }
        int schid { get; set; }
        int classid { get; set; }
        int classlistid { get; set; }
        int stdid { get; set; }
        int subjectid { get; set; }
        int sbjClassid { get; set; }
        int staffid { get; set; }
        int classTeacherID { get; set; }
        int schoolPrincipalID { get; set; }

        string academicSession { get; set; }
        string selectedSchool { get; set; }
        string selectedClass { get; set; }
        string selectedStudent { get; set; }
        string selectedSubject { get; set; }
        string selectedTeacher { get; set; }
        string classTeacher { get; set; }
        string selectedSubjectClassification { get; set; }
        string selectedSubjectTeacher { get; set; } = string.Empty;

        bool isLoading { get; set; } = true;
        string loadingmessage { get; set; } = "Waiting for your selection...";
        string headermessage { get; set; }

        string cognitiveSearchString { get; set; } = string.Empty;
        string otherMarksSearchString { get; set; } = string.Empty;
        bool ToggleDeleteColumn { get; set; }
        bool ShowDeleteColumn { get; set; }
        bool MarkEntryValidationResult { get; set; } = false;

        //Psycho, Others
        int _ratingID { get; set; }
        int ratingOptionID { get; set; }
        int ratingTextID { get; set; }

        bool RatingValueCheck { get; set; }
        #endregion

        #region [Section - Load And Click Events]
        async Task LoadDefaultList()
        {
            sessions = await schoolSessionsService.GetAllAsync("Settings/GetAccademicSessions/1/0");
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            maxTermID = sessions.FirstOrDefault(s => s.TermID == sessions.Max(t => t.TermID)).TermID;
            staffs = await staffService.GetAllAsync("AdminStaff/GetStaffs/0/0/0/0/0");
            markSettingsList = await markSettingService.GetAllAsync("AcademicsMarkSettings/GetMarkSettings/1");

            //Psycho, Others
            ratingList = await ratingService.GetAllAsync("AcademicsMarkSettings/GeRatingSettings/1");
            ratingOptionsList = await ratingOptionsService.GetAllAsync("AcademicsMarkSettings/GeRatingOptionSettings/1");
            ratingTextList = await ratingTextService.GetAllAsync("AcademicsMarkSettings/GeRatingTextSettings/1");

            ratingOptionID = ratingOptionsList.FirstOrDefault(o => o.UsedOption == true).OptionID;
            ratingTextID = ratingTextList.FirstOrDefault(t => t.UsedText == true).TextID;
        }

        async Task LoadStudents(int id)
        {
            if (maxTermID == termid)
            {
                students = await studentService.GetAllAsync("AdminStudent/GetStudents/3/0/" + id + "/0/1");
            }
            else
            {
                students = await studentService.GetAllAsync("AdminStudent/GetStudents/7/0/" + id + "/0/0");
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
            classList = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + schid + "/0");

            selectedSubject = string.Empty;
            subjects.Clear();

            selectedStudent = string.Empty;
            students.Clear();

            if (toolBarMenuId == 3)
            {
                cognitiveMarks.Clear();
                selectedSubject = string.Empty;
                subjects.Clear();
                subjects = await subjectService.GetAllAsync("AcademicsSubjects/GetSubjects/6/" + schid + "/0/1/true");
            }
            else if (toolBarMenuId == 4)
            {
                otherMarks.Clear();
                selectedSubjectClassification = string.Empty;
                subjectsClassifications.Clear();
                subjectsClassifications = await subjectClassificationService.GetAllAsync("AcademicsSubjects/GetSubjectsClassifications/2");
            }
        }

        async Task OnSelectedClassChanged(IEnumerable<string> e)
        {
            selectedClass = e.ElementAt(0);
            classid = classList.FirstOrDefault(c => c.ClassName == selectedClass).ClassID;
            classTeacherID = classList.FirstOrDefault(c => c.ClassName == selectedClass).StaffID;
            classTeacher = classList.FirstOrDefault(c => c.ClassName == selectedClass).ClassTeacher;

            subjectAllocations.Clear();
            subjectAllocations = await subjectAllocationTeacherService.GetAllAsync(
            "AcademicsSubjects/GetTeacherAllocations/10/true/" + termid + "/" + schid + "/0/" + classid + "/" + subjectid + "/0");

            students.Clear();
            cognitiveMarks.Clear();
            otherMarks.Clear();
            await LoadStudents(classid);

            if (toolBarMenuId == 3)
            {
                cognitiveMarks.Clear();
            }
            else if (toolBarMenuId == 4)
            {
                otherMarks.Clear();
            }
        }

        void OnSelectedStudentChanged(IEnumerable<string> e)
        {
            selectedStudent = e.ElementAt(0);
            stdid = students.FirstOrDefault(s => s.StudentNameWithNo == selectedStudent).STDID;

            if (toolBarMenuId == 3)
            {
                cognitiveMarks.Clear();
            }
            else if (toolBarMenuId == 4)
            {
                otherMarks.Clear();
            }
        }

        async Task OnSelectedSubjectChanged(IEnumerable<string> e)
        {
            selectedSubject = e.ElementAt(0);
            subjectid = subjects.FirstOrDefault(s => s.Subject == selectedSubject).SubjectID;

            subjectAllocations.Clear();
            subjectAllocations = await subjectAllocationTeacherService.GetAllAsync(
            "AcademicsSubjects/GetTeacherAllocations/10/true/" + termid + "/" + schid + "/0/" + classid + "/" + subjectid + "/0");

            if (toolBarMenuId == 3)
            {
                cognitiveMarks.Clear();
            }
            else if (toolBarMenuId == 4)
            {
                otherMarks.Clear();
            }
        }

        void OnSelectedTeacherChanged(IEnumerable<string> e)
        {
            selectedTeacher = e.ElementAt(0);
            staffid = staffs.FirstOrDefault(s => s.StaffNameWithNo == selectedTeacher).StaffID;

            if (toolBarMenuId == 3)
            {
                cognitiveMarks.Clear();
            }
            else if (toolBarMenuId == 4)
            {
                otherMarks.Clear();
            }
        }

        #endregion

        #region [Section - Cognitive Mark Transactions]

        async Task LoadCognitiveMarks(int switchid, int _termid, int _schid, int _classid, int _stdid, int _staffid, int _subjectid)
        {
            isLoading = true;
            StateHasChanged();

            try
            {
                await Task.Delay(500);
                cognitiveMarks = await studentMarksCognitiveService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/" + switchid + "/" +
                                            _termid + "/" + _schid + "/" + _classid + "/" + _stdid + "/" + _staffid + "/" + _subjectid);
            }
            finally
            {
                isLoading = false;
            }
        }

        int GetCognitiveFilterID()
        {
            if (!String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClass) &&
                String.IsNullOrWhiteSpace(selectedStudent) && String.IsNullOrWhiteSpace(selectedSubject) &&
                String.IsNullOrWhiteSpace(selectedTeacher))
            {
                //Filter By School
                headermessage = academicSession + " MARKS FOR " + selectedSchool;
                return 9;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClass) &&
                String.IsNullOrWhiteSpace(selectedStudent) && String.IsNullOrWhiteSpace(selectedSubject) &&
                String.IsNullOrWhiteSpace(selectedTeacher))
            {
                //Filter By School And Class
                headermessage = academicSession + " MARKS FOR " + selectedClass;
                return 10;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClass) &&
                !String.IsNullOrWhiteSpace(selectedStudent) && String.IsNullOrWhiteSpace(selectedSubject) &&
                String.IsNullOrWhiteSpace(selectedTeacher))
            {
                //Filter By  Student
                headermessage = academicSession + " MARKS FOR " + selectedStudent;
                return 11;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClass) &&
                    !String.IsNullOrWhiteSpace(selectedStudent) && !String.IsNullOrWhiteSpace(selectedSubject) &&
                    String.IsNullOrWhiteSpace(selectedTeacher))
            {
                //Filter By Student and Subject
                //string selectedSubjectTeacher = subjectAllocations.FirstOrDefault(s => s.ClassID == classid && s.SubjectID == subjectid).SubjectTeacher;
                if (subjectAllocations.Count() > 0)
                {
                    selectedSubjectTeacher = subjectAllocations.FirstOrDefault().SubjectTeacher;
                }                
                headermessage = academicSession + " MARKS FOR " + selectedStudent + ": " + selectedSubject + " - " + selectedSubjectTeacher;
                return 12;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClass) &&
               String.IsNullOrWhiteSpace(selectedStudent) && !String.IsNullOrWhiteSpace(selectedSubject) &&
               String.IsNullOrWhiteSpace(selectedTeacher))
            {
                //Filter By School And Subject
                headermessage = academicSession + " MARKS FOR " + selectedSchool + ": " + selectedSubject;
                return 13;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClass) &&
                String.IsNullOrWhiteSpace(selectedStudent) && !String.IsNullOrWhiteSpace(selectedSubject) &&
                String.IsNullOrWhiteSpace(selectedTeacher))
            {
                //Filter By School, Class and Subject
                //string selectedSubjectTeacher = subjectAllocations.FirstOrDefault(s => s.ClassID == classid && s.SubjectID == subjectid).SubjectTeacher;
                if (subjectAllocations.Count() > 0)
                {
                    selectedSubjectTeacher = subjectAllocations.FirstOrDefault().SubjectTeacher;
                }
                headermessage = academicSession + " MARKS FOR " + selectedClass + ": " + selectedSubject + " - " + selectedSubjectTeacher;
                return 14;
            }
            else if (String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClass) &&
                      String.IsNullOrWhiteSpace(selectedStudent) && String.IsNullOrWhiteSpace(selectedSubject) &&
                      !String.IsNullOrWhiteSpace(selectedTeacher))
            {
                //Filter By Teacher
                headermessage = academicSession + " MARKS FOR " + selectedTeacher;
                return 15;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClass) &&
              String.IsNullOrWhiteSpace(selectedStudent) && String.IsNullOrWhiteSpace(selectedSubject) &&
              !String.IsNullOrWhiteSpace(selectedTeacher))
            {
                //Filter By School And Teacher
                headermessage = academicSession + " MARKS FOR " + selectedSchool + ": " + selectedTeacher;
                return 16;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClass) &&
                    String.IsNullOrWhiteSpace(selectedStudent) && !String.IsNullOrWhiteSpace(selectedSubject) &&
                    !String.IsNullOrWhiteSpace(selectedTeacher))
            {
                //Filter By School, Subject And Teacher
                headermessage = academicSession + " MARKS FOR " + selectedTeacher + ": " + selectedSubject;
                return 17;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClass) &&
                String.IsNullOrWhiteSpace(selectedStudent) && String.IsNullOrWhiteSpace(selectedSubject) &&
                !String.IsNullOrWhiteSpace(selectedTeacher))
            {
                //Filter By School, Class and Teacher
                headermessage = academicSession + " MARKS FOR " + selectedClass + ": " + selectedTeacher;
                return 18;
            }

            return 0;
        }

        async Task ListCognitiveMarksTransactions()
        {
            loadingmessage = "Please wait, while loading...";
            switch (GetCognitiveFilterID())
            {
                case 9:
                    await LoadCognitiveMarks(9, termid, schid, classid, stdid, staffid, subjectid);
                    break;
                case 10:
                    await LoadCognitiveMarks(10, termid, schid, classid, stdid, staffid, subjectid);
                    break;
                case 11:
                    await LoadCognitiveMarks(11, termid, schid, classid, stdid, staffid, subjectid);
                    break;
                case 12:
                    await LoadCognitiveMarks(12, termid, schid, classid, stdid, staffid, subjectid);
                    break;
                case 13:
                    await LoadCognitiveMarks(13, termid, schid, classid, stdid, staffid, subjectid);
                    break;
                case 14:
                    await LoadCognitiveMarks(14, termid, schid, classid, stdid, staffid, subjectid);
                    break;
                case 15:
                    await LoadCognitiveMarks(15, termid, schid, classid, stdid, staffid, subjectid);
                    break;
                case 16:
                    await LoadCognitiveMarks(16, termid, schid, classid, stdid, staffid, subjectid);
                    break;
                case 17:
                    await LoadCognitiveMarks(17, termid, schid, classid, stdid, staffid, subjectid);
                    break;
                case 18:
                    await LoadCognitiveMarks(18, termid, schid, classid, stdid, staffid, subjectid);
                    break;
            }
        }

        async Task RefreshCognitiveTrans()
        {
            headermessage = string.Empty;
            loadingmessage = "Waiting for your selection...";

            ToggleDeleteColumn = false;

            selectedSchool = string.Empty;
            schid = 0;
            schoolPrincipalID = 0;
            schools.Clear();
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            classid = 0;
            classTeacherID = 0;
            selectedClass = string.Empty;
            classList.Clear();
            classList = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + schid + "/0");

            selectedStudent = string.Empty;
            students.Clear();

            subjectid = 0;
            selectedSubject = string.Empty;
            selectedSubjectTeacher = string.Empty;
            subjects.Clear();

            selectedTeacher = string.Empty;
            staffs.Clear();
            staffs = await staffService.GetAllAsync("AdminStaff/GetStaffs/0/0/0/0/0");

            cognitiveMarks.Clear();
            cognitiveSearchString = string.Empty;            
        }

        private bool CognitiveFilterFunc(ACDStudentsMarksCognitive model)
        {
            if (string.IsNullOrWhiteSpace(cognitiveSearchString))
                return true;
            if (model.Subject.Contains(cognitiveSearchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.StudentName.Contains(cognitiveSearchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.Teacher.Contains(cognitiveSearchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        async Task UpdateCognitiveMarks()
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
                await Swal.FireAsync("Invalid Mid-Term Mark", selectedItemCognitiveMark.SN + ". " +
                                       selectedItemCognitiveMark.StudentName + ": " + selectedItemCognitiveMark.Mark_Mid + " > " +
                                       markSettingsList.FirstOrDefault(m => m.MarkID == 1).Mark + ".", "error");
                MarkEntryValidationResult = true;
            }
            else if (selectedItemCognitiveMark.Mark_CA1 > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 2).Mark))
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
            else if (selectedItemCognitiveMark.Mark_CBT > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 5).Mark))
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
            else if (selectedItemCognitiveMark.Mark_ICGC > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 7).Mark))
            {
                await Swal.FireAsync("Invalid Check Point / IGCSE Mark", selectedItemCognitiveMark.SN + ". " +
                                        selectedItemCognitiveMark.StudentName + ": " + selectedItemCognitiveMark.Mark_ICGC + " > " +
                                        markSettingsList.FirstOrDefault(m => m.MarkID == 7).Mark + ".", "error");
                MarkEntryValidationResult = true;
            }

            if (!MarkEntryValidationResult)
            {
                cognitiveMark.StudentMarkID = selectedItemCognitiveMark.StudentMarkID;
                cognitiveMark.Mark_MidCBT = selectedItemCognitiveMark.Mark_MidCBT;
                cognitiveMark.Mark_Mid = selectedItemCognitiveMark.Mark_Mid;
                cognitiveMark.Mark_ICGC = selectedItemCognitiveMark.Mark_ICGC;
                cognitiveMark.Mark_CA1 = selectedItemCognitiveMark.Mark_CA1;
                cognitiveMark.Mark_CA2 = selectedItemCognitiveMark.Mark_CA2;
                cognitiveMark.Mark_CA3 = selectedItemCognitiveMark.Mark_CA3;
                cognitiveMark.Mark_CBT = selectedItemCognitiveMark.Mark_CBT;
                cognitiveMark.Mark_Exam = selectedItemCognitiveMark.Mark_Exam;
                cognitiveMark.EntryStatus_ICGCS = selectedItemCognitiveMark.EntryStatus_ICGCS;
                cognitiveMark.EntryStatus_MidTerm = selectedItemCognitiveMark.EntryStatus_MidTerm;
                cognitiveMark.EntryStatus_TermEnd = selectedItemCognitiveMark.EntryStatus_TermEnd;

                await studentMarksCognitiveService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 12, cognitiveMark);
                await Swal.FireAsync("Update Operation", "Selected Student Marks Successfully Updated.", "success");
            }
            else
            {
                await Swal.FireAsync("Student Mark Entry Error",
                    "Please, Check Your Entry. You have Entered A Mark Greater Than The Maximum Mark Set.", "error");
            }
        }

        async Task DeleteMarkCognitive(int studentmarkid)
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Delete Mark Operation",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                cognitiveMark.StudentMarkID = studentmarkid;
                cognitiveMark.Mark_Delete = true;
                await studentMarksCognitiveService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 7, cognitiveMark);
                await Swal.FireAsync("Delete Operation", "Selected Student Marks Deleted Successfully.", "success");
                await ListCognitiveMarksTransactions();
            }
            else
            {
                await Swal.FireAsync("Delete Operation", "Operation Aborted!", "error");
            }
        }

        void OnToggledChanged(bool toggled)
        {
            // Because variable is not two-way bound, we need to update it ourself
            ToggleDeleteColumn = toggled;

            if (ToggleDeleteColumn)
            {
                ShowDeleteColumn = true;
            }
            else
            {
                ShowDeleteColumn = false;
            }
        }

        #endregion

        #region [Section - Other Marks Transactions]
        void OnSelectedSubjectClassificationChanged(IEnumerable<string> e)
        {
            selectedSubjectClassification = e.ElementAt(0);
            sbjClassid = subjectsClassifications.FirstOrDefault(s => s.SbjClassification == selectedSubjectClassification).SbjClassID;

            otherMarks.Clear();
        }

        async Task LoadotherMarks(int switchid, int _termid, int _schid, int _classid, int _sbjclassid, 
                                    int _subjectid, int _stdid, int _staffid)
        {
            isLoading = true;
            StateHasChanged();

            try
            {
                await Task.Delay(500);
                otherMarks = await studentOtherMarksService.GetAllAsync("AcademicsMarks/GetOtherMarks/" + switchid  + "/" + 
                    _termid + "/" + _schid + "/" + _classid + "/" + _sbjclassid + "/" + _subjectid + "/" + _stdid + "/" + _staffid);
            }
            finally
            {
                isLoading = false;
            }
        }

        int GetOtherMarksFilterID()
        {
            if (!String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClass) &&
                String.IsNullOrWhiteSpace(selectedStudent) && String.IsNullOrWhiteSpace(selectedSubjectClassification))
            {
                //Filter By School
                headermessage = academicSession + " MARKS FOR " + selectedSchool;
                return 4;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClass) &&
                        String.IsNullOrWhiteSpace(selectedStudent) && String.IsNullOrWhiteSpace(selectedSubjectClassification))
            {
                //Filter By Class
                headermessage = academicSession + " MARKS FOR " + selectedClass + " - " + classTeacher;
                return 5;
            }
            else  if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClass) &&
                        !String.IsNullOrWhiteSpace(selectedStudent) && String.IsNullOrWhiteSpace(selectedSubjectClassification))
            {
                //Filter By  Student
                headermessage = academicSession + " MARKS FOR " + selectedStudent;
                return 6;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClass) &&
                      !String.IsNullOrWhiteSpace(selectedStudent) && !String.IsNullOrWhiteSpace(selectedSubjectClassification))
            {
                //Filter By Student and Accessment Type
                headermessage = academicSession + " MARKS FOR " + selectedStudent + ": " + selectedSubject + " - " + classTeacher;
                return 7;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && String.IsNullOrWhiteSpace(selectedClass) &&
                     String.IsNullOrWhiteSpace(selectedStudent) && !String.IsNullOrWhiteSpace(selectedSubjectClassification))
            {
                //Filter By School and Accessment Type
                headermessage = academicSession + " MARKS FOR " + selectedSubject;
                return 8;
            }
            else if (!String.IsNullOrWhiteSpace(selectedSchool) && !String.IsNullOrWhiteSpace(selectedClass) &&
                     String.IsNullOrWhiteSpace(selectedStudent) && !String.IsNullOrWhiteSpace(selectedSubjectClassification))
            {
                //Filter By School, Class and Accessment Type
                headermessage = academicSession + " MARKS FOR " + selectedClass + " - " + selectedSubject;
                return 9;
            }

            return 0;
        }

        async Task ListOtherMarksTransactions()
        {
            loadingmessage = "Please wait, while loading...";
            switch (GetOtherMarksFilterID())
            {
                case 4:
                    await LoadotherMarks(4, termid, schid, classid, sbjClassid, 0, stdid, staffid);
                    break;
                case 5:
                    await LoadotherMarks(5, termid, schid, classid, sbjClassid, 0, stdid, staffid);
                    break;
                case 6:
                    await LoadotherMarks(6, termid, schid, classid, sbjClassid, 0, stdid, staffid);
                    break;
                case 7:
                    await LoadotherMarks(7, termid, schid, classid, sbjClassid, 0, stdid, staffid);
                    break;
                case 8:
                    await LoadotherMarks(8, termid, schid, classid, sbjClassid, 0, stdid, staffid);
                    break;
                case 9:
                    await LoadotherMarks(9, termid, schid, classid, sbjClassid, 0, stdid, staffid);
                    break;
            }
        }

        async Task RefreshOtherMarksTrans()
        {
            headermessage = string.Empty;
            loadingmessage = "Waiting for your selection...";

            selectedSchool = string.Empty;
            schid = 0;
            schoolPrincipalID = 0;
            schools.Clear();
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            classid = 0;
            classTeacherID = 0;
            selectedClass = string.Empty;
            classList.Clear();
            classList = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + schid + "/0");

            selectedStudent = string.Empty;
            students.Clear();

            sbjClassid = 0;
            selectedSubjectClassification = string.Empty;
            subjectsClassifications.Clear();

            otherMarks.Clear();
            otherMarksSearchString = string.Empty;
        }

        private bool OtherMarksFilterFunc(ACDStudentsMarksAssessment model)
        {
            if (string.IsNullOrWhiteSpace(otherMarksSearchString))
                return true;
            if (model.Subject.Contains(otherMarksSearchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.StudentName.Contains(otherMarksSearchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
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
                otherMark.Rating = selectedItemOtherMark.Rating;
                otherMark.OptionID = ratingOptionID;
                otherMark.TextID = ratingTextID;
                otherMark.RatingID = _ratingID;

                await studentOtherMarksService.UpdateAsync("AcademicsMarks/UpdateOtherMark/", 1, otherMark);
                await Swal.FireAsync("Update Operation", "Selected Student Marks Successfully Updated.", "success");
            }
        }

        async Task DeleteOtherMarks(int studentmarkid)
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Delete Mark Operation",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                otherMark.StudentMarkID = studentmarkid;
                otherMark.Mark_Delete = true;
                await studentOtherMarksService.UpdateAsync("AcademicsMarks/UpdateOtherMark/", 2, otherMark);
                await Swal.FireAsync("Delete Operation", "Selected Student Marks Deleted Successfully.", "success");
                await ListOtherMarksTransactions();
            }
            else
            {
                await Swal.FireAsync("Delete Operation", "Operation Aborted!", "error");
            }
        }

        #endregion

        #region [Section - Click Events]

        void RetrieveTeachersSubjectsTrans()
        {
            toolBarMenuId = 1;
        }

        void RetrieveStudentsSubjectsTrans()
        {
            toolBarMenuId = 2;
        }

        async Task RetrieveCognitiveMarks()
        {
            toolBarMenuId = 3;
            await RefreshCognitiveTrans();

            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Retrieve All Students Marks For " + academicSession,
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                headermessage = academicSession + " All Marks";
                loadingmessage = "Please wait, while loading...";
                await LoadCognitiveMarks(0, termid, 0, 0, 0, 0, 0);
            }
            else
            {
                loadingmessage = "Waiting for your selection...";
            }
        }

        async Task RetrieveOtherMarks()
        {
            toolBarMenuId = 4;
            await RefreshOtherMarksTrans();

            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Retrieve All Students Psychomotor And Other Marks For " + academicSession,
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                headermessage = academicSession + " All Marks";
                loadingmessage = "Please wait, while loading...";
                await LoadotherMarks(0, termid, 0, 0, 0, 0, 0, 0);
            }
            else
            {
                loadingmessage = "Waiting for your selection...";
            }
        }

        async Task RefreshTransactions()
        {
            if (toolBarMenuId == 1)
            {

            }
            else if (toolBarMenuId == 2)
            {

            }
            else if (toolBarMenuId == 3)
            {
                await RefreshCognitiveTrans();
            }
            else if (toolBarMenuId == 4)
            {
                await RefreshOtherMarksTrans();
            }
        }

        async Task ListTransactions()
        {
            if (toolBarMenuId == 1)
            {
                
            }
            else if (toolBarMenuId == 2)
            {

            }
            else if (toolBarMenuId == 3)
            {
                await ListCognitiveMarksTransactions();
            }
            else if (toolBarMenuId == 4)
            {
                await ListOtherMarksTransactions();
            }
        }
        #endregion
    }
}
