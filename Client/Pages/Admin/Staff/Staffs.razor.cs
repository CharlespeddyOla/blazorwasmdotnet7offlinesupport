using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Administration.Staff;
using WebAppAcademics.Shared.Models.Financials.Banks;
using WebAppAcademics.Shared.Models.Settings;
using WebAppAcademics.Client.Extensions;

namespace WebAppAcademics.Client.Pages.Admin.Staff
{
    public partial class Staffs
    {
        #region [Injection Declaration]
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<SETStatusType> statusTypeService { get; set; }
        [Inject] IAPIServices<ADMEmployeeDepts> departmentService { get; set; }
        [Inject] IAPIServices<ADMEmployeeLocation> staffLocationService { get; set; }
        [Inject] IAPIServices<ADMEmployeeJobType> jobTypeService { get; set; }
        [Inject] IAPIServices<ADMEmployee> staffService { get; set; }
        [Inject] IAPIServices<SETGender> genderService { get; set; }
        [Inject] IAPIServices<SETCountries> countryService { get; set; }
        [Inject] IAPIServices<SETStates> stateService { get; set; }
        [Inject] IAPIServices<SETLGA> lgaService { get; set; }
        [Inject] IAPIServices<SETReligion> religionService { get; set; }
        [Inject] IAPIServices<ADMEmployeeTitle> staffTitleService { get; set; }
        [Inject] IAPIServices<ADMEmployeeMaritalStatus> staffMaritalService { get; set; }
        [Inject] IAPIServices<FINBankDetails> bankService { get; set; }

        [Parameter] public EventCallback<bool> OnBirthDayReminder { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }

        int toolBarMenuId { get; set; }
        bool isLoading { get; set; } = true;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Staff List";
            toolBarMenuId = 1;
            await LoadList();
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            roleid = await localStorageService.ReadEncryptedItemAsync<int>("roleid");
            schinfoid = await localStorageService.ReadEncryptedItemAsync<int>("schinfoid");
            await base.OnInitializedAsync();
        }


        #region [Section - Staff List]

        #region [Variables Declaration]
        int roleid { get; set; }
        int deptid { get; set; }
        int jobtypeid { get; set; }
        int locid { get; set; }
        int statustypeid { get; set; } = 1;
        int deptcount { get; set; }
        int jobtypecount { get; set; }
        int statusTypeCount { get; set; }

        string selectedDepartment { get; set; }
        string selectedJobType { get; set; }
        string selectedLocation { get; set; }
        string selectedStatusType { get; set; } = "Active";
        string deptCountDisplay { get; set; }
        string jobTypeCountDisplay { get; set; }
        string statusTypeCountDisplay { get; set; }
        string searchString { get; set; } = "";
        string checkboxtitle { get; set; } = "Tick For Birthday Reminder";

        protected bool birthdayreminder { get; set; }

        #endregion

        #region [Models Declaration]        
        List<ADMEmployee> stafflist = new();
        List<ADMEmployeeDepts> deptlist = new();
        List<ADMEmployeeJobType> jobtypelist = new();
        List<ADMEmployeeLocation> loclist = new();
        List<ADMEmployee> staffCount = new();
        List<SETStatusType> statusTypeList = new();
        #endregion

        #region [Load / Filter Events]
        async Task LoadList()
        {
            deptlist = await departmentService.GetAllAsync("AdminStaff/GetDepartments/1");
            jobtypelist = await jobTypeService.GetAllAsync("AdminStaff/GetJobTypes/1");
            loclist = await staffLocationService.GetAllAsync("AdminStaff/GetStaffLocations/1");
            statusTypeList = await statusTypeService.GetAllAsync("Settings/GetStatusTypes/1");
                        
            staffCount = await staffService.GetAllAsync("AdminStaff/GetStaffs/0/0/0/0/0");
            deptCountDisplay = string.Empty;
            jobTypeCountDisplay = string.Empty;
                        
            await LoadStaffs(1,1,0,0,0);
            statusTypeCountDisplay = "Total " + selectedStatusType + " Staff Count: " + stafflist.Count();
        }
        
