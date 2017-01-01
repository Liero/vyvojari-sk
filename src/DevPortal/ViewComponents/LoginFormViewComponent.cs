using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.ViewComponents
{
    public class LoginFormViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {         
            return View(new Models.AccountViewModels.LoginFormViewModel());
        }
    }
}
