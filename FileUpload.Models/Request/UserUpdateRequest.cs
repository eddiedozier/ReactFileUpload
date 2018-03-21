using System;
namespace FileUpload.Models.Request
{
    public class UserUpdateRequest : UserAddRequest
    {
        public int Id { get; set; }
    }
}