        async Task LoadStaffs(int switchid, int statustypeid, int deptid, int jobtypeid, int locid)
        {
            isLoading = true;
            StateHasChanged();

            try
            {
                await Task.Delay(500);
                stafflist = await staffService.GetAllAsync("AdminStaff/GetStaffs/" + switchid + "/" + statustypeid + "/" + 
                                                            deptid + "/" + jobtypeid + "/" + locid);
            }
            finally
            {
                isLoading = false;
            }
        }

        async Task CheckBoxChanged(bool value)
        {
            birthdayreminder = value;

            if (birthdayreminder)
            {
                checkboxtitle = "Remove Birthday Reminder";
                await LoadStaffs(12, 1, 0, 0, 0);
            }
            else
            {
                checkboxtitle = "Birthday Reminder";
                await LoadStaffs(1, statustypeid, deptid, jobtypeid, locid);
            }
        }

        async Task<bool> StatusTypeFilter()
        {
            deptCountDisplay = string.Empty;
            jobTypeCountDisplay = string.Empty;
            statusTypeCountDisplay = string.Empty;

            if (String.IsNullOrWhiteSpace(selectedDepartment) && String.IsNullOrWhiteSpace(selectedJobType))
            {
                await LoadStaffs(1, statustypeid, deptid, jobtypeid, locid);

                deptCountDisplay = string.Empty;
                jobTypeCountDisplay = string.Empty;
                statusTypeCountDisplay = "Total " + selectedStatusType + " Staff Count: " + stafflist.Count();

                return true;
            }
            else if (!String.IsNullOrWhiteSpace(selectedDepartment) && String.IsNullOrWhiteSpace(selectedJobType))
            {
                statusTypeCount = staffCount.Where(s => s.StatusTypeID == statustypeid).Count();

                await LoadStaffs(2, statustypeid, deptid, jobtypeid, locid);
                deptCountDisplay = selectedStatusType + " Staff Count For " + selectedDepartment + " Department: " + stafflist.Count();
                jobTypeCountDisplay = string.Empty;
                statusTypeCountDisplay = "Total " + selectedStatusType + " Staff Count: " + statusTypeCount;

                return true;
            }
            else if (!String.IsNullOrWhiteSpace(selectedDepartment) && !String.IsNullOrWhiteSpace(selectedJobType))
            {
                deptcount = staffCount.Where(d => d.StatusTypeID == statustypeid && d.EmployeeGroupID == deptid).Count();
                statusTypeCount = staffCount.Where(s => s.StatusTypeID == statustypeid).Count();

                await LoadStaffs(5, statustypeid, deptid, jobtypeid, locid);
                deptCountDisplay = selectedStatusType + " Staff Count For " + selectedDepartment + " Department: " + deptcount;
                jobTypeCountDisplay = selectedStatusType + " Staff Count For " + selectedJobType + " Job: " + stafflist.Count();
                statusTypeCountDisplay = "Total " + selectedStatusType + " Student Count: " + statusTypeCount;

                return true;
            }

            return false;
        }

        async Task<bool> DepartmentFilter()
        {
            deptCountDisplay = string.Empty;
            jobTypeCountDisplay = string.Empty;
            statusTypeCountDisplay = string.Empty;

            if (!String.IsNullOrWhiteSpace(selectedDepartment) && String.IsNullOrWhiteSpace(selectedJobType))
            {
                statusTypeCount = staffCount.Where(s => s.StatusTypeID == statustypeid).Count();

                await LoadStaffs(2, statustypeid, deptid, jobtypeid, locid);
                deptCountDisplay = selectedStatusType + " Staff Count For " + selectedDepartment + " Department: " + stafflist.Count();
                jobTypeCountDisplay = string.Empty;
                statusTypeCountDisplay = "Total " + selectedStatusType + " Staff Count: " + statusTypeCount;

                return true;
            }
            else if (!String.IsNullOrWhiteSpace(selectedDepartment) && !String.IsNullOrWhiteSpace(selectedJobType))
            {
                deptcount = staffCount.Where(d => d.StatusTypeID == statustypeid && d.EmployeeGroupID == deptid).Count();
                statusTypeCount = staffCount.Where(s => s.StatusTypeID == statustypeid).Count();

                await LoadStaffs(5, statustypeid, deptid, jobtypeid, locid);
                deptCountDisplay = selectedStatusType + " Staff Count For " + selectedDepartment + " Department: " + deptcount;
                jobTypeCountDisplay = selectedStatusType + " Staff Count For " + selectedJobType + " Job: " + stafflist.Count();
                statusTypeCountDisplay = "Total " + selectedStatusType + " Student Count: " + statusTypeCount;

                return true;
            }

            return false;
        }

