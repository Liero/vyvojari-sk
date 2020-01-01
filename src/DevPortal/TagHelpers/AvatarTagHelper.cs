using DevPortal.Web.AppCode.Cache;
using DevPortal.Web.Controllers;
using DevPortal.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;
using System.Text;

namespace DevPortal.Web.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "avatar-for-user")]
    [HtmlTargetElement("span", Attributes = "avatar-for-user")]
    [HtmlTargetElement("div", Attributes = "avatar-for-user")]
    [HtmlTargetElement("avatar", Attributes = "username")]
    public class AvatarTagHelper : TagHelper
    {
        private readonly AvatarsCache _avatarsCache;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccesor;
        private static readonly string[] Colors = new[]{
            "#6E398D","#C31A7F","#424F9B","#1D96BB","#8CBD3F","#FDC70F","#EC6224"
        };

        public AvatarTagHelper(
            AvatarsCache avatarsCache,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccesor)
        {
            _avatarsCache = avatarsCache;
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccesor = actionContextAccesor;
        }

        [HtmlAttributeName("avatar-for-user")]
        public string AvatarForUser { get; set; }

        [HtmlAttributeName("username")]
        public string UserName { get => AvatarForUser ?? "?"; set => AvatarForUser = value; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            var avatarUrl = _avatarsCache.GetAvatarUrl(UserName) ?? "/media/noface.jpg";
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccesor.ActionContext);

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
                output.Attributes.Add("title", UserName);
            }
            output.Attributes.RemoveAll("avatar-for-user");

            string background = GetBackgroundColor(UserName);

            var content = new StringBuilder();
            if (UserName.Length > 0)
            {
                content.AppendLine($"<span>{UserName.Substring(0, 1).ToUpper()}</span>");
            }

            if (!string.IsNullOrEmpty(avatarUrl)) //must check in order to avoid request to current page (empty url)
            {
                content.AppendLine($"<span class=\"cover\" style=\"background-image:url('{avatarUrl}')\"></span>");
            }

            if (output.TagName == "a" && !output.Attributes.ContainsName("href"))
            {
                var url = urlHelper.Action(nameof(UserController.Detail), "User", new { username = UserName });
                output.Attributes.Add("href", url);
            }

            output.Content.SetHtmlContent(content.ToString());
            output.Attributes.Add("style", $"background:{background}");

        }

        private string GetBackgroundColor(string userName)
        {
            int hashCode = userName.ToLower().GetHashCode() & 0xfffffff; //positive hash
            return Colors[hashCode % Colors.Length];
        }
    }
}
