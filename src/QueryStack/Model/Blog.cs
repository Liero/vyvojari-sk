using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.QueryStack.Model
{
    public class Blog : GenericContent
    {
        public string ExternalUrl { get; set; }
        public string Description { get; set; }
    }
}
