using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Administration.School;

namespace WebAppAcademics.Client.Pages.Admin.School
{
    public partial class ADMClassDisciplines
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMSchClassDiscipline> classDisciplineService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int toolBarMenuId { get; set; }
        int disciplineid { get; set; }

        // Set default page title and button text
        string pagetitle = "Create a new Class Discipline";
        string buttontitle = "Save";

        bool disableSaveButton { get; set; } = true;
        string editFormId { get; set; } = "editformid";
        #endregion

        #region [Models Declaration]
        List<ADMSchClassDiscipline> desciplines = new();
        ADMSchClassDiscipline descipline = new();

        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Class Disciplines";
            toolBarMenuId = 1;
            desciplines = await classDisciplineService.GetAllAsync("AdminSchool/GetDisciplines/1");
            await base.OnInitializedAsync();
        }

        #region [Section - List]

        #endregion

        #region [Section - Details]
        async Task RetrieveDiscipline(int _disciplineid)
        {
            descipline = await classDisciplineService.GetByIdAsync("AdminSchool/GetDiscipline/", _disciplineid);
            disciplineid = _disciplineid;
            // Change page title and button text since this is an edit.
            pagetitle = "Edit " + descipline.Discipline;
            buttontitle = "Update";
        }

        async Task SubmitValidForm()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Class Discipline Save/Update Operation",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                if (disciplineid == 0)
                {
                    var response = await classDisciplineService.SaveAsync("AdminSchool/AddDiscipline/", descipline);
                    descipline.DisciplineID = response.DisciplineID;
                    descipline.Id= response.DisciplineID;
                    await classDisciplineService.UpdateAsync("AdminSchool/UpdateDiscipline/", 2, descipline);
                    await Swal.FireAsync("New Class Discipline", "Has Been Successfully Saved.", "success");
                }
                else
                {
                    descipline.DisciplineID = disciplineid;
                    await classDisciplineService.UpdateAsync("AdminSchool/UpdateDiscipline/", 1, descipline);
                    await Swal.FireAsync("Selected Class Discipline", "Has Been Successfully Updated.", "success");
                }

                await ClassDisciplinesEvent();
            }
        }

        async Task InvalidEntries()
        {
            await Swal.FireAsync("Please Check Your Entries.", "One or More Entries has error.", "error");
        }


        #endregion

        #region [Section - Click Events]
        async Task ClassDisciplinesEvent()
        {
            toolBarMenuId = 1;
            buttontitle = "Save";
            disableSaveButton = true;
            desciplines.Clear();
            desciplines = await classDisciplineService.GetAllAsync("AdminSchool/GetDisciplines/1");
        }

        void CreateNewClassDiscipline()
        {
            toolBarMenuId = 2;
            buttontitle = "Save";
            disableSaveButton = false;
            pagetitle = "Create a new Class Discipline";
            disciplineid = 0;
            descipline = new ADMSchClassDiscipline();
        }

        async Task UpdateClassDiscipline(int _disciplineid)
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            descipline = new ADMSchClassDiscipline();
            await RetrieveDiscipline(_disciplineid);
        }

        void GoBack()
        {
            navManager.NavigateTo("/classlist");
        }
        #endregion



    }
}
