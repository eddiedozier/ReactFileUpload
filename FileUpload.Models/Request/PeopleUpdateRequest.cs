using System;
using System.ComponentModel.DataAnnotations;

namespace FileUpload.Models.Request
{
    public class PeopleUpdateRequest : PeopleAddRequest
    {
        [Range(1,1000000000, ErrorMessage = "Id out of Range.")]
        public int Id { get; set; }
    }
}
