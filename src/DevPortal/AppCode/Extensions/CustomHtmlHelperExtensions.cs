using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode.Extensions
{
    public static class CustomHtmlHelperExtensions
    {
        public static string DisplayAge(this IHtmlHelper html, DateTime date)
        {
            var age = DateTime.UtcNow - date.ToUniversalTime();
            if (age.TotalMinutes < 1) return $"{ age.Minutes} s";
            if (age.TotalHours < 1) return $"{age.Minutes} min";
            if (age.TotalDays < 1) return $"{age.Hours} h";
            if (age.TotalDays < 30) return $"{age.Days} d";
            else return date.ToString("d");
        }
    }
}
