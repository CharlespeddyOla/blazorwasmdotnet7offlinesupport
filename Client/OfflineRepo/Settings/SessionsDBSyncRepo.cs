using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Administration.Staff;
using WebAppAcademics.Shared.Models.Settings;

namespace WebAppAcademics.Client.OfflineRepo.Settings
{
    public class SessionsDBSyncRepo : AppDBSyncRepo<SETSchSessions>
    {
        public SessionsDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<SETSchSessions> sessionService, IJSRuntime jsRuntime)
      : base("SchoolMagnet", "TermID", true, dbFactory, sessionService, jsRuntime)
        {
        }
    }
}
