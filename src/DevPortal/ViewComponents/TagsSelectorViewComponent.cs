using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.ViewComponents
{
    public class TagsSelectorViewComponent: ViewComponent
    {
        public IViewComponentResult Invoke(ModelExpression aspFor)
        {
            return View(aspFor);
        }
    }
}
