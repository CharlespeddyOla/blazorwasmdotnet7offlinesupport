using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Administration.School;

namespace WebAppAcademics.Client.Pages.Admin.School
{
    public partial class ADMClassCategories
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMSchClassCategory> classNamesService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int toolBarMenuId { get; set; }
        int catid { get; set; }
        string pagetitle = "Create a new Class Name";
        string buttontitle = "Save";
        bool disableSaveButton { get; set; } = true;
        string editFormId { get; set; } = "editformid";
        #endregion

        #region [Models Declaration]
        List<ADMSchClassCategory> classnamelist = new();
        ADMSchClassCategory classname = new();

        #endregion


        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Class Names";
            toolBarMenuId = 1;
            classnamelist = await classNamesService.GetAllAsync("AdminSchool/GetCategories/1");
            await base.OnInitializedAsync();
        }

        #region [Section - Category List]

        #endregion

        #region [Section - Category Details]
        async Task RetrieveClassName(int _catid)
        {
            disableSaveButton = false;
            classname = await classNamesService.GetByIdAsync("AdminSchool/GetCategory/", _catid);
            catid = _catid;
            // Change page title and button text since this is an edit.
            pagetitle = "Edit " + classname.CATName;
            buttontitle = "Update";
        }

        async Task SubmitValidForm()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Class Name Save/Update Operation",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                if (catid == 0)
                {
                    var response = await classNamesService.SaveAsync("AdminSchool/AddCategory/", classname);
                    classname.CATID = response.CATID;
                    classname.Id = response.CATID;
                    await classNamesService.UpdateAsync("AdminSchool/UpdateCategory/", 2, classname);
                    await Swal.FireAsync("New Class Name", "Has Been Successfully Saved.", "success");
                }
                else
                {
                    classname.CATID = catid;
                    await classNamesService.UpdateAsync("AdminSchool/UpdateCategory/", 1, classname);
                    await Swal.FireAsync("Selected Class Name", "Has Been Successfully Updated.", "success");
                }

                await ClassCategoriesEvents();
            }
        }

        async Task InvalidEntries()
        {
            await Swal.FireAsync("Please Check Your Entries.", "One or More Entries has error.", "error");
        }
        #endregion

        #region [Section - Click Events]
        async Task ClassCategoriesEvents()
        {
            toolBarMenuId = 1;
            buttontitle = "Save";
            disableSaveButton = true;
            classnamelist.Clear();
            classnamelist = await classNamesService.GetAllAsync("AdminSchool/GetCategories/1");
        }

        void CreateNewCategory()
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            pagetitle = "Create a new Class Name";
            buttontitle = "Save";
            catid = 0;
            classname = new ADMSchClassCategory();
        }

        async Task UpdateClassCategory(int _catid)
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            classname = new ADMSchClassCategory();
            await RetrieveClassName(_catid);
        }

        void GoBack()
        {
            navManager.NavigateTo("/classlist");
        }

        #endregion

    }
}
