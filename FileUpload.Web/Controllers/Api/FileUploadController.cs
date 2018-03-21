using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FileUpload.Models.Response;
using FileUpload.Services;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using FileUpload.Models.Request;
using FileUpload.Models.Domain;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;
using log4net;
using FileUpload.Services.Interfaces;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;

namespace RockStarLab.Web.Controllers.Api
{
    [Route("api/file")]
    public class FileUploadController : Controller
    {
        static IAmazonS3 client;
        IFileUploadService _fileService;
        string serverFileName = string.Empty;

        FileUploadService fileService = new FileUploadService();


        [HttpGet("test")]
        public IActionResult Get(){
            return Ok("File ApI working");
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAsync(IFormFile files)
        {
            try
            {
                ItemResponse<int> response = new ItemResponse<int>();

                FileUploadAddRequest model = new FileUploadAddRequest();

                model.FileName = files.FileName;
                model.Size = (int)files.Length;
                model.Type = files.ContentType;
                model.ModifiedBy = "anonymous";

                serverFileName = string.Format("{0}_{1}{2}",
                    Path.GetFileNameWithoutExtension(files.FileName),
                    Guid.NewGuid().ToString(),
                    Path.GetExtension(files.FileName));

                model.SystemFileName = serverFileName;

                // Saving File to AWS S3 Folder
                try
                {
                    TransferUtility fileTransferUtility = new TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.USWest1));
                    TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();

                    string keyName = serverFileName;
                    string bucketName = "ed-projects";

                    request.BucketName = bucketName;
                    request.Key = keyName;
                    request.InputStream = files.OpenReadStream();
                    fileTransferUtility.Upload(request);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

                response.Item = await fileService.Insert(model);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            try
            {
                ItemListResponse<UploadedFile> response = new ItemListResponse<UploadedFile>();
                response.Items = fileService.GetAll();
                ResolveFilesUrl(response);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public IActionResult Update(FileUploadUpdate model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.ModifiedBy = "a7295eb3-eaee-4093-90ca-e4586b170420";
                    _fileService.Update(model);

                    SuccessResponse resp = new SuccessResponse();

                    return Ok(resp);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                ItemResponse<UploadedFile> response = new ItemResponse<UploadedFile>();
                response.Item = fileService.GetById(id);
                ResolveFileUrl(response);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        IActionResult ResolveFilesUrl(ItemListResponse<UploadedFile> response)
        {
            try
            {
                string serverPath = "https://s3-us-west-1.amazonaws.com/ed-projects/";
                foreach (UploadedFile file in response.Items)
                {
                    string filePath = Path.Combine(serverPath, file.SystemFileName);
                    file.SystemFileName = filePath;
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        IActionResult ResolveFileUrl(ItemResponse<UploadedFile> response)
        {
            try
            {
                string serverPath = "https://s3-us-west-1.amazonaws.com/ed-projects/";
                string filePath = Path.Combine(serverPath, response.Item.SystemFileName);
                response.Item.SystemFileName = filePath;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpDelete("delete/{fileId}")]
        public async Task<IActionResult> Delete(int fileId)
        {
            try
            {
                ItemResponse<UploadedFile> response = new ItemResponse<UploadedFile>();
                response.Item = fileService.Delete(fileId);
                await DeleteFile(response.Item);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        async Task<IActionResult> DeleteFile(UploadedFile uploadedFile)
        {

            using (client = new AmazonS3Client(Amazon.RegionEndpoint.USWest1))
            {
                string bucketName = "ed-projects";
                Amazon.S3.Model.DeleteObjectRequest deleteObjectRequest = new Amazon.S3.Model.DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = uploadedFile.SystemFileName
                };

                try
                {
                    await client.DeleteObjectAsync(deleteObjectRequest);
                    return Ok("File Deleted");
                }
                catch (AmazonS3Exception s3Exception)
                {
                    Console.WriteLine(s3Exception.Message, s3Exception.InnerException);
                    return BadRequest(s3Exception);
                }
            }
        }

        public FileUploadController(IFileUploadService FileService)
        {
            _fileService = FileService;
        }
    }
}
