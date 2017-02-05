using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode.Extensions
{
    public static class CustomCollectionExtensions
    {
        public static IEnumerable<T> TakeLast<T>(this ICollection<T> source, int count)
        {
            if (source.Count <= count) return source;
            return source.Skip(source.Count - count);
        }

        public static IEnumerable<T> Repeat<T>(this ICollection<T> source)
        {
            while (true)
            {
                foreach (var item in source)
                {
                    yield return item;
                }
            }
        }
    }
}
