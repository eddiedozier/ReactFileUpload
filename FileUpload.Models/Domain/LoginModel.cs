using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FileUpload.Models.Domain
{
    public class LoginModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        public string Salt { get; set; }

        public List<RolesModel> RolesList { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
