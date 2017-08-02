using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevPortal.Web.Models.NewsViewModels;
using DevPortal.Web.Data;
using DevPortal.CommandStack.Events;
using DevPortal.CommandStack.Infrastructure;

namespace DevPortal.Web.Controllers
{
    public class NewsController : Controller
    {
        readonly IEventStore _eventStore;

        public NewsController(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public IActionResult Index(int skip = 1, int take = 20)
        {
            var pageItems = SampleData.Instance.News
                .Skip(skip)
                .Take(take);

            return View(new IndexPageViewModel { Items = pageItems.ToList() });
        }
            
        public IActionResult Detail(string id)
        {
            NewsItemViewModel viewModel = SampleData.Instance.News.First(i => i.Id == id);
            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View(new NewsItemViewModel());
        }

        [HttpPost]
        public IActionResult Create(CreateNewsItemViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            var evt = new NewsItemCreated
            {
                NewsItemId = Guid.NewGuid(),
                AuthorUserName = User.Identity.Name,
                Title = viewModel.Title,
                Content = viewModel.Content,
                Tags = AppCode.TagsConverter.StringToArray(viewModel.Categories)
            };
            _eventStore.Save(evt);

            SampleData.Instance.News.Add(new NewsItemViewModel
            {
                Id = evt.NewsItemId.ToString(),
                UserName = evt.AuthorUserName,
                Categories = viewModel.Categories,
                Title = evt.Title,
                Content = evt.Content,
            });
            return RedirectToAction(nameof(Detail), new { id = evt.NewsItemId });
        }

        public IActionResult Edit(string id)
        {
            NewsItemViewModel viewModel = SampleData.Instance.News.First(i => i.Id == id);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(string id, NewsItemViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var evt = new NewsItemEdited
            {
                NewsItemId = Guid.Parse(id),
                EditorUserName = User.Identity.Name,
                Title = viewModel.Title,
                Content = viewModel.Content,
                Tags = AppCode.TagsConverter.StringToArray(viewModel.Categories),
            };
            _eventStore.Save(evt);

            NewsItemViewModel item = SampleData.Instance.News.First(i => i.Id == id);
            item.Content = viewModel.Content;
            item.Title = viewModel.Title;
            item.Categories = viewModel.Categories;
            return RedirectToAction(nameof(Detail), new { id = id });
        }

        [HttpPost]
        public IActionResult AddComment(string id, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                ModelState.AddModelError(nameof(comment), "Comment cannot be empty");
                return View(nameof(Detail), new { id = id });
            }
            var evt = new NewsItemCommented
            {
                NewsItemId = Guid.Parse(id),
                CommentId = Guid.NewGuid(),
                UserName = User.Identity.Name,
                Comment = comment
            };
            _eventStore.Save(evt);

            NewsItemViewModel item = SampleData.Instance.News.First(i => i.Id == id);
            item.Comments.Add(new Models.SharedViewModels.CommentViewModel
            {
                Message = comment,
                UserName = User.Identity.Name
            });
            return RedirectToAction(nameof(Detail), new { id = id });
        }

        [HttpPost]
        public IActionResult Publish(string id)
        {
            var evt = new NewsItemPublished
            {
                NewsItemId = Guid.Parse(id),
            };
            _eventStore.Save(evt);

            NewsItemViewModel item = SampleData.Instance.News.First(i => i.Id == id);
            item.IsPublished = true;
            return RedirectToAction(nameof(Detail), new { id = id });
        }
    }
}
