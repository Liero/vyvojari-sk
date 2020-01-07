using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.MessageViewModels
{
    public class CreateViewModel
    {
        [Required]
        public string Recipient { get; set; }

        [Required, MinLength(2)]
        public string Content { get; set; }
    }
}
