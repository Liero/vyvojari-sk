using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using DevPortal.Web.Models;
using Microsoft.EntityFrameworkCore;
using DevPortal.Web.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DevPortal.Web.Services;

namespace DevPortal.Web
{
    public class Startup
    {
        private IHostingEnvironment _env;

        public static IConfigurationRoot Configuration { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            this._env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase());
            //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(ConfigureIdentity)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();
            services.AddTransient<IEmailSender, EmailSender>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var builder = new ConfigurationBuilder()
                  .SetBasePath(env.ContentRootPath)
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            loggerFactory.AddConsole();

            app.UseDeveloperExceptionPage();
            if (env.IsDevelopment())
            {
                builder.AddUserSecrets();
                app.UseBrowserLink();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            app.UseIdentity();
            var facebookOptions = new FacebookOptions()
            {
                AppId = Configuration["Authentication:Facebook:AppId"],
                AppSecret = Configuration["Authentication:Facebook:AppSecret"],
            };
            app.UseFacebookAuthentication(facebookOptions);


            app.UseMvcWithDefaultRoute();
            app.UseStaticFiles();
        }

        private void ConfigureIdentity(IdentityOptions options)
        {
            options.Password.RequireUppercase = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            options.SignIn.RequireConfirmedEmail = false; //todo

            if (_env.IsDevelopment())
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 1;
            }
        }
    }

}
