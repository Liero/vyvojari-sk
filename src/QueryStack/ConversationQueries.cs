using DevPortal.QueryStack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevPortal.QueryStack
{
    public static class ConversationQueries
    {
        public static IQueryable<Conversation> WithUser(this IQueryable<Conversation> conversations, string userName)
        {
            return conversations.Where(c => c.UserName1 == userName || c.UserName2 == userName);
        }
    }
}
