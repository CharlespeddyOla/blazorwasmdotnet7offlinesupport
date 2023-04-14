using Blazored.LocalStorage;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Academics.Subjects;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Administration.Students;
using WebAppAcademics.Shared.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppAcademics.Client.Extensions;

namespace WebAppAcademics.Client.Pages.Academics.Exam.Marks.Entry
{
    public partial class MarkEntryStudent
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<SETSchSessions> schoolSessionsService { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassList> classService { get; set; }
        [Inject] IAPIServices<ADMStudents> studentService { get; set; }
        [Inject] IAPIServices<ACDSettingsMarks> markSettingService { get; set; }
        [Inject] IAPIServices<ACDSbjAllocationStudents> subjectAllocationStudentService { get; set; }
        [Inject] IAPIServices<ACDSbjAllocationTeachers> subjectAllocationTeacherService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksCognitive> studentMarksCognitiveService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int termid { get; set; }
        int maxTermID { get; set; }
        int schoolSession { get; set; }
        string academicSession { get; set; }
        int schid { get; set; }
        int classid { get; set; }
        int classlistid { get; set; }
        int stdid { get; set; }
        int principalid { get; set; }
        int classteacherid { get; set; }

        string selectedSchool { get; set; }
        string selectedClass { get; set; }
        string selectedStudent { get; set; }

        bool IsFinalYearClass { get; set; }
        bool ToggleDeleteColumn { get; set; }
        bool ShowDeleteColumn { get; set; }

        decimal i { get; set; } = 0;
        int j { get; set; } = 0;
        bool IsShow { get; set; } = true;
        string progressbarInfo { get; set; } = string.Empty;

        bool MarkEntryValidationResult { get; set; } = false;
        #endregion

        #region [Models Declaration]
        List<SETSchSessions> sessions { get; set; }
        List<ADMSchlList> schools { get; set; }
        List<ADMSchClassList> classList { get; set; }
        List<ADMStudents> students { get; set; }
        List<ACDSettingsMarks> markSettingsList { get; set; }
        List<ACDSbjAllocationStudents> studentSubjectsAllocation { get; set; }
        List<ACDSbjAllocationTeachers> teacherSubjectAllocation { get; set; }
        List<ACDStudentsMarksCognitive> cognitiveMarks { get; set; }

        ACDStudentsMarksCognitive cognitiveMark = new ACDStudentsMarksCognitive();
        ACDStudentsMarksCognitive selectedItemCognitiveMark = null;
        List<string> marksErrorLisitng = new List<string>();

        void InitializeModels()
        {
            sessions = new List<SETSchSessions>();
            schools = new List<ADMSchlList>();
            classList = new List<ADMSchClassList>();
            students = new List<ADMStudents>();
            markSettingsList = new List<ACDSettingsMarks>();
            studentSubjectsAllocation = new List<ACDSbjAllocationStudents>();
            teacherSubjectAllocation = new List<ACDSbjAllocationTeachers>();
            cognitiveMarks = new List<ACDStudentsMarksCognitive>();
        }

        #endregion

        protected override async Task OnInitializedAsync()
        {
            InitializeModels();

            Layout.currentPage = "Mark Entry Per Student";
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            schoolSession = await localStorageService.ReadEncryptedItemAsync<int>("schoolsession");
            academicSession = await localStorageService.ReadEncryptedItemAsync<string>("academicsession");
            await LoadDefaultList();
            await base.OnInitializedAsync();
        }

        #region [Section - Load Events]
        async Task LoadDefaultList()
        {
            sessions = await schoolSessionsService.GetAllAsync("Settings/GetAccademicSessions/1/0");
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            maxTermID = sessions.FirstOrDefault(s => s.TermID == sessions.Max(t => t.TermID)).TermID;
            markSettingsList = await markSettingService.GetAllAsync("AcademicsMarkSettings/GetMarkSettings/1");
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
                var _studentsAll = await studentService.GetAllAsync("AdminStudent/GetStudents/9/0/0/0/0");
                
                var _studentSubjectsAllocation = await subjectAllocationStudentService.GetAllAsync("AcademicsSubjects/GetStudentAllocations/4/" +
                            schoolSession + "/true/" + schid + "/" + classlistid + "/" + id + "/0/0");

                var _students = _studentSubjectsAllocation.Where(s => s.SbjClassID == 1)
                                .GroupBy(x => new { x.STDID, x.StudentNo, x.StudentName })
                                .Select(x => x.First())
                                .ToList();
                int sn = 1;

                foreach (var item in _students)
                {
                    students.Add(new ADMStudents()
                    {
                        SN = sn++,
                        STDID = item.STDID,
                        AdmissionNo = _studentsAll.SingleOrDefault(s => s.STDID == item.STDID).AdmissionNo,
                        StudentName = _studentsAll.SingleOrDefault(s => s.STDID == item.STDID).StudentName
                    });
                }
            }
        }

