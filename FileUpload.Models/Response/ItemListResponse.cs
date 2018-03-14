using System;
using System.Collections.Generic;

namespace FileUpload.Models.Response
{
    public class ItemListResponse<T> : SuccessResponse
    {
        public List<T> Items { get; set; }
    }
}
