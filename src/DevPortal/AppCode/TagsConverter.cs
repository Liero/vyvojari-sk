using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode
{
    public static class TagsConverter
    {
        public static string ArrayToString(string[] tags)
        {
            return string.Join(",", tags);
        }

        public static string[] StringToArray(string tags)
        {
            return tags.Split(',');
        }
    }
}
