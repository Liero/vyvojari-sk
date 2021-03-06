﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.ForumViewModels
{
    public class CreateForumThreadViewModel
    {
        [Required, MinLength(3)]
        public string Title { get; set; }

        [Required, MinLength(5)]
        public string Content { get; set; }

        [Required]
        public string Tags { get; set; }
    }
}
