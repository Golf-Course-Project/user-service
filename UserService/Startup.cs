using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using UserService.Helpers;
using UserService.Repos.Identity;
using UserService.Misc;
using UserService.Data;
using UserService.Repos;
using Microsoft.Extensions.Caching.Memory;

namespace UserService
{
    public class Startup
    {
        public IConfiguration _configuration { get; }

        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors(c => { c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin()); });
            services.AddHttpContextAccessor();

            //api versioning service
            //services.AddApiVersioning();

            //services.AddApiVersioning(config =>
            //{
            //    config.DefaultApiVersion = new ApiVersion(1, 0);
            //    config.AssumeDefaultVersionWhenUnspecified = true;
            //    config.ReportApiVersions = true;
            //});

            services.AddMemoryCache(); // Add this line for .NET 2.1

            // configure DI for application services
            services.Configure<AppSettings>(_configuration.GetSection("AppSettings"));

            services.AddSingleton<IStandardHelper, StandardHelper>();       
            services.AddTransient<IIdentityRepo, IdentityRepo>();
            services.AddTransient<IAvatarRepo, AvatarRepo>();
            services.AddTransient<ITokenAuthorization, TokenAuthorization>();

            services.AddScoped<TokenAuthorizationActionFilter>();

            // configure strongly typed settings objects
            IConfigurationSection appSettingsSection = _configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);                        
            AppSettings appSettings = appSettingsSection.Get<AppSettings>();

            // look for connection string from azure app settings first, if empty, then pull from appsettings.json
            string idenityConnectionString = string.IsNullOrEmpty(_configuration.GetConnectionString("IdentityServiceConnectionString")) ? _configuration.GetSection("AppSettings").Get<AppSettings>().IdentityServiceConnectionString : _configuration.GetConnectionString("IdentityServiceConnectionString");

            services.AddDbContext<IdentityDataContext>(options => options.UseSqlServer(idenityConnectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            // global cors policy
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
