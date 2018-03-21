using System;
using System.ComponentModel.DataAnnotations;

namespace FileUpload.Models.Request
{
    public class FileUploadAddRequest
    {
        [Required]
        public string FileName { get; set; }

        [Range(1, 1073741824, ErrorMessage = "File Size To Large, Must be 1GB or less")]
        public int Size { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string SystemFileName { get; set; }

        public string ModifiedBy { get; set; }

    }
}
