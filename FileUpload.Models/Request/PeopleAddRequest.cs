using System;
using System.ComponentModel.DataAnnotations;

namespace FileUpload.Models.Request
{
    public class PeopleAddRequest
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        public char MiddleInitial { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public DateTime DOB { get; set; }

        [MaxLength(128)]
        public string ModifiedBy { get; set; }
    }
}
