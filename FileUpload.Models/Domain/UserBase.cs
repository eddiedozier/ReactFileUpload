using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FileUpload.Models.Interfaces;

namespace FileUpload.Models.Domain
{
    public class UserBase : IUserAuthData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool EmailConfirmed { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