        async Task LoadStudentMarks(int _stdid)
        {
            teacherSubjectAllocation.Clear();
            studentSubjectsAllocation.Clear();
            cognitiveMarks.Clear();

            teacherSubjectAllocation = await subjectAllocationTeacherService.GetAllAsync("AcademicsSubjects/GetTeacherAllocations/3/true/" +
                                                    termid + "/" + schid + "/" + classlistid + "/" + classid + "/0/0");
            studentSubjectsAllocation = await subjectAllocationStudentService.GetAllAsync("AcademicsSubjects/GetStudentAllocations/10/" +
                            schoolSession + "/true/0/0/1/0/" + _stdid);
            var _studentMarks = await studentMarksCognitiveService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/8/" +
                                        termid + "/0/0/" + _stdid  +  "/0/0");

            IsShow = false;
            i = 0;
            j = 0;
            progressbarInfo = "Please wait loading student marks...";

            int maxValue = studentSubjectsAllocation.Count();

            foreach (var item in studentSubjectsAllocation)
            {
                j++;
                i = ((decimal)(j) / maxValue) * 100;

                bool IsMarkExist = _studentMarks.Where(m => m.SubjectID == item.SubjectID).Any();
              
                if (!IsMarkExist)
                {
                    bool IsSubjectAllocated = teacherSubjectAllocation.Where(s => s.SubjectID == item.SubjectID).Any();

                    if (IsSubjectAllocated)
                    {
                        cognitiveMark.TermID = termid;
                        cognitiveMark.SchSession = schoolSession;
                        cognitiveMark.SchID = schid;
                        cognitiveMark.ClassID = classid;
                        cognitiveMark.StaffID = teacherSubjectAllocation.FirstOrDefault(s => s.SubjectID == item.SubjectID).StaffID;
                        cognitiveMark.ClassTeacherID = classteacherid;
                        cognitiveMark.SchoolPrincipalID = principalid;
                        cognitiveMark.STDID = _stdid;
                        cognitiveMark.SubjectID = item.SubjectID;
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
                }

                StateHasChanged();
            }

            cognitiveMarks = await studentMarksCognitiveService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/4/" +
                                      termid + "/" + schid + "/" + classid + "/" + _stdid + "/0/0");

