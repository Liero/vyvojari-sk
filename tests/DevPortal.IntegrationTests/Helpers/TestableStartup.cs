using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevPortal.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DevPortal.IntegrationTests.Helpers
{
    public class TestableStartup : DevPortal.Web.Startup
    {
        public TestableStartup(IHostingEnvironment env, IConfiguration configuration) : base(env, configuration)
        {
        }

        protected override void AddAuthentication(IApplicationBuilder app)
        {
            app.UseMiddleware<AuthenticationMiddewareMock>();
        }
    }
}
