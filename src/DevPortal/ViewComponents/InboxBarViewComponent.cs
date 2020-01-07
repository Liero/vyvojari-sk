using DevPortal.QueryStack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.ViewComponents
{
    public class InboxBarViewComponent : ViewComponent
    {
        private readonly DevPortalDbContext _devPortalDb;

        public InboxBarViewComponent(DevPortalDbContext devPortalDb)
        {
            _devPortalDb = devPortalDb;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.UnreadMessages = await _devPortalDb.Conversations
                .WithUser(User.Identity.Name)
                .Where(c => c.LastPostedBy != User.Identity.Name)
                .SumAsync(c => c.UnreadMessages);
            return View();
        }
    }
}
