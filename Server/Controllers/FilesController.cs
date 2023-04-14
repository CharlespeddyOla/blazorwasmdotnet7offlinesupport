using Microsoft.AspNetCore.Mvc;
using WebAppAcademics.Shared.Helpers;

namespace WebAppAcademics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        [HttpGet]
        [Route("GetSqlFilePath/{filename}")]
        public async Task<IActionResult> GetSqlFilePath(string filename)
        {
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "SQLFiles");
            var fullPath = Path.Combine(directoryPath, filename);

            await Task.CompletedTask;

            return Ok(fullPath);
        }

        [HttpGet]
        [Route("GetResultsFilePath/{filename}")]
        public async Task<IActionResult> GetResultsFilePath(string filename)
        {
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Results");
            var fullPath = Path.Combine(directoryPath, filename);

            await Task.CompletedTask;

            return Ok(fullPath);
        }

        [HttpPost]
        [Route("UploadFileChunk")]
        public bool UploadFileChunk([FromBody] FileChunkDTO fileChunkDto)
        {
            try
            {
                string folderName = Path.Combine("StaticFiles", fileChunkDto.FolderName);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                string fileName = Path.Combine(filePath, fileChunkDto.FileName);
                          
                // delete the file if necessary
                if (fileChunkDto.FirstChunk && System.IO.File.Exists(fileName))
                    System.IO.File.Delete(fileName);

                // open for writing
                using (var stream = System.IO.File.OpenWrite(fileName))
                {
                    stream.Seek(fileChunkDto.Offset, SeekOrigin.Begin);
                    stream.Write(fileChunkDto.Data, 0, fileChunkDto.Data.Length);
                }

                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }
        } //UploadFileChunk


        [HttpGet]
        [Route("GetFiles/{foldername}/{searchpatern}")]
        public List<string> GetFiles(string foldername, string searchpatern)
        {
            //SearchPater => *.* => All Files
            var result = new List<string>();
            var files = Directory.GetFiles(Environment.CurrentDirectory + "\\StaticFiles\\" + foldername, searchpatern);

            foreach (var file in files)
            {
                var justTheFileName = Path.GetFileName(file);
                result.Add($"{justTheFileName}");
                //result.Add($"files/{justTheFileName}");
            }

            return result;
        } //GetFiles

        [HttpGet]
        [Route("GetPDFResult/{fileame}")]
        public IActionResult GetPDFResult(string fileame)
        {
            string folderName = Path.Combine("StaticFiles", "Results");
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            string fileName = Path.Combine(filePath, fileame);

            byte[] pdfBytes = System.IO.File.ReadAllBytes(fileName);
            MemoryStream ms = new MemoryStream(pdfBytes);

            return new FileStreamResult(ms, "application/pdf");
        }

    }
}
