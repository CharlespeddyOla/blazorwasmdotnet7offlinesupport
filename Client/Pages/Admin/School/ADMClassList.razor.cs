using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Academics.Subjects;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Administration.Staff;
using WebAppAcademics.Shared.Models.Administration.Students;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Client.Pages.Academics.Exam.Marks.Comments;

namespace WebAppAcademics.Client.Pages.Admin.School
{
    public partial class ADMClassList
    {
        #region [Injection Declaration]
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassList> classService { get; set; }
        [Inject] IAPIServices<ADMSchClassGroup> classGroupService { get; set; }
        [Inject] IAPIServices<ADMSchClassCategory> classNamesService { get; set; }
        [Inject] IAPIServices<ADMSchClassDiscipline> classDisciplineService { get; set; }
        [Inject] IAPIServices<ADMStudents> studentService { get; set; }
        [Inject] IAPIServices<ADMEmployee> staffService { get; set; }
        [Inject] IAPIServices<ACDSbjAllocationTeachers> subjectAllocationTeacherService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksCognitive> studentMarksCognitiveService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksAssessment> studentOtherMarksService { get; set; }
        [Inject] IAPIServices<ACDReportCommentsTerminal> termEndCommentService { get; set; }
        [Inject] IAPIServices<ACDReportCommentMidTerm> midTermCommentService { get; set; }
        [Inject] IAPIServices<ACDReportCommentCheckPointIGCSE> checkpointigcseCommentService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int toolBarMenuId { get; set; }
        int termid { get; set; }
        int schid { get; set; }
        int classid { get; set; }
        int classlistid { get; set; }
        int catid { get; set; }
        int disciplineid { get; set; }
        int staffid { get; set; }
        int _classTeacherID { get; set; }

        string ClassTeacherInitials { get; set; }
        string PrincipalInitials { get; set; }
        string selectedSchool { get; set; }
        string selectedClass { get; set; }
        string selectedClassName { get; set; }
        string selectedDiscipline { get; set; }
        string selectedClassTeacher { get; set; }
        bool isLoading { get; set; } = true;
        string pagetitle { get; set; }  = "Create a new Class";
        string buttontitle { get; set; } = "Save";

        bool disableSaveButton { get; set; } = true;
        string editFormId { get; set; } = "editformid";
        #endregion

        #region [Models Declaration]
        List<ADMSchlList> schools = new();
        List<ADMSchClassList> classListAll = new();
        List<ADMSchClassList> classList = new();
        List<ADMSchClassGroup> classGrouplist = new();
        List<ADMSchClassCategory> classNames = new();
        List<ADMSchClassDiscipline> classDisciplinelist = new();
        List<ADMEmployee> staffs = new();

        ADMSchClassList classDetails = new();
        ACDSbjAllocationTeachers teacherSubjectsAllocation = new();
        ACDStudentsMarksCognitive cognitiveMark = new();
        ACDStudentsMarksAssessment otherMark = new();
        ACDReportCommentsTerminal termEndComment = new();
        ACDReportCommentMidTerm midTermComment = new();
        ACDReportCommentCheckPointIGCSE checkpoinigcseComment = new();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Class List";
            toolBarMenuId = 1;
            await LoadList();
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            await base.OnInitializedAsync();
        }

        #region [Section - Class List]
        async Task LoadDefaultList()
        {
            _processing = true;
            timerDisplay = "Please wait, loading list...";
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            classListAll = await classService.GetAllAsync("AdminSchool/GetClassList/0/0/0");
            await LoadClassList();
            _processing = false;
        }

        async Task LoadList()
        {
            stopwatchvalue = new TimeSpan();
            Task task1 = Task.Run(() => LoadDefaultList());
            Task task2 = Task.Run(() => StartStopWatch());

            await Task.WhenAll(task1, task2);

            task1.Dispose();
            task2.Dispose();

            await Task.CompletedTask;
        }

