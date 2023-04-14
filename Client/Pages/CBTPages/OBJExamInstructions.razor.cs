using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.CBT;
using WebAppAcademics.Shared.Models.Administration.Students;
using WebAppAcademics.Shared.Models.Settings;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Client.Pages.CBTPages
{
    public partial class OBJExamInstructions
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] ISessionStorageService sessionStorageService { get; set; }
        [Inject] IAPIServices<SETSchSessions> academicSessionService { get; set; }
        [Inject] IAPIServices<CBTExams> examService { get; set; }
        [Inject] IAPIServices<ADMStudents> studentService { get; set; }
        [Inject] IAPIServices<CBTQuestions> questionsService { get; set; }
        [Inject] IAPIServices<CBTAnswers> answersService { get; set; }
        [Inject] IAPIServices<CBTExamTakenFlags> examTakenService { get; set; }
        [Inject] IAPIServices<ACDFlags> flagsService { get; set; }

        #endregion

        #region [Variables Declaration]
        private int stdid { get; set; }
        private int classlistid { get; set; }
        private string admissionNo { get; set; }
        private string studentName { get; set; }
        private int examID { get; set; }
        private int termid { get; set; }        
        private string academicSession { get; set; }
        private string schoolClass { get; set; }
        private string examCode { get; set; }
        private string examDate { get; set; }
        private double examTime { get; set; }
        private string examInstruction { get; set; }

        string selectedSubject { get; set; }
        #endregion

        #region [Models Declaration]
        List<SETSchSessions> sessions = new();
        List<CBTExams> exams = new();
        public List<CBTQuestions> Questions = new();
        public List<CBTAnswers> Answers = new();
        ADMStudents student = new();
        List<ACDFlags> flagList = new();
        List<CBTExamTakenFlags> examTakenList = new();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            stdid = await sessionStorageService.ReadEncryptedItemAsync<int>("stdid");
            classlistid = await sessionStorageService.ReadEncryptedItemAsync<int>("classlistid");
            admissionNo = await sessionStorageService.ReadEncryptedItemAsync<string>("studentno");
            studentName = await sessionStorageService.ReadEncryptedItemAsync<string>("studentname");           
            flagList = await flagsService.GetAllAsync("AcademicsCBT/GetFlags/1");
            await LoadExams();
            await base.OnInitializedAsync();
        }

        async Task LoadExams()
        {
            var _exams = await examService.GetAllAsync("AcademicsCBT/GetCBTExams/2/0/" + classlistid + "/0");
            exams = _exams.ToList();
            bool _lockCBT = flagList.Where(l => l.FlagID == 1 && l.Flag == true).Any();

            if (!_lockCBT)
            {
                int examTermID = exams.FirstOrDefault(ex => ex.TermID == exams.Max(t => t.TermID)).TermID;
                examTakenList = await examTakenService.GetAllAsync("AcademicsCBT/Search/2/" + examTermID + "/" + stdid + "/0/" + true);

                foreach (var item in exams)
                {
                    bool examHasBeenTaken = examTakenList.Where(ex => ex.ExamID == item.ExamID).Any();
                    if (examHasBeenTaken)
                    {
                        _exams.RemoveAll(r => r.ExamID == item.ExamID);
                    }
                }

                exams.Clear();
                exams = _exams.ToList();  
            }          
        }
               

        async Task OnSelectedSubject(IEnumerable<string> e)
        {
            selectedSubject = e.ElementAt(0);
            examID = exams.FirstOrDefault(e => e.Subject == selectedSubject).ExamID;
            termid = exams.FirstOrDefault(e => e.Subject == selectedSubject).TermID;
            var _academicSession = await academicSessionService.GetByIdAsync("Settings/GetAccademicSession/", termid);
            academicSession = _academicSession.AcademicSession;
            schoolClass = exams.FirstOrDefault(e => e.Subject == selectedSubject).SchClass;
            examCode = exams.FirstOrDefault(e => e.Subject == selectedSubject).ExamCode;
            examDate = exams.FirstOrDefault(e => e.Subject == selectedSubject).ExamDate?.ToString("dd-MMM-yyyy");
            examTime = exams.FirstOrDefault(e => e.Subject == selectedSubject).ExamTime;
            examInstruction = exams.FirstOrDefault(e => e.Subject == selectedSubject).ExamInstruction;

            await sessionStorageService.SaveItemEncryptedAsync("termid", termid);
            await sessionStorageService.SaveItemEncryptedAsync("academicSession", academicSession);
            await sessionStorageService.SaveItemEncryptedAsync("examDate", examDate);
            await sessionStorageService.SaveItemEncryptedAsync("examCode", examCode);
            await sessionStorageService.SaveItemEncryptedAsync("subject", selectedSubject);
            await sessionStorageService.SaveItemEncryptedAsync("examTime", examTime);

            Questions = await questionsService.GetAllAsync("AcademicsCBT/GetCBTExamQuestions/1/" + examID);
        }

        void LockCBT()
        {
            bool _lockCBT = flagList.Where(l => l.FlagID == 1 && l.Flag == true).Any();

            if (_lockCBT)
            {
                student.STDID = stdid;
                student.CBTLock = true;
                studentService.UpdateAsync("AdminStudent/UpdateStudent/", 9, student);
            }           
        }

        async Task StartExam()
        {
            if (examID == 0)
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Start CBT Examination",
                    Width = "500",
                    Icon = "info",
                    Text = "Please Select A Subject!"
                });
            }
            else
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Start CBT Examination",
                    Text = "Do You Want To Continue?",
                    Icon = SweetAlertIcon.Warning,
                    ShowCancelButton = true,
                    ConfirmButtonText = "Yes, Contnue!",
                    CancelButtonText = "No"
                });

                if (result.IsConfirmed)
                {
                    LockCBT();
                    navManager.NavigateTo("/objstudentexam/" + examID + "/" + stdid);
                }
            }
        }

        void CloseInstruction()
        {
            navManager.NavigateTo("/cbt");
        }

    }
}
