using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Entry
{
    public class ResultTypeDBSyncRepo : AppDBSyncRepo<ACDReportType>
    {
        public ResultTypeDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ACDReportType> resultTypeService, IJSRuntime jsRuntime)
        : base("SchoolMagnet", "ReportTypeID", true, dbFactory, resultTypeService, jsRuntime)
        {
        }
    }
}
