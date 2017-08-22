using DevPortal.Web.Models;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "avatar-for-user")]
    [HtmlTargetElement("avatar", Attributes = "username")]
    [OutputElementHint("a")]
    public class AvatarTagHelper : TagHelper
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccesor;
        private static readonly string[] Colors = new []{
            "#6E398D","#C31A7F","#424F9B","#1D96BB","#8CBD3F","#FDC70F","#EC6224","#C31A7F","#EC6224"
        };

        public AvatarTagHelper(
            UserManager<ApplicationUser> userManager, 
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccesor)
        {
            _userManager = userManager;
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccesor = actionContextAccesor;
        }

        [HtmlAttributeName("avatar-for-user")]
        public string AvatarForUser { get; set; }

        [HtmlAttributeName("username")]
        public string UserName { get => AvatarForUser; set => AvatarForUser = value; }


        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "a";
            if (!output.Attributes.ContainsName("class"))
            {
                output.Attributes.Add("class", "avatar");
            }
            if (output.TagName == "time")
            {
                output.TagName = "avatar";
                output.Attributes.RemoveAll("avatar-for-user");
            }
            if (string.IsNullOrWhiteSpace(UserName))
            {
                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccesor.ActionContext);
                output.Content.SetHtmlContent($"<img src=\"{urlHelper.Content("~/media/noface.jpg")}\" />");
            }
            else
            {
                var user = await _userManager.FindByNameAsync(UserName);
                string background = GetBackgroundColor(UserName);
                string content = $"<span style=\"background:{background}\">{UserName.Substring(0, 1).ToUpper()}</span>";
                if (!string.IsNullOrEmpty(user?.AvatarUrl))
                {
                    content += $"<img src=\"{user.AvatarUrl}\"/>";
                }
                output.Content.SetHtmlContent(content);
            }
        }

        private string GetBackgroundColor(string userName)
        {
            int hashCode = userName.ToLower().GetHashCode() & 0xfffffff; //positive hash
            return Colors[hashCode % Colors.Length];
        }

    }
}
