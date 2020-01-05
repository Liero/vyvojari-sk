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
using Microsoft.ApplicationInsights.Extensibility;
using DevPortal.Web.AppCode.Cache;

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

        public virtual void ConfigureServices(IServiceCollection services)
        {
            bool useInMemoryDatabase = Configuration.GetValue<bool>("UseInMemoryDatabase");

            void AddDbContext<T>(ServiceLifetime lifetime) where T : DbContext
            {
                services.AddDbContext<T>(options =>
                {
                    if (useInMemoryDatabase) options.UseInMemoryDatabase(typeof(T).Name);
                    else options.UseSqlServer(Configuration.GetConnectionString(typeof(T).Name));
                }, lifetime, ServiceLifetime.Transient);
            }

            AddDbContext<ApplicationDbContext>(ServiceLifetime.Scoped);
            AddDbContext<DevPortalDbContext>(ServiceLifetime.Scoped);
            AddDbContext<EventsDbContext>(ServiceLifetime.Scoped);

            services.AddIdentity<ApplicationUser, IdentityRole>(options => Configuration.GetSection("IdentityOptions").Bind(options))
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

            services.Configure<AppCode.Config.SendGrid>(Configuration.GetSection("SendGrid"));
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
            services.AddLogging();

            services.AddSingleton<AvatarsCache>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger)
        {
            if (Configuration.GetValue<string>("ApplicationInsights:InstrumentationKey") == string.Empty
                && Configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY") == string.Empty)
            {
                var telemetryConfig = app.ApplicationServices.GetService<TelemetryConfiguration>();
                if (telemetryConfig != null)
                {
                    logger.LogInformation("Disabling Application Insights Telemetry");
                    telemetryConfig.DisableTelemetry = true;
                }
            }

            if (Configuration.GetValue<bool>("UseDeveloperExceptionPage"))
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseRebusEventSourcing();

            AddAuthentication(app);
            app.UseMvcWithDefaultRoute();
            app.UseStaticFiles();

            UserRolesConfig.EnsureUserRoles(app.ApplicationServices).Wait();
        }

        /// <summary>Overridable in interations tests</summary>
        protected virtual void AddAuthentication(IApplicationBuilder app)
        {
            app.UseAuthentication();
        }
    }
}
