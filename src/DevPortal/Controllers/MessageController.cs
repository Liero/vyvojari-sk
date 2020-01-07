using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevPortal.Web.Models.MessageViewModels;
using Microsoft.AspNetCore.Authorization;
using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack;
using Microsoft.EntityFrameworkCore;
using DevPortal.Web.AppCode.Extensions;
using Microsoft.AspNetCore.Identity;
using DevPortal.Web.Models;
using DevPortal.CommandStack.Events;
using DevPortal.Web.AppCode.EventSourcing;
using DevPortal.QueryStack.Denormalizers;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace DevPortal.Web.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly IEventStore _eventStore;
        private readonly DevPortalDbContext _devPortalDb;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessageController(
            IEventStore eventStore,
            DevPortalDbContext devPortalDbContext,
            UserManager<ApplicationUser> userManager)
        {
            _eventStore = eventStore;
            _devPortalDb = devPortalDbContext;
            _userManager = userManager;
        }

        /// <summary>
        /// shows list of conversations
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Conversations(string userName = null, int pageNumber = 1)
        {
            int pageIndex = pageNumber - 1;

            var myConversationQuery = _devPortalDb.Conversations
                .WithUser(User.Identity.Name)
                .OrderByDescending(m => m.LastPosted);

            var viewModel = new ConversationsPageViewModel
            {
                PageNumber = pageNumber,
                TotalCount = await myConversationQuery.CountAsync(),
            };

            var conversations = await myConversationQuery
                .AsNoTracking()
                .Skip(pageIndex * viewModel.PageSize)
                .Take(viewModel.PageSize)
                .ToListAsync();


            viewModel.Conversations = conversations
                .Select(c => new ConversationItemViewModel
                {
                    UserName = User.IdentityNameEquals(c.UserName1) ?
                        c.UserName2 : c.UserName1, // select the other user
                    LastContent = c.LastContent,
                    LastPosted = c.LastPosted,
                    LastPostedBy = c.LastPostedBy,
                    UnreadMessages = User.IdentityNameEquals(c.LastPostedBy) ? 0 : c.UnreadMessages
                })
                .ToList();

            if (userName != null)
            {
                viewModel.SelectedUserName = userName;
                viewModel.Messages = await _devPortalDb.Messages.AsNoTracking()
                    .WithUser(User.Identity.Name) // my messages
                    .WithUser(userName) // messages with the selected user
                    .OrderByDescending(m => m.TimeStamp)
                    .Take(100)
                    .ToListAsync();

                viewModel.Messages.Reverse(); //show newest at the bottom
            }
            if (viewModel.Messages.Count > 0)
            {
                var newestMessage = viewModel.Messages[viewModel.Messages.Count - 1];
                if (User.IdentityNameEquals(newestMessage.RecipientUserName))
                {
                    _eventStore.Save(new MessageMarkedAsRead
                    {
                        MessageId = newestMessage.Id,
                        UserName = User.Identity.Name
                    });
                }
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Post([Bind(Prefix = nameof(ConversationsPageViewModel.NewMessage))]  CreateViewModel model)
        {
            ApplicationUser recipient = await _userManager.FindByNameAsync(model.Recipient);
            if (ModelState.IsValid)
            {
                var evt = new MessageSent
                {
                    SenderUserName = User.Identity.Name,
                    RecipientUserName = recipient.UserName,
                    Content = model.Content,
                };
                await _eventStore.SaveAndWaitForHandler(evt, typeof(MessageDenormalizer));
            }
            return RedirectToAction(nameof(Conversations), new { userName = recipient?.UserName });
        }


        [HttpPost]
        [ServiceFilter(typeof(ValidateReCaptchaAttribute))]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser recipient = await _userManager.FindByNameAsync(model.Recipient);
            if (recipient == null)
            {
                ModelState.AddModelError(nameof(model.Recipient), "Užívatel s takýmto menom neexistuje");
                return View(model);
            }
            var evt = new MessageSent
            {
                MessageId = Guid.NewGuid(),
                SenderUserName = User.Identity.Name,
                RecipientUserName = recipient.UserName,
                Content = model.Content,
            };
            await _eventStore.SaveAndWaitForHandler(evt, typeof(ConversationDenormalizer));
            return RedirectToAction(nameof(this.Conversations));
        }

        public IActionResult Create()
        {
            CreateViewModel model = new CreateViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult MarkAsRead()
        {
            return this.RedirectToAction(nameof(this.Conversations));
        }
    }
}
