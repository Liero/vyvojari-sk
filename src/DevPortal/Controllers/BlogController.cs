using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevPortal.Web.Models.BlogViewModels;
using DevPortal.Web.Data;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace DevPortal.Web.Controllers
{
    public class BlogController : Controller
    {
        public BlogController()
        {

        }

        public IActionResult Index()
        {
            IndexPageViewModel model = new IndexPageViewModel()
            {
                Items = SampleData.Instance.BlogPosts
            };

            return View(model);
        }
    }
}
