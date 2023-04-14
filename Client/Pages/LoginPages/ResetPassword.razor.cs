using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebAppAcademics.Shared.Models.LoginModels;
using System.Net.Http.Json;
using WebAppAcademics.Client.Authentication;
using WebAppAcademics.Client.Extensions;

namespace WebAppAcademics.Client.Pages.LoginPages
{
    public partial class ResetPassword
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] ILocalStorageService _localStorage { get; set; }
        [Inject] AuthenticationStateProvider authStateProvide { get; set; }
        [Inject] HttpClient httpClient { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        #endregion

        #region [Variable Declaration]       
        bool isShow;
        InputType PasswordInput = InputType.Password;
        string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
        #endregion

        #region [Models Declaration]   
        private PasswordResetRequest passwordResetRequest = new();
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

        private async Task PasswordReset()
        {
            var registrationResponse = await httpClient.PostAsJsonAsync<PasswordResetRequest>("api/Account/ResetPassword", passwordResetRequest);
            if (registrationResponse.IsSuccessStatusCode)
            {
                var userSession = await registrationResponse.Content.ReadFromJsonAsync<UserSession>();
                if (userSession.StaffID > 0)
                {
                    await _localStorage.SaveItemEncryptedAsync("AppUserType", "Academics");
                    var customAuthStateProvider = (CustomAuthStateProvider)authStateProvide;
                    await customAuthStateProvider.UpdateAuthenticationState(userSession);
                    navManager.NavigateTo("/", true);
                }
                else
                {
                    await Swal.FireAsync("Password Reset Error", userSession.Response, "error");
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }
}
