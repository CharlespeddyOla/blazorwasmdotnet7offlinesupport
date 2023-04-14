using Blazored.SessionStorage;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WebAppAcademics.Client.Extensions;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Shared.Models.Academics.CBT;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Administration.Students;
using WebAppAcademics.Shared.Models.Settings;

namespace WebAppAcademics.Client.Pages.ResultCheckerPages
{
    public partial class ResultChecker
    {

        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] ISessionStorageService sessionStorageService { get; set; }
        [Inject] IJSRuntime iJSRuntime { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<string> fileDownloadServices { get; set; }
        [Inject] IAPIServices<ADMStudents> studentService { get; set; }
        [Inject] IAPIServices<ACDReportType> resultTypeService { get; set; }
        [Inject] IAPIServices<byte[]> reportServices { get; set; }
        [Inject] IAPIServices<CBTConnectionInfo> cbtConnectionInfoService { get; set; }
        [Inject] IAPIServices<SETSchSessions> schoolSessionsService { get; set; }

        #endregion

        #region [Variables Declaration]
        int stdid { get; set; }
        int parentpincount { get; set; }
        int resulttermid { get; set; }
        string selectedResult { get; set; }
        string resultFilename { get; set; }
        string _academicTerm { get; set; }
        string _admissionNo { get; set; }
        string _studentName { get; set; }

        byte[] pdfFile { get; set; }
        private IJSObjectReference _jsModule;
        #endregion

        #region [Models Declaration]
        ADMStudents student = new();
        ACDReportType resulttype = new();
        CBTConnectionInfo cbtConnInfo = new();
        List<ResultsModel> _resultModel = new();
        List<string> _downloadedResults = new();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            _jsModule = await iJSRuntime.InvokeAsync<IJSObjectReference>("import", "./scripts/reports.js");

            stdid = await sessionStorageService.ReadEncryptedItemAsync<Int32>("stdid");
            _admissionNo = await sessionStorageService.ReadEncryptedItemAsync<string>("admissionno");
            _studentName = await sessionStorageService.ReadEncryptedItemAsync<string>("studentname");
            resulttermid = await sessionStorageService.ReadEncryptedItemAsync<Int32>("resulttermid");
            parentpincount = await sessionStorageService.ReadEncryptedItemAsync<Int32>("parentpincount");

            await LoadDefaults();

            await base.OnInitializedAsync();
        }

        async Task LoadDefaults()
        {
            if (resulttermid > 0)
            {
                var academicTerm = await schoolSessionsService.GetByIdAsync("Settings/GetAccademicSession/", resulttermid);
                _academicTerm = academicTerm.AcademicSession;
                await DownloadResults();
                await ParentPINCounter();
            }
        }

        async Task ParentPINCounter()
        {
            int _pinCount = parentpincount + 1;

            cbtConnInfo = await cbtConnectionInfoService.GetByIdAsync("AcademicsCBT/GetConnectionById/", 5);
            int _maxPINCount = Convert.ToInt32(cbtConnInfo.ConnectionValue);

            if (_pinCount >= _maxPINCount)
            {
                student.STDID = stdid;
                await studentService.UpdateAsync("AdminStudent/UpdateStudent/", 15, student);
            }

            student.STDID = stdid;
            student.ParentPinCount = _pinCount;
            await studentService.UpdateAsync("AdminStudent/UpdateStudent/", 14, student);
        }

        async Task DownloadResults()
        {
            string searchpattern = pad_an_int(stdid, 4);
            _downloadedResults = await fileDownloadServices.GetAllAsync("Files/GetFiles/Results/*" + searchpattern + "*");

            if (_downloadedResults.Count > 0)
            {
                for (int i = 0; i < _downloadedResults.Count; i++)
                {
                    string fileName = _downloadedResults[i];
                    string rigthString = fileName.Substring(fileName.Length - 5);
                    int resulttypeid = Convert.ToInt32(rigthString.Substring(0, 1));
                    resulttype = await resultTypeService.GetByIdAsync("AcademicsMarkSettings/GetResultTypeSetting/", resulttypeid);

                    _resultModel.Add(new ResultsModel()
                    {
                        ResultTypeId = resulttypeid,
                        FileName = _downloadedResults[i],
                        ResultType = resulttype.ReportType + " Result",
                    });
                }
            }
            else
            {
                await Swal.FireAsync("Student Academic Result", "No Result. Please contact the school.", "error");
            }
        }

        static string pad_an_int(int N, int P)
        {
            // string used in Format() method
            string s = "{0:";
            for (int i = 0; i < P; i++)
            {
                s += "0";
            }
            s += "}";

            // use of string.Format() method
            string value = string.Format(s, N);

            // return output
            return value;
        }

        async Task OnResultsChanged(IEnumerable<string> e)
        {
            selectedResult = e.ElementAt(0);
            resultFilename = _resultModel.FirstOrDefault(c => c.ResultType == selectedResult).FileName;

            pdfFile = null;
            pdfFile = await reportServices.GetResults("Files/GetPDFResult/" + resultFilename);

            await _jsModule.InvokeAsync<string>(
               "viewPdf",
               "iframeId",
               Convert.ToBase64String(pdfFile)
               );
        }


        void GoBack()
        {
            navManager.NavigateTo("/resultchecker");
        }
    }
}
