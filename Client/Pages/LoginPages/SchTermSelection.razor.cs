using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Client.OfflineRepo.Settings;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Settings;

namespace WebAppAcademics.Client.Pages.LoginPages
{
    public partial class SchTermSelection
    {

        #region [Injection Declaration]   
        [Inject] NavigationManager navManager { get; set; }
        [Inject] ILocalStorageService _localStorage { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] SessionsDBSyncRepo _sessionsService { get; set; }
        #endregion

        #region [Models Declaration]
        List<SETSchSessions> sessionlist = new();
        List<SETSchSessions> academicyearlist = new();
        List<SETSchSessions> schTermlist = new();
        #endregion

        #region [Variable Declaration]
        int schsession { get; set; }
        int termid { get; set; }

        string selectedSession { get; set; }
        string selectedTerm { get; set; }
        string schTerm { get; set; }

     
        bool Initialized = false;
        bool IsOnline = true;
        #endregion

        protected async void OnlineStatusChanged(object sender, OnlineStatusEventArgs args)
        {
            IsOnline = args.IsOnline;
            if (args.IsOnline == false)
            {
                // reload from IndexedDB
                sessionlist = (await _sessionsService.GetAllOfflineAsync()).ToList();
                academicyearlist = sessionlist.GroupBy(x => x.AcademicYear).Select(x => x.FirstOrDefault()).ToList();
            }
            else
            {
                if (Initialized)
                    // reload from API
                    await Reload();
                else
                    Initialized = true;
            }
            await InvokeAsync(StateHasChanged);
        }

        protected override async Task OnInitializedAsync()
        {
            _sessionsService.OnlineStatusChanged += OnlineStatusChanged;
            await base.OnInitializedAsync();
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            // execute conditionally for loading data, otherwise this will load
            // every time the page refreshes
            if (firstRender)
            {
                // Do work to load page data and set properties
                if (IsOnline)
                {
                    await LoadList();
                }
            }
        }

        async Task Reload()
        {
            var list = await _sessionsService.GetAllAsync("Settings/GetAccademicSessions/1/0");
            if (list != null)
            {
                sessionlist = list.ToList();
                academicyearlist = list.GroupBy(x => x.AcademicYear).Select(x => x.FirstOrDefault()).ToList();
                await InvokeAsync(StateHasChanged);
            }
        }

        async Task LoadList()
        {
            var all = await _sessionsService.GetAllAsync("Settings/GetAccademicSessions/1/0");
            if (all.Count() > 0)
            {
                await Reload();
                return;
            }
        }

        async Task OnSelectedAcademicYearChanged(IEnumerable<string> e)
        {
            await Reload();
            selectedSession = e.ElementAt(0);
            schsession = sessionlist.FirstOrDefault(a => a.AcademicYear == selectedSession).SchSession;

            selectedTerm = string.Empty;
            termid = 0;
            schTermlist = sessionlist.Where(s => s.SchSession == schsession).ToList();
        }

        async Task OnSelectedAcademicSessionChanged(IEnumerable<string> e)
        {
            selectedTerm = e.ElementAt(0);
            termid = schTermlist.FirstOrDefault(a => a.AcademicSession == selectedTerm).TermID;
            schTerm = schTermlist.FirstOrDefault(s => s.TermID == termid).SchTerm;

            string academicsession = schTermlist.FirstOrDefault(s => s.TermID == termid).AcademicSession;
            int schoolsession = schTermlist.FirstOrDefault(s => s.TermID == termid).SchSession;
            int calendarid = schTermlist.FirstOrDefault(s => s.TermID == termid).CalendarID;
           
            await _localStorage.SaveItemEncryptedAsync("termid", termid);
            await _localStorage.SaveItemEncryptedAsync("academicsession", academicsession);
            await _localStorage.SaveItemEncryptedAsync("schoolsession", schoolsession);
            await _localStorage.SaveItemEncryptedAsync("calendarid", calendarid);
            await _localStorage.SaveItemEncryptedAsync("schTerm", schTerm);
            await _localStorage.SaveItemEncryptedAsync("schinfoid", 1);
        }

        async Task Continue()
        {
            if (!String.IsNullOrWhiteSpace(selectedTerm))
            {
                navManager.NavigateTo("/homepage", true);
            }
            else
            {
                await Swal.FireAsync("Academic Session", "Please, select Academic Session.", "error");
            }
        }

        void IDisposable.Dispose()
        {
            _sessionsService.OnlineStatusChanged -= OnlineStatusChanged;
        }
    }
}
