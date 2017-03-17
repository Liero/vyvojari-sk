using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.ForumViewModels
{
    public class EditPostViewModel : NewPostViewModel
    {
        [HiddenInput]
        [Required]
        public string Id { get; set; }
    }
}
