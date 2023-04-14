using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Academics.Subjects;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Administration.Staff;
using WebAppAcademics.Shared.Models.Administration.Students;
using WebAppAcademics.Client.Extensions;

namespace WebAppAcademics.Client.Pages.Admin.School
{
    public partial class ADMSchoolList
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMStudents> studentService { get; set; }
        [Inject] IAPIServices<ADMEmployee> staffService { get; set; }
        [Inject] IAPIServices<ACDSbjAllocationTeachers> subjectAllocationTeacherService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksCognitive> studentMarksCognitiveService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int toolBarMenuId { get; set; }
        int termid { get; set; }
        int schid { get; set; }
        int newschid { get; set; }
        int staffid { get; set; }
        int schoolprincipalid { get; set; }
        string selectedSchoolHead { get; set; }
        string pagetitle = "Create a new School";
        string buttontitle = "Save";
        bool disableSaveButton { get; set; } = true;
        string editFormId { get; set; } = "editformid";
        #endregion

        #region [Models Declaration]
        List<ADMSchlList> schools = new();
        List<ADMEmployee> staffs = new();

        ADMSchlList school = new();
        ACDSbjAllocationTeachers teacherSubjectsAllocation = new();
        ACDStudentsMarksCognitive cognitiveMark = new();


        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "School List";
            toolBarMenuId = 1;
            await LoadSchools();
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            await base.OnInitializedAsync();
        }

        #region [Section - School List]
        async Task LoadSchools()
        {
            schools.Clear();
            var schoolsAll = await schoolService.GetAllAsync("AdminSchool/GetSchools/1");

            foreach (var item in schoolsAll.ToList())
            {
                int _studentCount = await studentService.CountAsync("AdminStudent/GetCount/2/" + item.SchID + "/0/0/", 1);

                schools.Add(new ADMSchlList
                {
                    SchID= item.SchID,
                    School = item.School,
                    SchoolCount = _studentCount,
                    Head = item.Head,
                    SchoolHeadWithNo = item.SchoolHeadWithNo,
                    Status = item.Status,   
                });
            }
        }

        #endregion

        #region [Section - School Details]
        async Task RetrieveSchoolDetails(int _schid)
        {
            // Change page title and button text since this is an edit.
            staffs = await staffService.GetAllAsync("AdminStaff/GetStaffs/9/1/0/0/0");
            school = await schoolService.GetByIdAsync("AdminSchool/GetSchool/", _schid);
            schid = _schid;
            pagetitle = "EDIT " + school.School + " SCHOOL DETAILS";
            buttontitle = "Update";
        }

        void OnSchoolHeadChanged(IEnumerable<string> value)
        {
            selectedSchoolHead = value.ElementAt(0);
            if (selectedSchoolHead == null)
            {
                school.SchoolHead = string.Empty;
            }
            else
            {
                school.SchoolHead = selectedSchoolHead;
                staffid = staffs.FirstOrDefault(s => s.StaffNameWithNo == selectedSchoolHead).StaffID;
                schoolprincipalid = staffid;
            }
        }

        async Task SubmitValidForm()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "School Details Save/Update Operation",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                school.StaffID = staffid;
                if (schid == 0)
                {                    
                    var response = await schoolService.SaveAsync("AdminSchool/AddSchool/", school);
                    newschid = response.SchID;
                    school.SchID = newschid;
                    school.Id = newschid;
                    await schoolService.UpdateAsync("AdminSchool/UpdateSchool/", 2, school);
                    await Swal.FireAsync("New School Details", "Has Been Successfully Saved.", "success");
                }
                else
                {
                    school.SchID = schid;
                    await schoolService.UpdateAsync("AdminSchool/UpdateSchool/", 1, school);

                    //Update Class Teacher Subjects Allocation When School Principal Change
                    teacherSubjectsAllocation.TermID = termid;
                    teacherSubjectsAllocation.SchID = schid;
                    teacherSubjectsAllocation.StaffID_Principal = schoolprincipalid;
                    await subjectAllocationTeacherService.UpdateAsync("AcademicsSubjects/UpdateTeacherAllocation/", 3, teacherSubjectsAllocation);

                    //Update Mark Entry When School Principal Change
                    cognitiveMark.TermID = termid;
                    cognitiveMark.SchID = schid;
                    cognitiveMark.SchoolPrincipalID = schoolprincipalid;
                    await studentMarksCognitiveService.UpdateAsync("AcademicsMarks/UpdateCognitiveMark/", 11, cognitiveMark);

                    await Swal.FireAsync("Selected School Details", "Has Been Successfully Updated.", "success");
                }

                await SchoolListEvent();
            }
        }

        async Task InvalidEntries()
        {
            await Swal.FireAsync("Please Check Your Entries.", "One or More Entries has error.", "error");
        }

        #endregion

        #region [Section - Click Events]
        async Task SchoolListEvent()
        {
            toolBarMenuId = 1;
            buttontitle = "Save";
            disableSaveButton = true;
            await LoadSchools();
        }

        async Task CreateNewSchool()
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            pagetitle = "Create a new School";
            buttontitle = "Save";
            schid = 0;
            school = new ADMSchlList();
            staffs = await staffService.GetAllAsync("AdminStaff/GetStaffs/9/1/0/0/0");
        }

        async Task UpdateSchoolDetails(int _schid)
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            school = new ADMSchlList();
            await RetrieveSchoolDetails(_schid);
        }

        void GoBack()
        {
            navManager.NavigateTo("/classlist");
        }

        #endregion



    }
}
