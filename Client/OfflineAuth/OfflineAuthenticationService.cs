using Microsoft.AspNetCore.Components;
using System.ComponentModel.Design;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.LoginModels;

namespace WebAppAcademics.Client.OfflineAuth
{    
    public class OfflineAuthenticationService : IOfflineAuthenticationService
    {
        private IHttpService _httpService;

        public OfflineAuthenticationService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<UserSession> Login(LoginRequest loginRequest)
        {
            return await _httpService.Post<UserSession>("/users/authenticate", loginRequest);
        }
    }
}
