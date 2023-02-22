using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace SShop.Services.FileStorage
{
    public interface IFileStorageService
    {
        string GetFileUrl(string fileName);

        Task<string> SaveFile(IFormFile image);

        Task<string> ConfirmSave(Stream stream, string fileName);

        Task DeleteFile(string fileName);
    }
}