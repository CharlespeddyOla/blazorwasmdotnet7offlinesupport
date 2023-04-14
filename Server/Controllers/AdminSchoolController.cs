using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebAppAcademics.Server.Interfaces;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Administration.School;

namespace WebAppAcademics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminSchoolController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        SwitchModel _switch = new SwitchModel();

        public AdminSchoolController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region [Administration - School]
        [HttpGet]
        [Route("GetSchools/{id}")]
        public async Task<IActionResult> GetSchools(int id)        
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.ADMSchlList.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetSchool/{id}")]
        public async Task<IActionResult> GetSchool(int id)
        {
            var data = await unitOfWork.ADMSchlList.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddSchool")]
        public async Task<IActionResult> AddSchool(ADMSchlList school)
        {
            var data = await unitOfWork.ADMSchlList.AddAsync(school);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateSchool/{id}")]
        public async Task<IActionResult> UpdateSchool(int id, ADMSchlList school)
        {
            var data = await unitOfWork.ADMSchlList.UpdateAsync(id, school);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteSchool")]
        public async Task<IActionResult> DeleteSchool(int id)
        {
            var data = await unitOfWork.ADMSchlList.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Administration - Class List]
        [HttpGet]
        [Route("GetClassList/{id}/{schid}/{classlistid}")]
        public async Task<IActionResult> GetClassList(int id, int schid, int classlistid)
        {
            _switch.SwitchID = id;
            _switch.SchID = schid;
            _switch.ClassListID = classlistid;
            var data = await unitOfWork.ADMSchClassList.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetClass/{id}")]
        public async Task<IActionResult> GetClass(int id)
        {
            var data = await unitOfWork.ADMSchClassList.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddClass")]
        public async Task<IActionResult> AddClass(ADMSchClassList _class)
        {
            var data = await unitOfWork.ADMSchClassList.AddAsync(_class);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateClass/{id}")]
        public async Task<IActionResult> UpdateClass(int id, ADMSchClassList _class)
        {
            var data = await unitOfWork.ADMSchClassList.UpdateAsync(id, _class);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteClass")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var data = await unitOfWork.ADMSchClassList.DeleteAsync(id);
            return Ok(data);
        }

        
        #endregion

        #region [Administration - Class Group]
        [HttpGet]
        [Route("GetClassGroups/{id}/{schid}")]
        public async Task<IActionResult> GetClassGroups(int id, int schid)
        {
            _switch.SwitchID = id;
            _switch.SchID = schid;
            var data = await unitOfWork.ADMSchClassGroup.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetClassGroup/{id}")]
        public async Task<IActionResult> GetClassGroup(int id)
        {
            var data = await unitOfWork.ADMSchClassGroup.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddClassGroup")]
        public async Task<IActionResult> AddClassGroup(ADMSchClassGroup catname)
        {
            var data = await unitOfWork.ADMSchClassGroup.AddAsync(catname);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateClassGroup/{id}")]
        public async Task<IActionResult> UpdateClassGroup(int id, ADMSchClassGroup _class)
        {
            var data = await unitOfWork.ADMSchClassGroup.UpdateAsync(id, _class);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteClassGroup")]
        public async Task<IActionResult> DeleteClassGroup(int id)
        {
            var data = await unitOfWork.ADMSchClassGroup.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Administration - Class Category]
        [HttpGet]
        [Route("GetCategories/{id}")]
        public async Task<IActionResult> GetCategories(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.ADMSchClassCategory.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetCategory/{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var data = await unitOfWork.ADMSchClassCategory.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddCategory")]
        public async Task<IActionResult> AddCategory(ADMSchClassCategory catname)
        {
            var data = await unitOfWork.ADMSchClassCategory.AddAsync(catname);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateCategory/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, ADMSchClassCategory _class)
        {
            var data = await unitOfWork.ADMSchClassCategory.UpdateAsync(id, _class);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var data = await unitOfWork.ADMSchClassCategory.DeleteAsync(id);
            return Ok(data);
        }

        #endregion

        #region [Administration - Class Discipline]
        [HttpGet]
        [Route("GetDisciplines/{id}")]
        public async Task<IActionResult> GetDisciplines(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.ADMSchClassDiscipline.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetDiscipline/{id}")]
        public async Task<IActionResult> GetDiscipline(int id)
        {
            var data = await unitOfWork.ADMSchClassDiscipline.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddDiscipline")]
        public async Task<IActionResult> AddDiscipline(ADMSchClassDiscipline catname)
        {
            var data = await unitOfWork.ADMSchClassDiscipline.AddAsync(catname);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateDiscipline/{id}")]
        public async Task<IActionResult> UpdateDiscipline(int id, ADMSchClassDiscipline _class)
        {
            var data = await unitOfWork.ADMSchClassDiscipline.UpdateAsync(id, _class);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteDiscipline")]
        public async Task<IActionResult> DeleteDiscipline(int id)
        {
            var data = await unitOfWork.ADMSchClassDiscipline.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Administration - Previous School]
        [HttpGet]
        [Route("GetPreviousSchools/{id}")]
        public async Task<IActionResult> GetPreviousSchools(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.ADMSchEducationInstitute.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetPreviousSchool/{id}")]
        public async Task<IActionResult> GetPreviousSchool(int id)
        {
            var data = await unitOfWork.ADMSchEducationInstitute.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddPreviousSchool")]
        public async Task<IActionResult> AddPreviousSchool(ADMSchEducationInstitute catname)
        {
            var data = await unitOfWork.ADMSchEducationInstitute.AddAsync(catname);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdatePreviousSchool/{id}")]
        public async Task<IActionResult> UpdatePreviousSchool(int id, ADMSchEducationInstitute _class)
        {
            var data = await unitOfWork.ADMSchEducationInstitute.UpdateAsync(id, _class);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeletePreviousSchool")]
        public async Task<IActionResult> DeletePreviousSchool(int id)
        {
            var data = await unitOfWork.ADMSchEducationInstitute.DeleteAsync(id);
            return Ok(data);
        }
        #endregion
    }
}
