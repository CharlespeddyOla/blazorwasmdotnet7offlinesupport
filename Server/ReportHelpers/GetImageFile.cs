using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppAcademics.Server.ReportHelpers
{
    public class GetImageFile
    {

        private readonly string fileName;

        public GetImageFile(string fileName)
        {
            this.fileName = fileName;
        }

        public byte[] ImageFile()
        {
            string folderName = Path.Combine("StaticFiles", "Images");
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            string fullFilePath = Path.Combine(filePath, fileName);

            Byte[] byteData;

            byteData = System.IO.File.ReadAllBytes(fullFilePath);

            return byteData;
        }
    }
}
