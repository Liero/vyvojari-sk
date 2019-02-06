using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DevPortal.IntegrationTests.Helpers
{
    public class AuthenticationMiddewareMock
    {
        public const string TestingAuthenticationType = "TestCookieAuthentication";
        public const string TestingHeaderPrefix = "X-Integration-Testing-";

        private readonly RequestDelegate _next;

        public AuthenticationMiddewareMock(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            /* request headers starting with X-Integration-Testing-
             * e.g. X-Integration-Testing-Name or X-Integration-Testing-Role
             */

            var testingClaimsHeaders = context.Request.Headers
                .Where(h => h.Key.StartsWith(TestingHeaderPrefix))
                .ToArray();

            if (testingClaimsHeaders.Any())
            {
                var claims = testingClaimsHeaders.SelectMany(header =>
                    header.Value.Select(value =>
                        new Claim(header.Key.Substring(TestingHeaderPrefix.Length), value)));

                context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, TestingAuthenticationType));
            }
            await _next(context);
        }
    }
}
