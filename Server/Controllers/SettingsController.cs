using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebAppAcademics.Server.Interfaces;
using WebAppAcademics.Shared;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Settings;

namespace WebAppAcademics.Server.Controllers
{
    [Route("api/[controller]")]    
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        SwitchModel _switch = new SwitchModel();

        public SettingsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region [Settings - Academic Session]
        [HttpGet]
        [Route("GetAccademicSessions/{id}/{schsession}")]
        public async Task<IActionResult> GetAccademicSessions(int id, int schsession)
        {
            _switch.SwitchID = id;
            _switch.SchSession = schsession;
            var data = await unitOfWork.SETSchSessions.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetAccademicSession/{id}")]
        public async Task<IActionResult> GetAccademicSession(int id)
        {
            var data = await unitOfWork.SETSchSessions.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddAccademicSession")]
        public async Task<IActionResult> AddAccademicSession(SETSchSessions model)
        {
            var data = await unitOfWork.SETSchSessions.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateAccademicSession/{id}")]
        public async Task<IActionResult> UpdateAccademicSession(int id, SETSchSessions model)
        {
            var data = await unitOfWork.SETSchSessions.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteAccademicSession")]
        public async Task<IActionResult> DeleteAccademicSession(int id)
        {
            var data = await unitOfWork.SETSchSessions.DeleteAsync(id);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetList/{id}/{schsession}")]
        public async Task<IActionResult> GetList(int id, int schsession)
        {
            _switch.SwitchID = id;
            _switch.SchSession = schsession;
            var data = await unitOfWork.SETSchSessions.GetAllAsync(_switch);
            return Ok(data);
        }

        #endregion

        #region [Settings - School Calendar]
        [HttpGet]
        [Route("GetSchoolCalendarList/{id}")]
        public async Task<IActionResult> GetSchoolCalendarList(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.SETSchCalendar.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetSchoolCalendar/{id}")]
        public async Task<IActionResult> GetSchoolCalendar(int id)
        {
            var data = await unitOfWork.SETSchCalendar.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddSchoolCalendar")]
        public async Task<IActionResult> AddSchoolCalendar(SETSchCalendar model)
        {
            var data = await unitOfWork.SETSchCalendar.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateSchoolCalendar/{id}")]
        public async Task<IActionResult> UpdateSchoolCalendar(int id, SETSchCalendar model)
        {
            var data = await unitOfWork.SETSchCalendar.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteSchoolCalendar")]
        public async Task<IActionResult> DeleteSchoolCalendar(int id)
        {
            var data = await unitOfWork.SETSchCalendar.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Settings - School Information]
        [HttpGet]
        [Route("GetSchoolInfo/{id}")]
        public async Task<IActionResult> GetSchoolInfo(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.SETSchInformation.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetSchoolDetails/{id}")]
        public async Task<IActionResult> GetSchoolDetails(int id)
        {
            var data = await unitOfWork.SETSchInformation.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddSchoolDetails")]
        public async Task<IActionResult> AddSchoolDetails(SETSchInformation model)
        {
            var data = await unitOfWork.SETSchInformation.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateSchoolDetails/{id}")]
        public async Task<IActionResult> UpdateSchoolDetails(int id, SETSchInformation model)
        {
            var data = await unitOfWork.SETSchInformation.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteSchoolDetails")]
        public async Task<IActionResult> DeleteSchoolDetails(int id)
        {
            var data = await unitOfWork.SETSchInformation.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Settings - Country]
        [HttpGet]
        [Route("GetCountries/{id}")]
        public async Task<IActionResult> GetCountries(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.SETCountries.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetCountry/{id}")]
        public async Task<IActionResult> GetCountry(int id)
        {
            var data = await unitOfWork.SETCountries.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddCountry")]
        public async Task<IActionResult> AddCountry(SETCountries model)
        {
            var data = await unitOfWork.SETCountries.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateCountry/{id}")]
        public async Task<IActionResult> UpdateCountry(int id, SETCountries model)
        {
            var data = await unitOfWork.SETCountries.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteCountry")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var data = await unitOfWork.SETCountries.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Settings - States]
        [HttpGet]
        [Route("GetStates/{id}/{countryid}")]
        public async Task<IActionResult> GetStates(int id, int countryid)
        {
            _switch.SwitchID = id;
            _switch.CountryID = countryid;
            var data = await unitOfWork.SETStates.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetState/{id}")]
        public async Task<IActionResult> GetState(int id)
        {
            var data = await unitOfWork.SETStates.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddState")]
        public async Task<IActionResult> AddState(SETStates model)
        {
            var data = await unitOfWork.SETStates.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateState/{id}")]
        public async Task<IActionResult> UpdateState(int id, SETStates model)
        {
            var data = await unitOfWork.SETStates.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteState")]
        public async Task<IActionResult> DeleteState(int id)
        {
            var data = await unitOfWork.SETStates.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Settings - LGA]
        [HttpGet]
        [Route("GetLGAs/{id}/{stateid}")]
        public async Task<IActionResult> GetLGAs(int id, int stateid)
        {
            _switch.SwitchID = id;
            _switch.StateID = stateid;
            var data = await unitOfWork.SETLGA.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetLGA/{id}")]
        public async Task<IActionResult> GetLGA(int id)
        {
            var data = await unitOfWork.SETLGA.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddLGA")]
        public async Task<IActionResult> AddLGA(SETLGA model)
        {
            var data = await unitOfWork.SETLGA.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateLGA/{id}")]
        public async Task<IActionResult> UpdateLGA(int id, SETLGA model)
        {
            var data = await unitOfWork.SETLGA.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteLGA")]
        public async Task<IActionResult> DeleteLGA(int id)
        {
            var data = await unitOfWork.SETLGA.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Settings - Status Type]
        [HttpGet]
        [Route("GetStatusTypes/{id}")]
        public async Task<IActionResult> GetStatusTypes(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.SETStatusType.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetStatusType/{id}")]
        public async Task<IActionResult> GetStatusType(int id)
        {
            var data = await unitOfWork.SETStatusType.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddStatusType")]
        public async Task<IActionResult> AddStatusType(SETStatusType model)
        {
            var data = await unitOfWork.SETStatusType.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateStatusType/{id}")]
        public async Task<IActionResult> UpdateStatusType(int id, SETStatusType model)
        {
            var data = await unitOfWork.SETStatusType.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteStatusType")]
        public async Task<IActionResult> DeleteStatusType(int id)
        {
            var data = await unitOfWork.SETStatusType.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Settings - Payee Type]
        [HttpGet]
        [Route("GetPayeeTypes/{id}")]
        public async Task<IActionResult> GetPayeeTypes(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.SETPayeeType.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetPayeeType/{id}")]
        public async Task<IActionResult> GetPayeeType(int id)
        {
            var data = await unitOfWork.SETPayeeType.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddPayeeType")]
        public async Task<IActionResult> AddPayeeType(SETPayeeType model)
        {
            var data = await unitOfWork.SETPayeeType.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdatePayeeType/{id}")]
        public async Task<IActionResult> UpdatePayeeType(int id, SETPayeeType model)
        {
            var data = await unitOfWork.SETPayeeType.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeletePayeeType")]
        public async Task<IActionResult> DeletePayeeType(int id)
        {
            var data = await unitOfWork.SETPayeeType.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Settings - Gender]
        [HttpGet]
        [Route("GetGenderList/{id}")]
        public async Task<IActionResult> GetGenderList(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.SETGender.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetGender/{id}")]
        public async Task<IActionResult> GetGender(int id)
        {
            var data = await unitOfWork.SETGender.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddGender")]
        public async Task<IActionResult> AddGender(SETGender model)
        {
            var data = await unitOfWork.SETGender.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateGender/{id}")]
        public async Task<IActionResult> UpdateGender(int id, SETGender model)
        {
            var data = await unitOfWork.SETGender.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteGender")]
        public async Task<IActionResult> DeleteGender(int id)
        {
            var data = await unitOfWork.SETGender.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Settings - Medical Information]
        [HttpGet]
        [Route("GetMedicalInfoList/{id}")]
        public async Task<IActionResult> GetMedicalInfoList(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.SETMedical.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetMedicalInfo/{id}")]
        public async Task<IActionResult> GetMedicalInfo(int id)
        {
            var data = await unitOfWork.SETMedical.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddMedicalInfo")]
        public async Task<IActionResult> AddMedicalInfo(SETMedical model)
        {
            var data = await unitOfWork.SETMedical.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateMedicalInfo/{id}")]
        public async Task<IActionResult> UpdateMedicalInfo(int id, SETMedical model)
        {
            var data = await unitOfWork.SETMedical.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteMedicalInfo")]
        public async Task<IActionResult> DeleteMedicalInfo(int id)
        {
            var data = await unitOfWork.SETMedical.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Settings - Religion]
        [HttpGet]
        [Route("GetReligionList/{id}")]
        public async Task<IActionResult> GetReligionList(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.SETReligion.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetReligion/{id}")]
        public async Task<IActionResult> GetReligion(int id)
        {
            var data = await unitOfWork.SETReligion.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddReligion")]
        public async Task<IActionResult> AddReligion(SETReligion model)
        {
            var data = await unitOfWork.SETReligion.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateReligion/{id}")]
        public async Task<IActionResult> UpdateReligion(int id, SETReligion model)
        {
            var data = await unitOfWork.SETReligion.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteReligion")]
        public async Task<IActionResult> DeleteReligion(int id)
        {
            var data = await unitOfWork.SETReligion.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Settings - Month List]
        [HttpGet]
        [Route("GetMonths/{id}")]
        public async Task<IActionResult> GetMonths(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.SETMonthList.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetMonth/{id}")]
        public async Task<IActionResult> GetMonth(int id)
        {
            var data = await unitOfWork.SETMonthList.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateMonth/{id}")]
        public async Task<IActionResult> UpdateMonth(int id, SETMonthList model)
        {
            var data = await unitOfWork.SETMonthList.UpdateAsync(id, model);
            return Ok(data);
        }
        #endregion

        #region [Settings - Staff Access Roles]
        [HttpGet]
        [Route("GetRoles/{id}")]
        public async Task<IActionResult> GetRoles(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.StaffAccessRoles.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRole/{id}")]
        public async Task<IActionResult> GetRole(int id)
        {
            var data = await unitOfWork.StaffAccessRoles.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddRole")]
        public async Task<IActionResult> AddRole(SETRole model)
        {
            var data = await unitOfWork.StaffAccessRoles.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateRole/{id}")]
        public async Task<IActionResult> UpdateRole(int id, SETRole model)
        {
            var data = await unitOfWork.StaffAccessRoles.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteRole")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var data = await unitOfWork.StaffAccessRoles.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Settings - Report Title Selection]
        [HttpGet]
        [Route("GetReports/{id}")]
        public async Task<IActionResult> GetReports(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.SETReports.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetReport/{id}")]
        public async Task<IActionResult> GetReport(int id)
        {
            var data = await unitOfWork.SETReports.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddReport")]
        public async Task<IActionResult> AddReport(SETReports model)
        {
            var data = await unitOfWork.SETReports.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateReport/{id}")]
        public async Task<IActionResult> UpdateReport(int id, SETReports model)
        {
            var data = await unitOfWork.SETReports.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteReport")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            var data = await unitOfWork.SETReports.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Settings - App License]
        [HttpGet]
        [Route("GetLicense/{id}")]
        public async Task<IActionResult> GetLicense(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.AppLicense.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetLicenserByID/{userid}")]
        public async Task<IActionResult> GetLicenserByID(int userid)
        {
            var data = await unitOfWork.AppLicense.GetByIdAsync(userid);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddLicense")]
        public async Task<IActionResult> AddLicense(SETAppLicense model)
        {
            var data = await unitOfWork.AppLicense.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateLicense/{id}")]
        public async Task<IActionResult> UpdateLicense(int id, SETAppLicense model)
        {
            var data = await unitOfWork.AppLicense.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetLicenseCount/{id}")]
        public async Task<IActionResult> GetLicenseCount(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.AppLicense.CountAsync(_switch);
            return Ok(data);
        }



        #endregion


    }
}
