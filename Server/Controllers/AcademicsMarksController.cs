using Microsoft.AspNetCore.Mvc;
using WebAppAcademics.Server.Interfaces;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicsMarksController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        SwitchModel _switch = new SwitchModel();

        public AcademicsMarksController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region [Academics - Cognitive Marks]
        [HttpGet]
        [Route("GetCognitiveMarks/{id}/{termid}/{schid}/{classid}/{stdid}/{staffid}/{subjectid}")]
        public async Task<IActionResult> GetCognitiveMarks(int id, int termid, int schid, int classid, int stdid, int staffid, int subjectid)
        {
            _switch.SwitchID = id;
            _switch.TermID = termid;
            _switch.SchID = schid;           
            _switch.ClassID = classid;
            _switch.STDID = stdid;
            _switch.StaffID = staffid;
            _switch.SubjectID = subjectid;
            var data = await unitOfWork.CognitiveMarkEntry.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetCognitiveMark/{id}")]
        public async Task<IActionResult> GetCognitiveMark(int id)
        {
            var data = await unitOfWork.CognitiveMarkEntry.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddCognitiveMark")]
        public async Task<IActionResult> AddCognitiveMark(ACDStudentsMarksCognitive model)
        {
            var data = await unitOfWork.CognitiveMarkEntry.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateCognitiveMark/{id}")]
        public async Task<IActionResult> UpdateCognitiveMark(int id, ACDStudentsMarksCognitive model)
        {
            var data = await unitOfWork.CognitiveMarkEntry.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteCognitiveMark")]
        public async Task<IActionResult> DeleteCognitiveMark(int id)
        {
            var data = await unitOfWork.CognitiveMarkEntry.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - Other Marks]
        [HttpGet]
        [Route("GetOtherMarks/{id}/{termid}/{schid}/{classid}/{sbjclassid}/{subjectid}/{stdid}/{staffid}")]
        public async Task<IActionResult> GetOtherMarks(int id, int termid, int schid, int classid, int sbjclassid, int subjectid, int stdid, int staffid)
        {
            _switch.SwitchID = id;
            _switch.TermID = termid;
            _switch.SchID = schid;
            _switch.ClassID = classid;
            _switch.SbjClassID = sbjclassid;
            _switch.SubjectID = subjectid;
            _switch.STDID = stdid;
            _switch.StaffID = staffid;            
            var data = await unitOfWork.OtherMarksEntry.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetOtherMark/{id}")]
        public async Task<IActionResult> GetOtherMark(int id)
        {
            var data = await unitOfWork.OtherMarksEntry.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddOtherMark")]
        public async Task<IActionResult> AddOtherMark(ACDStudentsMarksAssessment model)
        {
            var data = await unitOfWork.OtherMarksEntry.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateOtherMark/{id}")]
        public async Task<IActionResult> UpdateOtherMark(int id, ACDStudentsMarksAssessment model)
        {
            var data = await unitOfWork.OtherMarksEntry.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteOtherMark")]
        public async Task<IActionResult> DeleteOtherMark(int id)
        {
            var data = await unitOfWork.OtherMarksEntry.DeleteAsync(id);
            return Ok(data);
        }
        #endregion


    }
}
