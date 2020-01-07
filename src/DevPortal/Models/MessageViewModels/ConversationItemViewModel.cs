using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.MessageViewModels
{
    public class ConversationItemViewModel
    {
        /// <summary>
        /// name of the other user, with whom current user ist having conversation
        /// </summary>
        public string UserName { get; set; }
        public string LastPostedBy { get; set; }
        public DateTime LastPosted { get; set; }
        public string LastContent { get; set; }
        public int UnreadMessages { get; set; }
    }
}
