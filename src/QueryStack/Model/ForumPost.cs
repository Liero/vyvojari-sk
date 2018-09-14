using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DevPortal.QueryStack.Model
{
    public class ForumPost : ChildContent<ForumThread>
    {
        [NotMapped]
        public ForumThread Thread { get => Root; }
    }
}
