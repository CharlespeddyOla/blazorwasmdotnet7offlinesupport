using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Security.Principal;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Shared.Models.LoginModels;

namespace WebAppAcademics.Client.Authentication
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private ILocalStorageService _localStorage;
        private ISessionStorageService _sessionStorage;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        AuthenticationState task { get; set; }
        ClaimsPrincipal claimsPrincipal { get; set; }
        UserSession _userSession = new();
        CBTSession _userSessionCBT = new();
        ResultCheckerSession _userSessionResultChecker = new();

        public CustomAuthStateProvider(ILocalStorageService localStorage, ISessionStorageService sessionStorage)
        {
            _localStorage = localStorage;
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        { 
            try
            {
                string appUserType = await _localStorage.ReadEncryptedItemAsync<string>("AppUserType");                
                switch (appUserType)
                {
                    case "Academics":
                        var userSession = await _localStorage.ReadEncryptedItemAsync<UserSession>("UserSession");
                        if (userSession == null)
                            return await Task.FromResult(new AuthenticationState(_anonymous));
                        claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, userSession.StaffNameWithNo),
                            new Claim(ClaimTypes.Role, userSession.Role),
                            new Claim(ClaimTypes.Surname, userSession.Surname),
                            new Claim(ClaimTypes.Email, userSession.Email),
                            new Claim(ClaimTypes.PrimaryGroupSid, Convert.ToString(userSession.RoleID)),
                            new Claim(ClaimTypes.PrimarySid, Convert.ToString(userSession.StaffID))
                        }, "JwtAuth"));
                        task = await Task.FromResult(new AuthenticationState(claimsPrincipal));
                        break;
                    case "CBT":
                        var userSessionCBT = await _sessionStorage.ReadEncryptedItemAsync<CBTSession>("CBTSession");
                        if (userSessionCBT == null)
                            return await Task.FromResult(new AuthenticationState(_anonymous));
                        claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, userSessionCBT.StudentName),
                            new Claim(ClaimTypes.Role, userSessionCBT.AdmissionNo),
                            new Claim(ClaimTypes.PrimaryGroupSid, Convert.ToString(userSessionCBT.ClassListID)),
                            new Claim(ClaimTypes.PrimarySid, Convert.ToString(userSessionCBT.STDID))
                        }, "JwtAuth"));
                        task = await Task.FromResult(new AuthenticationState(claimsPrincipal));
                        break;
                    case "ResultChecker":
                        var userSessionResultChecker = await _sessionStorage.ReadEncryptedItemAsync<ResultCheckerSession>("ResultCheckerSession");
                        if (userSessionResultChecker == null)
                            return await Task.FromResult(new AuthenticationState(_anonymous));
                        claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                        {
                            new Claim(ClaimTypes.PrimarySid, Convert.ToString(userSessionResultChecker.STDID)),
                            new Claim(ClaimTypes.Name, userSessionResultChecker.StudentName),
                            new Claim(ClaimTypes.Country, userSessionResultChecker.AdmissionNo),
                            new Claim(ClaimTypes.PrimaryGroupSid, Convert.ToString(userSessionResultChecker.ResultTermID)),
                        }, "JwtAuth"));
                        task = await Task.FromResult(new AuthenticationState(claimsPrincipal));
                        break;
                    default:
                        task = await Task.FromResult(new AuthenticationState(_anonymous));
                        break;
                }

                return task;
            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
        }

        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            if (userSession != null)
            {
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.StaffNameWithNo),
                    new Claim(ClaimTypes.Role, userSession.Role),
                    new Claim(ClaimTypes.Surname, userSession.Surname),
                    new Claim(ClaimTypes.Email, userSession.Email),
                    new Claim(ClaimTypes.PrimaryGroupSid, Convert.ToString(userSession.RoleID)),
                    new Claim(ClaimTypes.PrimarySid, Convert.ToString(userSession.StaffID))
                }));
                userSession.ExpiryTimeStamp = DateTime.Now.AddSeconds(userSession.ExpiresIn);
                await _localStorage.SaveItemEncryptedAsync("UserSession", userSession);
            }
            else
            {
                claimsPrincipal = _anonymous;
                await _localStorage.RemoveItemAsync("UserSession");
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task UpdateCBTAuthenticationState(CBTSession userSession)
        {
            if (userSession != null)
            {
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.StudentName),
                    new Claim(ClaimTypes.Role, userSession.AdmissionNo),
                    new Claim(ClaimTypes.PrimaryGroupSid, Convert.ToString(userSession.ClassListID)),
                    new Claim(ClaimTypes.PrimarySid, Convert.ToString(userSession.STDID))
                }));
                userSession.ExpiryTimeStamp = DateTime.Now.AddSeconds(userSession.ExpiresIn);
                await _sessionStorage.SaveItemEncryptedAsync("CBTSession", userSession);
            }
            else
            {
                claimsPrincipal = _anonymous;
                await _sessionStorage.RemoveItemAsync("CBTSession");
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task UpdateAuthenticationStateResultChecker(ResultCheckerSession resultSession)
        {
            if (resultSession != null)
            {
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.PrimarySid, Convert.ToString(resultSession.STDID)),
                    new Claim(ClaimTypes.Name, resultSession.StudentName),
                    new Claim(ClaimTypes.Country, resultSession.AdmissionNo),
                    new Claim(ClaimTypes.PrimaryGroupSid, Convert.ToString(resultSession.ResultTermID)),
                }));
                resultSession.ExpiryTimeStamp = DateTime.Now.AddSeconds(resultSession.ExpiresIn);
                await _sessionStorage.SaveItemEncryptedAsync("ResultCheckerSession", resultSession);
            }
            else
            {
                claimsPrincipal = _anonymous;
                await _sessionStorage.RemoveItemAsync("ResultSession");
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task<string> GetToken()
        {
            var result = string.Empty;
            
            try
            {
                string appUserType = await _localStorage.ReadEncryptedItemAsync<string>("AppUserType");
                switch (appUserType)
                {
                    case "Academics":
                        var userSession = await _localStorage.ReadEncryptedItemAsync<UserSession>("UserSession");
                        if (userSession != null || DateTime.Now < userSession.ExpiryTimeStamp)
                            return userSession.Token;
                        break;
                    case "CBT":
                        var userSessionCBT = await _sessionStorage.ReadEncryptedItemAsync<CBTSession>("CBTSession");
                        if (userSessionCBT != null || DateTime.Now < userSessionCBT.ExpiryTimeStamp)
                            return userSessionCBT.Token;
                        break;
                    case "ResultChecker":
                        var userSessionResultChecker = await _sessionStorage.ReadEncryptedItemAsync<ResultCheckerSession>("ResultCheckerSession");
                        if (userSessionResultChecker != null || DateTime.Now < userSessionResultChecker.ExpiryTimeStamp)
                            return userSessionResultChecker.Token;
                        break;
                    default:
                        task = await Task.FromResult(new AuthenticationState(_anonymous));
                        break;
                }                
            }
            catch { }

            return result;
        }
    }
}
