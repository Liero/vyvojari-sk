using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.NewsViewModels
{
    public class AddCommentViewModel
    {
        public Guid? Parent { get; set; }

        [Required, MinLength(5)]
        public string Message { get; set; }
    }
}
