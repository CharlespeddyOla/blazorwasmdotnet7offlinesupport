using Microsoft.AspNetCore.Mvc;
using WebAppAcademics.Server.Interfaces;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicsResultsCommentsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        SwitchModel _switch = new SwitchModel();

        public AcademicsResultsCommentsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region [Academics - Mid Term Results Comments]
        [HttpGet]
        [Route("GetMidTermComments/{id}/{termid}/{classid}/{stdid}")]
        public async Task<IActionResult> GetMidTermComments(int id, int termid, int classid, int stdid)
        {
            _switch.SwitchID = id;
            _switch.TermID = termid;
            _switch.ClassID = classid;
            _switch.STDID = stdid;
            var data = await unitOfWork.MidTermComments.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetMidTermComment/{id}")]
        public async Task<IActionResult> GetMidTermComment(int id)
        {
            var data = await unitOfWork.MidTermComments.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddMidTermComment")]
        public async Task<IActionResult> AddMidTermComment(ACDReportCommentMidTerm model)
        {
            var data = await unitOfWork.MidTermComments.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateMidTermComment/{id}")]
        public async Task<IActionResult> UpdateMidTermComment(int id, ACDReportCommentMidTerm model)
        {
            var data = await unitOfWork.MidTermComments.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteMidTermComment")]
        public async Task<IActionResult> DeleteMidTermComment(int id)
        {
            var data = await unitOfWork.MidTermComments.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - Term End Results Comments]
        [HttpGet]
        [Route("GetTermEndComments/{id}/{termid}/{classid}/{stdid}")]
        public async Task<IActionResult> GetTermEndComments(int id, int termid, int classid, int stdid)
        {
            _switch.SwitchID = id;
            _switch.TermID = termid;
            _switch.ClassID = classid;
            _switch.STDID = stdid;
            var data = await unitOfWork.TermEndComments.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetTermEndComment/{id}")]
        public async Task<IActionResult> GetTermEndComment(int id)
        {
            var data = await unitOfWork.TermEndComments.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddTermEndComment")]
        public async Task<IActionResult> AddTermEndComment(ACDReportCommentsTerminal model)
        {
            var data = await unitOfWork.TermEndComments.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateTermEndComment/{id}")]
        public async Task<IActionResult> UpdateTermEndComment(int id, ACDReportCommentsTerminal model)
        {
            var data = await unitOfWork.TermEndComments.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteTermEndComment")]
        public async Task<IActionResult> DeleteTermEndComment(int id)
        {
            var data = await unitOfWork.TermEndComments.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - Check Point / IGCSE Results Comments]
        [HttpGet]
        [Route("GetCheckPointIGCSEComments/{id}/{termid}/{classid}/{stdid}")]
        public async Task<IActionResult> GetCheckPointIGCSEComments(int id, int termid, int classid, int stdid)
        {
            _switch.SwitchID = id;
            _switch.TermID = termid;
            _switch.ClassID = classid;
            _switch.STDID = stdid;
            var data = await unitOfWork.CheckPointIGCSEComments.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetCheckPointIGCSEComment/{id}")]
        public async Task<IActionResult> GetCheckPointIGCSEComment(int id)
        {
            var data = await unitOfWork.CheckPointIGCSEComments.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddCheckPointIGCSEComment")]
        public async Task<IActionResult> AddCheckPointIGCSEComment(ACDReportCommentCheckPointIGCSE model)
        {
            var data = await unitOfWork.CheckPointIGCSEComments.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateCheckPointIGCSEComment/{id}")]
        public async Task<IActionResult> UpdateCheckPointIGCSEComment(int id, ACDReportCommentCheckPointIGCSE model)
        {
            var data = await unitOfWork.CheckPointIGCSEComments.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteCheckPointIGCSEComment")]
        public async Task<IActionResult> DeleteCheckPointIGCSEComment(int id)
        {
            var data = await unitOfWork.CheckPointIGCSEComments.DeleteAsync(id);
            return Ok(data);
        }
        #endregion
    }
}
