using DevPortal.CommandStack.Events;
using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack;
using DevPortal.QueryStack.Denormalizers;
using DevPortal.Web.AppCode;
using DevPortal.Web.AppCode.Authorization;
using DevPortal.Web.AppCode.EventSourcing;
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


            IndexPageViewModel viewModel = new IndexPageViewModel
            {
                PageNumber = pageNumber,
                TotalCount = await _devPortalDb.ForumThreads.CountAsync(),
            };

            viewModel.Threads = await _devPortalDb.ForumThreads
                .AsNoTracking()
                .Include(t => t.Tags)
                .OrderByDescending(i => i.Created)
                .Skip(pageIndex * viewModel.PageSize)
                .Take(viewModel.PageSize)
                .ToListAsync();

            return View(viewModel);
        }

        /// <param name="answer">see POST NewPost action that is invoked from Detail page</param>
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Detail(Guid id, 
            NewAnswerViewModel answer = null)
        {
            var forumThread = await _devPortalDb.ForumThreads
                .AsNoTracking()
                .Include(f => f.Posts)
                .Include(f => f.Tags)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (forumThread == null) return NotFound();

            var viewModel = new ForumDetailViewModel
            {
                Thread = forumThread
            };
            return View(nameof(Detail), viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new CreateForumThreadViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateReCaptchaAttribute))]
        public async Task<IActionResult> Create(CreateForumThreadViewModel viewModel)
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
            await _eventStore.SaveAndWaitForHandler(evt, typeof(ForumThreadDenormalizer));

            return RedirectToAction(nameof(Detail), new { id = evt.ForumThreadId });
        }

        [Authorize(Policies.EditPolicy)]
        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var entity = _devPortalDb.ForumThreads.Find(id);
            if (entity == null)
            {
                return NotFound("Specified id does not exist");
            }

            var viewModel = new EditForumThreadViewModel
            {
                Id = entity.Id,
                Content = entity.Content,
                Title = entity.Title,
                Tags = TagsConverter.ArrayToString(entity.Tags.Select(t => t.Name))
            };

            return View(viewModel);
        }

        [Authorize(Policies.EditPolicy)]
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, EditForumThreadViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);
            var entity = _devPortalDb.ForumThreads.Find(id);
            if (entity == null)
            {
                return NotFound("Specified id does not exist");
            }

            var evt = new ForumThreadEdited
            {
                ForumThreadId = id,
                EditorUserName = User.Identity.Name,
                Title = viewModel.Title,
                Content = viewModel.Content,
                Tags = TagsConverter.StringToArray(viewModel.Tags),
            };
            await _eventStore.SaveAndWaitForHandler(evt, typeof(ForumThreadDenormalizer));

            return RedirectToAction(nameof(Detail), new { id });
        }

        [Authorize(Policies.DeletePolicy)]
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = _devPortalDb.ForumThreads.Find(id);
            if (entity == null)
            {
                return NotFound("Specified id does not exist");
            }

            var evt = new ForumThreadDeleted
            {
                ForumThreadId = id,
                UserName = User.Identity.Name,
            };
            await _eventStore.SaveAndWaitForHandler(evt, typeof(ForumThreadDenormalizer));

            return RedirectToAction(nameof(Index), routeValues: new { id });
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateReCaptchaAttribute))]
        public async Task<IActionResult> NewPost(
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
            await _eventStore.SaveAndWaitForHandler(evt, typeof(ForumThreadDenormalizer));

            return RedirectToAction(nameof(Detail), ControllerName, new { id }, fragment: evt.ForumItemId.ToString());
        }

        [Authorize(Policies.EditPolicy)]
        [HttpPost]
        public async Task<IActionResult> EditPost(Guid id, EditAnswerViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var evt = new ForumItemPosted
            {
                ForumThreadId = id,
                ForumItemId = viewModel.ForumPostId,
                AuthorUserName = User.Identity.Name,
                Content = viewModel.Content,
            };
            await _eventStore.SaveAndWaitForHandler(evt, typeof(ForumThreadDenormalizer));

            return RedirectToAction(nameof(Detail), ControllerName, new { id }, fragment: evt.ForumItemId.ToString());
        }

        [Authorize(Policies.DeletePolicy)]
        [HttpDelete("Forum/{id}/Posts/{forumPostId}")]
        public async Task<IActionResult> DeletePost(Guid id, Guid forumPostId)
        {
            var evt = new ForumItemDeleted
            {
                ForumThreadId = id,
                ForumItemId = forumPostId,
                UserName = User.Identity.Name,
            };
            await _eventStore.SaveAndWaitForHandler(evt, typeof(ForumThreadDenormalizer));

            return RedirectToAction(nameof(Index));
        }
    }
}
