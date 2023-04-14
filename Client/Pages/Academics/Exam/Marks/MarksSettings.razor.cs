using Microsoft.AspNetCore.Components;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Academics.Marks;
using WebAppAcademics.Shared.Models.Settings;

namespace WebAppAcademics.Client.Pages.Academics.Exam.Marks
{
    public partial class MarksSettings
    {
        #region [Injection Declaration]
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<ACDSettingsGrade> gradeService { get; set; }
        [Inject] IAPIServices<ACDSettingsGradeMock> mockGradeService { get; set; }
        [Inject] IAPIServices<ACDSettingsGradeOthers> otherGradeService { get; set; }
        [Inject] IAPIServices<ACDSettingsGradeCheckPoint> checkpointGradeService { get; set; }
        [Inject] IAPIServices<ACDSettingsGradeIGCSE> igcseGradeService { get; set; }
        [Inject] IAPIServices<ACDSettingsMarks> marksService { get; set; }
        [Inject] IAPIServices<ACDSettingsRating> ratingService { get; set; }
        [Inject] IAPIServices<ACDSettingsRatingOptions> ratingOptionsService { get; set; }
        [Inject] IAPIServices<ACDSettingsRatingText> ratingTextService { get; set; }
        [Inject] IAPIServices<ACDReportType> resultTypeService { get; set; }
        [Inject] IAPIServices<ACDReportFooter> reportFooterService { get; set; }
        [Inject] IAPIServices<ACDSettingsOthers> otherSettingsService { get; set; }
        [Inject] IAPIServices<SETReports> reportTitleService { get; set; }
        [Inject] IAPIServices<ACDFlags> flagsService { get; set; }

        [CascadingParameter]
        public MainLayout Layout { get; set; }

        int toolBarMenuId { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Layout.currentPage = "Academics Settings";
            toolBarMenuId = 1;
            await LoadList();
            await base.OnInitializedAsync();
        }

        async Task LoadList()
        {
            resultTypeList = await resultTypeService.GetAllAsync("AcademicsMarkSettings/GetResultTypeSettings/1");
            otherSetting = await otherSettingsService.GetByIdAsync("AcademicsMarkSettings/GeOtherSetting/", 1);
            gradeList = await gradeService.GetAllAsync("AcademicsMarkSettings/GetGrades/1");
            mockGradeList = await mockGradeService.GetAllAsync("AcademicsMarkSettings/GetMockGrades/1");
            otherGradeList = await otherGradeService.GetAllAsync("AcademicsMarkSettings/GeOtherGradeSettings/1");
            checkpointGradeList = await checkpointGradeService.GetAllAsync("AcademicsMarkSettings/GetCheckPointGrades/1");
            igcseGradeList = await igcseGradeService.GetAllAsync("AcademicsMarkSettings/GetIGCSEGrades/1");
            markSettingsList = await marksService.GetAllAsync("AcademicsMarkSettings/GetMarkSettings/1");
            ratingList = await ratingService.GetAllAsync("AcademicsMarkSettings/GeRatingSettings/1");
            ratingOptionsList = await ratingOptionsService.GetAllAsync("AcademicsMarkSettings/GeRatingOptionSettings/1");
            ratingTextList = await ratingTextService.GetAllAsync("AcademicsMarkSettings/GeRatingTextSettings/1");
            reportFooter = await reportFooterService.GetByIdAsync("AcademicsMarkSettings/GetResultHeaderFooterSetting/", 1);
            reportTitles = await reportTitleService.GetAllAsync("Settings/GetReports/1");
            flagList = await flagsService.GetAllAsync("AcademicsMarkSettings/GetFlags/1");

            reportTypeID = resultTypeList.FirstOrDefault(r => r.SelectedExam == true).ReportTypeID;
            usecbtcheckboxtitle = UseCBTCheckedBoxTitle();
            ratingOptionID = ratingOptionsList.FirstOrDefault(o => o.UsedOption == true).OptionID;
            ratingTextID = ratingTextList.FirstOrDefault(t => t.UsedText == true).TextID;
        }


