using Microsoft.AspNetCore.Mvc;
using WebAppAcademics.Server.Interfaces;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Administration.Staff;

namespace WebAppAcademics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminStaffController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        SwitchModel _switch = new SwitchModel();

        public AdminStaffController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region [Administration - Staff]
        [HttpGet]
        [Route("GetStaffs/{id}/{statustypeid}/{employegroupid}/{jobtypeid}/{locid}")]
        public async Task<IActionResult> GetStaffs(int id, int statustypeid, int employegroupid, int jobtypeid, int locid)
        {
            _switch.SwitchID = id;
            _switch.StatusTypeID = statustypeid;
            _switch.EmployeeGroupID = employegroupid;
            _switch.JobTypeID = jobtypeid;
            _switch.LocID = locid;
            var data = await unitOfWork.ADMEmployee.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetStaff/{id}")]
        public async Task<IActionResult> GetStaff(int id)
        {
            var data = await unitOfWork.ADMEmployee.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddStaff")]
        public async Task<IActionResult> AddStaff(ADMEmployee staff)
        {
            var data = await unitOfWork.ADMEmployee.AddAsync(staff);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateStaff/{id}")]
        public async Task<IActionResult> UpdateStaff(int id, ADMEmployee staff)
        {
            var data = await unitOfWork.ADMEmployee.UpdateAsync(id, staff);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteStaff")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var data = await unitOfWork.ADMEmployee.DeleteAsync(id);
            return Ok(data);
        }

        [HttpGet]
        [Route("Search/{id}/{statustypeid}/{searchcriteriaid}/{searchcreteriaa}/{searchcreteriab}")]
        public async Task<IActionResult> Search(int id, int statustypeid, int searchcriteriaid, string searchcreteriaa, string searchcreteriab)
        {
            _switch.SwitchID = id;
            _switch.StatusTypeID = statustypeid;
            _switch.SearchById = searchcriteriaid;
            _switch.SearchCriteriaA = searchcreteriaa;
            _switch.SearchCriteriaB = searchcreteriab;
            var data = await unitOfWork.ADMStudents.SearchAsync(_switch);
            return Ok(data);
        }
        #endregion

        #region [Administration - Staff Department]
        [HttpGet]
        [Route("GetDepartments/{id}")]
        public async Task<IActionResult> GetDepartments(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.ADMEmployeeDepts.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetDepartment/{id}")]
        public async Task<IActionResult> GetDepartment(int id)
        {
            var data = await unitOfWork.ADMEmployeeDepts.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddDepartment")]
        public async Task<IActionResult> AddDepartment(ADMEmployeeDepts dept)
        {
            var data = await unitOfWork.ADMEmployeeDepts.AddAsync(dept);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateDepartment/{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, ADMEmployeeDepts dept)
        {
            var data = await unitOfWork.ADMEmployeeDepts.UpdateAsync(id, dept);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteDepartment")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var data = await unitOfWork.ADMEmployeeDepts.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Administration - Staff Job Type]
        [HttpGet]
        [Route("GetJobTypes/{id}")]
        public async Task<IActionResult> GetJobTypes(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.ADMEmployeeJobType.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetJobType/{id}")]
        public async Task<IActionResult> GetJobType(int id)
        {
            var data = await unitOfWork.ADMEmployeeJobType.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddJobType")]
        public async Task<IActionResult> AddJobType(ADMEmployeeJobType dept)
        {
            var data = await unitOfWork.ADMEmployeeJobType.AddAsync(dept);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateJobType/{id}")]
        public async Task<IActionResult> UpdateJobType(int id, ADMEmployeeJobType dept)
        {
            var data = await unitOfWork.ADMEmployeeJobType.UpdateAsync(id, dept);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteJobType")]
        public async Task<IActionResult> DeleteJobType(int id)
        {
            var data = await unitOfWork.ADMEmployeeJobType.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Administration - Staff Location]
        [HttpGet]
        [Route("GetStaffLocations/{id}")]
        public async Task<IActionResult> GetStaffLocations(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.ADMEmployeeLocation.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetStaffLocation/{id}")]
        public async Task<IActionResult> GetStaffLocation(int id)
        {
            var data = await unitOfWork.ADMEmployeeLocation.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddStaffLocation")]
        public async Task<IActionResult> AddStaffLocation(ADMEmployeeLocation dept)
        {
            var data = await unitOfWork.ADMEmployeeLocation.AddAsync(dept);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateStaffLocation/{id}")]
        public async Task<IActionResult> UpdateStaffLocation(int id, ADMEmployeeLocation dept)
        {
            var data = await unitOfWork.ADMEmployeeLocation.UpdateAsync(id, dept);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteStaffLocation")]
        public async Task<IActionResult> DeleteStaffLocation(int id)
        {
            var data = await unitOfWork.ADMEmployeeLocation.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Administration - Staff Marital Status]
        [HttpGet]
        [Route("GetMaritalStatusList/{id}")]
        public async Task<IActionResult> GetMaritalStatusList(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.ADMEmployeeMaritalStatus.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetMaritalStatus/{id}")]
        public async Task<IActionResult> GetMaritalStatus(int id)
        {
            var data = await unitOfWork.ADMEmployeeMaritalStatus.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddMaritalStatus")]
        public async Task<IActionResult> AddMaritalStatus(ADMEmployeeMaritalStatus dept)
        {
            var data = await unitOfWork.ADMEmployeeMaritalStatus.AddAsync(dept);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateMaritalStatus/{id}")]
        public async Task<IActionResult> UpdateMaritalStatus(int id, ADMEmployeeMaritalStatus dept)
        {
            var data = await unitOfWork.ADMEmployeeMaritalStatus.UpdateAsync(id, dept);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteMaritalStatus")]
        public async Task<IActionResult> DeleteMaritalStatus(int id)
        {
            var data = await unitOfWork.ADMEmployeeMaritalStatus.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Administration - Staff Title]
        [HttpGet]
        [Route("GetStaffTitles/{id}")]
        public async Task<IActionResult> GetStaffTitles(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.ADMEmployeeTitle.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetStaffTitle/{id}")]
        public async Task<IActionResult> GetStaffTitle(int id)
        {
            var data = await unitOfWork.ADMEmployeeTitle.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddStaffTitle")]
        public async Task<IActionResult> AddStaffTitle(ADMEmployeeTitle dept)
        {
            var data = await unitOfWork.ADMEmployeeTitle.AddAsync(dept);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateStaffTitle/{id}")]
        public async Task<IActionResult> UpdateStaffTitle(int id, ADMEmployeeTitle dept)
        {
            var data = await unitOfWork.ADMEmployeeTitle.UpdateAsync(id, dept);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteStaffTitle")]
        public async Task<IActionResult> DeleteStaffTitle(int id)
        {
            var data = await unitOfWork.ADMEmployeeTitle.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

    }
}
