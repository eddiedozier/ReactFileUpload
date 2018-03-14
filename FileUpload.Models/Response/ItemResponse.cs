using System;
namespace FileUpload.Models.Response
{
    public class ItemResponse<T> : SuccessResponse
    {
        public T Item { get; set; }
    }
}
