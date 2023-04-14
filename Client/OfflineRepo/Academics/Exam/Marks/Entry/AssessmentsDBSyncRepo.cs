using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Entry
{
    public class AssessmentDBSyncRepo : AppDBSyncRepo<ACDStudentsMarksAssessment>
    {
        public AssessmentDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ACDStudentsMarksAssessment> assessmentService, IJSRuntime jsRuntime)
        : base("SchoolMagnet", "StudentMarkID", true, dbFactory, assessmentService, jsRuntime)
        {
        }
    }
}
