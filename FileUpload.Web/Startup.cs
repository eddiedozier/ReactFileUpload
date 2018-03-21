using System;
using System.IO;
using Amazon.S3;
using FileUpload.Data.Interfaces;
using FileUpload.Data.Providers;
using FileUpload.Services;
using FileUpload.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileUpload
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public string connectionString;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables();
            Configuration = builder.Build();

            connectionString = Configuration.GetConnectionString("DefaultConnection");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", "AKIAJVHD4YWMC32ZDQCQ");
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", "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");
            Environment.SetEnvironmentVariable("AWS_REGION", "us-west-1");
            
            services.AddDataProtection()
                .UseCryptographicAlgorithms(
                new AuthenticatedEncryptorConfiguration()
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                })
                .SetDefaultKeyLifetime(TimeSpan.FromDays(7));
            services.AddMvc();
            services.AddAWSService<IAmazonS3>();

            // Registering Components
            services.AddTransient<IPeopleService, PeopleService>();
            services.AddTransient<IFileUploadService, FileUploadService>();

            // Per Request
            services.AddTransient<IDataProvider>(s => new SqlDataProvider(connectionString));
            
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
