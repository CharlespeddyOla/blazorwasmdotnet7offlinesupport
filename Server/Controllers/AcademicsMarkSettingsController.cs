using Microsoft.AspNetCore.Mvc;
using WebAppAcademics.Server.Interfaces;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicsMarkSettingsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        SwitchModel _switch = new SwitchModel();

        public AcademicsMarkSettingsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region [Academics - Grade Settings]
        [HttpGet]
        [Route("GetGrades/{id}")]
        public async Task<IActionResult> GetGrades(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.GradeSettings.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetGrade/{id}")]
        public async Task<IActionResult> GetGrade(int id)
        {
            var data = await unitOfWork.GradeSettings.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddGrade")]
        public async Task<IActionResult> AddGrade(ACDSettingsGrade model)
        {
            var data = await unitOfWork.GradeSettings.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateGrade/{id}")]
        public async Task<IActionResult> UpdateGrade(int id, ACDSettingsGrade model)
        {
            var data = await unitOfWork.GradeSettings.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteGrade")]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var data = await unitOfWork.GradeSettings.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - Mock Grade Settings: Senior]
        [HttpGet]
        [Route("GetMockGrades/{id}")]
        public async Task<IActionResult> GetMockGrades(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.MockGradeSettings.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetMockGrade/{id}")]
        public async Task<IActionResult> GetMockGrade(int id)
        {
            var data = await unitOfWork.MockGradeSettings.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddMockGrade")]
        public async Task<IActionResult> AddMockGrade(ACDSettingsGradeMock model)
        {
            var data = await unitOfWork.MockGradeSettings.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateMockGrade/{id}")]
        public async Task<IActionResult> UpdateMockGrade(int id, ACDSettingsGradeMock model)
        {
            var data = await unitOfWork.MockGradeSettings.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteMockGrade")]
        public async Task<IActionResult> DeleteMockGrade(int id)
        {
            var data = await unitOfWork.MockGradeSettings.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - Other Grade Settings: Junior]
        [HttpGet]
        [Route("GeOtherGradeSettings/{id}")]
        public async Task<IActionResult> GeOtherGradeSettings(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.OtherGradeSettings.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GeOtherGradeSetting/{id}")]
        public async Task<IActionResult> GeOtherGradeSetting(int id)
        {
            var data = await unitOfWork.OtherGradeSettings.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddOtherGradeSetting")]
        public async Task<IActionResult> AddOtherGradeSetting(ACDSettingsGradeOthers model)
        {
            var data = await unitOfWork.OtherGradeSettings.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateOtherGradeSetting/{id}")]
        public async Task<IActionResult> UpdateOtherGradeSetting(int id, ACDSettingsGradeOthers model)
        {
            var data = await unitOfWork.OtherGradeSettings.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteOtherGradeSetting")]
        public async Task<IActionResult> DeleteOtherGradeSetting(int id)
        {
            var data = await unitOfWork.OtherGradeSettings.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - CheckPoint Grade Settings]
        [HttpGet]
        [Route("GetCheckPointGrades/{id}")]
        public async Task<IActionResult> GetCheckPointGrades(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.CheckPointGradeSettings.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetCheckPointGrade/{id}")]
        public async Task<IActionResult> GetCheckPointGrade(int id)
        {
            var data = await unitOfWork.CheckPointGradeSettings.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddCheckPointGrade")]
        public async Task<IActionResult> AddCheckPointGrade(ACDSettingsGradeCheckPoint model)
        {
            var data = await unitOfWork.CheckPointGradeSettings.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateCheckPointGrade/{id}")]
        public async Task<IActionResult> UpdateCheckPointGrade(int id, ACDSettingsGradeCheckPoint model)
        {
            var data = await unitOfWork.CheckPointGradeSettings.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteCheckPointGrade")]
        public async Task<IActionResult> DeleteCheckPointGrade(int id)
        {
            var data = await unitOfWork.CheckPointGradeSettings.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - IGCSE Grade Settings]
        [HttpGet]
        [Route("GetIGCSEGrades/{id}")]
        public async Task<IActionResult> GetIGCSEGrades(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.IGCSEGradeSettings.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetIGCSEGrade/{id}")]
        public async Task<IActionResult> GetIGCSEGrade(int id)
        {
            var data = await unitOfWork.IGCSEGradeSettings.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddIGCSEGrade")]
        public async Task<IActionResult> AddIGCSEGrade(ACDSettingsGradeIGCSE model)
        {
            var data = await unitOfWork.IGCSEGradeSettings.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateIGCSEGrade/{id}")]
        public async Task<IActionResult> UpdateIGCSEGrade(int id, ACDSettingsGradeIGCSE model)
        {
            var data = await unitOfWork.IGCSEGradeSettings.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteIGCSEGrade")]
        public async Task<IActionResult> DeleteIGCSEGrade(int id)
        {
            var data = await unitOfWork.IGCSEGradeSettings.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - Mark Settings]
        [HttpGet]
        [Route("GetMarkSettings/{id}")]
        public async Task<IActionResult> GetMarkSettings(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.MarkSettings.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetMarkSetting/{id}")]
        public async Task<IActionResult> GetMarkSetting(int id)
        {
            var data = await unitOfWork.MarkSettings.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddMarkSetting")]
        public async Task<IActionResult> AddMarkSetting(ACDSettingsMarks model)
        {
            var data = await unitOfWork.MarkSettings.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateMarkSetting/{id}")]
        public async Task<IActionResult> UpdateMarkSetting(int id, ACDSettingsMarks model)
        {
            var data = await unitOfWork.MarkSettings.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteMarkSetting")]
        public async Task<IActionResult> DeleteMarkSetting(int id)
        {
            var data = await unitOfWork.MarkSettings.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - Rating Settings]
        [HttpGet]
        [Route("GeRatingSettings/{id}")]
        public async Task<IActionResult> GeRatingSettings(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.RatingSettings.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GeRatingSetting/{id}")]
        public async Task<IActionResult> GeRatingSetting(int id)
        {
            var data = await unitOfWork.RatingSettings.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddRatingSetting")]
        public async Task<IActionResult> AddRatingSetting(ACDSettingsRating model)
        {
            var data = await unitOfWork.RatingSettings.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateRatingSetting/{id}")]
        public async Task<IActionResult> UpdateRatingSetting(int id, ACDSettingsRating model)
        {
            var data = await unitOfWork.RatingSettings.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteRatingSetting")]
        public async Task<IActionResult> DeleteRatingSetting(int id)
        {
            var data = await unitOfWork.RatingSettings.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - Rating Option Settings]
        [HttpGet]
        [Route("GeRatingOptionSettings/{id}")]
        public async Task<IActionResult> GeRatingOptionSettings(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.RatingOptionSettings.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GeRatingOptionSetting/{id}")]
        public async Task<IActionResult> GeRatingOptionSetting(int id)
        {
            var data = await unitOfWork.RatingOptionSettings.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddRatingOptionSetting")]
        public async Task<IActionResult> AddRatingOptionSetting(ACDSettingsRatingOptions model)
        {
            var data = await unitOfWork.RatingOptionSettings.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateRatingOptionSetting/{id}")]
        public async Task<IActionResult> UpdateRatingOptionSetting(int id, ACDSettingsRatingOptions model)
        {
            var data = await unitOfWork.RatingOptionSettings.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteRatingOptionSetting")]
        public async Task<IActionResult> DeleteRatingOptionSetting(int id)
        {
            var data = await unitOfWork.RatingOptionSettings.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - Rating Text Settings]
        [HttpGet]
        [Route("GeRatingTextSettings/{id}")]
        public async Task<IActionResult> GeRatingTextSettings(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.RatingTextSettings.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GeRatingTextSetting/{id}")]
        public async Task<IActionResult> GeRatingTextSetting(int id)
        {
            var data = await unitOfWork.RatingTextSettings.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddRatingTextSetting")]
        public async Task<IActionResult> AddRatingTextSetting(ACDSettingsRatingText model)
        {
            var data = await unitOfWork.RatingTextSettings.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateRatingTextSetting/{id}")]
        public async Task<IActionResult> UpdateRatingTextSetting(int id, ACDSettingsRatingText model)
        {
            var data = await unitOfWork.RatingTextSettings.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteRatingTextSetting")]
        public async Task<IActionResult> DeleteRatingTextSetting(int id)
        {
            var data = await unitOfWork.RatingTextSettings.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - Other Settings]
        [HttpGet]
        [Route("GeOtherSettings/{id}")]
        public async Task<IActionResult> GeOtherSettings(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.OtherSettings.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GeOtherSetting/{id}")]
        public async Task<IActionResult> GeOtherSetting(int id)
        {
            var data = await unitOfWork.OtherSettings.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddOtherSetting")]
        public async Task<IActionResult> AddOtherSetting(ACDSettingsOthers model)
        {
            var data = await unitOfWork.OtherSettings.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateOtherSetting/{id}")]
        public async Task<IActionResult> UpdateOtherSetting(int id, ACDSettingsOthers model)
        {
            var data = await unitOfWork.OtherSettings.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteOtherSetting")]
        public async Task<IActionResult> DeleteOtherSetting(int id)
        {
            var data = await unitOfWork.OtherSettings.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - Result Type Settings]
        [HttpGet]
        [Route("GetResultTypeSettings/{id}")]
        public async Task<IActionResult> GetResultTypeSettings(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.ResultTypeSettings.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetResultTypeSetting/{id}")]
        public async Task<IActionResult> GetResultTypeSetting(int id)
        {
            var data = await unitOfWork.ResultTypeSettings.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddResultTypeSetting")]
        public async Task<IActionResult> AddResultTypeSetting(ACDReportType model)
        {
            var data = await unitOfWork.ResultTypeSettings.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateResultTypeSetting/{id}")]
        public async Task<IActionResult> UpdateResultTypeSetting(int id, ACDReportType model)
        {
            var data = await unitOfWork.ResultTypeSettings.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteResultTypeSetting")]
        public async Task<IActionResult> DeleteResultTypeSetting(int id)
        {
            var data = await unitOfWork.ResultTypeSettings.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - Result Header Footer]
        [HttpGet]
        [Route("GetResultHeaderFooterSettings/{id}")]
        public async Task<IActionResult> GetResultHeaderFooterSettings(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.ResultHeaderFooterSettings.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetResultHeaderFooterSetting/{id}")]
        public async Task<IActionResult> GetResultHeaderFooterSetting(int id)
        {
            var data = await unitOfWork.ResultHeaderFooterSettings.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddResultHeaderFooterSetting")]
        public async Task<IActionResult> AddResultHeaderFooterSetting(ACDReportFooter model)
        {
            var data = await unitOfWork.ResultHeaderFooterSettings.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateResultHeaderFooterSetting/{id}")]
        public async Task<IActionResult> UpdateResultHeaderFooterSetting(int id, ACDReportFooter model)
        {
            var data = await unitOfWork.ResultHeaderFooterSettings.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteResultHeaderFooterSetting")]
        public async Task<IActionResult> DeleteResultHeaderFooterSetting(int id)
        {
            var data = await unitOfWork.ResultHeaderFooterSettings.DeleteAsync(id);
            return Ok(data);
        }
        #endregion


        #region [Academics - Flags]
        [HttpGet]
        [Route("GetFlags/{id}")]
        public async Task<IActionResult> GetFlags(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.AcademicsFlags.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateFlag/{id}")]
        public async Task<IActionResult> UpdateFlag(int id, ACDFlags model)
        {
            var data = await unitOfWork.AcademicsFlags.UpdateAsync(id, model);
            return Ok(data);
        }

        #endregion
    }
}
