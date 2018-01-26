using DevPortal.QueryStack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.ForumViewModels
{
    public class ForumThreadListItemViewModel
    {
        public ForumThread Thread { get; set; }
        public string[] Participants { get; set; }
    }
}
