using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevPortal.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevPortal.IntegrationTests.Helpers
{
    public class TestableStartup : DevPortal.Web.Startup
    {
        public TestableStartup(IHostingEnvironment env, IConfiguration configuration) : base(env, configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            // fixes 404 problem: https://github.com/aspnet/AspNetCore/issues/8428#issuecomment-480981352
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .ConfigureApplicationPartManager(p =>
                {
                    var assembly = typeof(DevPortal.Web.Startup).Assembly;
                    var partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
                    foreach (var part in partFactory.GetApplicationParts(assembly))
                    {
                        p.ApplicationParts.Add(part);
                    }
                });
        }

        protected override void AddAuthentication(IApplicationBuilder app)
        {
            app.UseMiddleware<AuthenticationMiddewareMock>();
        }
    }
}
