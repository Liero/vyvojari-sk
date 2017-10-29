using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.QueryStack.Model
{
    public class Blog
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ExternalUrl { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
    }
}
