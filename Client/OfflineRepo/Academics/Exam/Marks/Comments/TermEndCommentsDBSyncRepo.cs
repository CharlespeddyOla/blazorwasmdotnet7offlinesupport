using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Comments
{
    public class TermEndCommentsDBSyncRepo : AppDBSyncRepo<ACDReportCommentsTerminal>
    {
        public TermEndCommentsDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ACDReportCommentsTerminal> commentsService, 
                                            IJSRuntime jsRuntime)
        : base("SchoolMagnet", "CommentID", true, dbFactory, commentsService, jsRuntime)
        {
        }
    }
}
