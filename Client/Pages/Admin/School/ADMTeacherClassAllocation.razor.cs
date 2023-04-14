using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Administration.Staff;
using WebAppAcademics.Client.Extensions;

namespace WebAppAcademics.Client.Pages.Admin.School
{
    public partial class ADMTeacherClassAllocation
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassList> classService { get; set; }
        [Inject] IAPIServices<ADMSchClassDiscipline> classDisciplineService { get; set; }
        [Inject] IAPIServices<ADMEmployee> staffService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int termid { get; set; }
        int schid { get; set; }
        int classis { get; set; }
        string selectedSchool { get; set; }
        bool ToggleList { get; set; }
        #endregion

        #region [Models Declaration]
        List<ADMSchlList> schools = new();
        List<ADMSchClassList> schoolClassList = new();
        List<ADMSchClassDiscipline> desciplines = new();
        List<ADMEmployee> staffs = new();

        ADMSchClassList shoolclass = new();
        ADMSchClassList selectedItem = null;
               
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Class Allocation To Teachers";
            await LoadList();

            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            await base.OnInitializedAsync();
        }

        #region [Section - List]
        async Task LoadList()
        {
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            desciplines = await classDisciplineService.GetAllAsync("AdminSchool/GetDisciplines/1");
            staffs = await staffService.GetAllAsync("AdminStaff/GetStaffs/8/1/0/0/0");
        }

        async Task OnToggledChanged(bool toggled)
        {
            ToggleList = toggled;
            selectedSchool = string.Empty;
            schools.Clear();
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            if (ToggleList)
            {
                schoolClassList = await classService.GetAllAsync("AdminSchool/GetClassList/0/0/0");
            }
            else
            {
                schoolClassList.Clear();
            }
        }

        async Task OnSelectedSchoolChanged(IEnumerable<string> e)
        {
            selectedSchool = e.ElementAt(0);
            schid = schools.FirstOrDefault(s => s.School == selectedSchool).SchID;

            schoolClassList = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + schid + "/0");
        }

        async Task UpdateEntry()
        {
            shoolclass.ClassID = selectedItem.ClassID;
            shoolclass.SchID = selectedItem.SchID;
            shoolclass.ClassListID = selectedItem.ClassListID;
            shoolclass.DisciplineID = desciplines.FirstOrDefault(s => s.Discipline == selectedItem.Discipline).DisciplineID;
            shoolclass.StaffID = staffs.FirstOrDefault(s => s.StaffNameWithNo == selectedItem.ClassTeacherWithNo).StaffID;
            shoolclass.CATID = selectedItem.CATID;
            shoolclass.FinalYearClass = selectedItem.FinalYearClass;

            await classService.UpdateAsync("AdminSchool/UpdateClass/", 1, shoolclass);
            Snackbar.Add("Selected Row Entries Updated Successfully.");
        }

        async Task SaveSelection()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Class Allocation To Teachers Operation",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                foreach (var item in schoolClassList)
                {
                    shoolclass.ClassID = item.ClassID;
                    int _inactiveStaffID = schoolClassList.FirstOrDefault(c => c.ClassID == item.ClassID).StaffID;
                    shoolclass.SchID = item.SchID;
                    shoolclass.ClassListID = item.ClassListID;
                    shoolclass.DisciplineID = desciplines.FirstOrDefault(s => s.Discipline == item.Discipline).DisciplineID;
                    var _staffs = staffs.FirstOrDefault(s => s.StaffNameWithNo == item.ClassTeacherWithNo);
                    if (_staffs != null)
                    {
                        shoolclass.StaffID = staffs.FirstOrDefault(s => s.StaffNameWithNo == item.ClassTeacherWithNo).StaffID;
                    }
                    else
                    {
                        shoolclass.StaffID = item.StaffID;
                    }
                    shoolclass.CATID = item.CATID;
                    shoolclass.FinalYearClass = item.FinalYearClass;

                    await classService.UpdateAsync("AdminSchool/UpdateClass/", 1, shoolclass);;
                }

                await Swal.FireAsync("", "Class Has Been Successfully Allocation To Teachers.", "success");
            }
        }

        #endregion

        #region [Section - Click Events]
        void GoBack()
        {
            navManager.NavigateTo("/classlist");
        }
        #endregion


    }
}
