using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Client.Extensions;

namespace WebAppAcademics.Client.Pages.Academics.Exam.Marks.Preview
{
    public partial class TermEndPreview
    {
        #region [Injection Declaration]
        [Inject] ILocalStorageService localStorageService { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ADMSchlList> schoolService { get; set; }
        [Inject] IAPIServices<ADMSchClassList> classService { get; set; }
        [Inject] IAPIServices<ACDStudentsMarksCognitive> studentMarksCognitiveService { get; set; }
        [Inject] IAPIServices<ACDStudentsResultCognitive> cognitiveResultsService { get; set; }
        [Inject] IAPIServices<ACDBroadSheet> braodsheetService { get; set; }
        [Inject] IAPIServices<string> stringValueService { get; set; }
        [Inject] IAPIServices<dynamic> bradsheetMarkListService { get; set; }
        [Inject] IAPIServices<bool> deleteService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Check Point / IGCSE Mark Preview";
            termid = await localStorageService.ReadEncryptedItemAsync<int>("termid");
            schTerm = await localStorageService.ReadEncryptedItemAsync<string>("schTerm");
            staffid = await localStorageService.ReadEncryptedItemAsync<int>("staffid");
            roleid = await localStorageService.ReadEncryptedItemAsync<int>("roleid");
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");
            await base.OnInitializedAsync();
        }

        #region [Variables Declaration]
        int termid { get; set; }
        int roleid { get; set; }
        int staffid { get; set; }
        string schTerm { get; set; }
        int schid { get; set; }
        int classid { get; set; }

        string selectedSchool { get; set; }
        string selectedClass { get; set; }
        string ScriptFilePath { get; set; }
        string currentActionTitle { get; set; }

        decimal i { get; set; } = 0;
        int j { get; set; } = 0;
        bool IsShow { get; set; } = true;
        string progressbarInfo { get; set; } = string.Empty;

        bool isLoading { get; set; } = true;
        string loadingmessage { get; set; } = "Waiting for your selection...";

        DialogOptions dialogOptions = new() { FullWidth = true };
        bool visible { get; set; }
        void Submit() => visible = false;

        bool fixedheader { get; set; } = true;
        #endregion

        #region [Models Declaration]
        List<ADMSchlList> schools = new();
        List<ADMSchClassList> classList = new();
        List<ACDStudentsMarksCognitive> cognitiveMarks = new();
        List<string> fieldnames = new();
        List<dynamic> broadsheetMarkList = new();

        ACDBroadSheet broadsheet = new();
        ACDStudentsResultCognitive cognitiveResults = new();

        #endregion

        #region [Load / Click Events]

        async Task LoadClassList()
        {
            if (roleid == 1) //Administrator
            {
                classList = await classService.GetAllAsync("AdminSchool/GetClassList/1/" + schid + "/0");
            }
            else
            {
                var _classlist = await classService.GetAllAsync("AdminSchool/GetClassList/0/" + schid + "/0");
                classList = _classlist.Where(c => c.StaffID == staffid).ToList();
            }
        }

        async Task LoadCognitiveMarks(int switchid)
        {
            isLoading = true;
            StateHasChanged();

            try
            {
                await Task.Delay(500);
                cognitiveMarks = await studentMarksCognitiveService.GetAllAsync("AcademicsMarks/GetCognitiveMarks/" + switchid + "/" +
                                                                                    termid + "/" + schid + "/" + classid + "/0/0/0");
            }
            finally
            {
                isLoading = false;
            }
        }

        async Task OnSchoolChanged(IEnumerable<string> e)
        {
            selectedSchool = e.ElementAt(0);
            schid = schools.FirstOrDefault(s => s.School == selectedSchool).SchID;

            classid = 0;
            selectedClass = string.Empty;
            classList.Clear();
            await LoadClassList();

            cognitiveMarks.Clear();
            fieldnames.Clear();
            broadsheetMarkList.Clear();
        }

        async Task OnClassChanged(IEnumerable<string> e)
        {
            selectedClass = e.ElementAt(0);
            classid = classList.FirstOrDefault(c => c.ClassName == selectedClass).ClassID;

            cognitiveMarks.Clear();
            fieldnames.Clear();
            broadsheetMarkList.Clear();

            await RunTermEndScoresPreview();
        }

        #endregion

