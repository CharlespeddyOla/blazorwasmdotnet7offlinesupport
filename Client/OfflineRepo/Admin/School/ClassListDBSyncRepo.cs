using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Administration.School;

namespace WebAppAcademics.Client.OfflineRepo.Admin.School
{
    public class ClassListDBSyncRepo : AppDBSyncRepo<ADMSchClassList>
    {
        public ClassListDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ADMSchClassList> schoolService, IJSRuntime jsRuntime)
      : base("SchoolMagnet", "ClassID", true, dbFactory, schoolService, jsRuntime)
        {
        }
    }
}
