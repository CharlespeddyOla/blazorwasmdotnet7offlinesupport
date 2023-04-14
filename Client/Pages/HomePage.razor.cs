using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Client.Shared;

namespace WebAppAcademics.Client.Pages
{
    public partial class HomePage
    {
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }

        [Inject] ILocalStorageService _localStorage { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }

        int roleid { get; set; }
        string role { get; set; }
        int staffid { get; set; }
        string staffName { get; set; }
        string staffEmail { get; set; }
        string staffRole { get; set; }
        string academicYear;

        protected async override Task OnInitializedAsync()
        {
            Layout.currentPage = "Home Page";

            var authState = await authenticationState;
            roleid = Convert.ToInt32(authState.User.FindFirst(ClaimTypes.PrimaryGroupSid).Value);
            role = authState.User.FindFirst(ClaimTypes.Role).Value;
            staffid = Convert.ToInt32(authState.User.FindFirst(ClaimTypes.PrimarySid).Value);
            staffName = authState.User.Identity.Name;
            staffEmail = authState.User.FindFirst(ClaimTypes.Email).Value;
            staffRole = authState.User.FindFirst(ClaimTypes.Role).Value;
            await _localStorage.SaveItemEncryptedAsync("roleid", roleid);
            await _localStorage.SaveItemEncryptedAsync("role", role);
            await _localStorage.SaveItemEncryptedAsync("staffid", staffid);
            academicYear = await _localStorage.ReadEncryptedItemAsync<string>("academicsession");
            await base.OnInitializedAsync();
        }
    }
}
