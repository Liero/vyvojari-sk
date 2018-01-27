using DevPortal.Web.Models;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DevPortal.Web.TagHelpers
{

    [HtmlTargetElement("pagination", Attributes = "asp-for")]
    public class PaginationTagHelper : TagHelper
    {
        private readonly IActionContextAccessor _actionContextAccessor;
        private HtmlHelper _htmlHelper;
        private HtmlEncoder _htmlEncoder;

        public PaginationTagHelper(IHtmlHelper htmlHelper, HtmlEncoder htmlEncoder, IActionContextAccessor actionContextAccessor)
        {
            _htmlHelper = htmlHelper as HtmlHelper;
            _htmlEncoder = htmlEncoder as HtmlEncoder;
            _actionContextAccessor = actionContextAccessor;
        }

        [HtmlAttributeName("asp-for")]
        public PaginationViewModelBase For { get; set; }

        [ViewContext]
        public ViewContext ViewContext
        {
            set { _htmlHelper.Contextualize(value); }
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            var partial = await _htmlHelper.PartialAsync(
               "TagHelpers/Pagination",
                For);

            var writer = new StringWriter();
            partial.WriteTo(writer, _htmlEncoder);

            output.Content.SetHtmlContent(writer.ToString());
        }

    }
}
