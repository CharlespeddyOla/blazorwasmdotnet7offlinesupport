using Microsoft.AspNetCore.Mvc;
using WebAppAcademics.Server.Interfaces;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.CBT;
using WebAppAcademics.Shared.Models.Academics.Marks;

namespace WebAppAcademics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicsCBTController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        SwitchModel _switch = new SwitchModel();

        public AcademicsCBTController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region [Academics - CBT Exam Type]
        [HttpGet]
        [Route("GetCBTExamTypes/{id}")]
        public async Task<IActionResult> GetCBTExamTypes(int id)
        {
            _switch.SwitchID = id;;
            var data = await unitOfWork.CBTExamType.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetCBTExamType/{id}")]
        public async Task<IActionResult> GetCBTExamType(int id)
        {
            var data = await unitOfWork.CBTExamType.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddCBTExamType")]
        public async Task<IActionResult> AddCBTExamType(CBTExamType model)
        {
            var data = await unitOfWork.CBTExamType.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateCBTExamType/{id}")]
        public async Task<IActionResult> UpdateCBTExamType(int id, CBTExamType model)
        {
            var data = await unitOfWork.CBTExamType.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteCBTExamType")]
        public async Task<IActionResult> DeleteCBTExamType(int id)
        {
            var data = await unitOfWork.CBTExamType.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - CBT Exams]
        [HttpGet]
        [Route("GetCBTExams/{id}/{termid}/{classlistid}/{staffid}")]
        public async Task<IActionResult> GetCBTExams(int id, int termid, int classlistid, int staffid)
        {
            _switch.SwitchID = id;
            _switch.TermID = termid;
            _switch.ClassListID = classlistid;
            _switch.StaffID = staffid;
            var data = await unitOfWork.CBTExams.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetCBTExam/{id}")]
        public async Task<IActionResult> GetCBTExam(int id)
        {
            var data = await unitOfWork.CBTExams.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddCBTExam")]
        public async Task<IActionResult> AddCBTExam(CBTExams model)
        {
            var data = await unitOfWork.CBTExams.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateCBTExam/{id}")]
        public async Task<IActionResult> UpdateCBTExam(int id, CBTExams model)
        {
            var data = await unitOfWork.CBTExams.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteCBTExam")]
        public async Task<IActionResult> DeleteCBTExam(int id)
        {
            var data = await unitOfWork.CBTExams.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - CBT Exam Question Type]
        [HttpGet]
        [Route("GetCBTExamQuestionTypes/{id}")]
        public async Task<IActionResult> GetCBTExamQuestionTypes(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.CBTExamQuestionType.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetCBTExamQuestionType/{id}")]
        public async Task<IActionResult> GetCBTExamQuestionType(int id)
        {
            var data = await unitOfWork.CBTExamQuestionType.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddCBTExamQuestionType")]
        public async Task<IActionResult> AddCBTExamQuestionType(CBTQuestionType model)
        {
            var data = await unitOfWork.CBTExamQuestionType.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateCBTExamQuestionType/{id}")]
        public async Task<IActionResult> UpdateCBTExamQuestionType(int id, CBTQuestionType model)
        {
            var data = await unitOfWork.CBTExamQuestionType.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteCBTExamQuestionType")]
        public async Task<IActionResult> DeleteCBTExamQuestionType(int id)
        {
            var data = await unitOfWork.CBTExamQuestionType.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - CBT Exam Questions]
        [HttpGet]
        [Route("GetCBTExamQuestions/{id}/{examid}")]
        public async Task<IActionResult> GetCBTExamQuestions(int id, int examid)
        {
            _switch.SwitchID = id;
            _switch.ExamID = examid;
            var data = await unitOfWork.CBTExamQuestions.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetCBTExamQuestion/{id}")]
        public async Task<IActionResult> GetCBTExamQuestion(int id)
        {
            var data = await unitOfWork.CBTExamQuestions.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddCBTExamQuestion")]
        public async Task<IActionResult> AddCBTExamQuestion(CBTQuestions model)
        {
            var data = await unitOfWork.CBTExamQuestions.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateCBTExamQuestion/{id}")]
        public async Task<IActionResult> UpdateCBTExamQuestion(int id, CBTQuestions model)
        {
            var data = await unitOfWork.CBTExamQuestions.UpdateAsync(id, model);           
           return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteCBTExamQuestion")]
        public async Task<IActionResult> DeleteCBTExamQuestion(int id)
        {
            var data = await unitOfWork.CBTExamQuestions.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - CBT Exam Answers]
        [HttpGet]
        [Route("GetCBTExamAnswers/{id}/{examid}")]
        public async Task<IActionResult> GetCBTExamAnswers(int id, int examid)
        {
            _switch.SwitchID = id;
            _switch.ExamID = examid;
            var data = await unitOfWork.CBTExamAnswers.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetCBTExamAnswer/{id}")]
        public async Task<IActionResult> GetCBTExamAnswer(int id)
        {
            var data = await unitOfWork.CBTExamAnswers.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddCBTExamAnswer")]
        public async Task<IActionResult> AddCBTExamAnswer(CBTAnswers model)
        {
            var data = await unitOfWork.CBTExamAnswers.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateCBTExamAnswer/{id}")]
        public async Task<IActionResult> UpdateCBTExamAnswer(int id, CBTAnswers model)
        {
            var data = await unitOfWork.CBTExamAnswers.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteCBTExamAnswer")]
        public async Task<IActionResult> DeleteCBTExamAnswer(int id)
        {
            var data = await unitOfWork.CBTExamAnswers.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - CBT Exam Student Answers]
        [HttpGet]
        [Route("GetCBTStudentAnswers/{id}/{examid}/{stdid}/{cbttouse}")]
        public async Task<IActionResult> GetCBTStudentAnswers(int id, int examid, int stdid, bool cbttouse)
        {
            _switch.SwitchID = id;
            _switch.ExamID = examid;
            _switch.STDID = stdid;
            _switch.CBTToUse = cbttouse;
            var data = await unitOfWork.CBTExamStudentAnswers.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetCBTStudentAnswer/{id}")]
        public async Task<IActionResult> GetCBTStudentAnswer(int id)
        {
            var data = await unitOfWork.CBTExamStudentAnswers.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddCBTStudentAnswer")]
        public async Task<IActionResult> AddCBTStudentAnswer(CBTStudentAnswers model)
        {
            var data = await unitOfWork.CBTExamStudentAnswers.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateCBTStudentAnswer/{id}")]
        public async Task<IActionResult> UpdateCBTStudentAnswer(int id, CBTStudentAnswers model)
        {
            var data = await unitOfWork.CBTExamStudentAnswers.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteCBTStudentAnswer")]
        public async Task<IActionResult> DeleteCBTStudentAnswer(int id)
        {
            var data = await unitOfWork.CBTExamStudentAnswers.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - CBT Exam Student Score]
        [HttpGet]
        [Route("GetCBTStudentScores/{id}/{examid}/{stdid}/{cbttouse}")]
        public async Task<IActionResult> GetCBTStudentScores(int id, int examid, int stdid, bool cbttouse)
        {
            _switch.SwitchID = id;
            _switch.ExamID = examid;
            _switch.STDID = stdid;
            _switch.CBTToUse = cbttouse;
            var data = await unitOfWork.CBTExamStudentScore.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetCBTStudentScore/{id}")]
        public async Task<IActionResult> GetCBTStudentScore(int id)
        {
            var data = await unitOfWork.CBTExamStudentScore.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddCBTStudentScore")]
        public async Task<IActionResult> AddCBTStudentScore(CBTStudentScores model)
        {
            var data = await unitOfWork.CBTExamStudentScore.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateCBTStudentScore/{id}")]
        public async Task<IActionResult> UpdateCBTStudentScore(int id, CBTStudentScores model)
        {
            var data = await unitOfWork.CBTExamStudentScore.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteCBTStudentScore")]
        public async Task<IActionResult> DeleteCBTStudentScore(int id)
        {
            var data = await unitOfWork.CBTExamStudentScore.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - CBT Exam Latex]
        [HttpGet]
        [Route("GetLatexList/{id}")]
        public async Task<IActionResult> GetLatexList(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.CBTExamLatex.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetLatex/{id}")]
        public async Task<IActionResult> GetLatex(int id)
        {
            var data = await unitOfWork.CBTExamLatex.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddLatex")]
        public async Task<IActionResult> AddLatex(CBTLatex model)
        {
            var data = await unitOfWork.CBTExamLatex.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateLatex/{id}")]
        public async Task<IActionResult> UpdateLatex(int id, CBTLatex model)
        {
            var data = await unitOfWork.CBTExamLatex.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpDelete]
        [Route("DeleteLatexe")]
        public async Task<IActionResult> DeleteLatexe(int id)
        {
            var data = await unitOfWork.CBTExamLatex.DeleteAsync(id);
            return Ok(data);
        }
        #endregion

        #region [Academics - CBT Connection Info]
        [HttpGet]
        [Route("GetConnectionInfo/{id}")]
        public async Task<IActionResult> GetConnectionInfo(int id)
        {
            _switch.SwitchID = id;
            var data = await unitOfWork.CBTConnectionInfo.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetConnectionById/{id}")]
        public async Task<IActionResult> GetConnectionById(int id)
        {
            var data = await unitOfWork.CBTConnectionInfo.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateConnectionInfo/{id}")]
        public async Task<IActionResult> UpdateConnectionInfo(int id, CBTConnectionInfo model)
        {
            var data = await unitOfWork.CBTConnectionInfo.UpdateAsync(id, model);
            return Ok(data);
        }

        #endregion

        #region [Academics - CBT Exam Taken Flags]
        [HttpGet]
        [Route("GetExamTakenFlags/{id}/{termid}/{classid}/{stdid}")]
        public async Task<IActionResult> GetExamTakenFlags(int id, int termid, int classid, int stdid)
        {
            _switch.SwitchID = id;
            _switch.TermID = termid;
            _switch.ClassID = classid;
            _switch.STDID = stdid;
            var data = await unitOfWork.CBTExamTaken.GetAllAsync(_switch);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetExamTakenFlag/{id}")]
        public async Task<IActionResult> GetExamTakenFlag(int id)
        {
            var data = await unitOfWork.CBTExamTaken.GetByIdAsync(id);
            if (data == null) return Ok();
            return Ok(data);
        }

        [HttpPost]
        [Route("AddExamTakenFlag")]
        public async Task<IActionResult> AddExamTakenFlag(CBTExamTakenFlags model)
        {
            var data = await unitOfWork.CBTExamTaken.AddAsync(model);
            return Ok(data);
        }

        [HttpPut]
        [Route("UpdateExamTakenFlag/{id}")]
        public async Task<IActionResult> UpdateExamTakenFlag(int id, CBTExamTakenFlags model)
        {
            var data = await unitOfWork.CBTExamTaken.UpdateAsync(id, model);
            return Ok(data);
        }

        [HttpGet]
        [Route("Search/{id}/{termid}/{stdid}/{examid}/{flag}")]
        public async Task<IActionResult> Search(int id, int termid, int stdid, int examid, bool flag)
        {
            _switch.SwitchID = id;
            _switch.TermID = termid;
            _switch.STDID = stdid;
            _switch.ExamID = examid;
            _switch.Flag = flag;
            var data = await unitOfWork.CBTExamTaken.SearchAsync(_switch);
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
