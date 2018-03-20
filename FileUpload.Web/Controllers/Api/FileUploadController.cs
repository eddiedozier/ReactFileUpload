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
using Microsoft.Net.Http.Headers;

namespace RockStarLab.Web.Controllers.Api
{
    [Route("api/file")]
    public class FileUploadController : Controller
    {
        static IAmazonS3 client;
        IPrincipal _principal;

        FileUploadService fileService = new FileUploadService();

        string serverFileName = string.Empty;

        IFileUploadService _fileService;

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAsync(IFormFile file)
        {
            try
            {
                ItemResponse<int> response = new ItemResponse<int>();

                //FileDetails fileDetails;
                //using (var reader = new StreamReader(file.OpenReadStream()))
                //{
                //    var fileContent = reader.ReadToEnd();
                //    var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                //    var fileName = parsedContentDisposition.FileName;
                   
                //}

                FileUploadAddRequest model = new FileUploadAddRequest
                {
                    FileName = file.FileName,
                    Size = (int)file.Length,
                    Type = file.ContentType,
                    ModifiedBy = "anonymous"
                };

                //System.Web.HttpPostedFile postedFile = HttpContext.Current.Request.Files[0];

                serverFileName = string.Format("{0}_{1}{2}",
                    Path.GetFileNameWithoutExtension(file.FileName),
                    Guid.NewGuid().ToString(),
                    Path.GetExtension(file.FileName));

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
                    request.InputStream = file.OpenReadStream();
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
                    await client.DeleteAsync(deleteObjectRequest);

                    client.DeleteObject(deleteObjectRequest);
                    return Ok("File Deleted");
                }
                catch (AmazonS3Exception s3Exception)
                {
                    Console.WriteLine(s3Exception.Message, s3Exception.InnerException);
                    return BadRequest(s3Exception);
                }
            }
        }

        public FileUploadController(IFileUploadService FileService, IPrincipal principal)
        {
            _fileService = FileService;
            _principal = principal;
        }
    }
}
