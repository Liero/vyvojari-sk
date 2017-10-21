using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.ForumViewModels
{
    public class EditAnswerViewModel : NewAnswerViewModel
    {
        [HiddenInput]
        [Required]
        public Guid ForumPostId { get; set; }
    }
}
