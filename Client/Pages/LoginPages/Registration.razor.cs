using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebAppAcademics.Shared.Models.LoginModels;
using System.Net.Http.Json;
using WebAppAcademics.Client.Authentication;
using WebAppAcademics.Client.Extensions;

namespace WebAppAcademics.Client.Pages.LoginPages
{
    public partial class Registration
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
        private RegistrationRequest registrationRequest = new();
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

        private async Task Register()
        {
            var registrationResponse = await httpClient.PostAsJsonAsync<RegistrationRequest>("api/Account/Register", registrationRequest);
            if (registrationResponse.IsSuccessStatusCode)
            {
                var userSession = await registrationResponse.Content.ReadFromJsonAsync<UserSession>();
                if (userSession.StaffID > 0)
                {
                    await _localStorage.SaveItemEncryptedAsync("AppUserType", "Academics");
                    var customAuthStateProvider = (CustomAuthStateProvider)authStateProvide;
                    await customAuthStateProvider.UpdateAuthenticationState(userSession);
                    navManager.NavigateTo("/academics", true);              
                }
                else
                {
                    await Swal.FireAsync("Registration Error", userSession.Response, "error");
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
