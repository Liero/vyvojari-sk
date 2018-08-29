using DevPortal.Web.Data;
using DevPortal.Web.Models;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevPortal.Web.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "avatar-for-user")]
    [HtmlTargetElement("span", Attributes = "avatar-for-user")]
    [HtmlTargetElement("div", Attributes = "avatar-for-user")]
    [HtmlTargetElement("avatar", Attributes = "username")]
    public class AvatarTagHelper : TagHelper
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccesor;
        private static readonly string[] Colors = new[]{
            "#6E398D","#C31A7F","#424F9B","#1D96BB","#8CBD3F","#FDC70F","#EC6224"
        };

        public AvatarTagHelper(
            ApplicationDbContext dbContext,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccesor)
        {
            _dbContext = dbContext;
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccesor = actionContextAccesor;
        }

        [HtmlAttributeName("avatar-for-user")]
        public string AvatarForUser { get; set; }

        [HtmlAttributeName("username")]
        public string UserName { get => AvatarForUser ?? "?"; set => AvatarForUser = value; }

        static Dictionary<string, string> _avatars { get; set; }
        private readonly static object _loadingLock = new object();


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            var avatarUrl = GetAvatarUrl(UserName);
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
            else if (avatarUrl == null)
            {
                //in case there is no username and avatar url - can happed if you are logged out
                avatarUrl =  "/media/noface.jpg";
            }

            if (!string.IsNullOrEmpty(avatarUrl)) //must check in order to avoid request to current page (empty url)
            {
                content.AppendLine($"<span class=\"cover\" style=\"background-image:url('{avatarUrl}')\"></span>");
            }

            output.Content.SetHtmlContent(content.ToString());
            output.Attributes.Add("style", $"background:{background}");

        }


        private string GetAvatarUrl(string userName)
        {
            lock (_loadingLock)
            {
                if (_avatars == null)
                {
                    _avatars = _dbContext.Users.ToDictionary(u => u.UserName, u => u.AvatarUrl);
                    
                    if (!_avatars.TryGetValue(UserName, out string avatarUrl))
                    {
                        _avatars[UserName] = null;
                    }
                    return avatarUrl;
                }
                else
                {
                    if (!_avatars.TryGetValue(UserName, out string avatarUrl))
                    {
                        avatarUrl = _dbContext.Users.Where(u => u.UserName == UserName)
                            .Select(u => u.AvatarUrl)
                            .FirstOrDefault();

                        _avatars[userName] = avatarUrl;
                    }
                    return avatarUrl;
                }
            }
        }

        private string GetBackgroundColor(string userName)
        {
            int hashCode = userName.ToLower().GetHashCode() & 0xfffffff; //positive hash
            return Colors[hashCode % Colors.Length];
        }
    }
}
