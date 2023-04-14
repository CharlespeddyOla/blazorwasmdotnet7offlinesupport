using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Grades
{
    public class CheckPointGradesDBSyncRepo : AppDBSyncRepo<ACDSettingsGradeCheckPoint>
    {
        public CheckPointGradesDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ACDSettingsGradeCheckPoint> gradesService,
                                             IJSRuntime jsRuntime)
     : base("SchoolMagnet", "GradeID", true, dbFactory, gradesService, jsRuntime)
        {
        }
    }
}
