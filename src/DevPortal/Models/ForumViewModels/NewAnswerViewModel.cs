using DevPortal.Web.Models.SharedViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.ForumViewModels
{
    public class NewAnswerViewModel
    {
        [Required]
        public string Content { get; set; }
    }
}
