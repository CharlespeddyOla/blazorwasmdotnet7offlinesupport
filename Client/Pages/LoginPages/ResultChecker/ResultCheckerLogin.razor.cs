using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Net;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Shared.Models.LoginModels;
using WebAppAcademics.Client.Authentication;

namespace WebAppAcademics.Client.Pages.LoginPages.ResultChecker
{
    public partial class ResultCheckerLogin
    {

        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] ILocalStorageService _localStorage { get; set; }
        [Inject] ISessionStorageService sessionStorageService { get; set; }
        [Inject] AuthenticationStateProvider authStateProvide { get; set; }
        [Inject] HttpClient httpClient { get; set; }
        [Inject] SweetAlertService Swal { get; set; }

        #endregion


        #region [Models Declaration]   
        private ResultCheckerLoginRequest resultRequest = new();
        #endregion


        private async Task Authenticate()
        {
            var resultResponse = await httpClient.PostAsJsonAsync<ResultCheckerLoginRequest>("api/Account/ResultCheckerLogin", resultRequest);

            if (resultResponse.IsSuccessStatusCode)
            {
                var resultSession = await resultResponse.Content.ReadFromJsonAsync<ResultCheckerSession>();
                if (resultSession.STDID > 0)
                {
                    await _localStorage.SaveItemEncryptedAsync("AppUserType", "ResultChecker");
                    var customAuthStateProvider = (CustomAuthStateProvider)authStateProvide;
                    await customAuthStateProvider.UpdateAuthenticationStateResultChecker(resultSession);

                    await sessionStorageService.SaveItemEncryptedAsync("stdid", resultSession.STDID);
                    await sessionStorageService.SaveItemEncryptedAsync("admissionno", resultSession.AdmissionNo);
                    await sessionStorageService.SaveItemEncryptedAsync("studentname", resultSession.StudentName);
                    await sessionStorageService.SaveItemEncryptedAsync("parentpincount", resultSession.ParentPinCount);
                    await sessionStorageService.SaveItemEncryptedAsync("resulttermid", resultSession.ResultTermID);

                    navManager.NavigateTo("/studentresults");
                }
                else
                {
                    await Swal.FireAsync("Login Error", resultSession.Response, "error");
                    return;
                }
            }
            else if (resultResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                return;
            }
        }
    }
}
