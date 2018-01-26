using DevPortal.CommandStack.Events;
using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack;
using DevPortal.Web.AppCode;
using DevPortal.Web.AppCode.Extensions;
using DevPortal.Web.Data;
using DevPortal.Web.Models.ForumViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Controllers
{
    [Authorize]
    public class ForumController : Controller
    {
        public const string ControllerName = "Forum";

        readonly IEventStore _eventStore;
        readonly DevPortalDbContext _devPortalDb;

        public ForumController(IEventStore eventStore, DevPortalDbContext devPortalDbContext)
        {
            _eventStore = eventStore;
            _devPortalDb = devPortalDbContext;
        }

        [HttpGet, AllowAnonymous]
        public async Task<ActionResult> Index(int pageNumber = 1)
        {
            int pageIndex = pageNumber - 1;
            const int pageSize = 20;
            const int editorsCount = 5;

            IndexPageViewModel viewModel = new IndexPageViewModel
            {
                PageNumber = pageNumber,
            };

            viewModel.Threads = await _devPortalDb.ForumThreads
                .AsNoTracking()
                .OrderBy(i => i.Created)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(t => new ForumThreadListItemViewModel
                {
                    Thread = t,
                    Participants = t.Posts
                                   .OrderByDescending(p => p.Created)
                                   .Select(p => p.CreatedBy)
                                   .Distinct()
                                   .Take(editorsCount)
                                   .ToArray()
                })
                .ToListAsync();

            return View(viewModel);
        }

        [HttpGet, AllowAnonymous]
        public async Task<ActionResult> Detail(Guid id, NewAnswerViewModel answer = null)
        {
            var forumThread = await _devPortalDb.ForumThreads
                .AsNoTracking()
                .Include(f =>f.Posts)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (forumThread == null) return NotFound();

            var viewModel = new ForumDetailViewModel
            {
                Thread = forumThread
            };
            return View(nameof(Detail), viewModel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = new CreateForumThreadViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateForumThreadViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var evt = new ForumThreadCreated
            {
                ForumThreadId = Guid.NewGuid(),
                AuthorUserName = User.Identity.Name,
                Title = viewModel.Title,
                Content = viewModel.Content,
                Tags = TagsConverter.StringToArray(viewModel.Tags),
            };
            _eventStore.Save(evt);

            return RedirectToAction(nameof(Detail), new { id = evt.ForumThreadId });
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var entity = _devPortalDb.ForumThreads.Find(id);
            if (entity == null)
            {
                return NotFound("Specified id does not exist");
            }
            if (entity.CreatedBy != User.Identity.Name)
            {
                return Unauthorized();
            }

            var viewModel = new EditForumThreadViewModel
            {
                Id = entity.Id,
                Content = entity.Content,
                Title = entity.Title,
                Tags = entity.Tags
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(Guid id, EditForumThreadViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);
            var entity = _devPortalDb.ForumThreads.Find(id);
            if (entity == null)
            {
                return NotFound("Specified id does not exist");
            }
            if (entity.CreatedBy != User.Identity.Name)
            {
                return Unauthorized();
            }

            var evt = new ForumThreadEdited
            {
                ForumThreadId = id,
                EditorUserName = User.Identity.Name,
                Title = viewModel.Title,
                Content = viewModel.Content,
                Tags = TagsConverter.StringToArray(viewModel.Tags),
            };
            _eventStore.Save(evt);

            return RedirectToAction(nameof(Detail), new { id });
        }

        [HttpDelete]
        public ActionResult Delete(Guid id)
        {
            var entity = _devPortalDb.ForumThreads.Find(id);
            if (entity == null)
            {
                return NotFound("Specified id does not exist");
            }
            if (entity.CreatedBy != User.Identity.Name)
            {
                return Unauthorized();
            }

            var evt = new ForumThreadDeleted
            {
                ForumThreadId = id,
                UserName = User.Identity.Name,
            };
            _eventStore.Save(evt);

            return RedirectToAction(nameof(Index), routeValues: new { id });
        }

        [HttpPost]
        public async Task<ActionResult> NewPost(
            Guid id, 
            [Bind(Prefix = nameof(ForumDetailViewModel.NewAnswer))] NewAnswerViewModel viewModel)
        {
            if (!ModelState.IsValid) return await Detail(id, viewModel);

            var evt = new ForumItemPosted
            {
                ForumThreadId = id,
                ForumItemId = Guid.NewGuid(),
                AuthorUserName = User.Identity.Name,
                Content = viewModel.Content,
            };
            _eventStore.Save(evt);

            return RedirectToAction(nameof(Detail), ControllerName, new { id }, fragment: evt.ForumItemId.ToString());
        }

        [HttpPost]
        public IActionResult EditPost(Guid id, EditAnswerViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var evt = new ForumItemPosted
            {
                ForumThreadId = id,
                ForumItemId = viewModel.ForumPostId,
                AuthorUserName = User.Identity.Name,
                Content = viewModel.Content,
            };
            _eventStore.Save(evt);

            return RedirectToAction(nameof(Detail), ControllerName, new { id }, fragment: evt.ForumItemId.ToString());
        }

        [HttpDelete("Forum/{id}/Posts/{forumPostId}")]
        public IActionResult DeletePost(Guid id, Guid forumPostId)
        {
            var evt = new ForumItemDeleted
            {
                ForumThreadId = id, 
                ForumItemId = forumPostId,
                UserName = User.Identity.Name,
            };
            _eventStore.Save(evt);
            return RedirectToAction(nameof(Index));
        }
    }
}
