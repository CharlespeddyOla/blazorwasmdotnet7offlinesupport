using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Settings;

namespace WebAppAcademics.Client.Pages.Settings.School
{
    public partial class SETSchoolCalendar
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<SETSchCalendar> _schoolCalendarService { get; set; }
        [Inject] IAPIServices<SETMonthList> _monthListService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int toolBarMenuId { get; set; }
        bool saveStatus { get; set; } = true;
        #endregion

        #region [Models Declaration]
        List<SETSchCalendar> schoolcalendar = new();
        List<SETMonthList> months = new();
        SETSchCalendar calendar = new();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "School Calendar";
            toolBarMenuId = 1;
            schoolcalendar = await _schoolCalendarService.GetAllAsync("Settings/GetSchoolCalendarList/1");
            await base.OnInitializedAsync();
        }

        #region [Display School Calendar]
        
        async Task EditSchoolCalendar(int calendarid)
        {
            toolBarMenuId = 2;
            saveStatus = false;
            months = await _monthListService.GetAllAsync("Settings/GetMonths/1");
            var schcalendar = await _schoolCalendarService.GetByIdAsync("Settings/GetSchoolCalendar/", calendarid);
            calendar.CalendarID = schcalendar.CalendarID;
            calendar.SchTerm = schcalendar.SchTerm;
            calendar.StartMonthID = schcalendar.StartMonthID;
            calendar.StartMonth = schcalendar.StartMonth;
            calendar.EndMonthID = schcalendar.EndMonthID;
            calendar.EndMonth = schcalendar.EndMonth;
        }

        void OnSelectedStartMonthChanged(IEnumerable<string> value)
        {
            string selectedValue = value.ElementAt(0);
            calendar.StartMonth = selectedValue;
            calendar.StartMonthID = months.FirstOrDefault(s => s.StartMonth == selectedValue).StartMonthID;
        }

        void OnSelectedEndMonthChanged(IEnumerable<string> value)
        {
            string selectedValue = value.ElementAt(0);
            calendar.EndMonth = selectedValue;
            calendar.EndMonthID = months.FirstOrDefault(s => s.EndMonth == selectedValue).EndMonthID;
        }

        async Task SaveSchoolCalendar()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Update School Calendar Details Operation",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                await _schoolCalendarService.UpdateAsync("Settings/UpdateSchoolCalendar/", 1, calendar);
                await Swal.FireAsync("School Calendar Details", "Has Been Successfully Updated.", "success");
                await DisplaySchoolCalendar();
            }
        }

        async Task DisplaySchoolCalendar()
        {
            toolBarMenuId = 1;
            saveStatus = true;
            schoolcalendar = await _schoolCalendarService.GetAllAsync("Settings/GetSchoolCalendarList/1");
        }

        #endregion

        #region [Section - Click Events]

        void GoBack()
        {
            navManager.NavigateTo("/academics/setacademicsessions");
        }

        #endregion

    }
}