            IsShow = true;
        }

        async Task OnSelectedSchoolChanged(IEnumerable<string> e)
        {
            selectedSchool = e.ElementAt(0);
            schid = schools.FirstOrDefault(s => s.School == selectedSchool).SchID;
            principalid = schools.FirstOrDefault(s => s.School == selectedSchool).StaffID;

            classid = 0;
            selectedClass = string.Empty;
            classList.Clear();
            classList = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + schid + "/0");

            students.Clear();
            cognitiveMarks.Clear();
        }

        async Task OnSelectedClassChanged(IEnumerable<string> e)
        {
            selectedClass = e.ElementAt(0);
            classid = classList.FirstOrDefault(c => c.ClassName == selectedClass).ClassID;
            classlistid = classList.FirstOrDefault(c => c.ClassName == selectedClass).ClassListID;
            classteacherid = classList.FirstOrDefault(c => c.ClassName == selectedClass).StaffID;
            IsFinalYearClass = classList.FirstOrDefault(c => c.ClassID == classid).FinalYearClass;

            students.Clear();
            await LoadStudents(classid);
            cognitiveMarks.Clear();
        }

        async Task OnSelectedStudentChanged(IEnumerable<string> e)
        {
            selectedStudent = e.ElementAt(0);
            stdid = students.FirstOrDefault(s => s.StudentName == selectedStudent).STDID;


            await LoadStudentMarks(stdid);

            //cognitiveMarks = await studentMarksCognitiveService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/4/" +
            //                           termid + "/" + schid + "/" + classid + "/" + stdid + "/0/0");
        }

        bool SetMarkEntryStatus(int switchid, decimal igcse, decimal midtermexam, decimal termendexam)
        {
            switch(switchid)
            {
                case 1: //Check Point / IGCSE
                    if (igcse > 0)
                    {
                        return true;
                    }
                    break;
                case 2: //Mid-Tern Exam
                    if (midtermexam > 0)
                    {
                        return true;
                    }
                    break;
                case 3: //End of Term Exam
                    if (termendexam > 0)
                    {
                        return true;
                    }
                    break;
            }
                    
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
            else if(selectedItemCognitiveMark.Mark_Mid > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 1).Mark))
            {
                await Swal.FireAsync("Invalid Mid-Term Mark", selectedItemCognitiveMark.SN + ". " +
                                       selectedItemCognitiveMark.StudentName + ": " + selectedItemCognitiveMark.Mark_Mid + " > " +
                                       markSettingsList.FirstOrDefault(m => m.MarkID == 1).Mark + ".", "error");
                MarkEntryValidationResult = true;
            }
            else if(selectedItemCognitiveMark.Mark_CA1 > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 2).Mark))
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
                decimal TotalMidTermExamMark = selectedItemCognitiveMark.Mark_MidCBT + selectedItemCognitiveMark.Mark_Mid;
                decimal TotalCAMark = selectedItemCognitiveMark.Mark_CA1 + selectedItemCognitiveMark.Mark_CA2 +
                                                selectedItemCognitiveMark.Mark_CA3 + selectedItemCognitiveMark.Mark_CBT;
                decimal TotalExamMark = TotalCAMark + selectedItemCognitiveMark.Mark_Exam;

                bool _entryStatusIGSCE = SetMarkEntryStatus(1, selectedItemCognitiveMark.Mark_ICGC, 0, 0);
                bool _entryStatusMidTerm = SetMarkEntryStatus(2, 0, TotalMidTermExamMark, 0);
                bool _entryStatusTermEnd = SetMarkEntryStatus(3, 0, 0, TotalExamMark);

                cognitiveMark.StudentMarkID = selectedItemCognitiveMark.StudentMarkID;
                cognitiveMark.Mark_MidCBT = selectedItemCognitiveMark.Mark_MidCBT;
                cognitiveMark.Mark_Mid = selectedItemCognitiveMark.Mark_Mid;
                cognitiveMark.Mark_ICGC = selectedItemCognitiveMark.Mark_ICGC;
                cognitiveMark.Mark_CA1 = selectedItemCognitiveMark.Mark_CA1;
                cognitiveMark.Mark_CA2 = selectedItemCognitiveMark.Mark_CA2;
                cognitiveMark.Mark_CA3 = selectedItemCognitiveMark.Mark_CA3;
                cognitiveMark.Mark_CBT = selectedItemCognitiveMark.Mark_CBT;
                cognitiveMark.Mark_Exam = selectedItemCognitiveMark.Mark_Exam;
                cognitiveMark.EntryStatus_ICGCS = _entryStatusIGSCE;
                cognitiveMark.EntryStatus_MidTerm = _entryStatusMidTerm;
                cognitiveMark.EntryStatus_TermEnd = _entryStatusTermEnd;

                await studentMarksCognitiveService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 12, cognitiveMark);
            }
            else
            {
                await Swal.FireAsync("Student Mark Entry Error",
                    "Please, Check Your Entry. You have Entered A Mark Greater Than The Maximum Mark Set.", "error");
            }
        }

        async Task SaveMarksPerStudent()
        {
            marksErrorLisitng.Clear();
            int k1 = 0; int k2 = 0; int k3 = 0; int k4 = 0; int k5 = 0; int k6 = 0; int k7 = 0; int k8 = 0; int k = 0;

            IsShow = false;
            i = 0;
            j = 0;
            progressbarInfo = "Please wait saving student marks...";

            int maxValue = cognitiveMarks.Count();

            foreach (var item in cognitiveMarks)
            {
                j++;
                i = ((decimal)(j) / maxValue) * 100;

                if (item.Mark_Mid > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 1).Mark))
                {
                    k1++;
                }
                else if (item.Mark_CA1 > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 2).Mark))
                {
                    k2++;
                }
                else if (item.Mark_CA2 > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 3).Mark))
                {
                    k3++;
                }
                else if (item.Mark_CA3 > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 4).Mark))
                {
                    k4++;
                }
                else if (item.Mark_CBT > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 5).Mark))
                {
                    k5++;
                }
                else if (item.Mark_Exam > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 6).Mark))
                {
                    k6++;
                }
                else if (item.Mark_ICGC > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 7).Mark))
                {
                    k7++;
                }
                else if (item.Mark_MidCBT > Convert.ToDecimal(markSettingsList.FirstOrDefault(m => m.MarkID == 8).Mark))
                {
                    k8++;
                }

                if (k1 > 0 || k2 > 0 || k3 > 0 || k4 > 0 || k5 > 0 || k6 > 0 || k7 > 0 || k8 > 0)
                {
                    k++; k1 = 0; k2 = 0; k3 = 0; k4 = 0; k5 = 0; k6 = 0; k7 = 0; k8 = 0;
                    marksErrorLisitng.Add(item.SN + ". " + item.StudentName);
                }
                else
                {
                    decimal TotalMidTermExamMark = item.Mark_MidCBT + item.Mark_Mid;
                    decimal TotalCAMark = item.Mark_CA1 + item.Mark_CA2 + item.Mark_CA3 + item.Mark_CBT;
                    decimal TotalExamMark = TotalCAMark + item.Mark_Exam;

                    bool _entryStatusIGSCE = SetMarkEntryStatus(1, item.Mark_ICGC, 0, 0);
                    bool _entryStatusMidTerm = SetMarkEntryStatus(2, 0, TotalMidTermExamMark, 0);
                    bool _entryStatusTermEnd = SetMarkEntryStatus(3, 0, 0, TotalExamMark);

                    cognitiveMark.StudentMarkID = item.StudentMarkID;
                    cognitiveMark.Mark_MidCBT = item.Mark_MidCBT;
                    cognitiveMark.Mark_Mid = item.Mark_Mid;
                    cognitiveMark.Mark_ICGC = item.Mark_ICGC;
                    cognitiveMark.Mark_CA1 = item.Mark_CA1;
                    cognitiveMark.Mark_CA2 = item.Mark_CA2;
                    cognitiveMark.Mark_CA3 = item.Mark_CA3;
                    cognitiveMark.Mark_CBT = item.Mark_CBT;
                    cognitiveMark.Mark_Exam = item.Mark_Exam;
                    cognitiveMark.EntryStatus_ICGCS = _entryStatusIGSCE;
                    cognitiveMark.EntryStatus_MidTerm = _entryStatusMidTerm;
                    cognitiveMark.EntryStatus_TermEnd = _entryStatusTermEnd;

                    await studentMarksCognitiveService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 12, cognitiveMark);
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

        async Task SaveCognitiveMarksPerStudent()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "SAVE " + selectedStudent + " MARKS",
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
                    await SaveMarksPerStudent();
                }
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
                cognitiveMarks = await studentMarksCognitiveService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/4/" +
                                       termid + "/" + schid + "/" + classid + "/" + stdid + "/0/0");
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
       
        async Task RefreshCogvitivePerStudent()
        {
            schid = 0;
            principalid = 0;
            selectedSchool = string.Empty;
            schools.Clear();
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            classid = 0;
            classteacherid = 0;
            selectedClass = string.Empty;
            classList.Clear();
            classList = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + schid + "/0");

            selectedStudent = string.Empty;
            students.Clear();
            cognitiveMarks.Clear();
        }

        #endregion

        #region [Section - Click Events]

        void GoBack()
        {
            navManager.NavigateTo("/exammarkentry");
        }
        #endregion

    }
}
