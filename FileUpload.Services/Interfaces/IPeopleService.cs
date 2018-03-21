using System.Collections.Generic;
using FileUpload.Models.Domain;
using FileUpload.Models.Request;

namespace FileUpload.Services.Interfaces
{
    public interface IPeopleService
    {
        List<People> GetAll();
        int Insert(PeopleAddRequest model);
    }
}