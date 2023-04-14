
using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Settings;

namespace WebAppAcademics.Client.Pages.Settings.School
{
    public partial class SETAcademicSessions
    {
        #region [Injection Declaration]
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<SETSchSessions> _academicSessionService { get; set; }
        [Inject] IAPIServices<SETSchInformation> _schoolInfoService { get; set; }
        [Inject] IAPIServices<SETSchCalendar> _schoolCalendarService { get; set; }
        [Inject] IAPIServices<SETMonthList> _monthListService { get; set; }
        [Inject] IAPIServices<SETCountries> countryService { get; set; }
        [Inject] IAPIServices<SETStates> stateService { get; set; }
        [Inject] IAPIServices<SETLGA> lgaService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int toolBarMenuId { get; set; } //1 - Refresh List Or List; 2 - Create A New Term; 3 - Display School Calendar
        int formId { get; set; } //1 - Create A New Term Form; 2 - Edit Term Details Form
        string selectedYear { get; set; }
        bool saveStatus { get; set; } = true;
        Utilities utilities = new Utilities();
        #endregion

        #region [Models Declaration]
        List<SETSchSessions> sessionlist = new();
        List<SETSchSessions> academicyearlist = new();

        SETSchSessions session = new();

        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Academic Sessions";
            await LoadList();
            toolBarMenuId = 1;
        }

        #region [Load Academic Sessions]

        async Task LoadList()
        {
            sessionlist = await _academicSessionService.GetAllAsync("Settings/GetAccademicSessions/1/0");
            academicyearlist = await _academicSessionService.GetAllAsync("Settings/GetAccademicSessions/2/0");            
        }

        async Task OnSelectedAcademicYearChanged(IEnumerable<string> e)
        {
            selectedYear = e.ElementAt(0);
            int _schsession = academicyearlist.FirstOrDefault(y => y.AcademicYear == selectedYear).SchSession;
            sessionlist = await _academicSessionService.GetAllAsync("Settings/GetAccademicSessions/3/" + _schsession);
        }



        #endregion

        #region [School Term Forms]       
        string pagetitle = "Create a new Term";
        string buttontitle = "Save";
        string schoolTerm { get; set; } = string.Empty;
        string startMonth { get; set; } = string.Empty;
        string endMonth { get; set; } = string.Empty;

        DateTime startDate { get; set; } = DateTime.Now;
        DateTime endDate { get; set; } = DateTime.Now;
        int _attendance { get; set; } = 0;

        async Task CreateNewTermForm()
        {
            saveStatus = false;
            formId = 1;
            sessionlist = await _academicSessionService.GetAllAsync("Settings/GetAccademicSessions/1/0");

            int sessioncount = sessionlist.Count();
            int lastsession = sessionlist.FirstOrDefault(s => s.TermID == sessionlist.Max(t => t.TermID)).SchSession;
            int termcount = sessionlist.Where(t => t.SchSession == lastsession).Count();

            if (sessioncount == 0) // First Session in Database Table
            {

            }
            else
            {
                if (termcount == 3)
                {
                    int newSession = Convert.ToInt32(lastsession.ToString().Substring(4, 4) + (Int32.Parse(lastsession.ToString().Substring(4, 4)) + 1).ToString());
                    int newTermCount = sessionlist.Where(t => t.SchSession == newSession).Count(); ;

                    session.AcademicYear = newSession.ToString().Substring(0, 4) + "/" + newSession.ToString().Substring(4, 4);
                    session.SchTerm = utilities.NewTerm(newTermCount);
                    session.SchSession = Convert.ToInt32(newSession);
                    session.CalendarID = 1;
                }
                else
                {
                    int schsession = Convert.ToInt32(lastsession.ToString().Substring(0, 4) + lastsession.ToString().Substring(4, 4));
                    session.AcademicYear = lastsession.ToString().Substring(0, 4) + "/" + lastsession.ToString().Substring(4, 4);
                    session.SchTerm = utilities.NewTerm(termcount);
                    session.SchSession = schsession;
                    session.CalendarID = termcount + 1;
                }
            }
        }

        async Task EditTermForm(int termid)
        {
            toolBarMenuId = 2;
            saveStatus = false;
            formId = 2;

            var a = await _academicSessionService.GetByIdAsync("Settings/GetAccademicSession/", termid);
            session.TermID = termid;
            session.AcademicYear = a.AcademicYear;
            session.SchTerm = a.SchTerm;
            session.CalendarID = a.CalendarID;
            session.StartDate = a.StartDate;
            session.EndDate = a.EndDate;
            session.Attendance = a.Attendance;

            pagetitle = "Edit " + a.SchTerm + " Term Details";
            buttontitle = "Update";
        }

        private async Task<bool> SelectedDateValidation()
        {
            bool result = false;

            var schcalendar = await _academicSessionService.GetByIdAsync("Settings/GetSchoolCalendar/", session.CalendarID);
            int selectedStartMonthID = session.StartDate.Value.Month;
            int selectedEndMonthID = session.EndDate.Value.Month;
            int calendarStartID = schcalendar.StartMonthID;
            int calendarEndID = schcalendar.EndMonthID;
            schoolTerm = schcalendar.SchTerm;
            startMonth = schcalendar.StartMonth;
            endMonth = schcalendar.EndMonth;

            if (selectedStartMonthID == calendarStartID && selectedEndMonthID == calendarEndID)
            {
                result = true;
            }
            
            return result;
        }

        private async Task SubmitValidForm()
        {
            if (await SelectedDateValidation())
            {
                SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
                {
                    Title = "Create/Update School Term Details Operation",
                    Text = "Do You Want To Continue With This Operation?",
                    Icon = SweetAlertIcon.Warning,
                    ShowCancelButton = true,
                    ConfirmButtonText = "Yes, Contnue!",
                    CancelButtonText = "No"
                });

                if (result.IsConfirmed)
                {
                    if (formId == 1)
                    {
                        var response = await _academicSessionService.SaveAsync("Settings/AddAccademicSession/", session);
                        session.TermID = response.TermID;
                        session.Id  = response.TermID;
                        await _academicSessionService.UpdateAsync("Settings/UpdateAccademicSession/", 2, session);
                        await Swal.FireAsync("A New School Term", "Has Been Successfully Created.", "success");
                    }
                    else if (formId == 2)
                    {
                        await _academicSessionService.UpdateAsync("Settings/UpdateAccademicSession/", 1, session);
                        await Swal.FireAsync("School Term Details", "Has Been Successfully Updated.", "success");
                    }

                    await LoadAacademicSessions();
                }                    
            }
            else
            {
                await Swal.FireAsync("Selected Start Date and End Date Does Not Fall Within The School Calanedar Month Range.",
                   schoolTerm + " Term School Calendar Is Between " + startMonth + " And " + endMonth + ".", "error");
            }
        }


        #endregion

        #region [Click Events]
        async Task LoadAacademicSessions()
        {
            toolBarMenuId = 1;
            saveStatus = true;
            selectedYear = string.Empty;
            academicyearlist.Clear();
            await LoadList();
        }

        async Task CreateNewTerm()
        {
            toolBarMenuId = 2;
            formId = 1;
            pagetitle = "Create a new Term";
            buttontitle = "Save";
            _attendance = 0;
            session.StartDate = startDate;
            session.EndDate = endDate;
            session.Attendance = 0;
            await CreateNewTermForm();
        }

        #endregion

    }
}
