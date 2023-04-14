using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.CBT;

namespace WebAppAcademics.Client.OfflineRepo.Academics.Exam.CBT
{
    public class CBTScoresDBSyncRepo : AppDBSyncRepo<CBTStudentScores>
    {
        public CBTScoresDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<CBTStudentScores> cbtScoresService, IJSRuntime jsRuntime)
        : base("SchoolMagnet", "ExamID", true, dbFactory, cbtScoresService, jsRuntime)
        {
        }
    }
}