        async Task<bool> JobTypeFilter()
        {
            deptCountDisplay = string.Empty;
            jobTypeCountDisplay = string.Empty;
            statusTypeCountDisplay = string.Empty;

            if (String.IsNullOrWhiteSpace(selectedDepartment) && !String.IsNullOrWhiteSpace(selectedJobType))
            {
                statusTypeCount = staffCount.Where(s => s.StatusTypeID == statustypeid).Count();

                await LoadStaffs(3, statustypeid, deptid, jobtypeid, locid);
                deptCountDisplay = string.Empty;
                jobTypeCountDisplay = selectedStatusType + " Staff Count For " + selectedJobType + " Job: " + stafflist.Count();
                statusTypeCountDisplay = "Total " + selectedStatusType + " Staff Count: " + statusTypeCount;

                return true;
            }
            else if (!String.IsNullOrWhiteSpace(selectedDepartment) && !String.IsNullOrWhiteSpace(selectedJobType))
            {
                deptcount = staffCount.Where(d => d.StatusTypeID == statustypeid && d.EmployeeGroupID == deptid).Count();
                statusTypeCount = staffCount.Where(s => s.StatusTypeID == statustypeid).Count();

                await LoadStaffs(5, statustypeid, deptid, jobtypeid, locid);
                deptCountDisplay = selectedStatusType + " Staff Count For " + selectedDepartment + " Department: " + deptcount;
                jobTypeCountDisplay = selectedStatusType + " Staff Count For " + selectedJobType + " Job: " + stafflist.Count();
                statusTypeCountDisplay = "Total " + selectedStatusType + " Student Count: " + statusTypeCount;

                return true;
            }

            return false;
        }

        #endregion

        #region [OnChange Events]

        async Task OnSelectedDeptChanged(IEnumerable<string> e)
        {
            selectedDepartment = e.ElementAt(0);
            deptid = deptlist.FirstOrDefault(d => d.EmployeeGroup == selectedDepartment).EmployeeGroupID;

            if (await DepartmentFilter()) { }
        }

        async Task OnSelectedJobTypeChanged(IEnumerable<string> e)
        {
            selectedJobType = e.ElementAt(0);
            jobtypeid = jobtypelist.FirstOrDefault(j => j.JobType == selectedJobType).JobTypeID;

            if (await JobTypeFilter()) { }
        }

        async Task OnSelectedStatusTypeChanged(IEnumerable<string> e)
        {
            selectedStatusType = e.ElementAt(0);
            statustypeid = statusTypeList.FirstOrDefault(s => s.StatusType == selectedStatusType).StatusTypeID;

            if (await StatusTypeFilter()) { }
        }

        async Task RefreshList()
        {
            selectedDepartment = string.Empty;
            deptlist.Clear();
            deptlist = await departmentService.GetAllAsync("AdminStaff/GetDepartments/1");

            selectedJobType = string.Empty;
            jobtypelist.Clear();
            jobtypelist = await jobTypeService.GetAllAsync("AdminStaff/GetJobTypes/1");

            selectedLocation = string.Empty;
            loclist.Clear();
            loclist = await staffLocationService.GetAllAsync("AdminStaff/GetStaffLocations/1");

            selectedStatusType = string.Empty;
            statusTypeList.Clear();
            statustypeid = 1;
            selectedStatusType = "Active";
            statusTypeList = await statusTypeService.GetAllAsync("Settings/GetStatusTypes/1");

            stafflist.Clear();
            await LoadStaffs(1, 1, 0, 0, 0);
            deptCountDisplay = string.Empty;
            jobTypeCountDisplay = string.Empty;
            statusTypeCountDisplay = "Total " + selectedStatusType + " Staff Count: " + stafflist.Count();

            searchString = string.Empty;
        }

