using System;
using FileUpload.Models.Request;
using FileUpload.Models.View;

namespace FileUpload.Services.Interfaces
{
    public interface IUserService
    {
        void UpdatePassById(UserUpdateRequest model);
        int InsertNewUser(RegisterAddModel model);
        LogInRole LogIn(string email, string password);
        bool LogInTest(string email, string password, int id, string[] roles = null);
    }
}
