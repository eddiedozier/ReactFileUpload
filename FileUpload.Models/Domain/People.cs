using System;
using FileUpload.Models.Request;

namespace FileUpload.Models.Domain
{
    public class People : PeopleUpdateRequest
    {
        public DateTime DateTime { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
