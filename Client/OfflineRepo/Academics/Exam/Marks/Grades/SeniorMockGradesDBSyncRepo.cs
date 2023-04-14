using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Grades
{
    public class SeniorMockGradesDBSyncRepo : AppDBSyncRepo<ACDSettingsGradeMock>
    {
        public SeniorMockGradesDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ACDSettingsGradeMock> gradesService,
                                               IJSRuntime jsRuntime)
       : base("SchoolMagnet", "GradeID", true, dbFactory, gradesService, jsRuntime)
        {
        }
    }
}
