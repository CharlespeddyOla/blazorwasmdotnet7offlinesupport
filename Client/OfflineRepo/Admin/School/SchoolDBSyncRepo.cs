using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Administration.School;

namespace WebAppAcademics.Client.OfflineRepo.Admin.School
{
    public class SchoolDBSyncRepo : AppDBSyncRepo<ADMSchlList>
    {
        public SchoolDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ADMSchlList> schoolService, IJSRuntime jsRuntime)
      : base("SchoolMagnet", "SchID", true, dbFactory, schoolService, jsRuntime)
        {
        }
    }
}
