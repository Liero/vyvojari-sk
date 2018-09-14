using DevPortal.QueryStack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode
{
    public class ControllerNameConverter
    {
        public static string ControllerNameFromContentType(string contentType)
        {
            switch (contentType)
            {
                case nameof(ForumThread): return "Forum";
                case nameof(ForumPost): return "Forum";
                case nameof(NewsItem): return "News";
                case nameof(NewsItemComment): return "News";
                case nameof(Blog): return "Blog";
                default: return null;
            }
        }
    }
}
