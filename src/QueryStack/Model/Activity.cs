using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DevPortal.QueryStack.Model
{
    public class Activity
    {
        public Guid ActivityId { get; set; }
        public Guid ContentId { get; set; }
        public Guid? Fragment { get; set; }

        [Required]
        public string ContentType { get; set; }
        public string ContentTitle { get; set; }

        [Required]
        public string Action { get; set; }
        public string UserName { get; set; }
        public DateTime TimeStamp { get; set; }
        public string ExternalUrl { get; set; }
    }
}
