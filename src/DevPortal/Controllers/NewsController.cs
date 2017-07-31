using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevPortal.Web.Models.NewsViewModels;
using DevPortal.Web.Data;

namespace DevPortal.Web.Controllers
{
    public class NewsController : Controller
    {
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
        public IActionResult Create(NewsItemViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            viewModel.Id = Guid.NewGuid().ToString();
            SampleData.Instance.News.Add(viewModel);
            return RedirectToAction(nameof(Detail), new { id = viewModel.Id });
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
            NewsItemViewModel item = SampleData.Instance.News.First(i => i.Id == id);
            item.Content = viewModel.Content;
            item.Title = viewModel.Title;
            item.Categories = viewModel.Categories;
            return RedirectToAction(nameof(Detail), new { id = id });
        }

        [HttpPost]
        public IActionResult AddComment(string id, string comment)
        {
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
            NewsItemViewModel item = SampleData.Instance.News.First(i => i.Id == id);
            item.IsPublished = true;
            return RedirectToAction(nameof(Detail), new { id = id });
        }
    }
}
