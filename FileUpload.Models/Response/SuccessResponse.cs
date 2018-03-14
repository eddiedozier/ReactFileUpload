using System;
namespace FileUpload.Models.Response
{
    public class SuccessResponse : Response
    {
       public SuccessResponse()
        {
            this.IsSuccessful = true;
        }
    }
}
