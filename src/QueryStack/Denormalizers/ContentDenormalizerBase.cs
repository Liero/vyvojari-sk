using DevPortal.CommandStack.Events;
using DevPortal.QueryStack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevPortal.QueryStack.Denormalizers
{
    public abstract class ContentDenormalizerBase<TReadModel> where TReadModel : GenericContent, new()
    {
        public TReadModel MapCreated(IContentCreated message)
        {
            TReadModel entity = new TReadModel
            {
                Id = message.ContentId,
                Title = message.Title,
                Content = message.Content,
                Created = message.TimeStamp,
                CreatedBy = message.AuthorUserName,
            };

            foreach (var tag in message.Tags)
            {
                entity.Tags.Add(new Tag
                {
                    ContentId = message.ContentId,
                    Name = tag
                });
            }
            return entity;
        }

        public void MapEdited(IContentEdited message, TReadModel entity)
        {
            entity.Content = message.Content;
            entity.LastModified = message.TimeStamp;
            entity.LastModifiedBy = message.EditorUserName;
            entity.Title = message.Title;

            var removedTags = entity.Tags.Where(t => !message.Tags.Contains(t.Name));
            var addedTags = message.Tags.Except(entity.Tags.Select(t => t.Name));

            foreach (Tag tag in removedTags)
            {
                entity.Tags.Remove(tag);
            }

            foreach (string tag in addedTags)
            {
                entity.Tags.Add(new Tag
                {
                    ContentId = message.ContentId,
                    Name = tag
                });
            }
        }

    }
}
