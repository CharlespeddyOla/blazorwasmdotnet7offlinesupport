using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Entry
{
    public class RatingDBSyncRepo : AppDBSyncRepo<ACDSettingsRating>
    {
        public RatingDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ACDSettingsRating> ratingService,
            IJSRuntime jsRuntime)
        : base("SchoolMagnet", "RatingID", true, dbFactory, ratingService, jsRuntime)
        {
        }
    }
}
