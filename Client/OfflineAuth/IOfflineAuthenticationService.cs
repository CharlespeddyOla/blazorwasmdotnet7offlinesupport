using WebAppAcademics.Shared.Models.LoginModels;

namespace WebAppAcademics.Client.OfflineAuth
{
    public interface IOfflineAuthenticationService
    {
        Task<UserSession> Login(LoginRequest loginRequest);
    }
}