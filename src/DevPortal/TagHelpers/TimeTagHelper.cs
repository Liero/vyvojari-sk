using Humanizer;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.TagHelpers
{
    [HtmlTargetElement("span", Attributes = "asp-time")]
    [HtmlTargetElement("time", Attributes = "for")]
    [OutputElementHint("span")]
    public class TimeTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-time")]
        public DateTime Time { get; set; }

        [HtmlAttributeName("for")]
        public DateTime For { get => Time; set => Time = value; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;

            if (output.TagName == "time")
            {
                output.TagName = "span";
                output.Attributes.RemoveAll("for");
            }
            if (!output.Attributes.ContainsName("asp-time"))
            {
                output.Attributes.Add("asp-time", Time);
            }
            if (!output.Attributes.ContainsName("class"))
            {
                output.Attributes.Add("class", "text-nowrap");
            }

            string content = FormatTime(Time);
            output.Content.SetContent(content);
        }

        private string FormatTime(DateTime time)
        {
            var now = DateTime.Now;
            if ((now - Time).TotalSeconds < 5) return "just now";
            if (now.Date == Time.Date)
            {
                return (now - Time).Humanize();
            }
            else if (now.Date == Time.Date.AddDays(1))
            {
                return "yesterday";
            }
            else if (now.Year == Time.Year)
            {
                return Time.ToString("{0:MMM} {0:d}");
            }
            else
            {
                return Time.ToLongDateString();
            }
        }
    }
}
