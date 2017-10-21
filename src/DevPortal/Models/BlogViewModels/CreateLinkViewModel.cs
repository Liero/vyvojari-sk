using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.BlogViewModels
{
    public class CreateLinkViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required, DataType(DataType.Url)]
        public string Link { get; set; }

        [Required]
        public string Description { get; set; }

        public CreateLinkViewModel()
        {
            this.Link = "https://";
        }
    }
}
