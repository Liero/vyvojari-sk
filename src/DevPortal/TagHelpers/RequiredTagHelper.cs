using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.TagHelpers
{
    [HtmlTargetElement("input", Attributes = "asp-for")]
    public class RequiredTagHelper : TagHelper
    {
        public override int Order
        {
            get { return int.MaxValue; }
        }

        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (context.AllAttributes["required"] == null)
            {
                var isRequired = GetIsRequired(For.ModelExplorer.Metadata.ValidatorMetadata);
                if (isRequired)
                {
                    var requiredAttribute = new TagHelperAttribute("required");
                    output.Attributes.Add(requiredAttribute);
                }
            }
        }

        private static bool GetIsRequired(IReadOnlyList<object> validatorMetadata)
            =>  validatorMetadata.OfType<RequiredAttribute>().Any();
    }
}
