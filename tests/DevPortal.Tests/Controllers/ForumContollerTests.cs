using DevPortal.CommandStack.Events;
using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack;
using DevPortal.Web.Models.ForumViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using DevPortal.QueryStack.Model;
using System.Threading.Tasks;

namespace DevPortal.Web.Controllers
{
    [TestClass]
    public class ForumControllerTests : ControllerTestsBase<ForumController>
    {
        readonly Guid ExistingThreadId = Guid.Parse("ab4b69a9-664b-40b3-bebb-67e9f236828f");
        readonly Guid ExistingPostId = Guid.Parse("23583b86-f32c-4705-be29-da4ceb8f11c0");

        public override void Init()
        {
            base.Init();

            //setup existing data, so Edit/Delete can be tested
            using (var dbContext = ServiceProvider.GetRequiredService<DevPortalDbContext>())
            {
                dbContext.ForumThreads.Add(new ForumThread
                {
                    Id = ExistingThreadId,
                    Title = "Exiting Forum Thread",
                    Content = "Some content",
                    CreatedBy = Config.UserName,
                    Created = DateTime.Now,
                    Tags = CreateTags(ExistingThreadId, "ABC", "DEF"),
                    Posts = new List<ForumPost>
                    {
                        new ForumPost
                        {
                            Id = ExistingPostId,
                            Content = "Some Answer",
                            CreatedBy = Config.UserName,
                            Created = DateTime.Now,
                        }
                    },
                });
                dbContext.SaveChanges();
            }
        }

        [TestMethod]
        public void ForumController_CreateThread_Produces_ForumThreadCreated_Event()
        {
            //setup
            var expectedEvent = ListenInEventStoreFor<ForumThreadCreated>();
            var createVm = new CreateForumThreadViewModel
            {
                Title = "Title",
                Content = "Content",
                Tags = "ABC,DEF"
            };

            ForumController controller = CreateAuthenticatedController(Config.UserName);

            //action
            IActionResult actionResult = controller.Create(createVm);

            //assert
            expectedEvent.VerifySavedOnce(actualEvent =>
            {
                Assert.AreNotEqual(default(Guid), actualEvent.ForumThreadId, "ForumThreadId");
                Assert.AreEqual(createVm.Title, actualEvent.Title, "Title");
                Assert.AreEqual(createVm.Content, actualEvent.Content, "Content");
                Assert.AreEqual(2, actualEvent.Tags.Length, "Tags Count");
                Assert.AreEqual(Config.UserName, actualEvent.AuthorUserName);
            });
        }

        [TestMethod]
        public void ForumController_EditThread_Produces_ForumThreadEdited_Event()
        {
            //setup
            var expectedEvent = ListenInEventStoreFor<ForumThreadEdited>();
            var editVm = new EditForumThreadViewModel
            {
                Id = ExistingThreadId,
                Title = "Title_edited",
                Content = "Content_edited",
                Tags = "ABC"
            };

            ForumController controller = CreateAuthenticatedController(Config.UserName);

            //action
            IActionResult actionResult = controller.Edit(ExistingThreadId, editVm);

            //assert
            expectedEvent.VerifySavedOnce(actualEvent =>
            {
                Assert.AreEqual(ExistingThreadId, actualEvent.ForumThreadId, "ForumThreadId");
                Assert.AreEqual(editVm.Title, actualEvent.Title, "Title");
                Assert.AreEqual(editVm.Content, actualEvent.Content, "Content");
                Assert.AreEqual(1, actualEvent.Tags.Length, "Tags Count");
                Assert.AreEqual(Config.UserName, actualEvent.EditorUserName);
            });
        }

        [TestMethod]
        public void ForumController_DeleteThread_Produces_ForumThreadDeleted_Event()
        {
            //setup
            var expectedEvent = ListenInEventStoreFor<ForumThreadDeleted>();

            ForumController controller = CreateAuthenticatedController(Config.UserName);

            //action
            IActionResult actionResult = controller.Delete(ExistingThreadId);

            //assert
            expectedEvent.VerifySavedOnce(actualEvent =>
            {
                Assert.AreEqual(ExistingThreadId, actualEvent.ForumThreadId, "Title");
                Assert.AreEqual(Config.UserName, actualEvent.UserName, "UserName");
            });
        }

        [TestMethod]
        public async Task ForumController_NewPost_Produces_ForumItemPosted_Event()
        {
            //setup
            var expectedEvent = ListenInEventStoreFor<ForumItemPosted>();
            var newAnwserVm = new NewAnswerViewModel { Content = "Content", };

            ForumController controller = CreateAuthenticatedController(Config.UserName);

            //action
            IActionResult actionResult = await controller.NewPost(ExistingThreadId, newAnwserVm);

            //assert
            expectedEvent.VerifySavedOnce(actualEvent =>
            {
                Assert.AreEqual(ExistingThreadId, actualEvent.ForumThreadId, "ForumThreadId");
                Assert.AreEqual(newAnwserVm.Content, actualEvent.Content, "Content");
                Assert.AreEqual(Config.UserName, actualEvent.AuthorUserName, "Username");
            });
        }

        [TestMethod]
        public void ForumController_EditPost_Produces_ForumItemEdited_Event()
        {
            //setup
            var expectedEvent = ListenInEventStoreFor<ForumItemPosted>();
            var editAnwserVm = new EditAnswerViewModel
            {
                ForumPostId = ExistingPostId,
                Content = "Content-edited",
            };

            ForumController controller = CreateAuthenticatedController(Config.UserName);

            //action
            IActionResult actionResult = controller.EditPost(ExistingThreadId, editAnwserVm);

            //assert
            expectedEvent.VerifySavedOnce(actualEvent =>
            {
                Assert.AreEqual(ExistingThreadId, actualEvent.ForumThreadId, "ForumThreadId");
                Assert.AreEqual(ExistingPostId, actualEvent.ForumItemId, "ForumItemId");
                Assert.AreEqual(editAnwserVm.Content, actualEvent.Content, "Content");
                Assert.AreEqual(Config.UserName, actualEvent.AuthorUserName, "Username");
            });
        }

        [TestMethod]
        public void ForumController_DeletePost_Produces_ForumItemEdited_Event()
        {
            //setup
            var expectedEvent = ListenInEventStoreFor<ForumItemDeleted>();
            ForumController controller = CreateAuthenticatedController(Config.UserName);

            //action
            IActionResult actionResult = controller.DeletePost(ExistingThreadId, ExistingPostId);

            //assert
            expectedEvent.VerifySavedOnce(actualEvent =>
            {
                Assert.AreEqual(ExistingThreadId, actualEvent.ForumThreadId, "ForumThreadId");
                Assert.AreEqual(ExistingPostId, actualEvent.ForumItemId, "ForumItemId");
                Assert.AreEqual(Config.UserName, actualEvent.UserName, "Username");
            });
        }
    }
}
