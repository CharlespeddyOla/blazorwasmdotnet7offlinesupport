using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Grades
{
    public class GeneralGradesDBSyncRepo : AppDBSyncRepo<ACDSettingsGrade>
    {
        public GeneralGradesDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ACDSettingsGrade> gradesService,
                                            IJSRuntime jsRuntime)
    : base("SchoolMagnet", "GradeID", true, dbFactory, gradesService, jsRuntime)
        {
        }
    }
}
