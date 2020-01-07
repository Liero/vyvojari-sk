using DevPortal.CommandStack.Events;
using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevPortal.QueryStack.Denormalizers
{
    public class ConversationDenormalizer:
        IHandleMessages<MessageSent>
    {
        private readonly DbSet<Conversation> _denormalizedView;
        private readonly ILogger<ConversationDenormalizer> _logger;
        private readonly IEventStore _eventStore;

        public ConversationDenormalizer(
            DevPortalDbContext queryModelDb, 
            IEventStore eventStore, 
            ILoggerFactory loggerFactory)
        {
            _eventStore = eventStore;
            _denormalizedView = queryModelDb.Conversations;
            _logger = loggerFactory.CreateLogger<ConversationDenormalizer>();
        }

        public async Task Handle(MessageSent message)
        {
            var userNames = GetUserNames(message);
            var conversation = await FindConversationAsync(userNames);

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    UserName1 = userNames[0],
                    UserName2 = userNames[1],
                };
                _denormalizedView.Add(conversation);
            }
            conversation.LastContent = message.Content;
            if (conversation.LastPostedBy != message.SenderUserName)
            {
                conversation.UnreadMessages = 0;
            }
            conversation.LastPostedBy = message.SenderUserName;
            conversation.LastPosted = message.TimeStamp;
            conversation.UnreadMessages++;
        }

        public async Task Handle(MessageMarkedAsRead message)
        {
            var evt = _eventStore.Find<MessageSent>(e => e.MessageId == message.MessageId).FirstOrDefault();
            if (evt == null)
            {
                _logger.LogWarning("Message with id {MessageId} not found", message.MessageId);
                return;
            }
            var conversation = await FindConversationAsync(GetUserNames((MessageSent)evt.Event));
            conversation.UnreadMessages = 0;
        }

        private List<string> GetUserNames(MessageSent message)
        {
            var userNames = new List<string> { message.SenderUserName, message.RecipientUserName };
            userNames.Sort();
            return userNames;
        }

        private async Task<Conversation> FindConversationAsync(List<string> userNames)
        {
            var conversation = await _denormalizedView.FindAsync(userNames[0], userNames[1]);
            if (conversation == null)
            {
                conversation = await _denormalizedView.FindAsync(userNames[1], userNames[0]);
            }
            return conversation;
        }
    }
}
