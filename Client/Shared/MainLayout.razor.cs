using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using WebAppAcademics.Client.Extensions;

namespace WebAppAcademics.Client.Shared
{
    public partial class MainLayout
    {
        #region [Layout Declaration]  
        private MudTheme _currentTheme = new MudTheme();
        private bool _sidebarOpen = false;

        private void ToggleTheme(MudTheme changedTheme) => _currentTheme = changedTheme;

        private void ToggleSidebar() => _sidebarOpen = !_sidebarOpen;

        protected override void OnInitialized()
        {
            _currentTheme = _darkTheme;
        }

        private readonly MudTheme _darkTheme =
        new MudTheme()
        {
            Palette = new Palette()
            {
                Black = "#27272f",
                Background = "#32333d",
                BackgroundGrey = "#27272f",
                Surface = "#373740",
                DrawerBackground = "#27272f",
                DrawerText = "rgba(255,255,255, 0.50)",
                DrawerIcon = "rgba(255,255,255, 0.50)",
                AppbarBackground = "#27272f",
                AppbarText = "rgba(255,255,255, 0.70)",
                TextPrimary = "rgba(255,255,255, 0.70)",
                TextSecondary = "rgba(255,255,255, 0.50)",
                ActionDefault = "#adadb1",
                ActionDisabled = "rgba(255,255,255, 0.26)",
                ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                Divider = "rgba(255,255,255, 0.12)",
                DividerLight = "rgba(255,255,255, 0.06)",
                TableLines = "rgba(255,255,255, 0.12)",
                LinesDefault = "rgba(255,255,255, 0.12)",
                LinesInputs = "rgba(255,255,255, 0.3)",
                TextDisabled = "rgba(255,255,255, 0.2)"
            }
        };

        #endregion

        #region [Variables Declaration]
        [Inject] ILocalStorageService _localStorage { get; set; }
        [CascadingParameter]
        private Task<AuthenticationState> authenticationState { get; set; }
        public string _appTitle { get; set; }
        string _staffname { get; set; }
        string _appUserType { get; set; }


        public string currentPage { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            string appUserType = await _localStorage.ReadEncryptedItemAsync<string>("AppUserType");
            if (appUserType != null)
            {
                _appUserType = appUserType;
            }
            else
            {
                _appUserType = string.Empty;
            }

            string appTitle = "Academic Session" + ": " + await _localStorage.ReadEncryptedItemAsync<string>("academicsession");
            if (appTitle != null)
            {
                _appTitle = appTitle;
            }
            else
            {
                _appTitle = string.Empty;  
            }
            var authState = await authenticationState;
            _staffname = authState.User.Identity.Name;
        }
    }
}
