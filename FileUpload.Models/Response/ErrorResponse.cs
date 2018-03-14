using System;
namespace FileUpload.Models.Response
{
    public class ErrorResponse : Response
    {
        public ErrorResponse()
        {
            this.IsSuccessful = false;
        }
    }
}