        #region [Section - Set Active Exam Mark Entry]
        #region [Models Declaration]
        List<ACDReportType> resultTypeList = new();
        ACDReportType resultType = new();

        ACDSettingsOthers otherSetting = new();
        #endregion

        int reportTypeID { get; set; }
        protected bool usecbtcolumn { get; set; }
        string usecbtcheckboxtitle { get; set; }

        async Task SetActiveExam()
        {
            foreach (var item in resultTypeList)
            {
                resultType.ReportTypeID = item.ReportTypeID;
                resultType.SelectedExam = false;
                await resultTypeService.UpdateAsync("AcademicsMarkSettings/UpdateResultTypeSetting/", 1, resultType);
            }

            resultType.ReportTypeID = reportTypeID;
            resultType.SelectedExam = true;
            await resultTypeService.UpdateAsync("AcademicsMarkSettings/UpdateResultTypeSetting/", 1, resultType);

            string selectedExam = resultTypeList.FirstOrDefault(r => r.ReportTypeID == reportTypeID).ReportType;
            await Swal.FireAsync("Operation Completed Successfully", selectedExam + " Has Been Set As The Active Exam For Mark Entry.", "success");
        }

        string UseCBTCheckedBoxTitle()
        {
            if (otherSetting.BoolValue)
            {
                usecbtcolumn = true;
                return otherSetting.TextValue;
            }
            else
            {
                usecbtcolumn = false;
                return otherSetting.Description;
            }
        }

        async Task CBTCheckBoxChanged(bool value)
        {
            usecbtcolumn = value;
            otherSetting.OtherSettingID = 1;

            if (usecbtcolumn)
            {
                otherSetting.BoolValue = true;
                await otherSettingsService.UpdateAsync("AcademicsMarkSettings/UpdateOtherSetting/", 1, otherSetting);
            }
            else
            {
                otherSetting.BoolValue = false;
                await otherSettingsService.UpdateAsync("AcademicsMarkSettings/UpdateOtherSetting/", 1, otherSetting);
            }

            usecbtcheckboxtitle = UseCBTCheckedBoxTitle();
        }

        #endregion

        #region [Section - General Grade]
        #region [Models Declaration]
        List<ACDSettingsGrade> gradeList = new();
        ACDSettingsGrade gradeDetails = new();
        ACDSettingsGrade selectedItemGrade = null;
        #endregion

        async Task SaveAllEntriesGrade()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Update Grade Settings",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                foreach (var item in gradeList)
                {
                    gradeDetails.GradeID = item.GradeID;
                    gradeDetails.LowerGrade = item.LowerGrade;
                    gradeDetails.HigherGrade = item.HigherGrade;
                    gradeDetails.GradeLetter = item.GradeLetter;
                    gradeDetails.GradeRemark = item.GradeRemark;
                    gradeDetails.TeachersComment = item.TeachersComment;
                    gradeDetails.PrincipalComment = item.PrincipalComment;

                    await gradeService.UpdateAsync("AcademicsMarkSettings/UpdateGrade/", 1, gradeDetails);
                }

