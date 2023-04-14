using Microsoft.AspNetCore.Mvc;
using WebAppAcademics.Server.Interfaces;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.Subjects;

namespace WebAppAcademics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicsSubjectsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        SwitchModel _switch = new SwitchModel();

        public AcademicsSubjectsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region [Academics - Subjects]
        [HttpGet]
        [Route("GetSubjects/{id}/{schid}/{sbjdeptid}/{sbjclassid}/{subjectstatus}")]
        public async Task<IActionResult> GetSubjects(int id, int schid, int sbjdeptid, int sbjclassid, bool subjectstatus)
        {
            _switch.SwitchID = id;
            _switch.SchID = schid;
            _switch.SbjDeptID = sbjdeptid;
            _switch.SbjClassID = sbjclassid;
            _switch.SubjectStatus = subjectstatus;
            var data = await unitOfWork.Subjects.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetSubject/{id}")]
        public async Task<IActionResult> GetSubject(int id)
        {
            var data = await unitOfWork.Subjects.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddSubject")]
        public async Task<IActionResult> AddSubject(ACDSubjects model)
        {
            var data = await unitOfWork.Subjects.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateSubject/{id}")]
        public async Task<IActionResult> UpdateSubject(int id, ACDSubjects model)
        {
            var data = await unitOfWork.Subjects.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteSubject")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var data = await unitOfWork.Subjects.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - Subjects Classification]
        [HttpGet]
        [Route("GetSubjectsClassifications/{id}")]
        public async Task<IActionResult> GetSubjectsClassifications(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.SubjectsClassification.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetSubjectsClassification/{id}")]
        public async Task<IActionResult> GetSubjectsClassification(int id)
        {
            var data = await unitOfWork.SubjectsClassification.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddSubjectsClassification")]
        public async Task<IActionResult> AddSubjectsClassification(ACDSbjClassification model)
        {
            var data = await unitOfWork.SubjectsClassification.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateSubjectsClassification/{id}")]
        public async Task<IActionResult> UpdateSubjectsClassification(int id, ACDSbjClassification model)
        {
            var data = await unitOfWork.SubjectsClassification.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteSubjectsClassification")]
        public async Task<IActionResult> DeleteSubjectsClassification(int id)
        {
            var data = await unitOfWork.SubjectsClassification.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - Subjects Department]
        [HttpGet]
        [Route("GetDepartments/{id}")]
        public async Task<IActionResult> GetDepartments(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.SubjectsDepartment.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetDepartment/{id}")]
        public async Task<IActionResult> GetDepartment(int id)
        {
            var data = await unitOfWork.SubjectsDepartment.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddDepartment")]
        public async Task<IActionResult> AddDepartment(ACDSbjDept model)
        {
            var data = await unitOfWork.SubjectsDepartment.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateDepartment/{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, ACDSbjDept model)
        {
            var data = await unitOfWork.SubjectsDepartment.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteDepartment")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var data = await unitOfWork.SubjectsDepartment.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - Student Subjects Allocation]
        [HttpGet]
        [Route("GetStudentAllocations/{id}/{schsession}/{sbjselection}/{schid}/{classlistid}/{classid}/{subjectid}/{stdid}")]
        public async Task<IActionResult> GetStudentAllocations(int id, int schsession, bool sbjselection, int schid, int classlistid, 
                                                                int classid, int subjectid, int stdid)
        {
            _switch.SwitchID = id;
            _switch.SchSession = schsession;
            _switch.SbjSelection = sbjselection;
            _switch.SchID  = schid;
            _switch.ClassListID = classlistid;
            _switch.ClassID = classid;
            _switch.SubjectID = subjectid;
            _switch.STDID = stdid;
            var data = await unitOfWork.StudentSubjectsAllocation.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetStudentAllocation/{id}")]
        public async Task<IActionResult> GetStudentAllocation(int id)
        {
            var data = await unitOfWork.StudentSubjectsAllocation.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddStudentAllocation")]
        public async Task<IActionResult> AddStudentAllocation(ACDSbjAllocationStudents model)
        {
            var data = await unitOfWork.StudentSubjectsAllocation.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateStudentAllocation/{id}")]
        public async Task<IActionResult> UpdateStudentAllocation(int id, ACDSbjAllocationStudents model)
        {
            var data = await unitOfWork.StudentSubjectsAllocation.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteStudentAllocation")]
        public async Task<IActionResult> DeleteStudentAllocation(int id)
        {
            var data = await unitOfWork.StudentSubjectsAllocation.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - Teacher Subjects Allocation]
        [HttpGet]
        [Route("GetTeacherAllocations/{id}/{sbjselection}/{termid}/{schid}/{classlistid}/{classid}/{subjectid}/{staffid}")]
        public async Task<IActionResult> GetTeacherAllocations(int id, bool sbjselection, int termid, int schid, int classlistid, int classid, int subjectid, int staffid)
        {
            _switch.SwitchID = id;
            _switch.SbjSelection = sbjselection;
            _switch.TermID = termid;            
            _switch.SchID = schid;
            _switch.ClassListID = classlistid;
            _switch.ClassID = classid;
            _switch.SubjectID = subjectid;
            _switch.StaffID = staffid;
            var data = await unitOfWork.TeacherSubjectsAllocation.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetTeacherAllocation/{id}")]
        public async Task<IActionResult> GetTeacherAllocation(int id)
        {
            var data = await unitOfWork.TeacherSubjectsAllocation.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddTeacherAllocation")]
        public async Task<IActionResult> AddTeacherAllocation(ACDSbjAllocationTeachers model)
        {
            var data = await unitOfWork.TeacherSubjectsAllocation.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateTeacherAllocation/{id}")]
        public async Task<IActionResult> UpdateTeacherAllocation(int id, ACDSbjAllocationTeachers model)
        {
            var data = await unitOfWork.TeacherSubjectsAllocation.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteTeacherAllocation")]
        public async Task<IActionResult> DeleteTeacherAllocation(int id)
        {
            var data = await unitOfWork.TeacherSubjectsAllocation.DeleteAsync(id);
            return Ok(data);
        }
        #endregion


    }
}
