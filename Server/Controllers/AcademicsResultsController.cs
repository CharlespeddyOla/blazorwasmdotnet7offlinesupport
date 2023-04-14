using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebAppAcademics.Server.Interfaces;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicsResultsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        SwitchModel _switch = new SwitchModel();

        public AcademicsResultsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region [Academics - Cognitive Results]
        [HttpGet]
        [Route("GetCognitiveResults/{id}")]
        public async Task<IActionResult> GetCognitiveResults(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.CognitiveResults.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetCognitiveResult/{id}")]
        public async Task<IActionResult> GetCognitiveResult(int id)
        {
            var data = await unitOfWork.CognitiveResults.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddCognitiveResult")]
        public async Task<IActionResult> AddCognitiveResult(ACDStudentsResultCognitive model)
        {
            var data = await unitOfWork.CognitiveResults.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateCognitiveResult/{id}")]
        public async Task<IActionResult> UpdateCognitiveResult(int id, ACDStudentsResultCognitive model)
        {
            var data = await unitOfWork.CognitiveResults.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteCognitiveResult/{id}")]
        public async Task<IActionResult> DeleteCognitiveResult(int id)
        {
            var data = await unitOfWork.CognitiveResults.DeleteAsync(id);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetCount/{id}/{stdid}")]
        public async Task<IActionResult> GetCount(int id, int stdid)
        {
            _switch.SwitchID = id;
            _switch.STDID= stdid;
            var data = await unitOfWork.CognitiveResults.CountAsync(_switch);
            return Ok(data);
        }
        #endregion

        #region [Academics - Other Marks Results]
        [HttpGet]
        [Route("GetOtherMarksResults/{id}")]
        public async Task<IActionResult> GetOtherMarksResults(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.OtherMarksResults.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpPost]
        [Route("AddOtherMarksResults")]
        public async Task<IActionResult> AddOtherMarksResults(ACDStudentsResultAssessmentBool model)
        {
            var data = await unitOfWork.OtherMarksResults.AddAsync(model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteOtherMarksResult/{id}")]
        public async Task<IActionResult> DeleteOtherMarksResult(int id)
        {
            var data = await unitOfWork.OtherMarksResults.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - Broadsheet]
        [HttpGet]
        [Route("GetBroadSheet")]
        public async Task<List<dynamic>> GetBroadSheet()
        {
            return await unitOfWork.BroadSheet.GetAllAsync();
        }

        [HttpGet]
        [Route("GetFieldNames")]
        public async Task<List<string>> GetFieldNames()
        {
            return await unitOfWork.BroadSheet.GetFieldNamesAsync();
        }

        [HttpPost]
        [Route("ExecuteScript")]
        public async Task<IActionResult> ExecuteScript(ACDBroadSheet model)
        {
            var data = await unitOfWork.BroadSheet.ExecuteScriptAsync(model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteBroadSheet/{id}")]
        public async Task<IActionResult> DeleteBroadSheet(int id)
        {
            var data = await unitOfWork.BroadSheet.DeleteAsync(id);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateBroadSheet/{id}")]
        public async Task<IActionResult> UpdateBroadSheet(int id, ACDBroadSheet model)
        {
            var data = await unitOfWork.BroadSheet.UpdateAsync(model);
            return Ok(data);
        }

        #endregion


    }
}
