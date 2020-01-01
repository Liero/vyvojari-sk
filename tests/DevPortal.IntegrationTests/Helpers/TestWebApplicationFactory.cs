using DevPortal.Web;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DevPortal.IntegrationTests.Helpers
{
    
    class TestWebApplicationFactory : WebApplicationFactory<Startup>
    {
        public TestWebApplicationFactory()
        {
            ClientOptions.BaseAddress = new Uri("https://localhost:5001/");
            ClientOptions.AllowAutoRedirect = false;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder<TestableStartup>(new string[0]);
        }
    }
}
