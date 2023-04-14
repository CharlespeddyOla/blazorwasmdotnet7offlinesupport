using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Academics.Subjects;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Administration.Students;
using WebAppAcademics.Shared.Models.Settings;

namespace WebAppAcademics.Client.Pages.Admin.Student
{
    public partial class StudentsClassAllocation
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassList> classService { get; set; }
        [Inject] IAPIServices<ADMStudents> studentService { get; set; }
        [Inject] IAPIServices<SETStatusType> statusTypeService { get; set; }
        [Inject] IAPIServices<SETSchSessions> schoolSessionsService { get; set; }
        [Inject] IAPIServices<ACDSbjAllocationStudents> subjectAllocationStudentService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksCognitive> studentMarksCognitiveService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksAssessment> studentOtherMarksService { get; set; }
        [Inject] IAPIServices<ACDReportCommentsTerminal> termEndCommentService { get; set; }
        [Inject] IAPIServices<ACDReportCommentMidTerm> midTermCommentService { get; set; }
        [Inject] IAPIServices<ACDReportCommentCheckPointIGCSE> checkpointigcseCommentService { get; set; }



        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int schoolSession { get; set; }
        int schid { get; set; }
        int classid { get; set; }
        int schsession { get; set; }
        string selectedSchool { get; set; }
        string selectedClass { get; set; }       
        string searchString { get; set; } = "";

        bool ToggleList { get; set; }
        decimal i { get; set; } = 0;
        int j { get; set; } = 0;
        bool IsShow { get; set; } = true;
        string progressbarInfo { get; set; } = string.Empty;
        #endregion

        #region [Models Declaration]
        List<ADMSchlList> schools { get; set; }
        List<ADMSchClassList> classList { get; set; }
        List<ADMSchClassList> allClassList { get; set; }
        List<SETStatusType> statusType { get; set; }
        List<SETSchSessions> academicTerms { get; set; }
        List<ADMStudents> students { get; set; }
        ADMStudents selectedItem = null;
        ADMStudents student = new();
        ACDSbjAllocationStudents subjectAllocation = new();
        ACDStudentsMarksCognitive cognitiveMark = new();
        ACDStudentsMarksAssessment otherMark = new();
        ACDReportCommentsTerminal termEndComment = new();
        ACDReportCommentMidTerm midTermComment = new();
        ACDReportCommentCheckPointIGCSE checkpoinigcseComment = new();

        async void InitializeModels()
        {
            schools = new List<ADMSchlList>();
            classList = new List<ADMSchClassList>();
            allClassList = new List<ADMSchClassList>();
            statusType = new List<SETStatusType>();
            academicTerms = new List<SETSchSessions>();
            await Task.Delay(500); // simulate loading
            students = new List<ADMStudents>();
        }

        #endregion


        protected override async Task OnInitializedAsync()
        {
            InitializeModels();
            Layout.currentPage = "Class Allocation To Students";
            schoolSession = await localStorageService.ReadEncryptedItemAsync<int>("schoolsession");
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            allClassList = await classService.GetAllAsync("AdminSchool/GetClassList/0/0/0");
            statusType = await statusTypeService.GetAllAsync("Settings/GetStatusTypes/1");
            academicTerms = await schoolSessionsService.GetAllAsync("Settings/GetAccademicSessions/1/0");
            await base.OnInitializedAsync();
        }

        async Task OnSelectedSchoolChanged(IEnumerable<string> e)
        {
            selectedSchool = e.ElementAt(0);
            schid = schools.FirstOrDefault(s => s.School == selectedSchool).SchID;

            selectedClass = string.Empty;
            classid = 0;           
            students.Clear();
            classList.Clear();
            classList = allClassList.Where(a => a.SchID == schid).ToList();

            await Task.CompletedTask;
            //classList = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + schid + "/0");
        }

        async Task OnSelectedClassChanged(IEnumerable<string> e)
        {
            selectedClass = e.ElementAt(0);
            classid = classList.FirstOrDefault(s => s.ClassName == selectedClass).ClassID;

            students.Clear();
            students = await studentService.GetAllAsync("AdminStudent/GetStudents/3/0/" + classid + "/0/1");
        }

        async Task UpdateEntry()
        {
            student.STDID = selectedItem.STDID;            
            if (allClassList.FirstOrDefault(s => s.ClassName == selectedItem.ClassName) != null)
            {
                student.SchID = allClassList.FirstOrDefault(s => s.ClassName == selectedItem.ClassName).SchID;
                student.ClassListID = allClassList.FirstOrDefault(s => s.ClassName == selectedItem.ClassName).ClassListID;
                student.ClassID = allClassList.FirstOrDefault(s => s.ClassName == selectedItem.ClassName).ClassID;
            }
            else
            {
                student.SchID = selectedItem.SchID;
                student.ClassListID = selectedItem.ClassListID;
                student.ClassID = selectedItem.ClassID;
            }
            student.TermID = academicTerms.FirstOrDefault(t => t.AcademicSession == selectedItem.AcademicSession).TermID;
            student.StatusTypeID = statusType.FirstOrDefault(s => s.StatusType == selectedItem.StatusType).StatusTypeID;
            await studentService.UpdateAsync("AdminStudent/UpdateStudent/", 3, student);

            //Update Subjects Allocated When Student Class Change
            subjectAllocation.STDID = student.STDID;
            subjectAllocation.SchID = student.SchID;
            subjectAllocation.ClassID = student.ClassID;
            subjectAllocation.ClassListID = student.ClassListID;
            subjectAllocation.SchSession = schoolSession;
            await subjectAllocationStudentService.UpdateAsync("AcademicsSubjects/UpdateStudentAllocation/", 2, subjectAllocation);

            //Update Comments Entry When Class Teacher Change
            termEndComment.STDID = selectedItem.STDID;
            termEndComment.ClassID = selectedItem.ClassID;
            termEndComment.ClassTeacherID = allClassList.FirstOrDefault(s => s.ClassID == selectedItem.ClassID).StaffID;
            await termEndCommentService.UpdateAsync("AcademicsResultsComments/UpdateTermEndComment/", 5, termEndComment);

            midTermComment.STDID = selectedItem.STDID;
            midTermComment.ClassID = selectedItem.ClassID;
            midTermComment.ClassTeacherID = allClassList.FirstOrDefault(s => s.ClassID == selectedItem.ClassID).StaffID;
            await midTermCommentService.UpdateAsync("AcademicsResultsComments/UpdateMidTermComment/", 5, midTermComment);

            checkpoinigcseComment.STDID = selectedItem.STDID;
            checkpoinigcseComment.ClassID = selectedItem.ClassID;
            checkpoinigcseComment.ClassTeacherID = allClassList.FirstOrDefault(s => s.ClassID == selectedItem.ClassID).StaffID;
            await checkpointigcseCommentService.UpdateAsync("AcademicsResultsComments/UpdateCheckPointIGCSEComment/", 5, checkpoinigcseComment);


            Snackbar.Add("Selected Row Entries Updated Successfully.");
        }

