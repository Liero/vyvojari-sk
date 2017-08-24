using DevPortal.Web.AppCode;
using DevPortal.Web.AppCode.Extensions;
using DevPortal.Web.Data;
using DevPortal.Web.Models.ForumViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Controllers
{
    [Authorize]
    public class ForumController : Controller
    {
        [HttpGet, AllowAnonymous]
        public ActionResult Index()
        {
            var viewModel = new IndexPageViewModel();
            return View(viewModel);
        }

        [HttpGet, AllowAnonymous]
        public ActionResult Detail(string id)
        {
            var post = SampleData.Instance.ForumPosts.FirstOrDefault(p => p.Id == id);
            if (post == null) return NotFound();
            var viewModel = new ForumDetailViewModel
            {
                Question = post,
                Answers = Enumerable.Range(0, post.Answers).Select(i => new ForumPostViewModel
                {
                    Avatar = SampleData.UserPictures.Repeat().ElementAt(i),
                    Content = SampleData.LoremIpsum.Repeat().ElementAt(i),
                    Created = DateTime.Now.AddDays(-post.Answers).AddHours(6.32 * i),
                    UserName = SampleData.UserNames.Repeat().ElementAt(i)
                }).ToList()
            };
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult NewPost()
        {
            return View(new NewPostViewModel());
        }

        [HttpPost]
        public ActionResult NewPost(NewPostViewModel model)
        {
            if (!TryValidateModel(model))
            {
                return View(model);
            }

            var sampleData = new ForumQuestionPostViewModel()
            {
                UserName = User.Identity.Name,
                Id = Guid.NewGuid().ToString(),
                Content = model.Content,
                Title = model.Title,
                Tags = TagsConverter.StringToArray(model.Tags)
            };
            SampleData.Instance.ForumPosts.Add(sampleData);

            return RedirectToAction(nameof(Detail), new { id = sampleData.Id });
        }

        [HttpGet]
        public ActionResult EditPost(string id)
        {
            var entity = SampleData.Instance.ForumPosts.FirstOrDefault(p => p.Id == id);
            if (entity == null)
            {
                return NotFound("Specified id does not exist");
            }
            if (entity.UserName != User.Identity.Name)
            {
                return Unauthorized();
            }

            var viewModel = new EditPostViewModel
            {
                Id = entity.Id,
                Content = entity.Content,
                Title = entity.Title,
                Tags = TagsConverter.ArrayToString(entity.Tags)
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult NewAnswer(string id, ForumDetailViewModel model)
        {
            var entity = SampleData.Instance.ForumPosts.FirstOrDefault(p => p.Id == id);
            if (entity == null)
            {
                return NotFound("Specified id does not exist");
            }

            if (!TryValidateModel(model))
            {
                return View(nameof(Detail), model);
            }

            entity.Answers++;

            return RedirectToAction(nameof(Detail), new { id = id });
        }
    }
}
