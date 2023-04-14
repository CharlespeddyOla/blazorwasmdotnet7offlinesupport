using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.Subjects;

namespace WebAppAcademics.Client.OfflineRepo.Academics.Subjects
{
    public class SbjAllocTeacherDBSyncRepo : AppDBSyncRepo<ACDSbjAllocationTeachers>
    {
        public SbjAllocTeacherDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ACDSbjAllocationTeachers> sbjAllocService, IJSRuntime jsRuntime)
        : base("SchoolMagnet", "SbjAllocID", true, dbFactory, sbjAllocService, jsRuntime)
        {
        }
    }
}
