using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevPortal.Web.Models.BlogViewModels;
using DevPortal.Web.Data;
using Microsoft.AspNetCore.Authorization;
using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack;
using DevPortal.CommandStack.Events;
using Microsoft.EntityFrameworkCore;
using DevPortal.Web.AppCode.EventSourcing;
using DevPortal.QueryStack.Denormalizers;
using DevPortal.Web.AppCode.Authorization;
using DevPortal.Web.AppCode.Extensions;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace DevPortal.Web.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {
        private readonly IEventStore _eventStore;
        private readonly DevPortalDbContext _devPortalDb;

        public BlogController(IEventStore eventStore, DevPortalDbContext devPortalDbContext)
        {
            _eventStore = eventStore;
            _devPortalDb = devPortalDbContext;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            int pageIndex = pageNumber - 1;

            var viewModel = new IndexPageViewModel
            {
                PageNumber = pageNumber,
                TotalCount = await _devPortalDb.Blogs.CountAsync(),
            };

            viewModel.Blogs = await _devPortalDb.Blogs
                .OrderByDescending(i => i.Created)
                .Skip(pageIndex * viewModel.PageSize)
                .Take(viewModel.PageSize)
                .ToListAsync();

            return View(viewModel);
        }

        public IActionResult Create()
        {
            CreateViewModel model = new CreateViewModel();
            return View(model);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateReCaptchaAttribute))]
        public IActionResult Create(CreateViewModel model)
        {

            return this.RedirectToAction(nameof(this.Index));
        }

        public IActionResult CreateLink()
        {
            CreateLinkViewModel model = new CreateLinkViewModel();
            return View(model);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateReCaptchaAttribute))]
        public async Task<IActionResult> CreateLink(CreateLinkViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var evt = new BlogLinkCreated
            {
                BlogId = Guid.NewGuid(),
                UserName = User.Identity.Name,
                Title = viewModel.Title,
                Url = viewModel.Link,
                Description = viewModel.Description,
            };
            await _eventStore.SaveAndWaitForHandler(evt, typeof(BlogDenormalizer));

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policies.DeletePolicy)]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = _devPortalDb.Blogs.Find(id);
            if (entity == null)
            {
                return NotFound("Specified id does not exist");
            }

            var evt = new BlogDeleted
            {
                BlogId = id,
                UserName = User.Identity.Name,
            };
            await _eventStore.SaveAndWaitForHandler(evt, typeof(BlogDenormalizer));

            return RedirectToAction(nameof(Index), routeValues: new { id });
        }
    }
}