                await Swal.FireAsync("Update Completed Successfull", "Grade Settings Update.", "success");
            }
        }

        async Task UpdateEntryGrade()
        {
            gradeDetails.GradeID = selectedItemGrade.GradeID;
            gradeDetails.LowerGrade = selectedItemGrade.LowerGrade;
            gradeDetails.HigherGrade = selectedItemGrade.HigherGrade;
            gradeDetails.GradeLetter = selectedItemGrade.GradeLetter;
            gradeDetails.GradeRemark = selectedItemGrade.GradeRemark;
            gradeDetails.TeachersComment = selectedItemGrade.TeachersComment;
            gradeDetails.PrincipalComment = selectedItemGrade.PrincipalComment;

            await gradeService.UpdateAsync("AcademicsMarkSettings/UpdateGrade/", 1, gradeDetails);
            Snackbar.Add("Current Grade Settings Has Been Successfully Updated");
        }


        #endregion

        #region [Section - Mock Grade Senior]

        #region [Models Declaration]
        List<ACDSettingsGradeMock> mockGradeList = new();
        ACDSettingsGradeMock mockGradeDetails = new();
        ACDSettingsGradeMock selectedItemMockGrade = null;
        #endregion

        async Task SaveAllEntriesMockGrade()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Update Grade Settings",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                foreach (var item in mockGradeList)
                {
                    mockGradeDetails.GradeID = item.GradeID;
                    mockGradeDetails.LowerGrade = item.LowerGrade;
                    mockGradeDetails.HigherGrade = item.HigherGrade;
                    mockGradeDetails.GradeLetter = item.GradeLetter;
                    mockGradeDetails.GradeRemark = item.GradeRemark;

                    await mockGradeService.UpdateAsync("AcademicsMarkSettings/UpdateMockGrade/", 1, mockGradeDetails);
                }

                await Swal.FireAsync("Update Completed Successfull", "Grade Settings Update.", "success");
            }
        }

        async Task UpdateEntryMockGrade()
        {
            mockGradeDetails.GradeID = selectedItemMockGrade.GradeID;
            mockGradeDetails.LowerGrade = selectedItemMockGrade.LowerGrade;
            mockGradeDetails.HigherGrade = selectedItemMockGrade.HigherGrade;
            mockGradeDetails.GradeLetter = selectedItemMockGrade.GradeLetter;
            mockGradeDetails.GradeRemark = selectedItemMockGrade.GradeRemark;

            await mockGradeService.UpdateAsync("AcademicsMarkSettings/UpdateMockGrade/", 1, mockGradeDetails);
            Snackbar.Add("Current Grade Settings Has Been Successfully Updated");
        }

        #endregion

        #region [Section - Mock Grade Junior]

        #region [Models Declaration]
        List<ACDSettingsGradeOthers> otherGradeList = new();
        ACDSettingsGradeOthers otherGradeDetails = new();
        ACDSettingsGradeOthers selectedItemOtherGrade = null;
        #endregion

        async Task SaveAllEntriesOtherGrade()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Update Grade Settings",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                foreach (var item in otherGradeList)
                {
                    otherGradeDetails.GradeID = item.GradeID;
                    otherGradeDetails.LowerGrade = item.LowerGrade;
                    otherGradeDetails.HigherGrade = item.HigherGrade;
                    otherGradeDetails.GradeLetter = item.GradeLetter;
                    otherGradeDetails.GradeRemark = item.GradeRemark;

                    await otherGradeService.UpdateAsync("AcademicsMarkSettings/UpdateOtherGradeSetting/", 1, otherGradeDetails);
                }

                await Swal.FireAsync("Update Completed Successfull", "Grade Settings Update.", "success");
            }
        }

        async Task UpdateEntryOtherGrade()
        {
            otherGradeDetails.GradeID = selectedItemOtherGrade.GradeID;
            otherGradeDetails.LowerGrade = selectedItemOtherGrade.LowerGrade;
            otherGradeDetails.HigherGrade = selectedItemOtherGrade.HigherGrade;
            otherGradeDetails.GradeLetter = selectedItemOtherGrade.GradeLetter;
            otherGradeDetails.GradeRemark = selectedItemOtherGrade.GradeRemark;

            await otherGradeService.UpdateAsync("AcademicsMarkSettings/UpdateOtherGradeSetting/", 1, otherGradeDetails);
            Snackbar.Add("Current Grade Settings Has Been Successfully Updated");
        }


        #endregion

        #region [Section - CheckPoint Grade]
        #region [Models Declaration]
        List<ACDSettingsGradeCheckPoint> checkpointGradeList = new();
        ACDSettingsGradeCheckPoint checkpointGradeDetails = new();
        ACDSettingsGradeCheckPoint selectedItemCheckPointGrade = null;
        #endregion

        async Task SaveAllEntriesCheckPointGrade()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Update Grade Settings",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                foreach (var item in checkpointGradeList)
                {
                    checkpointGradeDetails.GradeID = item.GradeID;
                    checkpointGradeDetails.HigherGrade = item.HigherGrade;
                    checkpointGradeDetails.LowerGrade = item.LowerGrade;
                    checkpointGradeDetails.HigherRating = item.HigherRating;
                    checkpointGradeDetails.LowerRating = item.LowerRating;
                    checkpointGradeDetails.GradeLetter = item.GradeLetter;
                    checkpointGradeDetails.GradeRemark = item.GradeRemark;
                    checkpointGradeDetails.AutoComments = item.AutoComments;

                    await checkpointGradeService.UpdateAsync("AcademicsMarkSettings/UpdateCheckPointGrade/", 1, checkpointGradeDetails);
                }

                await Swal.FireAsync("Update Completed Successfull", "Grade Settings Update.", "success");
            }
        }

        async Task UpdateEntryCheckPointGrade()
        {
            checkpointGradeDetails.GradeID = selectedItemCheckPointGrade.GradeID;
            checkpointGradeDetails.HigherGrade = selectedItemCheckPointGrade.HigherGrade;
            checkpointGradeDetails.LowerGrade = selectedItemCheckPointGrade.LowerGrade;
            checkpointGradeDetails.HigherRating = selectedItemCheckPointGrade.HigherRating;
            checkpointGradeDetails.LowerRating = selectedItemCheckPointGrade.LowerRating;
            checkpointGradeDetails.GradeLetter = selectedItemCheckPointGrade.GradeLetter;
            checkpointGradeDetails.GradeRemark = selectedItemCheckPointGrade.GradeRemark;
            checkpointGradeDetails.AutoComments = selectedItemCheckPointGrade.AutoComments;

            await checkpointGradeService.UpdateAsync("AcademicsMarkSettings/UpdateCheckPointGrade/", 1, checkpointGradeDetails);
            Snackbar.Add("Current Grade Settings Has Been Successfully Updated");
        }

        #endregion

        #region [Section - IGCSE Grade]
        #region [Models Declaration]
        List<ACDSettingsGradeIGCSE> igcseGradeList = new();
        ACDSettingsGradeIGCSE igcseGradeDetails = new();
        ACDSettingsGradeIGCSE selectedItemIGCSEGrade = null;
        #endregion

        async Task SaveAllEntriesIGCSEGrade()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Update Grade Settings",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                foreach (var item in igcseGradeList)
                {
                    igcseGradeDetails.GradeID = item.GradeID;
                    igcseGradeDetails.LowerGrade = item.LowerGrade;
                    igcseGradeDetails.HigherGrade = item.HigherGrade;
                    igcseGradeDetails.GradeLetter = item.GradeLetter;
                    igcseGradeDetails.GradeRemark = item.GradeRemark;
                    igcseGradeDetails.AutoComments = item.AutoComments;

                    await igcseGradeService.UpdateAsync("AcademicsMarkSettings/UpdateIGCSEGrade/", 1, igcseGradeDetails);
                }

                await Swal.FireAsync("Update Completed Successfull", "Grade Settings Update.", "success");
            }
        }

        async Task UpdateEntryIGCSEGrade()
        {
            igcseGradeDetails.GradeID = selectedItemIGCSEGrade.GradeID;
            igcseGradeDetails.LowerGrade = selectedItemIGCSEGrade.LowerGrade;
            igcseGradeDetails.HigherGrade = selectedItemIGCSEGrade.HigherGrade;
            igcseGradeDetails.GradeLetter = selectedItemIGCSEGrade.GradeLetter;
            igcseGradeDetails.GradeRemark = selectedItemIGCSEGrade.GradeRemark;
            igcseGradeDetails.AutoComments = selectedItemIGCSEGrade.AutoComments;

            await igcseGradeService.UpdateAsync("AcademicsMarkSettings/UpdateIGCSEGrade/", 1, igcseGradeDetails);
            Snackbar.Add("Current Grade Settings Has Been Successfully Updated");
        }


        #endregion

        #region [Section - Marks Setting]

        #region [Models Declaration]
        List<ACDSettingsMarks> markSettingsList = new();
        ACDSettingsMarks markSettingsDetails = new();
        ACDSettingsMarks selectedItemMarkSettings = null;
        #endregion

        async Task SaveAllEntriesMarkSettings()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Update Mark Settings",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                foreach (var item in markSettingsList)
                {
                    markSettingsDetails.MarkID = item.MarkID;
                    markSettingsDetails.MarkType = item.MarkType;
                    markSettingsDetails.Mark = item.Mark;
                    markSettingsDetails.PassMark = item.PassMark;
                    markSettingsDetails.ApplyPassMark = item.ApplyPassMark;
                    markSettingsDetails.ApplyCBT = item.ApplyCBT;

                    await marksService.UpdateAsync("AcademicsMarkSettings/UpdateMarkSetting/", 1, markSettingsDetails);
                }

                await Swal.FireAsync("Update Completed Successfull", "Mark Settings Update.", "success");
            }
        }

        async Task UpdateEntryMarkSettings()
        {
            markSettingsDetails.MarkID = selectedItemMarkSettings.MarkID;
            markSettingsDetails.MarkType = selectedItemMarkSettings.MarkType;
            markSettingsDetails.Mark = selectedItemMarkSettings.Mark;
            markSettingsDetails.PassMark = selectedItemMarkSettings.PassMark;
            markSettingsDetails.ApplyPassMark = selectedItemMarkSettings.ApplyPassMark;
            markSettingsDetails.ApplyCBT = selectedItemMarkSettings.ApplyCBT;

            await marksService.UpdateAsync("AcademicsMarkSettings/UpdateMarkSetting/", 1, markSettingsDetails);
            Snackbar.Add("Current Mark Settings Has Been Successfully Updated");
        }


        #endregion

        #region [Section - Rating Setting]
        #region [Models Declaration]
        List<ACDSettingsRating> ratingList = new();
        List<ACDSettingsRatingOptions> ratingOptionsList = new();
        List<ACDSettingsRatingText> ratingTextList = new();

        ACDSettingsRating ratingDetails = new();
        ACDSettingsRatingOptions ratingOptionsDetails = new();
        ACDSettingsRatingText ratingTextDetails = new();
        ACDSettingsRating selectedItemRating = null;
        #endregion

        async Task SaveAllEntriesRating()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Update Rating Settings",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                foreach (var item in ratingList)
                {
                    ratingDetails.RatingID = item.RatingID;
                    ratingDetails.Rating = item.Rating;
                    ratingDetails.LowScore = item.LowScore;
                    ratingDetails.HighScore = item.HighScore;
                    ratingDetails.GradeLetter = item.GradeLetter;
                    ratingDetails.RatingLevel = item.RatingLevel;
                    ratingDetails.RatingKey = item.RatingKey;

                    await ratingService.UpdateAsync("AcademicsMarkSettings/UpdateRatingSetting/", 1, ratingDetails);
                }

                await Swal.FireAsync("Update Completed Successfull", "Rating Settings Update.", "success");
            }
        }

        async Task UpdateEntryRating()
        {
            ratingDetails.RatingID = selectedItemRating.RatingID;
            ratingDetails.Rating = selectedItemRating.Rating;
            ratingDetails.LowScore = selectedItemRating.LowScore;
            ratingDetails.HighScore = selectedItemRating.HighScore;
            ratingDetails.GradeLetter = selectedItemRating.GradeLetter;
            ratingDetails.RatingLevel = selectedItemRating.RatingLevel;
            ratingDetails.RatingKey = selectedItemRating.RatingKey;

            await ratingService.UpdateAsync("AcademicsMarkSettings/UpdateRatingSetting/", 1, ratingDetails);
            Snackbar.Add("Current Rating Settings Has Been Successfully Updated");
        }

        int ratingOptionID { get; set; }
        async Task UpdateEntryRatingOptions()
        {
            foreach (var item in ratingOptionsList)
            {
                ratingOptionsDetails.OptionID = item.OptionID;
                ratingOptionsDetails.UsedOption = false;
                await ratingOptionsService.UpdateAsync("AcademicsMarkSettings/UpdateRatingOptionSetting/", 1, ratingOptionsDetails);
            }

            ratingOptionsDetails.OptionID = ratingOptionID;
            ratingOptionsDetails.UsedOption = true;
            await ratingOptionsService.UpdateAsync("AcademicsMarkSettings/UpdateRatingOptionSetting/", 1, ratingOptionsDetails);
            Snackbar.Add("Current Rating Options Has Been Successfully Updated");
        }

        int ratingTextID { get; set; }
        async Task UpdateEntryRatingText()
        {
            foreach (var item in ratingTextList)
            {
                ratingTextDetails.TextID = item.TextID;
                ratingTextDetails.UsedText = false;
                await ratingTextService.UpdateAsync("AcademicsMarkSettings/UpdateRatingTextSetting/", 1, ratingTextDetails);
            }

            ratingTextDetails.TextID = ratingTextID;
            ratingTextDetails.UsedText = true;
            await ratingTextService.UpdateAsync("AcademicsMarkSettings/UpdateRatingTextSetting/", 1, ratingTextDetails);
            Snackbar.Add("Current Rating Text Has Been Successfully Updated");
        }


        #endregion

        #region [Section - Result Header And Footer]
        #region [Models Declaration]
        ACDReportFooter reportFooter = new();
        #endregion
        async Task UpdateReportFooter()
        {
            reportFooter.FooterID = 1;
            await reportFooterService.UpdateAsync("AcademicsMarkSettings/UpdateResultHeaderFooterSetting/", 1, reportFooter);
            await Swal.FireAsync("Report Header & Footer", "Update Completed Successfull.", "success");
        }
        #endregion

        #region [Section - Report Titles]
        #region [Models Declaration]
        List<SETReports> reportTitles = new();
        SETReports reportTitle = new();
        SETReports selectedReportTitle = null;
        #endregion

        async Task UpdateResultsTitles()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Update Results Titles",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                foreach (var item in reportTitles)
                {
                    reportTitle.ReportID = item.ReportID;
                    reportTitle.ReportFileName = item.ReportFileName;
                    reportTitle.ReportDescr = item.ReportDescr;

                    await reportTitleService.UpdateAsync("Settings/UpdateReport/", 1, reportTitle);
                }

                await Swal.FireAsync("Update Completed Successfull", "Results Titles Update.", "success");
            }
        }

        async Task UpdateResultTitle()
        {
            reportTitle.ReportID = selectedReportTitle.ReportID;
            reportTitle.ReportFileName = selectedReportTitle.ReportFileName;
            reportTitle.ReportDescr = selectedReportTitle.ReportDescr;

            await reportTitleService.UpdateAsync("Settings/UpdateReport/", 1, reportTitle);
            Snackbar.Add("Current Result Title Has Been Successfully Updated");
        }
        #endregion

        #region [Section - Flags]
        #region [Models Declaration]
        List<ACDFlags> flagList = new();
        ACDFlags flag = new();
        ACDFlags selectedFlag = null;
        #endregion

        async Task UpdateFlag()
        {
            flag.FlagID = selectedFlag.FlagID;
            flag.Flag = selectedFlag.Flag;

            await flagsService.UpdateAsync("AcademicsMarkSettings/UpdateFlag/", 1, flag);
            Snackbar.Add("CurrentFlag Has Been Successfully Updated");
        }


        #endregion

        #region [Section - Click Events]
        void SetAciveExamEvent()
        {
            toolBarMenuId = 1;
        }

        void GeneralGradeEvent()
        {
            toolBarMenuId = 2;
        }

        void MockGradeSeniorEvent()
        {
            toolBarMenuId = 3;
        }

        void MockGradeJuniorEvent()
        {
            toolBarMenuId = 4;
        }

        void CheckPointGradeEvent()
        {
            toolBarMenuId = 5;
        }

        void IGCSEGradeEvent()
        {
            toolBarMenuId = 6;
        }

        void MarksSettingEvent()
        {
            toolBarMenuId = 7;
        }

        void RatingSettingEvent()
        {
            toolBarMenuId = 8;
        }

        void ResultHeaderFooterEvent()
        {
            toolBarMenuId = 9;
        }

        void ResultsTitlesEvent()
        {
            toolBarMenuId = 10;
        }

        void FlagsEvent()
        {
            toolBarMenuId = 11;
        }

        #endregion

    }
}
