using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Administration.Students;
using WebAppAcademics.Shared.Models.Settings;
using WebAppAcademics.Client.Extensions;

namespace WebAppAcademics.Client.Pages.Admin.Student
{
    public partial class Parents
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<SETStatusType> statusTypeService { get; set; }
        [Inject] IAPIServices<ADMSchParents> parentService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }

        int toolBarMenuId { get; set; }
        #endregion

        #region [Models Declaration]
        List<SETStatusType> statusType = new();
        List<ADMSchParents> parents = new();
        ADMSchParents parent = new();

        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Parent List";
            toolBarMenuId = 1;
            parents = await parentService.GetAllAsync("AdminStudent/GetParents/1/1");
            statusType = await statusTypeService.GetAllAsync("Settings/GetStatusTypes/1");
            statusTypeCountDisplay = "Total Active Parent Count: " + parents.Count();
            schinfoid = await localStorageService.ReadEncryptedItemAsync<int>("schinfoid");
            await base.OnInitializedAsync();
        }

        #region [Section - List]
        #region [Variables Declaration]
        int statustypeid { get; set; } = 1;
        string selectedStatusType { get; set; } = "Active";
        string statusTypeCountDisplay { get; set; }
        string searchString { get; set; } = string.Empty;
        #endregion

        async Task OnSelectedStatusTypeChanged(IEnumerable<string> e)
        {
            selectedStatusType = e.ElementAt(0);
            statustypeid = statusType.FirstOrDefault(s => s.StatusType == selectedStatusType).StatusTypeID;

            parents = await parentService.GetAllAsync("AdminStudent/GetParents/1/" + statustypeid);
            statusTypeCountDisplay = "Total " + selectedStatusType + " Parent Count: " + parents.Count();
        }


        async Task DeleteParent(int _payeeid)
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Parent Deletion Operation",
                Text = "Do You Want To Continue With This Operation? This Operation Cannot Be Undo.",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                parent.PayeeID = _payeeid;
                parent.DeletePayee = true;
                await parentService.UpdateAsync("AdminStudent/UpdateParent/", 2, parent);
                await Swal.FireAsync("Parent Deletion Operation", "Selected Parent Has Been Successfully Deleted.", "success");
                await ParentListEvent();
            }
        }



        private bool FilterFunc(ADMSchParents model)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (model.ParentNo.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.ParentSurname.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            //if (model.FatherName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            //    return true;
            //if (model.MotherName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            //    return true;

            return false;
        }

        #endregion

        #region [Section - Details]
        #region [Variables Declaration]
        int payeeid { get; set; }
        int schinfoid { get; set; }
        string pagetitle { get; set; } = "Add a new Parent";
        string buttontitle = "Save";

        //Photo Declaration Section
        string imgSrc { get; set; } = "";
        string imgSrcM { get; set; } = "";
        IBrowserFile file { get; set; } = null;
        IBrowserFile fileM { get; set; } = null;
        byte[] _fileBytes { get; set; } = null;
        byte[] _fileBytesM { get; set; } = null;
        Utilities utilities = new Utilities();
        long maxFileSize { get; set; } = 1024 * 1024 * 15;

        string parentTitle { get; set; } = "Mr. & Mrs.";
        bool disableControl { get; set; } = true;
        string parentStatus { get; set; } = "Active";
        bool parentStatuschecked { get; set; } = true;

        bool disableSaveButton { get; set; } = true;
        string editFormId { get; set; } = "editformid";
        #endregion

        #region [Photo Processing]

        async Task UploadFatherPhoto(InputFileChangeEventArgs e)
        {
            try
            {
                file = e.File;
                using var ms = new MemoryStream();
                var stream = file.OpenReadStream(maxFileSize);

                await stream.CopyToAsync(ms);
                _fileBytes = ms.ToArray();

                var photo = utilities.GetImage(Convert.ToBase64String(_fileBytes));
                imgSrc = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(photo), 50, 50);

                //TODO upload the files to the server
            }
            catch (Exception ex)
            {
                var msg = file.Name + ex.Message;
            }
        }

        async Task UploadMotherPhoto(InputFileChangeEventArgs e)
        {
            try
            {
                fileM = e.File;
                using var ms = new MemoryStream();
                var stream = fileM.OpenReadStream(maxFileSize);

                await stream.CopyToAsync(ms);
                _fileBytesM = ms.ToArray();

                var photo = utilities.GetImage(Convert.ToBase64String(_fileBytesM));
                imgSrcM = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(photo), 50, 50);

                //TODO upload the files to the server
            }
            catch (Exception ex)
            {
                var msg = fileM.Name + ex.Message;
            }
        }

        #endregion

        async Task RetrieveParentDetails(int _payeeid)
        {
            disableControl = false;
            parent = await parentService.GetByIdAsync("AdminStudent/GetParent/", _payeeid);
            payeeid = _payeeid;
                       
            buttontitle = "Update";

            if (parent.StatusTypeID == 1)
            {
                parentStatuschecked = true;
            }
            else
            {
                parentStatuschecked = false;
            }

            if (parent.fatherPhoto != null)
            {
                parent.fatherPhoto = utilities.GetImage(Convert.ToBase64String(parent.fatherPhoto));
                parent.ImageUrl = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(parent.fatherPhoto));
            }

            if (parent.motherPhoto != null)
            {
                parent.motherPhoto = utilities.GetImage(Convert.ToBase64String(parent.motherPhoto));
                parent.ImageUrlM = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(parent.motherPhoto));
            }

            imgSrc = parent.ImageUrl;
            imgSrcM = parent.ImageUrlM;
        }

        async Task SubmitValidForm()
        {
            if (_fileBytes != null)
            {
                parent.photoStatusFather = 1;
                parent.fatherPhoto = _fileBytes;
            }

            if (_fileBytesM != null)
            {
                parent.photoStatusMother = 1;
                parent.motherPhoto = _fileBytesM;
            }

            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Parent Details Save/Update Operation",
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
                    parent.SchInfoID = schinfoid;
                    if (parent.PayeeID == 0)
                    {
                        parent.StatusTypeID = 1;
                        parent.PrefixID = 3;
                        var response = await parentService.SaveAsync("AdminStudent/AddParent/", parent);
                        parent.PayeeID  = response.PayeeID;
                        parent.Id = response.PayeeID;
                        await parentService.UpdateAsync("AdminStudent/UpdateParent/", 4, parent);
                        await Swal.FireAsync("The New Parent Details", "Has Been Successfully Saved.", "success");
                    }
                    else
                    {
                        if (parentStatuschecked == true)
                        {
                            parent.StatusTypeID = 1;
                        }
                        else if (parentStatuschecked == false)
                        {
                            parent.StatusTypeID = 2;
                        }

                        await parentService.UpdateAsync("AdminStudent/UpdateParent/", 1, parent);
                        await Swal.FireAsync("Selected Parent Details", "Has Been Successfully Updated.", "success");
                    }

                    await ParentListEvent();
                }
            }
        }

        async Task InvalidEntries()
        {
            await Swal.FireAsync("Please Check Your Entries.", "One or More Entries has error.", "error");
        }

        #endregion

        #region [Section - Click Events]
        async Task ParentListEvent()
        {
            toolBarMenuId = 1;
            buttontitle = "Save";
            disableSaveButton = true;
            statustypeid = 1;
            searchString = string.Empty;
            parents.Clear();
            parents = await parentService.GetAllAsync("AdminStudent/GetParents/1/1");
            selectedStatusType = "Active";
            statusTypeCountDisplay = "Total Active Parent Count: " + parents.Count();
        }

        void CreateNewParent()
        {
            toolBarMenuId = 2;
            buttontitle = "Save";
            disableSaveButton = false;
            pagetitle = "Add a new Parent";           
            payeeid = 0;
            parentStatus = "Active";
            parentStatuschecked = true;
            disableControl = true;
            imgSrc = String.Empty;
            imgSrcM = String.Empty;
            parent = new ADMSchParents();
            parent.ParentTitle = parentTitle;
        }

        async Task UpdateParentDetails(int _payeeid)
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            parent = new ADMSchParents();
            await RetrieveParentDetails(_payeeid);
        }

        void GoBack()
        {
            navManager.NavigateTo("/students");
        }
        #endregion


    }
}
