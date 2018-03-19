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
using FileUpload.Services.Security;

namespace RockStarLab.Web.Controllers.Api
{
    [Route("api/file")]
    public class FileUploadController : Controller
    {
        static IAmazonS3 client;
        private IPrincipal _principal;

        private static readonly ILog log = LogManager.GetLogger(typeof(FileUploadController));
        FileUploadService fileService = new FileUploadService();

        string serverFileName = string.Empty;

        private IFileUploadService _fileService;

        [HttpPost("upload/{accountId}")]
        public async Task<HttpResponseMessage> UploadAsync(int accountId)
        {
            try
            {
                if (accountId == 0)
                {
                    accountId = _principal.Identity.GetCurrentUser().Id;
                }

                ItemResponse<int> response = new ItemResponse<int>();
                System.Web.HttpPostedFile postedFile = HttpContext.Current.Request.Files[0];
                FileUploadAddRequest model = new FileUploadAddRequest
                {
                    FileName = postedFile.FileName,
                    Size = postedFile.ContentLength,
                    Type = postedFile.ContentType,
                    ModifiedBy = HttpContext.Current.User.Identity.IsAuthenticated ? HttpContext.Current.User.Identity.Name : "anonymous",
                    AccountId = accountId
                };
                string contentType = Request.Content.Headers.ContentType.MediaType;

                serverFileName = string.Format("{0}_{1}{2}",
                    Path.GetFileNameWithoutExtension(postedFile.FileName),
                    Guid.NewGuid().ToString(),
                    Path.GetExtension(postedFile.FileName));

                model.SystemFileName = serverFileName;

                // Saving File to AWS S3 Folder
                try
                {
                    TransferUtility fileTransferUtility = new TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.USWest2));
                    TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();

                    string keyName = serverFileName;
                    string bucketName = "ed-projects";

                    request.BucketName = bucketName;
                    request.Key = keyName;
                    request.InputStream = postedFile.InputStream;
                    fileTransferUtility.Upload(request);
                }
                catch (Exception ex)
                {
                    log.Error("File Upload to AWS Failed!", ex);
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }

                response.Item = await fileService.Insert(model);

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                log.Error("Error Uploading File", ex);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpGet("getall")]
        public HttpResponseMessage GetAll()
        {
            try
            {
                ItemsResponse<UploadedFile> response = new ItemsResponse<UploadedFile>();
                response.Items = fileService.GetAll();
                ResolveFilesUrl(response);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                log.Error("Error Getting All Files", ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpGet("getall/{accountId}")]
        public HttpResponseMessage GetByAccountId(int accountId)
        {
            try
            {
                ItemsResponse<UploadedFile> response = new ItemsResponse<UploadedFile>();
                response.Items = fileService.GetByAccountId(accountId);
                ResolveFilesUrl(response);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                log.Error("Error Getting All Files", ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPut("update")]
        public HttpResponseMessage Update(FileUploadUpdate model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.ModifiedBy = "a7295eb3-eaee-4093-90ca-e4586b170420";
                    _fileService.Update(model);

                    SuccessResponse resp = new SuccessResponse();

                    return Request.CreateResponse(HttpStatusCode.OK, resp);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error Updating File", ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPut("update/profileimg")]
        public HttpResponseMessage UpdateProfilePhoto(ProfilePhotoUpdate model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    _fileService.UpdateProfilePhoto(model);

                    SuccessResponse resp = new SuccessResponse();

                    return Request.CreateResponse(HttpStatusCode.OK, resp);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error Updating User Profile Photo", ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpGet("{id}")]
        public HttpResponseMessage GetById(int id)
        {
            try
            {
                ItemResponse<UploadedFile> response = new ItemResponse<UploadedFile>();
                response.Item = fileService.GetById(id);
                ResolveFileUrl(response);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                log.Error("Error Getting File", ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        private void ResolveFilesUrl(ItemsResponse<UploadedFile> response)
        {
            try
            {
                string serverPath = _configServices.GetConfigValueByName("AWS:BaseURL").ConfigValue;
                foreach (UploadedFile file in response.Items)
                {
                    string filePath = Path.Combine(serverPath, file.SystemFileName);
                    file.SystemFileName = filePath;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error Returning File Path on AWS", ex);
            }
        }

        private void ResolveFileUrl(ItemResponse<UploadedFile> response)
        {
            try
            {
                string serverPath = _configServices.GetConfigValueByName("AWS:BaseURL").ConfigValue;
                string filePath = Path.Combine(serverPath, response.Item.SystemFileName);
                response.Item.SystemFileName = filePath;
            }
            catch (Exception ex)
            {
                log.Error("Error Returning File Path on AWS", ex);
            }
        }

        [HttpDelete("delete/{fileId}/{accountId}")]
        public HttpResponseMessage Delete(int fileId, int accountId)
        {
            try
            {
                ItemResponse<UploadedFile> response = new ItemResponse<UploadedFile>();
                response.Item = fileService.Delete(fileId, accountId);
                DeleteFile(response.Item);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                log.Error("Error Deleting File", ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        private void DeleteFile(UploadedFile uploadedFile)
        {
            using (client = new AmazonS3Client(Amazon.RegionEndpoint.USWest2))
            {
                string bucketName = "ed-projects";
                DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = uploadedFile.SystemFileName
                };
                try
                {
                    client.DeleteObject(deleteObjectRequest);
                }
                catch (AmazonS3Exception s3Exception)
                {
                    log.Error("Error Deleting File From AWS", s3Exception);
                    Console.WriteLine(s3Exception.Message, s3Exception.InnerException);
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
