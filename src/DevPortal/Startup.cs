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
using DevPortal.Web.Models;
using Microsoft.EntityFrameworkCore;
using DevPortal.Web.Data;
using DevPortal.Web.Services;
using DevPortal.Web.AppCode.Startup;
using DevPortal.QueryStack;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Facebook;
using DevPortal.Web.AppCode.Config;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DevPortal.Web
{
    public class Startup
    {
        private IHostingEnvironment _env;

        public static IConfigurationRoot Configuration { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            this._env = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("ApplicationDbContext"), ServiceLifetime.Transient);

            services.AddDbContext<DevPortalDbContext>(options =>
                options.UseInMemoryDatabase("DevPortalDbContext"), ServiceLifetime.Transient);

            //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(ConfigureIdentity)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication()
                .AddFacebook(options =>
                {
                    options.AppId = Configuration["Authentication:Facebook:AppId"];
                    options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddMvc().AddMvcOptions(opt => opt.Filters.Add<GlobalExceptionFilter>());

            services.AddTransient<IEmailSender, EmailSender>();

            services.Configure<Imgur>(Configuration.GetSection("Imgur"));
            services.AddSingleton<IImageStore, ImgurImageStore>();

            services.AddEventSourcing();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
            }

            app.UseAuthentication();
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