        async Task LoadClassList()
        {
            classList.Clear();
            if (schid == 0)
            {
                foreach (var item in classListAll)
                {
                    var _classTeacher = await staffService.GetByIdAsync("AdminStaff/GetStaff/", item.StaffID);
                    if (_classTeacher != null)
                    {
                        if (_classTeacher.StatusTypeID == 1)
                        {
                            //Active Class Teacher
                            //ClassTeacherInitials = _classTeacher.Surname + " " + GetInitials(_classTeacher.FirstName) + " " + GetInitials(_classTeacher.MiddleName);
                            ClassTeacherInitials = item.ClassTeacherInitials;
                        }
                        else
                        {
                            //In-Active Class Teacher
                            ClassTeacherInitials = string.Empty;
                        }
                    }
                    else
                    {
                        ClassTeacherInitials = string.Empty;
                    }

                    var _principal = await staffService.GetByIdAsync("AdminStaff/GetStaff/", item.PrincipalID);
                    if (_principal != null)
                    {
                        if (_principal.StatusTypeID == 1)
                        {
                            //Active Principal
                            PrincipalInitials = item.PrincipalInitials;
                        }
                        else
                        {
                            //In-Active Principal
                            PrincipalInitials = string.Empty;
                        }
                    }
                    else
                    {
                        PrincipalInitials = string.Empty;
                    }

                    int _studentCount = await studentService.CountAsync("AdminStudent/GetCount/1/0/" + item.ClassID + "/0/", 1);

                    classList.Add(new ADMSchClassList
                    {
                        SchID = item.SchID,
                        School = item.School,
                        ClassID = item.ClassID,
                        ClassName = item.ClassName,
                        ClassCount = _studentCount,
                        Discipline = item.Discipline,
                        ClassTeacherInitials = ClassTeacherInitials,
                        PrincipalInitials = PrincipalInitials,
                    });
                }
            }
            else
            {
                foreach (var item in classListAll.Where(c => c.SchID == schid))
                {
                    var _classTeacher = await staffService.GetByIdAsync("AdminStaff/GetStaff/", item.StaffID);
                    if (_classTeacher != null)
                    {
                        if (_classTeacher.StatusTypeID == 1)
                        {
                            //Active Class Teacher
                            //ClassTeacherInitials = _classTeacher.Surname + " " + GetInitials(_classTeacher.FirstName) + " " + GetInitials(_classTeacher.MiddleName);
                            ClassTeacherInitials = item.ClassTeacherInitials;
                        }
                        else
                        {
                            //In-Active Class Teacher
                            ClassTeacherInitials = string.Empty;
                        }
                    }
                    else
                    {
                        ClassTeacherInitials = string.Empty;
                    }

                    var _principal = await staffService.GetByIdAsync("AdminStaff/GetStaff/", item.PrincipalID);
                    if (_principal != null)
                    {
                        if (_principal.StatusTypeID == 1)
                        {
                            //Active Principal
                            PrincipalInitials = item.PrincipalInitials;
                        }
                        else
                        {
                            //In-Active Principal
                            PrincipalInitials = string.Empty;
                        }
                    }
                    else
                    {
                        PrincipalInitials = string.Empty;
                    }

                    int _studentCount = await studentService.CountAsync("AdminStudent/GetCount/1/0/" + item.ClassID + "/0/", 1);

                    classList.Add(new ADMSchClassList
                    {
                        SchID = item.SchID,
                        School = item.School,
                        ClassID = item.ClassID,
                        ClassName = item.ClassName,
                        ClassCount = _studentCount,
                        Discipline = item.Discipline,
                        ClassTeacherInitials = ClassTeacherInitials,
                        PrincipalInitials = PrincipalInitials,
                    });
                }
            }
        }

