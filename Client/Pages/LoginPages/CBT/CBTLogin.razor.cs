using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net.Http.Json;
using System.Net;
using System.Security.Claims;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Shared.Models.LoginModels;
using WebAppAcademics.Client.Authentication;

namespace WebAppAcademics.Client.Pages.LoginPages.CBT
{
    public partial class CBTLogin
    {

        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] ILocalStorageService _localStorage { get; set; }
        [Inject] ISessionStorageService sessionStorageService { get; set; }
        [Inject] AuthenticationStateProvider authStateProvide { get; set; }
        [Inject] HttpClient httpClient { get; set; }
        [Inject] SweetAlertService Swal { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }

        #endregion

        #region [Variable Declaration]   
        int stdid { get; set; }
        int classlistid { get; set; }
        string studentno { get; set; }
        string studentname { get; set; }

        bool isShow;
        InputType PasswordInput = InputType.Password;
        string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
        #endregion

        #region [Models Declaration]   
        private CBTLoginRequest loginRequest = new();
        #endregion

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


        private async Task Authenticate()
        {
            var loginResponse = await httpClient.PostAsJsonAsync<CBTLoginRequest>("api/Account/CBTLogin", loginRequest);

            if (loginResponse.IsSuccessStatusCode)
            {
                var userSession = await loginResponse.Content.ReadFromJsonAsync<CBTSession>();
                if (userSession.STDID > 0)
                {
                    await _localStorage.SaveItemEncryptedAsync("AppUserType", "CBT");
                    var customAuthStateProvider = (CustomAuthStateProvider)authStateProvide;
                    await customAuthStateProvider.UpdateCBTAuthenticationState(userSession);
                    var authState = await authenticationState;
                    stdid = Convert.ToInt32(authState.User.FindFirst(ClaimTypes.PrimarySid).Value);
                    classlistid = Convert.ToInt32(authState.User.FindFirst(ClaimTypes.PrimaryGroupSid).Value);
                    studentname = authState.User.Identity.Name;
                    studentno = authState.User.FindFirst(ClaimTypes.Role).Value;
                    await sessionStorageService.SaveItemEncryptedAsync("stdid", stdid);
                    await sessionStorageService.SaveItemEncryptedAsync("classlistid", classlistid);
                    await sessionStorageService.SaveItemEncryptedAsync("studentno", studentno);
                    await sessionStorageService.SaveItemEncryptedAsync("studentname", studentname);

                    navManager.NavigateTo("/cbtobjexaminstructions", true);
                }
                else
                {
                    await Swal.FireAsync("CBT Login Error", userSession.Response, "error");
                    return;
                }
            }
            else if (loginResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                return;
            }
        }
    }
}
