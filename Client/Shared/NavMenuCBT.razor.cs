using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Administration.Students;

namespace WebAppAcademics.Client.Shared
{
    public partial class NavMenuCBT
    {

        #region [Inject Declaration]
        [Parameter] public bool SideBarOpen { get; set; }
        [Inject] ISessionStorageService sessionStorageService { get; set; }
        [Inject] IAPIServices<ADMStudents> studentService { get; set; }

        #endregion

        #region [Variable Declaration]  
        int stdid { get; set; }
        string examDate { get; set; }
        string examCode { get; set; }
        string subject { get; set; }
        double examTime { get; set; }

        byte[] studentPhoto { get; set; }
        string imgSrcphoto { get; set; } = "";
        Utilities utilities = new Utilities();

        #endregion

        #region [Models Declaration]
        ADMStudents student = new();

        #endregion

        protected override async Task OnInitializedAsync()
        {
            stdid = await sessionStorageService.ReadEncryptedItemAsync<int>("stdid");
            examDate = await sessionStorageService.ReadEncryptedItemAsync<string>("examDate");
            examCode = await sessionStorageService.ReadEncryptedItemAsync<string>("examCode");
            subject = await sessionStorageService.ReadEncryptedItemAsync<string>("subject");
            examTime = await sessionStorageService.ReadEncryptedItemAsync<double>("examTime");
            await StudentPhoto();

            await base.OnInitializedAsync();
        }

        async Task StudentPhoto()
        {
            student = await studentService.GetByIdAsync("AdminStudent/GetStudent/", stdid);
            studentPhoto = student.studentPhoto;
            if (studentPhoto != null)
            {
                studentPhoto = utilities.GetImage(Convert.ToBase64String(student.studentPhoto));
                imgSrcphoto = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(studentPhoto));
            }
        }
    }
}
