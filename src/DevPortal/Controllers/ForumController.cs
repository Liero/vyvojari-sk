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
    public class ForumController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var viewModel = new IndexPageViewModel();
            return View(viewModel);
        }

        [HttpGet]
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
        [AllowAnonymous]
        public ActionResult NewPost()
        {
            return View();
        }
    }
}
