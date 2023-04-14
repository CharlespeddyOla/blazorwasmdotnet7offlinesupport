using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Administration.Staff;

namespace WebAppAcademics.Client.OfflineRepo.Auth
{
    public class AuthDBSyncRepo : AppDBSyncRepo<ADMEmployee>
    {
        public AuthDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ADMEmployee> authService, IJSRuntime jsRuntime)
       : base("SchoolMagnet", "StaffID", true, dbFactory, authService, jsRuntime)
        {
        }
    }
}
