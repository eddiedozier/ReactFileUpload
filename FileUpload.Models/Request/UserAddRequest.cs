using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FileUpload.Models.Request
{
    public class UserAddRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool EmailConfirmed { get; set; }

        public string ModifiedBy { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
