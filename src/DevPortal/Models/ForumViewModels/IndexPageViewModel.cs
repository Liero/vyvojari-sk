using DevPortal.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.ForumViewModels
{
    public class IndexPageViewModel
    {
        public ForumQuestionPostViewModel[] Posts { get; }

        public string[] MostUsedTags { get; }

        public IndexPageViewModel()
        {
            Posts = SampleData.Instance.ForumPosts;
            MostUsedTags = SampleData.Tags;
        }
    }

}
