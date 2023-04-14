using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.Subjects;

namespace WebAppAcademics.Client.OfflineRepo.Academics.Subjects
{
    public class SubjectClassificationDBSyncRepo : AppDBSyncRepo<ACDSbjClassification>
    {
        public SubjectClassificationDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ACDSbjClassification> subjectClassService, 
            IJSRuntime jsRuntime)
        : base("SchoolMagnet", "SbjClassID", true, dbFactory, subjectClassService, jsRuntime)
        {
        }
    }
}