        private bool FilterFunc(ADMEmployee model)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (model.StaffNo.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.StaffName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.EmployeeGroup.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (model.JobTitle.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            //if (model.StaffLocation.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            //    return true;

            return false;
        }

        #endregion

        async Task DeleteStaff(int _staffid)
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Staff Deletion Operation",
                Text = "Do You Want To Continue With This Operation? This Operation Cannot Be Undo.",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                staffdetails.StaffID = _staffid;
                staffdetails.DeleteName = true;
                await staffService.UpdateAsync("AdminStaff/UpdateStaff/", 7, staffdetails);
                await Swal.FireAsync("Staff Deletion Operation", "Selected Staff Has Been Successfully Deleted.", "success");
                await RefreshList();
            }
        }

        #endregion

        #region [Section - Staff Details]

        #region [Variables Declaration]
        int staffid { get; set; }
        int termid { get; set; }
        int schinfoid { get; set; }
        int newstaffid { get; set; }

        string _selectedDepartment { get; set; }
        string _selectedJobType { get; set; }
        string _selectedLocation { get; set; }
        string selectedGender { get; set; }
        string selectedRegion { get; set; }
        string selectedCountry { get; set; }
        string selectedState { get; set; }
        string selectedLGA { get; set; }
        string selectedTitle { get; set; }
        string selectedMaritalStatus { get; set; }
        string selectedBank { get; set; }

        protected bool staffStatus { get; set; } = true;
        string _checkboxtitle { get; set; } = "Active";

        // Set default page title and button text
        string pagetitle { get; set; } = "Add a new Staff";
        string buttontitle = "Save";

        //Photo Declaration Section
        string imgSrc { get; set; } = "";
        string signSrc { get; set; } = "";
        IBrowserFile file { get; set; } = null;
        byte[] _fileBytes { get; set; } = null;
        byte[] _fileBytesSign { get; set; } = null;
        long maxFileSize { get; set; } = 1024 * 1024 * 15;
        Utilities utilities = new Utilities();

        bool disableControl { get; set; } = true;
        bool selectedImageType { get; set; } = false;
        private bool _isOpen { get; set; }

        bool disableSaveButton { get; set; } = true;
        string editFormId { get; set; } = "editformid";

        #endregion

        #region [Models Declaration]        
        List<ADMEmployeeDepts> _deptlist = new();
        List<ADMEmployeeLocation> _loclist = new();
        List<ADMEmployeeJobType> _jobtypelist = new();
        List<SETStatusType> _statusTypeList = new();
        List<SETGender> gender = new();
        List<SETCountries> country = new();
        List<SETStates> states = new();
        List<SETLGA> lgas = new();
        List<SETReligion> religion = new();
        List<ADMEmployeeTitle> titles = new();
        List<ADMEmployeeMaritalStatus> maritalstatus = new();
        List<FINBankDetails> bankList = new();
        List<ADMEmployee> _stafflist = new();

        ADMEmployee staffdetails = new();
        #endregion

        #region [Load Events]
        async Task LoadStates(int countryid)
        {
            states = await stateService.GetAllAsync("Settings/GetStates/1/" + countryid);
        }

        async Task LoadLGA(int stateid)
        {
            lgas = await lgaService.GetAllAsync("Settings/GetLGAs/1/" + stateid);
        }

        async Task LoadDefaultList()
        {
            _deptlist = await departmentService.GetAllAsync("AdminStaff/GetDepartments/1");
            _jobtypelist = await jobTypeService.GetAllAsync("AdminStaff/GetJobTypes/1");
            _loclist = await staffLocationService.GetAllAsync("AdminStaff/GetStaffLocations/1");
            _statusTypeList = await statusTypeService.GetAllAsync("Settings/GetStatusTypes/1");
            gender = await genderService.GetAllAsync("Settings/GetGenderList/1");
            country = await countryService.GetAllAsync("Settings/GetCountries/1");
            religion = await religionService.GetAllAsync("Settings/GetReligionList/1");
            titles = await staffTitleService.GetAllAsync("AdminStaff/GetStaffTitles/1");
            maritalstatus = await staffMaritalService.GetAllAsync("AdminStaff/GetMaritalStatusList/1");
            bankList = await bankService.GetAllAsync("FinancialsBanks/GetBanks/1/0");
            _stafflist = await staffService.GetAllAsync("AdminStaff/GetStaffs/0/0/0/0/0");
        }

        async Task RetrieveStaffDetails(int _staffid)
        {
            disableControl = false;

            staffdetails = await staffService.GetByIdAsync("AdminStaff/GetStaff/", _staffid);
            staffid = _staffid;

            pagetitle = staffdetails.StaffName;
            buttontitle = "Update";

            if (staffdetails.StatusTypeID == 1)
            {
                staffStatus = true;
            }
            else
            {
                staffStatus = false;
            }

            if (staffdetails.employeePicture != null)
            {
                staffdetails.employeePicture = utilities.GetImage(Convert.ToBase64String(staffdetails.employeePicture));
                staffdetails.ImageUrl = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(staffdetails.employeePicture));
                imgSrc = staffdetails.ImageUrl;
            }

            await LoadStates(staffdetails.CountryID);
            await LoadLGA(staffdetails.StateID);
        }
        #endregion

        #region [OnChange Events]
        void StaffPhotoSelected()
        {
            selectedImageType = true;
        }

        void SignatureImageSelected()
        {
            selectedImageType = false;
        }

        void CheckBoxChangedDetails(bool value)
        {
            staffStatus = value;

            if (staffStatus)
            {
                _checkboxtitle = "Active";
                staffdetails.StatusTypeID = 1;
            }
            else
            {
                _checkboxtitle = "In-Active";
                staffdetails.StatusTypeID = 2;
            }
        }

        void OnDepartmentChange(IEnumerable<string> e)
        {
            selectedDepartment = e.ElementAt(0);
            staffdetails.EmployeeGroupID = _deptlist.FirstOrDefault(d => d.EmployeeGroup == selectedDepartment).EmployeeGroupID;
            staffdetails.EmployeeGroup = selectedDepartment;
        }

        void OnJobTypeChange(IEnumerable<string> e)
        {
            selectedJobType = e.ElementAt(0);
            staffdetails.JobTypeID = _jobtypelist.FirstOrDefault(j => j.JobType == selectedJobType).JobTypeID;
            staffdetails.JobType = selectedJobType;
        }

        void OnGenderChange(IEnumerable<string> value)
        {
            selectedGender = value.ElementAt(0);
            staffdetails.Gender = selectedGender;
            staffdetails.GenderID = gender.FirstOrDefault(s => s.Gender == selectedGender).GenderID;
        }

        void OnTitleChange(IEnumerable<string> value)
        {
            selectedTitle = value.ElementAt(0);
            staffdetails.Title = selectedTitle;
            staffdetails.TitleID = titles.FirstOrDefault(s => s.Title == selectedTitle).TitleID;
        }

        void OnMaritalStatusChange(IEnumerable<string> value)
        {
            selectedMaritalStatus = value.ElementAt(0);
            staffdetails.MStatus = selectedMaritalStatus;
            staffdetails.MStatusID = maritalstatus.FirstOrDefault(s => s.MStatus == selectedMaritalStatus).MStatusID;
        }

        void OnReligionChanged(IEnumerable<string> value)
        {
            selectedRegion = value.ElementAt(0);
            staffdetails.Religion = selectedRegion;
            staffdetails.ReligionID = religion.FirstOrDefault(s => s.Religion == selectedRegion).ReligionID;
        }

        async Task OnCountryChanged(IEnumerable<string> value)
        {
            selectedCountry = value.ElementAt(0);
            staffdetails.Country = selectedCountry;
            staffdetails.CountryID = country.FirstOrDefault(s => s.Country == selectedCountry).CountryID;
            staffdetails.State = string.Empty;

            await LoadStates(staffdetails.CountryID);
        }

        async Task OnStateChanged(IEnumerable<string> value)
        {
            selectedState = value.ElementAt(0);
            staffdetails.State = selectedState;
            staffdetails.StateID = states.FirstOrDefault(s => s.State == selectedState).StateID;
            staffdetails.LGA = string.Empty;

            await LoadLGA(staffdetails.StateID);
        }

        void OnLGAChanged(IEnumerable<string> value)
        {
            selectedLGA = value.ElementAt(0);

            if (selectedLGA != "none")
            {
                staffdetails.LGA = selectedLGA;
                staffdetails.LGAID = lgas.FirstOrDefault(s => s.LGA == selectedLGA).LGAID;
            }
        }

        void OnBankChanged(IEnumerable<string> value)
        {
            selectedBank = value.ElementAt(0);
            staffdetails.BankID = bankList.FirstOrDefault(s => s.BankAcctName == selectedBank).BankID;
            staffdetails.BankAcctName = selectedBank;
        }

        #endregion

        #region [Photo Processing]

        async Task UploadFiles(InputFileChangeEventArgs e)
        {
            try
            {
                file = e.File;
                using var ms = new MemoryStream();
                var stream = file.OpenReadStream(maxFileSize);

                await stream.CopyToAsync(ms);

                if (selectedImageType == true)
                {
                    _fileBytes = ms.ToArray();
                    var photo = utilities.GetImage(Convert.ToBase64String(_fileBytes));
                    imgSrc = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(photo));
                }
                else
                {
                    _fileBytesSign = ms.ToArray();
                    var photo = utilities.GetImage(Convert.ToBase64String(_fileBytesSign));
                    signSrc = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(photo));
                }
                //TODO upload the files to the server
            }
            catch (Exception ex)
            {
                var msg = file.Name + ex.Message;
            }
        }

        #region [Signature Preview]
        bool visible;
        void OpenDialog() => visible = true;
        void Submit() => visible = false;

        DialogOptions dialogOptions = new() { FullWidth = true };

        #endregion

        #endregion

        #region [Save Events]
        async Task SubmitValidForm()
        {
            if (_fileBytes != null)
            {
                staffdetails.photoStatus = 1;
                staffdetails.employeePicture = _fileBytes;
            }

            if (_fileBytesSign != null)
            {
                staffdetails.signPicture = _fileBytesSign;
            }

            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Staff Details Save/Update Operation",
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
                    if (staffdetails.StaffID == 0)
                    {
                        if (staffdetails.EmployeeID > 0)
                        {
                            staffdetails.SchInfoID = schinfoid;
                            staffdetails.TermID = termid;
                            staffdetails.RoleID = 0;
                            staffdetails.LocID = 1;
                            // Insert if StaffID is zero.                           
                            var response = await staffService.SaveAsync("AdminStaff/AddStaff/", staffdetails);
                            newstaffid = response.StaffID;
                            staffdetails.StaffID = newstaffid;
                            staffdetails.Id = newstaffid;
                            await staffService.UpdateAsync("AdminStaff/UpdateStaff/", 10, staffdetails);
                            await Swal.FireAsync("The New Staff Details", "Has Been Successfully Saved.", "success");
                        }
                    }
                    else
                    {
                        if (staffStatus == true)
                        {
                            staffdetails.StatusTypeID = 1;
                        }
                        else if (staffStatus == false)
                        {
                            staffdetails.StatusTypeID = 2;
                        }
                        await staffService.UpdateAsync("AdminStaff/UpdateStaff/", 1, staffdetails);
                        await Swal.FireAsync("Selected Staff Details", "Has Been Successfully Updated.", "success");
                    }
                }

                await StaffListEvent();
            }
        }

        async Task InvalidEntries()
        {
            await Swal.FireAsync("Please Check Your Entries.", "One or More Entries has error.", "error");
        }


        #endregion


        #endregion

        #region [Section - Click Events]
        async Task StaffListEvent()
        {
            toolBarMenuId = 1;
            buttontitle = "Save";
            disableSaveButton = true;
            await RefreshList();
        }

        async Task AddNewStaff()
        {
            toolBarMenuId = 2;
            buttontitle = "Save";
            disableSaveButton = false;
            staffid = 0;
            staffdetails = new ADMEmployee();
            await LoadDefaultList();
            staffdetails.Country = "NA";
            staffdetails.CountryID = 1;
            staffdetails.State = "NA";
            staffdetails.StateID = 1;
            staffdetails.LGA = "NA";
            staffdetails.LGAID = 1;
            staffdetails.StatusTypeID = 1;
            imgSrc = String.Empty;
            signSrc = String.Empty;
            staffdetails.EmployeeID = stafflist.Count() + 1;
        }

        async Task UpdateStaffDetails(int _staffid)
        {
            toolBarMenuId = 2;
            disableSaveButton = false;
            staffdetails = new ADMEmployee();
            await LoadDefaultList();
            await RetrieveStaffDetails(_staffid);
        }
               

        #endregion

    }
}
