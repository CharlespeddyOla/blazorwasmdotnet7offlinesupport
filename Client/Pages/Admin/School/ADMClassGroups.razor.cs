using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Shared.Models.Administration.Students;

namespace WebAppAcademics.Client.Pages.Admin.School
{
    public partial class ADMClassGroups
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassGroup> classGroupService { get; set; }
        [Inject] IAPIServices<ADMStudents> studentService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int toolBarMenuId { get; set; }
        int classlistid { get; set; }
        int schid { get; set; }
        string selectedSchool { get; set; }
        // Set default page title and button text
        string pagetitle = "Create a new Class Group";
        string buttontitle = "Save";
        bool useconvension { get; set; }

        bool disableSaveButton { get; set; } = true;
        string editFormId { get; set; } = "editformid";
        #endregion

        #region [Models Declaration]
        List<ADMSchlList> schools = new();
        List<ADMSchClassGroup> groups = new();
        ADMSchClassGroup groupname = new();

        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Class Group";
            toolBarMenuId = 1;
            await LoadList();
            await base.OnInitializedAsync();
        }

        #region [Section - Group List]
        async Task LoadList()
        {
            groups.Clear();
            var groupsAll = await classGroupService.GetAllAsync("AdminSchool/GetClassGroups/0/0");
            useconvension = groupsAll.FirstOrDefault(g => g.ClassListID == 1).UseConvension;

            foreach (var item in groupsAll)
            {
                int _studentCount = await studentService.CountAsync("AdminStudent/GetCount/3/" + item.ClassListID + "/0/0/", 1);

                groups.Add(new ADMSchClassGroup
                {
                    ClassListID = item.ClassListID,
                    School = item.School,
                    SchClass = item.SchClass,
                    ConvensionalName = item.ConvensionalName,
                    ClassGroupCount = _studentCount,
                    UseConvension = item.UseConvension,
                });
            }
        }

        async Task CheckBoxChanged(bool value)
        {
            useconvension = value;

            groupname.UseConvension = useconvension;
            await classGroupService.UpdateAsync("AdminSchool/UpdateClassGroup/", 2, groupname);

            await LoadList();
        }

        #endregion

        #region [Section - Group Details]
        async Task RetrieveGroupName(int _classlistid)
        {
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            groupname = await classGroupService.GetByIdAsync("AdminSchool/GetClassGroup/", _classlistid);
            classlistid = _classlistid;
            // Change page title and button text since this is an edit.
            pagetitle = "Edit Group Details";
            buttontitle = "Update";
        }

        void OnSelectedSchoolChanged(IEnumerable<string> e)
        {
            selectedSchool = e.ElementAt(0);
            schid = schools.FirstOrDefault(s => s.School == selectedSchool).SchID;
            groupname.SchID = schid;
            groupname.School = selectedSchool;
        }

        private async Task SubmitValidForm()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Class Group Save/Update Operation",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                if (classlistid == 0)
                {
                    groupname.UseConvension = false;
                    var response = await classGroupService.SaveAsync("AdminSchool/AddClassGroup/", groupname);
                    groupname.ClassListID = response.ClassListID;
                    groupname.Id = response.ClassListID;
                    await classGroupService.UpdateAsync("AdminSchool/UpdateClassGroup/", 3, groupname);
                    await Swal.FireAsync("New Class Group", "Has Been Successfully Saved.", "success");
                }
                else
                {
                    groupname.ClassListID = classlistid;
                    await classGroupService.UpdateAsync("AdminSchool/UpdateClassGroup/", 1, groupname);
                    await Swal.FireAsync("Selected Class Group", "Has Been Successfully Updated.", "success");
                }

                await ClassGroupEvent();
            }
        }

        async Task InvalidEntries()
        {
            await Swal.FireAsync("Please Check Your Entries.", "One or More Entries has error.", "error");
        }

        #endregion

        #region [Section - Click Events]
        async Task ClassGroupEvent()
        {
            toolBarMenuId = 1;
            buttontitle = "Save";
            disableSaveButton = true;
            groups.Clear();
            await LoadList();
        }

        async Task CreateNewGroup()
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            pagetitle = "Create a new Class Group";
            buttontitle = "Save";
            classlistid = 0;
            groupname = new ADMSchClassGroup();
            schools.Clear();
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
        }

        async Task UpdateClassGroup(int _classlistid)
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            groupname = new ADMSchClassGroup();            
            await RetrieveGroupName(_classlistid);
        }

        void GoBack()
        {
            navManager.NavigateTo("/classlist");
        }

        #endregion


    }
}
