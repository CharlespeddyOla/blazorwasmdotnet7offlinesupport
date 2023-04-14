using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Administration.School;
using WebAppAcademics.Client.Extensions;

namespace WebAppAcademics.Client.Pages.Academics.Exam.Marks.Preview
{
    public partial class CheckPointPreview
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

        decimal i { get; set; } = 0;
        int j { get; set; } = 0;
        bool IsShow { get; set; } = true;
        string progressbarInfo { get; set; } = string.Empty;

        bool isLoading { get; set; } = true;
        string loadingmessage { get; set; } = "Waiting for your selection...";

        bool fixedheader { get; set; } = true;
        bool IsCheckPointClass { get; set; }
        bool IsIGCSEClass { get; set; }
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
            IsCheckPointClass = classList.FirstOrDefault(c => c.ClassID == classid).CheckPointClass;
            IsIGCSEClass = classList.FirstOrDefault(c => c.ClassID == classid).IGCSEClass;

            cognitiveMarks.Clear();
            fieldnames.Clear();
            broadsheetMarkList.Clear();
           
            await RunCheckPointIGCSEPreview();
        }

        string SelectedPreviewType()
        {
            string result = string.Empty;

            if (IsCheckPointClass)
            {
                result = "Check Point Marks Preview for " + schTerm + " Term";
            }
            else if (IsIGCSEClass)
            {
                result = "IGSCE Preview Marks for " + schTerm + " Term";
            }

            return result;
        }

        #endregion

        #region [Scores Preview]
        async Task RunCheckPointIGCSEPreview()
        {
            await cognitiveResultsService.DeleteAsync("AcademicsResults/DeleteCognitiveResult/", 0);
            await deleteService.DeleteAsync("AcademicsResults/DeleteBroadSheet/", 0);

            ScriptFilePath = await stringValueService.GetStringAsync("Files/GetSqlFilePath/generatebroadsheettotal.sql");
            broadsheet.scriptFilePath = ScriptFilePath;

            loadingmessage = "Please wait, while loading...";
            await LoadCognitiveMarks(3);

            IsShow = false;
            i = 0;
            j = 0;
            progressbarInfo = "Please wait loading CheckPoint Or IGCSE Scores...";

            int maxValue = cognitiveMarks.Count();

            foreach (var item in cognitiveMarks)
            {
                j++;
                i = ((decimal)(j) / maxValue) * 100;

                if (item.Mark_ICGC > 0)
                {
                    cognitiveResults.STDID = item.STDID;
                    cognitiveResults.StudentNo = item.AdmissionNo;
                    cognitiveResults.FullName = item.StudentName;
                    cognitiveResults.ClassID = item.ClassID;
                    cognitiveResults.SubjectID = item.SubjectID;
                    cognitiveResults.SubjectCode = item.SubjectCode;
                    cognitiveResults.Subject = item.Subject;
                    cognitiveResults.TotalMark = Convert.ToInt32(Math.Round(item.Mark_ICGC, MidpointRounding.AwayFromZero));

                    await cognitiveResultsService.SaveAsync("AcademicsResults/AddCognitiveResult/", cognitiveResults);
                }
                StateHasChanged();
            }

            IsShow = true;

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
