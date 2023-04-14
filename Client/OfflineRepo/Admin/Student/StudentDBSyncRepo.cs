using Microsoft.JSInterop;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Administration.Students;

namespace WebAppAcademics.Client.OfflineRepo.Admin.Student
{
    public class StudentDBSyncRepo : AppDBSyncRepo<ADMStudents>
    {
        public StudentDBSyncRepo(IBlazorDbFactory dbFactory, IAPIServices<ADMStudents> studentService, IJSRuntime jsRuntime)
        : base("SchoolMagnet", "STDID", true, dbFactory, studentService, jsRuntime)
        {
        }
    }
}
