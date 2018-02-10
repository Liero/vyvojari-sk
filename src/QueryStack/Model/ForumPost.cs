using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.QueryStack.Model
{
    public class ForumPost : ContentBase
    {
        public ForumThread Thread { get; set; }
    }
}
