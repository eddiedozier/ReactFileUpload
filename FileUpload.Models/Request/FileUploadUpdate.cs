using System;
using System.ComponentModel.DataAnnotations;

namespace FileUpload.Models.Request
{
    public class FileUploadUpdate
    {
        [Range(1, 100000000, ErrorMessage = "File Id to cannot be 0 or is too long, update model requirements")]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Range(1, 1000000000, ErrorMessage = "Category Id out of Range")]
        public int CategoryId { get; set; }

        public string ModifiedBy { get; set; }
    }
}
