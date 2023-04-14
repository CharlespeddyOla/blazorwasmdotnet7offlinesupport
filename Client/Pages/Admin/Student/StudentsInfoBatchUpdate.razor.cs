using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Administration.Students;

namespace WebAppAcademics.Client.Pages.Admin.Student
{
    public partial class StudentsInfoBatchUpdate
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassList> classService { get; set; }
        [Inject] IAPIServices<ADMStudents> studentService { get; set; }
        
        [CascadingParameter]
        public MainLayout Layout { get; set; }

        #endregion

        #region [Variables Declaration]
        int schid { get; set; }
        int classid { get; set; }
        int schsession { get; set; }
        string selectedSchool { get; set; }
        string selectedClass { get; set; }
        string searchString { get; set; } = "";

        #endregion

        #region [Models Declaration]
        List<ADMSchlList> schools { get; set; }
        List<ADMSchClassList> classList { get; set; }        
        List<ADMStudents> students { get; set; }
        ADMStudents selectedItem = null;
        ADMStudents student = new ADMStudents();

        void InitializeModels()
        {
            schools = new List<ADMSchlList>();
            classList = new List<ADMSchClassList>();
            students = new List<ADMStudents>();
        }

        #endregion

        protected override async Task OnInitializedAsync()
        {
            InitializeModels();
            Layout.currentPage = "Student Info Batch Update";
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            await base.OnInitializedAsync();
        }
      
        async Task OnSelectedSchoolChanged(IEnumerable<string> e)
        {
            selectedSchool = e.ElementAt(0);
            schid = schools.FirstOrDefault(s => s.School == selectedSchool).SchID;

            selectedClass = string.Empty;
            classid = 0;
            students.Clear();

            classList = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + schid + "/0");            
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
            student.StudentID = selectedItem.StudentID;
            student.Surname = selectedItem.Surname;
            student.FirstName = selectedItem.FirstName;
            student.MiddleName = selectedItem.MiddleName;
            student.Email = selectedItem.Email;
            student.PhoneNumber = selectedItem.PhoneNumber;

            await studentService.UpdateAsync("AdminStudent/UpdateStudent/", 4, student);
            Snackbar.Add("Selected Student Info (" + selectedItem.StudentName + ") Has Been Successfully Updated");
        }

        async Task SaveEntries()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Update Student Email Operation",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                foreach (var item in students)
                {
                    student.STDID = item.STDID;
                    student.StudentID = item.StudentID;
                    student.Surname = item.Surname;
                    student.FirstName = item.FirstName;
                    student.MiddleName = item.MiddleName;
                    student.Email = item.Email;
                    student.PhoneNumber = item.PhoneNumber;

                    await studentService.UpdateAsync("AdminStudent/UpdateStudent/", 4, student);
                }
                await Swal.FireAsync("Selected Student(s)", " Info Has Been Successfully Updated.", "success");
            }
        }

        void GoBack()
        {
            navManager.NavigateTo("/students");
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
