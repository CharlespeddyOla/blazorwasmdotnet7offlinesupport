using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.CBT;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Client.OfflineRepo.Academics.Exam.CBT
{
    public class CBTExamsDBSyncRepo : AppDBSyncRepo<CBTExams>
    {
        public CBTExamsDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<CBTExams> cbtExamService, IJSRuntime jsRuntime)
        : base("SchoolMagnet", "ExamID", true, dbFactory, cbtExamService, jsRuntime)
        {
        }
    }
}
