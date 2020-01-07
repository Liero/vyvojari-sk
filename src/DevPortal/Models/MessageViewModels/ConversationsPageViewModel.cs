using DevPortal.QueryStack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.MessageViewModels
{
    public class ConversationsPageViewModel: PaginationViewModelBase
    {
        public List<ConversationItemViewModel> Conversations { get; set; }

        public string SelectedUserName { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();
        public CreateViewModel NewMessage { get; set; } = new CreateViewModel();
    }
}
