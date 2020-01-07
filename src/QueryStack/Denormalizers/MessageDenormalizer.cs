using DevPortal.CommandStack.Events;
using DevPortal.QueryStack.Model;
using Microsoft.EntityFrameworkCore;
using Rebus.Handlers;
using System.Threading.Tasks;

namespace DevPortal.QueryStack.Denormalizers
{
    public class MessageDenormalizer :
        IHandleMessages<MessageSent>
    {
        private readonly DbSet<Message> _denormalizedView;

        public MessageDenormalizer(DevPortalDbContext queryModelDb)
        {
            _denormalizedView = queryModelDb.Messages;
        }

        public Task Handle(MessageSent message)
        {
            _denormalizedView.Add(new Message
            {
                Id = message.MessageId,
                Content = message.Content,
                RecipientUserName = message.RecipientUserName,
                SenderUserName = message.SenderUserName,
                TimeStamp = message.TimeStamp
            });
            return Task.CompletedTask;
        }
    }
}
