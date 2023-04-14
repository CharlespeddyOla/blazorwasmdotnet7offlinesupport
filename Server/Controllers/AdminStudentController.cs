using Microsoft.AspNetCore.Mvc;
using WebAppAcademics.Server.Interfaces;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Administration.Students;

namespace WebAppAcademics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminStudentController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        SwitchModel _switch = new SwitchModel();

        public AdminStudentController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region [Administration - Students]
        [HttpGet]
        [Route("GetStudents/{id}/{schid}/{classid}/{studenttypeid}/{statustypeid}")]
        public async Task<IActionResult> GetStudents(int id, int schid, int classid, int studenttypeid, int statustypeid)
        {
            _switch.SwitchID = id;
            _switch.SchID = schid;
            _switch.ClassID = classid;
            _switch.StudentTypeID = studenttypeid;
            _switch.StatusTypeID = statustypeid;
            var data = await unitOfWork.ADMStudents.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetStudent/{id}")]
        public async Task<IActionResult> GetStudent(int id)
        {
            var data = await unitOfWork.ADMStudents.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddStudent")]
        public async Task<IActionResult> AddStudent(ADMStudents student)
        {
            var data = await unitOfWork.ADMStudents.AddAsync(student);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateStudent/{id}")]
        public async Task<IActionResult> UpdateStudent(int id, ADMStudents student)
        {
            var data = await unitOfWork.ADMStudents.UpdateAsync(id, student);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteStudent")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var data = await unitOfWork.ADMStudents.DeleteAsync(id);
            return Ok(data);
        }

        [HttpGet]
        [Route("Search/{id}/{searchcreteriaa}/{searchcreteriab}")]
        public async Task<IActionResult> Search(int id, string searchcreteriaa, string searchcreteriab)
        {
            _switch.SwitchID = id;
            _switch.SearchCriteriaA = searchcreteriaa;
            _switch.SearchCriteriaB = searchcreteriab;
            var data = await unitOfWork.ADMStudents.SearchAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetCount/{id}/{schid}/{classid}/{studenttypeid}/{statustypeid}")]
        public async Task<IActionResult> GetCount(int id, int schid, int classid, int studenttypeid, int statustypeid)
        {
            _switch.SwitchID = id;
            _switch.SchID = schid;
            _switch.ClassID = classid;
            _switch.StudentTypeID = studenttypeid;
            _switch.StatusTypeID = statustypeid;
            var data = await unitOfWork.ADMStudents.CountAsync(_switch);
            return Ok(data);
        }
        #endregion

        #region [Administration - Student Type]
        [HttpGet]
        [Route("GetStudentType/{id}")]
        public async Task<IActionResult> GetStudentType(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.ADMStudentType.GetAllAsync(_switch);
            return Ok(data);
        }
        #endregion

        #region [Settings - Student Medical History]
        [HttpGet]
        [Route("GetMedicalHistoryList/{id}/{stdid}")]
        public async Task<IActionResult> GetMedicalHistoryList(int id, int stdid)
        {
            _switch.SwitchID = id;
            _switch.STDID = stdid;
            var data = await unitOfWork.ADMStudentMEDHistory.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetMedicalHistory/{id}")]
        public async Task<IActionResult> GetMedicalHistory(int id)
        {
            var data = await unitOfWork.ADMStudentMEDHistory.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddMedicalHistory")]
        public async Task<IActionResult> AddMedicalHistory(ADMStudentMEDHistory model)
        {
            var data = await unitOfWork.ADMStudentMEDHistory.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateMedicalHistory/{id}")]
        public async Task<IActionResult> UpdateMedicalHistory(int id, ADMStudentMEDHistory model)
        {
            var data = await unitOfWork.ADMStudentMEDHistory.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteMedicalHistory")]
        public async Task<IActionResult> DeleteMedicalHistory(int id)
        {
            var data = await unitOfWork.ADMStudentMEDHistory.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Settings - Student Exit Type]
        [HttpGet]
        [Route("GetExitTypeList/{id}")]
        public async Task<IActionResult> GetExitTypeList(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.ADMStudentExit.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetExitType/{id}")]
        public async Task<IActionResult> GetExitType(int id)
        {
            var data = await unitOfWork.ADMStudentExit.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddExitType")]
        public async Task<IActionResult> AddExitType(ADMStudentExit model)
        {
            var data = await unitOfWork.ADMStudentExit.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateExitType/{id}")]
        public async Task<IActionResult> UpdateExitType(int id, ADMStudentExit model)
        {
            var data = await unitOfWork.ADMStudentExit.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteExitType")]
        public async Task<IActionResult> DeleteExitType(int id)
        {
            var data = await unitOfWork.ADMStudentExit.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Settings - Student Club]
        [HttpGet]
        [Route("GetClubs/{id}")]
        public async Task<IActionResult> GetClubs(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.ADMSchClub.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetClub/{id}")]
        public async Task<IActionResult> GetClub(int id)
        {
            var data = await unitOfWork.ADMSchClub.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddClub")]
        public async Task<IActionResult> AddClub(ADMSchClub model)
        {
            var data = await unitOfWork.ADMSchClub.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateClub/{id}")]
        public async Task<IActionResult> UpdateClub(int id, ADMSchClub model)
        {
            var data = await unitOfWork.ADMSchClub.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteClub")]
        public async Task<IActionResult> DeleteClub(int id)
        {
            var data = await unitOfWork.ADMSchClub.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Settings - Student Club Role]
        [HttpGet]
        [Route("GetClubRoles/{id}")]
        public async Task<IActionResult> GetClubRoles(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.ADMSchClubRole.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetClubRole/{id}")]
        public async Task<IActionResult> GetClubRole(int id)
        {
            var data = await unitOfWork.ADMSchClubRole.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddClubRole")]
        public async Task<IActionResult> AddClubRole(ADMSchClubRole model)
        {
            var data = await unitOfWork.ADMSchClubRole.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateClubRole/{id}")]
        public async Task<IActionResult> UpdateClubRole(int id, ADMSchClubRole model)
        {
            var data = await unitOfWork.ADMSchClubRole.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteClubRole")]
        public async Task<IActionResult> DeleteClubRole(int id)
        {
            var data = await unitOfWork.ADMSchClubRole.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Administration - Student Parent]
        [HttpGet]
        [Route("GetParents/{id}/{statustypeid}")]
        public async Task<IActionResult> GetParents(int id, int statustypeid)
        {
            _switch.SwitchID = id;
            _switch.StatusTypeID = statustypeid;
            var data = await unitOfWork.ADMSchParents.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetParent/{id}")]
        public async Task<IActionResult> GetParent(int id)
        {
            var data = await unitOfWork.ADMSchParents.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddParent")]
        public async Task<IActionResult> AddParent(ADMSchParents model)
        {
            var data = await unitOfWork.ADMSchParents.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateParent/{id}")]
        public async Task<IActionResult> UpdateParent(int id, ADMSchParents model)
        {
            var data = await unitOfWork.ADMSchParents.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteParent")]
        public async Task<IActionResult> DeleteParent(int id)
        {
            var data = await unitOfWork.ADMSchParents.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

    }
}