        async Task SaveSelection()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Student Class Allocation Operation",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                IsShow = false;
                i = 0;
                j = 0;
                progressbarInfo = "Please wait for update to complete...";

                int maxValue = students.Count();

                foreach (var item in students)
                {
                    j++;
                    i = ((decimal)(j) / maxValue) * 100;

                    student.STDID = item.STDID;                    
                    if (allClassList.FirstOrDefault(s => s.ClassName == item.ClassName) != null)
                    {
                        student.SchID = allClassList.FirstOrDefault(s => s.ClassName == item.ClassName).SchID;
                        student.ClassListID = allClassList.FirstOrDefault(s => s.ClassName == item.ClassName).ClassListID;
                        student.ClassID = allClassList.FirstOrDefault(s => s.ClassName == item.ClassName).ClassID;
                    }
                    else
                    {
                        student.SchID = item.SchID;
                        student.ClassListID = item.ClassListID;
                        student.ClassID = item.ClassID;
                    }
                    student.TermID = academicTerms.FirstOrDefault(t => t.AcademicSession == item.AcademicSession).TermID;
                    student.StatusTypeID = statusType.FirstOrDefault(s => s.StatusType == item.StatusType).StatusTypeID;
                    await studentService.UpdateAsync("AdminStudent/UpdateStudent/", 3, student);

                    ////Update Subjects Allocated When Student Class Change
                    //subjectAllocation.STDID = student.STDID;
                    //subjectAllocation.SchID = student.SchID;
                    //subjectAllocation.ClassID = student.ClassID;
                    //subjectAllocation.ClassListID = student.ClassListID;
                    //subjectAllocation.SchSession = schoolSession;
                    //await subjectAllocationStudentService.UpdateAsync("AcademicsSubjects/UpdateStudentAllocation/", 2, subjectAllocation);

                    ////Update Comments Entry When Class Teacher Change
                    //termEndComment.STDID = student.STDID;
                    //termEndComment.ClassID = student.ClassID;
                    //termEndComment.ClassTeacherID = allClassList.FirstOrDefault(s => s.ClassID == student.ClassID).StaffID;
                    //await termEndCommentService.UpdateAsync("AcademicsResultsComments/UpdateTermEndComment/", 5, termEndComment);

                    //midTermComment.STDID = student.STDID;
                    //midTermComment.ClassID = student.ClassID;
                    //midTermComment.ClassTeacherID = allClassList.FirstOrDefault(s => s.ClassID == student.ClassID).StaffID;
                    //await midTermCommentService.UpdateAsync("AcademicsResultsComments/UpdateMidTermComment/", 5, midTermComment);

                    //checkpoinigcseComment.STDID = student.STDID;
                    //checkpoinigcseComment.ClassID = student.ClassID;
                    //checkpoinigcseComment.ClassTeacherID = allClassList.FirstOrDefault(s => s.ClassID == student.ClassID).StaffID;
                    //await checkpointigcseCommentService.UpdateAsync("AcademicsResultsComments/UpdateCheckPointIGCSEComment/", 5, checkpoinigcseComment);

                    StateHasChanged();
                }

                IsShow = true;
                await Swal.FireAsync("Selected Student(s)", "Has Been Successfully Allocation To Selected Class.", "success");
            }
        }

        void GoBack()
        {
            navManager.NavigateTo("/students");
        }

        async Task Clear()
        {
            selectedSchool = string.Empty;
            schid = 0;
            selectedClass = string.Empty;
            classid = 0;;
            classList.Clear();
            students.Clear();
            schools.Clear();
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
        }

        async Task OnToggledChanged(bool toggled)
        {
            ToggleList = toggled;

            if (ToggleList)
            {
                await Clear();
                allClassList = await classService.GetAllAsync("AdminSchool/GetClassList/0/0/0");
                var _students = await studentService.GetAllAsync("AdminStudent/GetStudents/1/0/0/0/1");
                int _sn = 1;
                foreach (var item in _students)
                {
                    students.Add(new ADMStudents
                    {
                        STDID= item.STDID,
                        SN = _sn++,
                        AdmissionNo = item.AdmissionNo,
                        StudentName = item.StudentName,
                        ClassName= item.ClassName,
                        AcademicSession= item.AcademicSession,
                        StatusType = item.StatusType,
                    });                    
                }
            }
            else
            {
                await Clear();
            }
        }

        private bool FilterFunc(ADMStudents model)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (model.StudentName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

    }
}
