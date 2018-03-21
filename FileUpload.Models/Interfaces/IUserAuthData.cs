using System;
using System.Collections.Generic;

namespace FileUpload.Models.Interfaces
{
    public interface IUserAuthData
    {
        int Id { get; }
        string Name { get; }
        IEnumerable<string> Roles { get; }
    }
}
