using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DevPortal.QueryStack.Model
{
    public abstract class ChildContent : ContentBase
    {
        public GenericContent Root { get; set; }
    }

    public abstract class ChildContent<TRootContent> : ChildContent
        where TRootContent: GenericContent
    {
        [NotMapped]
        public new TRootContent Root
        {
            get => (TRootContent)base.Root;
            set => base.Root = value;
        }
    }
}
