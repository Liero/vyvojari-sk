using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.ForumViewModels
{
    public class EditForumThreadViewModel : CreateForumThreadViewModel
    {
        public Guid Id { get; set; }
    }
}
