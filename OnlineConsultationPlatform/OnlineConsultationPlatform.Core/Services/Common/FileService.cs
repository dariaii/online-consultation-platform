using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace OnlineConsultationPlatform.Core.Services
{
    public interface IFileService
    {
        Task<string> UploadProfilePicture(IFormFile file);

        Task<bool> UploadReportFile(IFormFile file, string? fileName);

        byte[]? GetReportFile(string fileName);
    }

    public class FileService(IWebHostEnvironment webHostEnvironment) : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

        public async Task<string> UploadProfilePicture(IFormFile file)
        {
            if (file != null)
            {
                string dirPath = Path.Combine(_webHostEnvironment.WebRootPath, "profile-pictures");

                if (file.Length > 0)
                {
                    string filePath = Path.Combine(dirPath, file.FileName);
                    using Stream fileStream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(fileStream);

                    return file.FileName;
                }
            }

            return null;
        }

        public async Task<bool> UploadReportFile(IFormFile file, string? fileName)
        {
            if (file != null && fileName != null)
            {
                string dirPath = Path.Combine(_webHostEnvironment.WebRootPath, "reports");

                if (file.Length > 0)
                {
                    string filePath = Path.Combine(dirPath, fileName);
                    using Stream fileStream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(fileStream);

                    return true;
                }
            }

            return false;
        }

        public byte[]? GetReportFile(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string fileFullPath = Path.Combine(_webHostEnvironment.WebRootPath, "reports/", fileName);

                var file = File.ReadAllBytes(fileFullPath);

                return file;
            }

            return null;
        }
    }
}
