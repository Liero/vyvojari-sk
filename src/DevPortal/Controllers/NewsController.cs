using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevPortal.Web.Models.NewsViewModels;
using DevPortal.Web.Data;

namespace DevPortal.Web.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult Index(int skip = 1, int take = 20)
        {
            var pageItems = SampleData.Instance.News
                .Skip(skip)
                .Take(take);

            return View(new IndexPageViewModel { Items = pageItems.ToList() });
        }
            
        public IActionResult View(Guid id)
        {
            return View("Detail");
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(Guid id)
        {
            return View();
        }

        public IActionResult Publish(Guid id)
        {
            return View();
        }
    }
}
