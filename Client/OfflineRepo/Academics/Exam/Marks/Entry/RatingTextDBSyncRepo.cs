using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Client.OfflineRepo.Academics.Exam.Marks.Entry
{
    public class RatingTextDBSyncRepo : AppDBSyncRepo<ACDSettingsRatingText>
    {
        public RatingTextDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ACDSettingsRatingText> ratingTextService,
            IJSRuntime jsRuntime)
        : base("SchoolMagnet", "TextID", true, dbFactory, ratingTextService, jsRuntime)
        {
        }
    }
}
