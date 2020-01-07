using DevPortal.QueryStack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevPortal.QueryStack
{
    public static class MessagesQueries
    {
        public static IQueryable<Message> WithUser(this IQueryable<Message> messages, string userName)
        {
            return messages.Where(c => c.RecipientUserName == userName || c.SenderUserName == userName);
        }
    }
}
