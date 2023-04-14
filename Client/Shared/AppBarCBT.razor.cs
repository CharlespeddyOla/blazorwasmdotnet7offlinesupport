using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Extensions;

namespace WebAppAcademics.Client.Shared
{
    public partial class AppBarCBT
    {

        #region [Inject Declaration]
        [Parameter] public EventCallback OnSidebarToggled { get; set; }
        [Inject] ISessionStorageService sessionStorageService { get; set; }

        #endregion

        #region [Variable Declaration]  
        string academicSession { get; set; }
        string admissionNo { get; set; }
        string studentName { get; set; }

        #endregion

        protected override async Task OnInitializedAsync()
        {
            academicSession = await sessionStorageService.ReadEncryptedItemAsync<string>("academicSession");
            admissionNo = await sessionStorageService.ReadEncryptedItemAsync<string>("studentno");
            studentName = await sessionStorageService.ReadEncryptedItemAsync<string>("studentname");

            await base.OnInitializedAsync();
        }

    }
}
