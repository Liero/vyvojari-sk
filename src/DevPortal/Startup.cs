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
using DevPortal.Web.AppCode.Config;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using DevPortal.Web.AppCode.Extensions;
using DevPortal.CommandStack.Infrastructure;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Mvc;
using DevPortal.Web.AppCode.EventSourcing;
using DevPortal.Web.AppCode.Authorization;
using Microsoft.AspNetCore.Authorization;
using DevPortal.Web.AppCode.Authorization.Handlers;

namespace DevPortal.Web
{
    public class Startup
    {
        private IHostingEnvironment _env;

        public static IConfiguration Configuration { get; private set; }

        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            this._env = env;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            bool useInMemoryDatabase = Configuration.GetValue<bool>("UseInMemoryDatabase");

            void AddDbContext<T>(ServiceLifetime lifetime) where T : DbContext
            {
                services.AddDbContext<T>(options =>
                {
                    if (useInMemoryDatabase) options.UseInMemoryDatabase(typeof(T).Name);
                    else options.UseSqlServer(Configuration.GetConnectionString(typeof(T).Name));
                }, lifetime, ServiceLifetime.Singleton);
            }

            AddDbContext<ApplicationDbContext>(ServiceLifetime.Scoped);
            AddDbContext<DevPortalDbContext>(ServiceLifetime.Scoped);
            AddDbContext<EventsDbContext>(ServiceLifetime.Scoped);

            services.AddIdentity<ApplicationUser, IdentityRole>(ConfigureIdentity)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var authenticationBuilder = services.AddAuthentication();
            if (!string.IsNullOrEmpty(Configuration["Authentication:Facebook:AppId"]))
            {
                authenticationBuilder.AddFacebook(options =>
                    {
                        options.AppId = Configuration["Authentication:Facebook:AppId"];
                        options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                    });
            }
            services.ConfigureApplicationCookie(c => c.AccessDeniedPath = null);

            services.AddAuthorization(Policies.Configure)
                .AddAuthorizationHandlers(this.GetType().Assembly, ServiceLifetime.Scoped);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddMvc().AddMvcOptions(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
                options.Filters.Add(new RequireHttpsAttribute());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddTransient<IEmailSender, EmailSender>();

            services.Configure<Imgur>(Configuration.GetSection("Imgur"));
            services.AddSingleton<IImageStore, ImgurImageStore>();

            services.Configure<ReCaptcha>(Configuration.GetSection("ReCaptcha"));
            services.AddSingleton<ValidateReCaptchaAttribute>();
            if (useInMemoryDatabase)
            {
                services.AddInMemoryEventSourcing();
            }
            else
            {
                services.AddSqlEventSourcing();
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            app.UseRewriter(new RewriteOptions().AddRedirectToHttps());
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRebus();

            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
            app.UseStaticFiles();

            UserRolesConfig.EnsureUserRoles(app.ApplicationServices).Wait();
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
