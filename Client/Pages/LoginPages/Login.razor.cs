using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Net;
using WebAppAcademics.Client.Authentication;
using WebAppAcademics.Shared.Models.Administration.Staff;
using WebAppAcademics.Shared.Models.LoginModels;
using WebAppAcademics.Client.OfflineRepo.Auth;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.OfflineAuth;
using System.Security.Claims;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Settings;
using WebAppAcademics.Client.OfflineServices;

namespace WebAppAcademics.Client.Pages.LoginPages
{
    public partial class Login
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] ILocalStorageService _localStorage { get; set; }
        [Inject] IAPIServices<FileChunkDTO> variablesModelService { get; set; }
        [Inject] IAPIServices<SETAppLicense> licenseService { get; set; }
        [Inject] AuthenticationStateProvider authStateProvide { get; set; }
        [Inject] HttpClient httpClient { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] AuthDBSyncRepo _authServices { get; set; }

        [Inject] IOfflineAuthenticationService offlineAuth { get; set; }
       
        #endregion

        #region [Variable Declaration]       
        bool isShow;
        InputType PasswordInput = InputType.Password;
        string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                
        bool Initialized = false;
        bool IsOnline = true;  
        #endregion

        #region [Models Declaration]   
        private LoginRequest loginRequest = new();
        UserSession _userSession = new();
        List<ADMEmployee> _stafflist = new();
        #endregion

        protected async void OnlineStatusChanged(object sender, OnlineStatusEventArgs args)
        {            
            IsOnline = args.IsOnline;
            if (args.IsOnline == false)
            {
                // reload from IndexedDB
                await _localStorage.SaveItemEncryptedAsync("AppUserType", "Academics");
                await CheckLicenseStatusOffline();
                _stafflist = (await _authServices.GetAllOfflineAsync()).ToList();
                loginRequest.staffList = _stafflist;
            }
            else
            {
                if (Initialized)
                    // reload from API
                    await Reload();
                else
                    Initialized = true;
            }
            await InvokeAsync(StateHasChanged);
        }

        protected override async Task OnInitializedAsync()
        {
            _authServices.OnlineStatusChanged += OnlineStatusChanged;           
            APICallParameters.IsAuth = true;
            await base.OnInitializedAsync();
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            // execute conditionally for loading data, otherwise this will load
            // every time the page refreshes
            if (firstRender)
            {
                // Do work to load page data and set properties
                if (IsOnline)
                {
                    await _localStorage.SaveItemEncryptedAsync("AppUserType", "Academics");
                    await CheckLicenseStatusOnline();
                    await LoadList();
                }                
            }
        }

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

        async Task Reload()
        {
            var list = await _authServices.GetAllAsync("AdminStaff/GetStaffs/1/1/0/0/0");
            if (list != null)
            {
                _stafflist = list.ToList();               
                await InvokeAsync(StateHasChanged);
            }
        }

        async Task LoadList()
        {
            if (IsOnline)
            {
                var all = await _authServices.GetAllAsync("AdminStaff/GetStaffs/1/1/0/0/0");
                if (all.Count() > 0)
                {
                    await Reload();
                    return;
                }
            }            
        }

        private async Task Authenticate()
        {
            if (IsOnline)
            {
                var loginResponse = await httpClient.PostAsJsonAsync<LoginRequest>("api/Account/Login", loginRequest);
                if (loginResponse.IsSuccessStatusCode)
                {
                    var userSession = await loginResponse.Content.ReadFromJsonAsync<UserSession>();
                    var customAuthStateProvider = ((CustomAuthStateProvider)authStateProvide);
                    await customAuthStateProvider.UpdateAuthenticationState(userSession);
                    navManager.NavigateTo("/schtermselection", true);
                }
                else if (loginResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await Swal.FireAsync("LogIn Error", "Invalid username or password or your Access has been Revoked.", "error");
                    return;
                }
            }
            else
            {
                var loginResponse = await offlineAuth.Login(loginRequest);
                if (loginResponse != null)
                {
                    _userSession = loginResponse;
                    var customAuthStateProvider = (CustomAuthStateProvider)authStateProvide;
                    await customAuthStateProvider.UpdateAuthenticationState(_userSession);
                    navManager.NavigateTo("/schtermselection", true);
                }
                else
                {
                    await Swal.FireAsync("LogIn Error", "Invalid username or password or your Access has been Revoked.", "error");
                    navManager.NavigateTo("/", true);
                }
            }
        }

        #region [Section - Registration License Verification]   
        bool _licenseStatus { get; set; }
        int _licenseCount { get; set; }
        DateTime _licenseDate { get; set; }
        DateTime _licenseExpirationDate { get; set; }   
        int _licenseDuration { get; set; }  
        bool _licenseType { get; set; } 
        string _LicenseInfo { get; set; }
        string _headerInfo { get; set; }
        string _buttonText { get; set; }

        FileChunkDTO stringdata = new();
        SETAppLicense appLicense = new();

        async Task CheckLicenseStatusOnline()
        {
            _licenseCount = await variablesModelService.LicenseCountAsync("Settings/GetLicenseCount/", 0);
            if (_licenseCount == 0)
            {
                _LicenseInfo = "You Are Using A Trial Version. Please, register the Software before it expires.";
                for (int i = 0; i < 3; i++)
                {
                    await SetDefaultLicense();
                }
            }
            else
            {
                var getLicenseDetails = await licenseService.GetAllAsync("Settings/GetLicense/0");
                stringdata.FileName = getLicenseDetails.SingleOrDefault(x => x.Id == 1).LicenseStatus;
                var result = await variablesModelService.SaveAsync("EncryptDecrypt/Decrypt/", stringdata);
                if (result != null)
                {
                    _licenseStatus = Convert.ToBoolean(result.FileName);
                }

                if (_licenseStatus)
                {
                    //License Has Expired - Renew License
                    _licenseStatus = true;
                    _LicenseInfo = "License has Expired. Please, renew your License with a new Activation Code!";
                }
                else
                {
                    //License Is Active
                    _licenseStatus = false;

                    stringdata.FileName = getLicenseDetails.SingleOrDefault(x => x.Id == 1).PerpetualStatus;
                    var resultlicenseType = await variablesModelService.SaveAsync("EncryptDecrypt/Decrypt/", stringdata);
                    if (resultlicenseType != null)
                    {
                        _licenseType = Convert.ToBoolean(resultlicenseType.FileName);
                    }                    

                    if (!_licenseType)
                    {
                        stringdata.FileName = getLicenseDetails.SingleOrDefault(x => x.Id == 1).LicenseDate;
                        var resultLicensDate = await variablesModelService.SaveAsync("EncryptDecrypt/Decrypt/", stringdata);
                        if (resultLicensDate != null)
                        {
                            _licenseDate = Convert.ToDateTime(resultLicensDate.FileName);
                        }                        

                        stringdata.FileName = getLicenseDetails.SingleOrDefault(x => x.Id == 1).LicenseDuration;
                        var resultLicensDuration = await variablesModelService.SaveAsync("EncryptDecrypt/Decrypt/", stringdata);

                        if (resultLicensDuration != null)
                        {
                            _licenseDuration = Convert.ToInt32(resultLicensDuration.FileName);
                            _licenseExpirationDate = _licenseDate.AddMonths(_licenseDuration);
                        }
                        
                        if (_licenseDuration == 1)
                        {
                            _LicenseInfo = "You are Currently Using Trial Software Which Will Expire On " + _licenseExpirationDate;
                        }
                        else
                        {
                            double dayesLeft = (_licenseExpirationDate - DateTime.Now).TotalDays;

                            if (dayesLeft <= 30)
                            {
                                _LicenseInfo = "License Will Expire In " + dayesLeft + " - " + _licenseExpirationDate;
                            }

                            if (_licenseExpirationDate <= DateTime.Now)
                            {
                                //Update License Status To Expired (True)
                                appLicense.LicenseStatus = await variablesModelService.GetStringAsync("EncryptDecrypt/Encrypt/" + "true");
                                appLicense.ExpiredStatus = await variablesModelService.GetStringAsync("EncryptDecrypt/Encrypt/" + "true");
                                await licenseService.UpdateAsync("Settings/UpdateLicense/", 1, appLicense);
                            }
                        }
                    }
                }
            }

            await _localStorage.SaveItemEncryptedAsync("licenseCount", _licenseCount);
            await _localStorage.SaveItemEncryptedAsync("licenseStatus", _licenseStatus);
            await _localStorage.SaveItemEncryptedAsync("licenseDuration", _licenseDuration);
        }

        async Task CheckLicenseStatusOffline()
        {
            _licenseCount = await _localStorage.ReadEncryptedItemAsync<int>("licenseCount");           

            if (_licenseCount == 0)
            {
                _LicenseInfo = "";
                
            }

            _LicenseInfo = "";
        }

        async Task SetDefaultLicense()
        {
            appLicense.UserID = 0;
            appLicense.LicenseStatus = await variablesModelService.GetStringAsync("EncryptDecrypt/Encrypt/" + "false");
            appLicense.ExpiredStatus = await variablesModelService.GetStringAsync("EncryptDecrypt/Encrypt/" + "false");
            appLicense.PerpetualStatus = await variablesModelService.GetStringAsync("EncryptDecrypt/Encrypt/" + "false");
            DateTime dateTime = DateTime.Now;
            string licenseDate = dateTime.ToString("dd-MMM-yyyy");
            appLicense.LicenseDate = await variablesModelService.GetStringAsync("EncryptDecrypt/Encrypt/" + licenseDate);
            appLicense.LicenseDuration = await variablesModelService.GetStringAsync("EncryptDecrypt/Encrypt/" + "1");
            appLicense.License = string.Empty;
            await licenseService.SaveAsync("Settings/AddLicense/", appLicense);
        }

        #endregion

        void IDisposable.Dispose()
        {
            _authServices.OnlineStatusChanged -= OnlineStatusChanged;
        }
    }
}
