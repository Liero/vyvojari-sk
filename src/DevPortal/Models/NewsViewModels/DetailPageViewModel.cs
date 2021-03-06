﻿using DevPortal.QueryStack.Model;
using DevPortal.Web.Models.SharedViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Models.NewsViewModels
{
    public class DetailPageViewModel
    {
        public NewsItem NewsItem { get; set; }

        public IEnumerable<CommentViewModel> Comments => NewsItem?.Comments.Select(c => new CommentViewModel
        {
            Id = c.Id,
            Message = c.Content,
            UserName = c.CreatedBy,
            Created = c.Created
        });

        public AddCommentViewModel AddComment { get; set; }
    }
}
