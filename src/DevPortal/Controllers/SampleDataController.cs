using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevPortal.CommandStack.Infrastructure;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DevPortal.Web.Controllers
{
    public class SampleDataController : Controller
    {
        private readonly IEventStore _eventStore;

        public SampleDataController(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult All()
        {
            var sampleData = new DesignTimeData.SampleEventsGenerator();
            int counter = 0;
            foreach (var evt in sampleData.News(10))
            {
                _eventStore.Save(evt);
                counter++;
            }
            ViewBag.Message = $"Generated {counter} events";
            return View(nameof(Index));
        }
    }
}
