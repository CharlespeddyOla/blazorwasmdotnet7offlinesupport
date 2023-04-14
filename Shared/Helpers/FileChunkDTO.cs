using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppAcademics.Shared.Helpers
{
    public class FileChunkDTO
    {
        public string FileName { get; set; } = "";
        public string FolderName { get; set; } = "";
        public long Offset { get; set; }
        public byte[] Data { get; set; }
        public bool FirstChunk = false;
        public string LicenseKey { get; set; } = "";
        public bool LicenseVerified { get; set; } = false;
    }
}
