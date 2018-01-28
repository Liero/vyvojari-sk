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
    [HtmlTargetElement("span", Attributes = "avatar-for-user")]
    [HtmlTargetElement("div", Attributes = "avatar-for-user")]
    [HtmlTargetElement("avatar", Attributes = "username")]
    public class AvatarTagHelper : TagHelper
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccesor;
        private static readonly string[] Colors = new[]{
            "#6E398D","#C31A7F","#424F9B","#1D96BB","#8CBD3F","#FDC70F","#EC6224"
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
        public string UserName { get => AvatarForUser ?? "?"; set => AvatarForUser = value; }


        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccesor.ActionContext);
            var user = await _userManager.FindByNameAsync(UserName);

            output.TagMode = TagMode.StartTagAndEndTag;
            if (context.TagName == "avatar")
            {
                output.TagName = "a";
            }
            if (!output.Attributes.ContainsName("class"))
            {
                output.Attributes.Add("class", "avatar");
            }           
            if (!output.Attributes.ContainsName("title"))
            {
                output.Attributes.Add("title", UserName + "\n" + user?.ShortDescription);
            }
            output.Attributes.RemoveAll("avatar-for-user");

            string background = GetBackgroundColor(UserName);
            string content = $"<span>{UserName.Substring(0, 1).ToUpper()}</span>"
                + $"<span class=\"cover\" style=\"background-image:url('{user?.AvatarUrl}')\"></span>";
            

            output.Content.SetHtmlContent(content);
            output.Attributes.Add("style", $"background:{background}");

        }

        private string GetBackgroundColor(string userName)
        {
            int hashCode = userName.ToLower().GetHashCode() & 0xfffffff; //positive hash
            return Colors[hashCode % Colors.Length];
        }

    }
}
