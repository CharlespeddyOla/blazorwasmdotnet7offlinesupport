using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Entry
{
    public class MarkSettingsDBSyncRepo : AppDBSyncRepo<ACDSettingsMarks>
    {
        public MarkSettingsDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ACDSettingsMarks> markSettingsService, IJSRuntime jsRuntime)
        : base("SchoolMagnet", "MarkID", true, dbFactory, markSettingsService, jsRuntime)
        {
        }
    }
}