        string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }
            var splitted = name?.Split(' ');
            var initials = $"{splitted[0][0]}{(splitted.Length > 1 ? splitted[splitted.Length - 1][0] : (char?)null)}";
            return initials;
        }

        async Task OnSelectedSchoolChanged(IEnumerable<string> e)
        {
            selectedSchool = e.ElementAt(0);
            schid = schools.FirstOrDefault(s => s.School == selectedSchool).SchID;
            await LoadList();
        }

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

        #endregion

        #region [Section - Class Details]
        async Task LoadListForDetails()
        {
            classNames = await classNamesService.GetAllAsync("AdminSchool/GetCategories/1");
            classDisciplinelist = await classDisciplineService.GetAllAsync("AdminSchool/GetDisciplines/1");
            staffs = await staffService.GetAllAsync("AdminStaff/GetStaffs/8/1/0/0/0");
        }

        async Task RetrieveClassDetails(int _classid)
        {
            // Change page title and button text since this is an edit.
            pagetitle = "Edit Class Details";
            buttontitle = "Update";
            var c = await classService.GetByIdAsync("AdminSchool/GetClass/", _classid);
            classid = _classid;
            classDetails.ClassID = classid;
            classDetails.SchID = c.SchID;
            classDetails.School = c.School;
            classDetails.ClassListID = c.ClassListID;
            classDetails.UseConvension = c.UseConvension;
            classDetails.SchClass = c.SchClass;
            classDetails.ConvensionalName = c.ConvensionalName;
            classDetails.ClassGroupName = c.ClassGroupName;
            classDetails.CATID = c.CATID;
            classDetails.CATName = c.CATName;
            classDetails.DisciplineID = c.DisciplineID;
            classDetails.Discipline = c.Discipline;
            classDetails.StaffID = c.StaffID;
            classDetails.ClassTeacherWithNo = c.ClassTeacherWithNo;
            classDetails.JuniorFinalYearClass = c.JuniorFinalYearClass;
            classDetails.FinalYearClass = c.FinalYearClass;
            classDetails.CheckPointClass = c.CheckPointClass;
            classDetails.IGCSEClass = c.IGCSEClass;           
        }

        async Task OnSchoolChanged(IEnumerable<string> value)
        {
            selectedSchool = value.ElementAt(0);
            classDetails.School = selectedSchool;
            schid = schools.FirstOrDefault(s => s.School == selectedSchool).SchID;
            classDetails.SchID = schid;

            classGrouplist = await classGroupService.GetAllAsync("AdminSchool/GetClassGroups/1/" + schid);
        }

        void OnClassChanged(IEnumerable<string> value)
        {
            selectedClass = value.ElementAt(0);
            classlistid = classGrouplist.FirstOrDefault(s => s.ClassName == selectedClass).ClassListID;
            classDetails.ClassListID = classlistid;
        }

        void OnClassNameChanged(IEnumerable<string> value)
        {
            selectedClassName = value.ElementAt(0);
            catid = classNames.FirstOrDefault(s => s.CATName == selectedClassName).CATID;
            classDetails.CATID = catid;
        }

        void OnDisciplineChanged(IEnumerable<string> value)
        {
            selectedDiscipline = value.ElementAt(0);
            disciplineid = classDisciplinelist.FirstOrDefault(s => s.Discipline == selectedDiscipline).DisciplineID;
            classDetails.DisciplineID = disciplineid;
        }

        void OnClassTeacherChanged(IEnumerable<string> value)
        {
            selectedClassTeacher = value.ElementAt(0);
            staffid = staffs.FirstOrDefault(s => s.StaffNameWithNo == selectedClassTeacher).StaffID;
            classDetails.StaffID = staffid;
            _classTeacherID = staffid;
        }

        async Task<bool> NewClassValidation()
        {
            bool result = false;

            classList = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + classDetails.SchID + "/0");
            int _classlistID = classDetails.ClassListID;
            int _catID = classDetails.CATID;
            bool ClassGroupExist = classList.Where(c => c.ClassListID == _classlistID).Any();
            bool ClassNameExist = classList.Where(c => c.CATID == _catID).Any();

            if (ClassGroupExist == true && ClassNameExist == true)
            {
                result = true;
            }

            return result;
        }

        async Task<bool> SaveOptionValidation()
        {
            bool result = false;

            if (classid == 0)
            {
                if (await NewClassValidation())
                {
                    result = true;
                }
            }

            return result;
        }

        async Task SubmitValidForm()
        {            
            if (await SaveOptionValidation())
            {
                await Swal.FireAsync("Class Duplication!", selectedClass + " " + selectedClassName + " Already Created " + ".", "error");
            }
            else
            {
                if (_classTeacherID != 0)
                {
                    SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                    {
                        Title = "Create/Update School Term Details Operation",
                        Text = "Do You Want To Continue With This Operation?",
                        Icon = SweetAlertIcon.Warning,
                        ShowCancelButton = true,
                        ConfirmButtonText = "Yes, Contnue!",
                        CancelButtonText = "No"
                    });

                    if (result.IsConfirmed)
                    {
                        classDetails.SchInfoID = 1;
                        if (classid == 0)
                        {
                            var response = await classService.SaveAsync("AdminSchool/AddClass/", classDetails);
                            classDetails.ClassID = response.ClassID;
                            classDetails.Id = response.ClassID;
                            await classService.UpdateAsync("AdminSchool/UpdateClass/", 2, classDetails);
                            await Swal.FireAsync("A New Class", "Has Been Successfully Created.", "success");
                        }
                        else
                        {
                            await classService.UpdateAsync("AdminSchool/UpdateClass/", 1, classDetails);

                            //Update Class Teacher Subjects Allocation When Class Teacher Change
                            teacherSubjectsAllocation.TermID = termid;
                            teacherSubjectsAllocation.SchID = classDetails.SchID;
                            teacherSubjectsAllocation.ClassID = classDetails.ClassID;
                            teacherSubjectsAllocation.StaffID = _classTeacherID;
                            teacherSubjectsAllocation.StaffID_ClassTeacher = _classTeacherID;
                            await subjectAllocationTeacherService.UpdateAsync("AcademicsSubjects/UpdateTeacherAllocation/", 2, teacherSubjectsAllocation);
                            await subjectAllocationTeacherService.UpdateAsync("AcademicsSubjects/UpdateTeacherAllocation/", 4, teacherSubjectsAllocation);

                            //Update Mark Entry When Class Teacher Change
                            cognitiveMark.TermID = termid;
                            cognitiveMark.SchID = classDetails.SchID;
                            cognitiveMark.ClassID = classDetails.ClassID;
                            cognitiveMark.ClassTeacherID = _classTeacherID;
                            await studentMarksCognitiveService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 10, cognitiveMark);

                            //Update PsychoMotor Entry When Class Teacher Change
                            otherMark.TermID = termid;
                            otherMark.SchID = classDetails.SchID;
                            otherMark.ClassID = classDetails.ClassID;
                            otherMark.StaffID = _classTeacherID;
                            await studentOtherMarksService.UpdateAsync("AcademicsMarks/UpdateOtherMark/", 3, otherMark);

                            ////Update Comments Entry When Class Teacher Change
                            //termEndComment.ClassID = classDetails.ClassID;
                            //termEndComment.ClassTeacherID = _classTeacherID;
                            //await termEndCommentService.UpdateAsync("AcademicsResultsComments/UpdateTermEndComment/", 3, termEndComment);

                            //midTermComment.ClassID= classDetails.ClassID;
                            //midTermComment.ClassTeacherID = _classTeacherID;
                            //await midTermCommentService.UpdateAsync("AcademicsResultsComments/UpdateMidTermComment/", 3, midTermComment);

                            //checkpoinigcseComment.ClassID = classDetails.ClassID;
                            //checkpoinigcseComment.ClassTeacherID = _classTeacherID;
                            //await checkpointigcseCommentService.UpdateAsync("AcademicsResultsComments/UpdateCheckPointIGCSEComment/", 3, checkpoinigcseComment);

                            await Swal.FireAsync("Class Details", "Has Been Successfully Updated.", "success");
                        }
                        await ClassListEvent();
                    }
                }
                else
                {
                    await Swal.FireAsync("Class Teacher Selection!", "Highlighted Class Teacher Not Properly Selected. " +
                        "Please, select any other name before reselecting your desired Class Teacher.", "error");
                }               
            }
        }

        async Task InvalidEntries()
        {
            await Swal.FireAsync("Please Check Your Entries.", "One or More Entries has error.", "error");
        }

        #endregion

        #region [Section - Click Events]
        async Task ClassListEvent()
        {
            toolBarMenuId = 1;
            disableSaveButton = true;
            selectedSchool = string.Empty;
            schid = 0;            
            classList.Clear();
            await LoadList();
        }

        async Task CreateNewClass()
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            pagetitle = "Create a new Class";
            buttontitle = "Save";
            classid = 0;
            classDetails = new ADMSchClassList();
            await LoadListForDetails();
        }

        async Task UpdateClassDetails(int _classid)
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            classDetails = new ADMSchClassList();
            await LoadListForDetails();
            await RetrieveClassDetails(_classid);
        }

        #endregion


    }
}
