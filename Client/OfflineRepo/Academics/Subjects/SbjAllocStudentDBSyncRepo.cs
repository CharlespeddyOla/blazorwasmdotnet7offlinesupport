using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.Subjects;

namespace WebAppAcademics.Client.OfflineRepo.Academics.Subjects
{
    public class SbjAllocStudentDBSyncRepo : AppDBSyncRepo<ACDSbjAllocationStudents>
    {
        public SbjAllocStudentDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ACDSbjAllocationStudents> sbjAllocService, IJSRuntime jsRuntime)
        : base("SchoolMagnet", "SbjAllocID", true, dbFactory, sbjAllocService, jsRuntime)
        {
        }
    }
}
