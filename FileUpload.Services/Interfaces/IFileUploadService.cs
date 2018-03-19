using System.Collections.Generic;
using System.Threading.Tasks;
using FileUpload.Models.Domain;
using FileUpload.Models.Request;

namespace FileUpload.Services.Interfaces
{
    public interface IFileUploadService
    {
        UploadedFile Delete(int fileId, int accountId);
        List<UploadedFile> GetAll();
        UploadedFile GetById(int id);
        Task<int> Insert(FileUploadAddRequest model);
        void Update(FileUploadUpdate model);
    }
}