        #region [Scores Preview]       
        async Task RunTermEndScoresPreview()
        {
            await cognitiveResultsService.DeleteAsync("AcademicsResults/DeleteCognitiveResult/", 0);

            loadingmessage = "Please wait, while loading...";
            await LoadCognitiveMarks(2);

            IsShow = false;
            i = 0;
            j = 0;
            progressbarInfo = "Please wait loading End of Term Scores...";

            int maxValue = cognitiveMarks.Count();

            foreach (var item in cognitiveMarks)
            {
                j++;
                i = ((decimal)(j) / maxValue) * 100;

                decimal totalMark = item.Mark_CA1 + item.Mark_CA2 + item.Mark_CA3 + item.Mark_CBT + item.Mark_Exam;

                if (totalMark > 0)
                {
                    cognitiveResults.STDID = item.STDID;
                    cognitiveResults.StudentNo = item.AdmissionNo;
                    cognitiveResults.FullName = item.StudentName;
                    cognitiveResults.ClassID = item.ClassID;
                    cognitiveResults.SubjectID = item.SubjectID;
                    cognitiveResults.SubjectCode = item.SubjectCode;
                    cognitiveResults.Subject = item.Subject;
                    cognitiveResults.CA1 = Convert.ToInt32(Math.Round(item.Mark_CA1, MidpointRounding.AwayFromZero));
                    cognitiveResults.CA2 = Convert.ToInt32(Math.Round(item.Mark_CA2, MidpointRounding.AwayFromZero));
                    cognitiveResults.CA3 = Convert.ToInt32(Math.Round(item.Mark_CA3, MidpointRounding.AwayFromZero));
                    cognitiveResults.PMark_Exam = Convert.ToInt32(Math.Round(item.Mark_CBT, MidpointRounding.AwayFromZero));
                    cognitiveResults.CA = Convert.ToInt32(Math.Round(item.Mark_CA1 + item.Mark_CA2 + item.Mark_CA3 + item.Mark_CBT, MidpointRounding.AwayFromZero));
                    cognitiveResults.Exam = Convert.ToInt32(Math.Round(item.Mark_Exam, MidpointRounding.AwayFromZero));
                    cognitiveResults.TotalMark = Convert.ToInt32(Math.Round(item.Mark_CA1 + item.Mark_CA2 + item.Mark_CA3 + item.Mark_CBT + item.Mark_Exam, MidpointRounding.AwayFromZero));

                    await cognitiveResultsService.SaveAsync("AcademicsResults/AddCognitiveResult/", cognitiveResults);
                }

                StateHasChanged();
            }

            IsShow = true;
            await Swal.FireAsync("Preview End of Term Scores",
                    "Please, Click The Icon Above Showing Preview End of Term Scores.", "info");
        }

        async Task OnScoresPreviewSelectedOptionChanged(int selectedOpt)
        {
            await deleteService.DeleteAsync("AcademicsResults/DeleteBroadSheet/", 0);

            switch (selectedOpt)
            {
                case 1:
                    currentActionTitle = "End of Term Scores Preview: CA 1";
                    ScriptFilePath = await stringValueService.GetStringAsync("Files/GetSqlFilePath/generatebroadsheetca1.sql");
                    break;
                case 2:
                    currentActionTitle = "End of Term Scores Preview: CA 2";
                    ScriptFilePath = await stringValueService.GetStringAsync("Files/GetSqlFilePath/generatebroadsheetca2.sql");
                    break;
                case 3:
                    currentActionTitle = "End of Term Scores Preview: CA 3";
                    ScriptFilePath = await stringValueService.GetStringAsync("Files/GetSqlFilePath/generatebroadsheetca3.sql");
                    break;
                case 4:
                    currentActionTitle = "End of Term Scores Preview: CA";
                    ScriptFilePath = await stringValueService.GetStringAsync("Files/GetSqlFilePath/generatebroadsheetca.sql");
                    break;
                case 5:
                    currentActionTitle = "End of Term Scores Preview: CA 4 / CBT";
                    ScriptFilePath = await stringValueService.GetStringAsync("Files/GetSqlFilePath/generatebroadsheetcbt.sql");
                    break;
                case 6:
                    currentActionTitle = "End of Term Scores Preview: Exam";
                    ScriptFilePath = await stringValueService.GetStringAsync("Files/GetSqlFilePath/generatebroadsheetexam.sql");
                    break;
                case 7:
                    currentActionTitle = "End of Term Scores Preview: Total";
                    ScriptFilePath = await stringValueService.GetStringAsync("Files/GetSqlFilePath/generatebroadsheettotal.sql");
                    break;
            }

            fieldnames.Clear();
            broadsheetMarkList.Clear();
            broadsheet.scriptFilePath = ScriptFilePath;
            await braodsheetService.SaveAsync("AcademicsResults/ExecuteScript", broadsheet);
            fieldnames = await stringValueService.GetAllAsync("AcademicsResults/GetFieldNames/");

            if (fieldnames.Count() > 0)
            {
                broadsheetMarkList = await bradsheetMarkListService.GetAllAsync("AcademicsResults/GetBroadSheet/");
            }
            else
            {
                await Swal.FireAsync("Empty Marks", "No Mark Entry For " + selectedClass + " Class.", "info");
            }
        }

        void SelectAPreview()
        {
            visible = true;
        }

        async Task Refresh()
        {
            selectedSchool = string.Empty;
            schid = 0;
            schools.Clear();
            schools = await schoolService.GetAllAsync("AdminSchool/GetSchools/0");

            selectedClass = string.Empty;
            classid = 0;
            classList.Clear();

            cognitiveMarks.Clear();
            fieldnames.Clear();
            broadsheetMarkList.Clear();
        }

        #endregion

    }
}
