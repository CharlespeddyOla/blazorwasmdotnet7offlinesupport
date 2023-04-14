using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Settings;

namespace WebAppAcademics.Client.Pages.Settings.School
{
    public partial class SETSchoolDetails
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<SETSchInformation> _schoolInfoService { get; set; }
        [Inject] IAPIServices<SETCountries> countryService { get; set; }
        [Inject] IAPIServices<SETStates> stateService { get; set; }
        [Inject] IAPIServices<SETLGA> lgaService { get; set; }
        [Inject] IAPIServices<string> stringValueService { get; set; }
        [Inject] IAPIServices<FileChunkDTO> stringModelService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int toolBarMenuId { get; set; }
        bool saveStatus { get; set; } = true;
        int schinfoid { get; set; }
        string imgSrc { get; set; } = "";
        string selectedCountry { get; set; }
        string selectedState { get; set; }
        string selectedLGA { get; set; }
        IBrowserFile file { get; set; } = null;
        byte[] _fileBytes { get; set; } = null;
        long maxFileSize { get; set; } = 1024 * 1024 * 15;
        bool isShow;
        InputType PasswordInput = InputType.Password;
        string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

        Utilities utilities = new Utilities();
        #endregion

        #region [Models Declaration]
        List<SETSchInformation> schoolInfoList = new();
        List<SETCountries> countrylist = new();
        List<SETStates> statelist = new();
        List<SETLGA> lgalist = new();

        SETSchInformation schInfo = new();
        FileChunkDTO stringdata = new();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "School Details";
            toolBarMenuId = 1;
            schoolInfoList = await _schoolInfoService.GetAllAsync("Settings/GetSchoolInfo/1");
            await base.OnInitializedAsync();
        }

        #region [Display School Information]
        void ShowHidePassword()
        {
            if (isShow)
            {
                isShow = false;
                PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                PasswordInput = InputType.Password;
            }
            else
            {
                isShow = true;
                PasswordInputIcon = Icons.Material.Filled.Visibility;
                PasswordInput = InputType.Text;
            }
        }

        async Task OnCountryChanged(IEnumerable<string> value)
        {
            selectedCountry = value.ElementAt(0);
            schInfo.CountryID = countrylist.FirstOrDefault(s => s.Country == selectedCountry).CountryID;
            schInfo.Country = selectedCountry;
            schInfo.State = string.Empty;

            statelist = await stateService.GetAllAsync("Settings/GetStates/1/" + schInfo.CountryID);
        }

        async Task OnStateChanged(IEnumerable<string> value)
        {
            selectedState = value.ElementAt(0);
            schInfo.StateID = statelist.FirstOrDefault(s => s.State == selectedState).StateID;
            schInfo.State = selectedState;
            schInfo.LGA = string.Empty;

            lgalist = await lgaService.GetAllAsync("Settings/GetLGAs/1/" + schInfo.StateID);
        }

        void OnLGAChanged(IEnumerable<string> value)
        {
            selectedLGA = value.ElementAt(0);

            if (selectedLGA != "none")
            {
                schInfo.LGAID = lgalist.FirstOrDefault(s => s.LGA == selectedLGA).LGAID;
                schInfo.LGA = selectedLGA;
            }
        }

        async Task UploadFiles(InputFileChangeEventArgs e)
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

        async Task EditSchoolInfo(int _schinfoid)
        {
            toolBarMenuId = 2;
            saveStatus = false;

            countrylist = await countryService.GetAllAsync("Settings/GetCountries/1");
            schInfo = await _schoolInfoService.GetByIdAsync("Settings/GetSchoolDetails/", _schinfoid);

            schinfoid = _schinfoid;

            if (schInfo.SchLogo != null)
            {
                schInfo.SchLogo = utilities.GetImage(Convert.ToBase64String(schInfo.SchLogo));
                schInfo.ImageUrl = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(schInfo.SchLogo));
            }
            imgSrc = schInfo.ImageUrl;

            if (schInfo.EmailPassword != null)
            {
                stringdata.FileName = schInfo.EmailPassword;
                var result = await stringModelService.SaveAsync("EncryptDecrypt/Decrypt/", stringdata);
                string _decrypt = result.FileName;
                schInfo.Password = _decrypt;
                schInfo.ConfirmPassword = _decrypt;
            }

            statelist = await stateService.GetAllAsync("Settings/GetStates/1/" + schInfo.CountryID);
            lgalist = await lgaService.GetAllAsync("Settings/GetLGAs/1/" + schInfo.StateID);
        }

        async Task UpdateSchoolInfo()
        {
            if (_fileBytes != null)
            {
                schInfo.SchLogo = _fileBytes;
            }

            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "School Information Update",
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
                    schInfo.SchInfoID = 1;
                    schInfo.StatusTypeID = 1;
                    string _encrypt = await stringValueService.GetStringAsync("EncryptDecrypt/Encrypt/" + schInfo.Password);
                    schInfo.EmailPassword = _encrypt;
                    await _schoolInfoService.UpdateAsync("Settings/UpdateSchoolDetails/", 1, schInfo);
                    await Swal.FireAsync("School Information", "Has Been Successfully Updated.", "success");

                    await SchoolInfo();
                }
            }
        }

        async Task SchoolInfo()
        {
            toolBarMenuId = 1;
            saveStatus = true;
            schinfoid = 0;
            schInfo = new SETSchInformation();
            schoolInfoList = await _schoolInfoService.GetAllAsync("Settings/GetSchoolInfo/1");
        }

        #endregion

    }
}
