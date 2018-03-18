using DevPortal.CommandStack.Events;
using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack;
using DevPortal.Web.Models.NewsViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace DevPortal.Web.Controllers
{
    [TestClass]
    public class NewsControllerTests : ControllerTestsBase<NewsController>
    {
        public readonly Guid ExistingNewsItemId = new Guid("00000000-0000-0000-0000-000000000001");

        [TestInitialize]
        public override  void Init()
        {
            base.Init();
            using (var dbContext = (DevPortalDbContext)ServiceProvider.GetService(typeof(DevPortalDbContext)))
            {
                dbContext.NewsItems.Add(new QueryStack.Model.NewsItem { Id = ExistingNewsItemId });
                dbContext.SaveChanges();
            }
        }

        [TestMethod]
        public void NewsController_Create_Produces_NewsItemCreated_Event()
        {
            //setup
            var createVm = new CreateNewsItemViewModel
            {
                Title = "Title",
                Content = "Content",
                Tags = "ABC,DEF"
            };
            
            EventStoreMock
                .Setup(es => es.Save(It.IsAny<NewsItemCreated>()))
                .Callback<DomainEvent>(NewsItemCreated);

            NewsController controller = CreateAuthenticatedController(Config.UserName);

            //action
            IActionResult actionResult = controller.Create(createVm);

            //assert
            void NewsItemCreated(DomainEvent savedEvent)
            {
                NewsItemCreated actualEvent = (NewsItemCreated)savedEvent;
                Assert.AreEqual(createVm.Title, actualEvent.Title, "Title");
                Assert.AreEqual(createVm.Content, actualEvent.Content, "Content");
                Assert.AreEqual(2, actualEvent.Tags.Length, "Tags Count");
                Assert.AreEqual(Config.UserName, actualEvent.AuthorUserName);
            }

            EventStoreMock.Verify(es => es.Save(It.IsAny<NewsItemCreated>()), Times.Once);
        }

        [TestMethod]
        public void NewsController_Edit_Produces_NewsItemEdited_Event()
        {
            var vm = new EditNewsItemViewModel
            {
                Title = "TitleEdited",
                Content = "ContentEdited",
                Tags = "ABC,DEF,EFG"
            };

            EventStoreMock.
                Setup(es => es.Save(It.IsAny<NewsItemEdited>()))
                .Callback<DomainEvent>(NewsItemEdited);

            NewsController controller = CreateAuthenticatedController(Config.UserName);

            //action
            IActionResult actionResult = controller.Edit(ExistingNewsItemId, vm);

            //assert
            void NewsItemEdited(DomainEvent savedEvent)
            {
                NewsItemEdited actualEvent = (NewsItemEdited)savedEvent;
                Assert.AreEqual(ExistingNewsItemId, actualEvent.NewsItemId, nameof(actualEvent.NewsItemId));
                Assert.AreEqual(vm.Title, actualEvent.Title, "Title");
                Assert.AreEqual(vm.Content, actualEvent.Content, "Content");
                Assert.AreEqual(3, actualEvent.Tags.Length, "Tags Count");
                Assert.AreEqual(Config.UserName, actualEvent.EditorUserName);
            }

            EventStoreMock.Verify(es => es.Save(It.IsAny<NewsItemEdited>()), Times.Once);
        }

        [TestMethod]
        public void NewsController_Publish_Produces_NewsItemPublished_Event()
        {
            //setup
            EventStoreMock
                .Setup(es => es.Save(It.IsAny<NewsItemPublished>()))
                .Callback<DomainEvent>(NewsItemPublished);

            NewsController controller = CreateAuthenticatedController(Config.UserName);

            //action
            IActionResult actionResult = controller.Publish(ExistingNewsItemId);

            //assert
            void NewsItemPublished(DomainEvent savedEvent)
            {
                NewsItemPublished actualEvent = (NewsItemPublished)savedEvent;
                Assert.AreEqual(ExistingNewsItemId, actualEvent.NewsItemId, nameof(actualEvent.NewsItemId));
            }

            EventStoreMock.Verify(es => es.Save(It.IsAny<NewsItemPublished>()), Times.Once);
        }

        [TestMethod]
        public void NewsController_AddComment_Produces_NewsItemCommented_Event()
        {
            //setup
            string comment = "expected comment";

            EventStoreMock
                .Setup(es => es.Save(It.IsAny<NewsItemCommented>()))
                .Callback<DomainEvent>(NewsItemCommented);

            NewsController controller = CreateAuthenticatedController(Config.UserName);

            //action
            IActionResult actionResult = controller.AddComment(ExistingNewsItemId, new AddCommentViewModel
            {
                Message = comment 
            });

            //assert
            void NewsItemCommented(DomainEvent savedEvent)
            {
                NewsItemCommented actualEvent = (NewsItemCommented)savedEvent;
                Assert.AreEqual(ExistingNewsItemId, actualEvent.NewsItemId, nameof(actualEvent.NewsItemId));
                Assert.AreEqual(comment, actualEvent.Content, nameof(actualEvent.Content));
                Assert.AreNotEqual(default(Guid), actualEvent.CommentId, nameof(actualEvent.CommentId));
            }

            EventStoreMock.Verify(es => es.Save(It.IsAny<NewsItemCommented>()), Times.Once);
        }
    }
}
