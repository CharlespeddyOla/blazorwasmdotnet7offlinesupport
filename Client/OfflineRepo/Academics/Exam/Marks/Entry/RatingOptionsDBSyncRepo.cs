using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Entry
{
    public class RatingOptionsDBSyncRepo : AppDBSyncRepo<ACDSettingsRatingOptions>
    {
        public RatingOptionsDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ACDSettingsRatingOptions> ratingOptionService, 
            IJSRuntime jsRuntime)
        : base("SchoolMagnet", "OptionID", true, dbFactory, ratingOptionService, jsRuntime)
        {
        }
    }
}
