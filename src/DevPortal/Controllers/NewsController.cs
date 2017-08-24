using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevPortal.Web.Models.NewsViewModels;
using DevPortal.Web.Data;
using DevPortal.CommandStack.Events;
using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack;
using Microsoft.EntityFrameworkCore;
using DevPortal.Web.Models.SharedViewModels;
using DevPortal.QueryStack.Model;
using Microsoft.AspNetCore.Authorization;

namespace DevPortal.Web.Controllers
{
    [Authorize]
    public class NewsController : Controller
    {
        public const string ControllerName = "News";

        readonly IEventStore _eventStore;
        readonly DevPortalDbContext _devPortalDb;

        public NewsController(IEventStore eventStore, DevPortalDbContext devPortalDbContext)
        {
            _eventStore = eventStore;
            _devPortalDb = devPortalDbContext;
        }

        [AllowAnonymous]
        public IActionResult Index(int pageNumber = 1, int maxCommentsPerItem = 3)
        {
            int pageIndex = pageNumber - 1;
            int pageSize = 20;

            IndexPageViewModel viewModel = new IndexPageViewModel
            {
                PageNumber = pageNumber,
                MaxCommentsPerItem = maxCommentsPerItem,
            };

            IQueryable<NewsItem> query = _devPortalDb.NewsItems.Include(i => i.Comments);

            if (!User.Identity.IsAuthenticated)
            {
                query = query.Where(newsItem => newsItem.IsPublished);
            }
            else
            {
                query = query.Where(newsItem => newsItem.IsPublished || newsItem.CreatedBy == User.Identity.Name);
            }

            viewModel.Items = query
                .OrderBy(i => i.IsPublished)
                .ThenByDescending(i => i.Published)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            viewModel.PageCount = _devPortalDb.NewsItems.Count();

            return View(viewModel);
        }

        [AllowAnonymous]
        public IActionResult Detail(Guid id)
        {
            NewsItem newsItem = _devPortalDb.NewsItems
                .Include(i => i.Comments)
                .FirstOrDefault(i => i.Id == id);

            if (newsItem == null)
            {
                return NotFound();
            }
            DetailPageViewModel viewModel = new DetailPageViewModel()
            {
                NewsItem = newsItem
            };
            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View(new CreateNewsItemViewModel());
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
                Tags = AppCode.TagsConverter.StringToArray(viewModel.Tags)
            };
            _eventStore.Save(evt);

            return RedirectToAction(nameof(Detail), new { id = evt.NewsItemId });
        }

        public IActionResult Edit(Guid id)
        {
            NewsItem newsItem = _devPortalDb.NewsItems.Find(id);
            var viewModel = new EditNewsItemViewModel
            {
                Id = newsItem.Id,
                Tags = newsItem.Tags,
                Content = newsItem.Content,
                Title = newsItem.Title,
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(Guid id, EditNewsItemViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var evt = new NewsItemEdited
            {
                NewsItemId = id,
                EditorUserName = User.Identity.Name,
                Title = viewModel.Title,
                Content = viewModel.Content,
                Tags = AppCode.TagsConverter.StringToArray(viewModel.Tags),
            };
            _eventStore.Save(evt);

            return RedirectToAction(nameof(Detail), new { id = id });
        }

        [HttpPost]
        public IActionResult AddComment(
            Guid id,
            [Bind(Prefix = nameof(DetailPageViewModel.AddComment))] AddCommentViewModel comment)
        {
       
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var evt = new NewsItemCommented
            {
                NewsItemId = id,
                CommentId = Guid.NewGuid(),
                UserName = User.Identity.Name,
                Content = comment.Message
            };
            _eventStore.Save(evt);
            return RedirectToAction(nameof(Detail), ControllerName, new { id = id }, fragment: evt.CommentId.ToString());
        }

        [HttpPost]
        public IActionResult Publish(Guid id)
        {
            var evt = new NewsItemPublished
            {
                NewsItemId = id,
            };
            _eventStore.Save(evt);
            return RedirectToAction(nameof(Detail), new { id = id });
        }
    }
}
