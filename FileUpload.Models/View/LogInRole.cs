using System;
using System.Collections.Generic;
using FileUpload.Models.Domain;

namespace FileUpload.Models.View
{
    public class LogInRole
    {
        public bool IsSuccessful { get; set; }
        public List<RolesModel> RolesList { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
