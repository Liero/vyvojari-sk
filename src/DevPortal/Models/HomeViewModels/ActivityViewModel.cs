using DevPortal.CommandStack.Events;
using DevPortal.QueryStack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.HomeViewModels
{
    public static class ActivityViewModel
    {
        public static string GetUserFriendlyTitle(Activity activity)
        {
            switch (activity.Action)
            {
                case nameof(NewsItemPublished): return "publikoval správičku";
                case nameof(NewsItemCommented): return "komentoval správičku";
                case nameof(ForumThreadCreated): return "položil otázku";
                case nameof(ForumItemPosted): return "reagoval na otázku";
                case nameof(BlogLinkCreated): return "vložil odkaz na blog";
                default: return activity.Action;
            }
        }
    }
}
