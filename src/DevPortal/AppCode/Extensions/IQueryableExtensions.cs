using DevPortal.QueryStack.Model;
using DevPortal.Web.Models.ForumViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<ForumThreadListItemViewModel> SelectViewModels(this IQueryable<ForumThread> forumThreads, int editorsCount = 5)
        {
            return forumThreads.Select(t => new ForumThreadListItemViewModel
            {
                Thread = t,
                Participants = t.Posts
                    .OrderByDescending(p => p.Created)
                    .Select(p => p.CreatedBy)
                    .Distinct()
                    .Take(editorsCount)
                    .ToArray()
            });
        }
    }
}
