using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevPortal.Web.Models.BlogViewModels;
using DevPortal.Web.Data;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace DevPortal.Web.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {
        public BlogController()
        {

        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            IndexPageViewModel model = new IndexPageViewModel()
            {
                Items = SampleData.Instance.BlogPosts
            };

            return View(model);
        }

        public IActionResult Create()
        {
            CrateViewModel model = new CrateViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CrateViewModel model)
        {

            return this.RedirectToAction(nameof(this.Index));
        }

        public IActionResult CreateLink()
        {
            CrateLinkViewModel model = new CrateLinkViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateLink(CrateLinkViewModel model)
        {

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
