using Microsoft.AspNetCore.Mvc.Rendering;
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
    [HtmlTargetElement("content-editor", Attributes = "asp-for", TagStructure = TagStructure.WithoutEndTag)]
    public class ContentEditorTagHelper : TagHelper
    {
        private HtmlHelper _htmlHelper;
        private HtmlEncoder _htmlEncoder;

        public ContentEditorTagHelper(IHtmlHelper htmlHelper, HtmlEncoder htmlEncoder)
        {
            _htmlHelper = htmlHelper as HtmlHelper;
            _htmlEncoder = htmlEncoder;
        }

        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        [ViewContext]
        public ViewContext ViewContext
        {
            set => _htmlHelper.Contextualize(value);
        }


        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            var partial = await _htmlHelper.PartialAsync(
               "TagHelpers/ContentEditor",
                For);

            var writer = new StringWriter();
            partial.WriteTo(writer, _htmlEncoder);

            output.Content.SetHtmlContent(writer.ToString());
        }
    }
}
