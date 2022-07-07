using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ridwan.Service.Util
{
    public class FileUploadRequest
    {
        public string FileName { get; set; }
        public IFormFile FileReference { get; set; }
        public string FileExtension { get; set; }
        public bool IsAdmin { get; set; }
        public long FileSize { get; set; }

        public static FileUploadRequest UploadFile(HttpRequest request)
        {
            var file = request.Form.Files.FirstOrDefault();
            if (file == null)
                throw new AppException("No File Uploaded!");

            string extension = Path.GetExtension(file.FileName).Replace(".", string.Empty).ToLower();

            if (file.Length > 5000000)
                throw new AppException("Please upload a file less than 5MB!");

            return new FileUploadRequest
            {
                FileName = file.FileName.Split('.')[0],
                FileExtension = Path.GetExtension(file.FileName).Replace(".", string.Empty).ToLower(),
                FileReference = file,
                FileSize = file.Length
            };
        }
    }
}
