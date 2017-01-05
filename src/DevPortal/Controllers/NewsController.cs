using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace DevPortal.Web.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
            
        public IActionResult Index(Guid id)
        {
            return View("Detail");
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Publish()
        {
            return View();
        }
    }
}
