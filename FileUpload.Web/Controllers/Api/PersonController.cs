using System;
using FileUpload.Models.Domain;
using FileUpload.Models.Response;
using Microsoft.AspNetCore.Mvc;
using FileUpload.Services;
using FileUpload.Models.Request;

namespace FileUpload.Web.Controllers.Api
{
    [Route("api/person")]
    public class PersonController : Controller
    {

        [HttpGet("getall")]
        public IActionResult Get()
        {
            try
            {
                PeopleService svc = new PeopleService();
                ItemListResponse<People> response = new ItemListResponse<People>();
                response.Items = svc.GetAll();
                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("getbyid")]
        public IActionResult GetById()
        {
            //return Request.CreateResponse(HttpStatusCode.OK, "You called Get By Id!");
            ItemResponse<string> response = new ItemResponse<string>();
            response.Item = "You called Get By Id";
            return Ok(response);

        }

        [HttpPost]
        public IActionResult Post([FromBody]PeopleAddRequest model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.ModifiedBy = "me";
                    PeopleService svc = new PeopleService();
                    ItemResponse<int> response = new ItemResponse<int>();
                    response.Item = svc.Insert(model);
                    return Ok(response);

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

        public void Put(int id, [FromBody]string value)
        {
        }

        public void Delete(int id)
        {
        }
    }
}
