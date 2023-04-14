using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Administration.Staff;

namespace WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Entry
{
    public class CognitiveDBSyncRepo : AppDBSyncRepo<ACDStudentsMarksCognitive>
    {
        public CognitiveDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ACDStudentsMarksCognitive> cognitiveService, IJSRuntime jsRuntime)
        : base("SchoolMagnet", "StudentMarkID", true, dbFactory, cognitiveService, jsRuntime)
        {
        }
    }
}
