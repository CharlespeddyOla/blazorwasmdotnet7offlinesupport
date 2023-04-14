using Microsoft.AspNetCore.Mvc;
using WebAppAcademics.Server.Interfaces;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Financials.Banks;

namespace WebAppAcademics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialsBanksController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        SwitchModel _switch = new SwitchModel();

        public FinancialsBanksController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region [Financials - Bank Type]
        [HttpGet]
        [Route("GetBankTypeList/{id}")]
        public async Task<IActionResult> GetBankTypeList(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.FINBankAcctType.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetBankType/{id}")]
        public async Task<IActionResult> GetBankType(int id)
        {
            var data = await unitOfWork.FINBankAcctType.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddBankType")]
        public async Task<IActionResult> AddBankType(FINBankAcctType model)
        {
            var data = await unitOfWork.FINBankAcctType.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateBankType/{id}")]
        public async Task<IActionResult> UpdateBankType(int id, FINBankAcctType model)
        {
            var data = await unitOfWork.FINBankAcctType.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteBankType")]
        public async Task<IActionResult> DeleteBankType(int id)
        {
            var data = await unitOfWork.FINBankAcctType.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Financials - Bank List]
        [HttpGet]
        [Route("GetBanks/{id}/{bnkaccttypeID}")]
        public async Task<IActionResult> GetBanks(int id, int bnkaccttypeID)
        {
            _switch.SwitchID = id;
            _switch.BnkAcctTypeID = bnkaccttypeID;
            var data = await unitOfWork.FINBankDetails.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetBank/{id}")]
        public async Task<IActionResult> GetBank(int id)
        {
            var data = await unitOfWork.FINBankDetails.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddBank")]
        public async Task<IActionResult> AddBank(FINBankDetails model)
        {
            var data = await unitOfWork.FINBankDetails.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateBank/{id}")]
        public async Task<IActionResult> UpdateBank(int id, FINBankDetails model)
        {
            var data = await unitOfWork.FINBankDetails.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteBank")]
        public async Task<IActionResult> DeleteBank(int id)
        {
            var data = await unitOfWork.FINBankDetails.DeleteAsync(id);
            return Ok(data);
        }
        #endregion
    }
}
