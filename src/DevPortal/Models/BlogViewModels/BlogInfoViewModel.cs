using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.BlogViewModels
{
    public class BlogInfoViewModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string UserName { get; set; }

        public string Avatar { get; set; }

        public DateTime Created { get; internal set; }

        public int CommentsCount { get; internal set; }

        public string ExternalLink { get; internal set; }

        public bool IsExternalLink
        {
            get
            {
                // Dammy implementation.
                return !this.ExternalLink.ToLowerInvariant().Contains("vyvojari.sk");
            }
        }
    }
}
