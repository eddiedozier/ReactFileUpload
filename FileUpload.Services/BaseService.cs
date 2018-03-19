using System;
using System.IO;
using FileUpload.Data.Interfaces;
using FileUpload.Data.Providers;
using Microsoft.Extensions.Configuration;

namespace FileUpload.Services
{
    public abstract class BaseService
    {
        public IDataProvider DataProvider { get; set; }
        public IConfigurationRoot Configuration { get; }

        public BaseService()
        {
            var builder = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json")
                     .AddEnvironmentVariables();
            Configuration = builder.Build();

            string connStr = Configuration.GetConnectionString("DefaultConnection");
            this.DataProvider = new SqlDataProvider(connStr);
        }

        public BaseService(IDataProvider dataProvider)
        {
            this.DataProvider = dataProvider;
        }
       
    }
}
