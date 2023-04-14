using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Academics.Subjects;

namespace WebAppAcademics.Client.OfflineRepo.Academics.Subjects
{
    public class SubjectsDBSyncRepo : AppDBSyncRepo<ACDSubjects>
    {
        public SubjectsDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ACDSubjects>subjectService, IJSRuntime jsRuntime)
        : base("SchoolMagnet", "SubjectID", true, dbFactory, subjectService, jsRuntime)
        {
        }
    }
}
