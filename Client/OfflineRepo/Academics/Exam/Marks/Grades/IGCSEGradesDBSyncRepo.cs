using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Grades
{
    public class IGCSEGradesDBSyncRepo : AppDBSyncRepo<ACDSettingsGradeIGCSE>
    {
        public IGCSEGradesDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ACDSettingsGradeIGCSE> gradesService,
                                            IJSRuntime jsRuntime)
    : base("SchoolMagnet", "GradeID", true, dbFactory, gradesService, jsRuntime)
        {
        }
    }
}
