using DevPortal.Web.Models.AccountViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.ViewComponents
{
    public class LoginFormViewComponent: ViewComponent
    {
        public IViewComponentResult Invoke(LoginFormViewModel model = null)
        {
            if (model == null) model = new LoginFormViewModel();
            return View(model);
        }
    }
}
