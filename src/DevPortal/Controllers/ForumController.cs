using DevPortal.Web.Models.ForumViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.Controllers
{
    public class ForumController: Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var viewModel = new IndexPageViewModel();
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
