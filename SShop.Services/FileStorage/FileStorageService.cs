using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace SShop.Services.FileStorage
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _userContent;
        private const string USER_CONTENT_FOLDER = "user-content";

        public FileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _userContent = Path.Combine(webHostEnvironment.WebRootPath, USER_CONTENT_FOLDER);
            if (!Directory.Exists(_userContent))
            {
                Directory.CreateDirectory(_userContent);
            }
        }

        public async Task DeleteFile(string fileName)
        {
            string filePath = Path.Combine(_userContent, Path.GetFileName(fileName));
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }

        public string GetFileUrl(string fileName)
        {
            return $"/{USER_CONTENT_FOLDER}/{fileName}";
        }

        public async Task<string> ConfirmSave(Stream stream, string fileName)
        {
            string filePath = Path.Combine(_userContent, fileName);
            using (var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                await stream.CopyToAsync(fs);
            }
            return GetFileUrl(fileName);
        }

        public async Task<string> SaveFile(IFormFile image)
        {
            string originalFileName = ContentDispositionHeaderValue.Parse(image.ContentDisposition).FileName.Trim('"');
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";

            return await ConfirmSave(image.OpenReadStream(), fileName);
        }
    }
}