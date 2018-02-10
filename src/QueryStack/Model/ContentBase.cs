using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.QueryStack.Model
{
    public abstract class ContentBase
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastModified { get; set; }
        public string LastModifiedBy { get; set; }
    }

    public class GenericContent : ContentBase
    {        
        public virtual string Title { get; set; }
        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }

    public class Tag
    {
        public string Name { get; set; }
        public Guid ContentId { get; set; }
    }
}
