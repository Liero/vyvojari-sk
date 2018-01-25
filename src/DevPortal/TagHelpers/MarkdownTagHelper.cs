using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.TagHelpers
{
    [HtmlTargetElement("p", Attributes = "markdown")]
    [HtmlTargetElement("markdown", Attributes= "asp-for")]
    [OutputElementHint("p")]
    public class MarkdownTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (output.TagName == "markdown")
            {
                output.TagName = null;
            }
            output.Attributes.RemoveAll("markdown");


            string markdownContent = (await output.GetChildContentAsync()).GetContent();
            string html = CommonMark.CommonMarkConverter.Convert(markdownContent);
            output.Content.SetHtmlContent(html ?? "");
        }
    }
}
