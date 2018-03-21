using System;
namespace FileUpload.Models.Domain
{
    public class UploadedFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public int Size { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string SystemFileName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Modifiedby { get; set; }
    }
}
