using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode.Startup
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var telemetry = new TelemetryClient();
            var properties = new Dictionary<string, string> { { "Source", nameof(GlobalExceptionFilter) } };
            telemetry.TrackException(context.Exception, properties);
        }
    }
}
