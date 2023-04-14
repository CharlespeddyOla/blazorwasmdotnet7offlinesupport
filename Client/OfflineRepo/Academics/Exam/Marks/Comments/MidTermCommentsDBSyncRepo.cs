using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Comments
{
    public class MidTermCommentsDBSyncRepo : AppDBSyncRepo<ACDReportCommentMidTerm>
    {
        public MidTermCommentsDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ACDReportCommentMidTerm> commentsService,
                                            IJSRuntime jsRuntime)
        : base("SchoolMagnet", "CommentID", true, dbFactory, commentsService, jsRuntime)
        {
        }
    }
}
