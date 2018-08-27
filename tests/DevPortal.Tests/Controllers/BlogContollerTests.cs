using DevPortal.CommandStack.Events;
using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack;
using DevPortal.Web.Models.BlogViewModels;
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
    public class BlogControllerTests : ControllerTestsBase<BlogController>
    {
        readonly Guid ExistingBlogId = Guid.Parse("ab4b69a9-664b-40b3-bebb-67e9f236828f");

        public override void Init()
        {
            base.Init();

            //setup existing data, so Edit/Delete can be tested
            using (var dbContext = ServiceProvider.GetRequiredService<DevPortalDbContext>())
            {
                dbContext.Blogs.Add(new Blog
                {
                    Id = ExistingBlogId,
                    Title = "Exiting Blog Thread",
                    Description = "Some content",
                    ExternalUrl = "http://someurl.test",
                    CreatedBy = Config.UserName,
                    Created = DateTime.UtcNow,                   
                });
                dbContext.SaveChanges();
            }
        }

        [TestMethod]
        public async Task BlogController_CreateLink_Produces_BlogLinkAdded_Event()
        {
            //setup
            var expectedEvent = ListenInEventStoreFor<BlogLinkCreated>();
            var createVm = new CreateLinkViewModel
            {
                Title = "Title",
                Description = "Description",
                Link = "http://linktoexternalblog.com"
            };

            BlogController controller = CreateAuthenticatedController(Config.UserName);

            //action
            IActionResult actionResult = await controller.CreateLink(createVm);

            //assert
            expectedEvent.VerifySavedOnce(actualEvent =>
            {
                Assert.AreNotEqual(default(Guid), actualEvent.BlogId, "BlogId");
                Assert.AreEqual(createVm.Title, actualEvent.Title, "Title");
                Assert.AreEqual(createVm.Description, actualEvent.Description, "Description");
                Assert.AreEqual(createVm.Link, actualEvent.Url, "Url");
                Assert.AreEqual(Config.UserName, actualEvent.UserName);
            });
        }

       
        [TestMethod]
        public async Task BlogController_DeleteBlog_Produces_BlogDeleted_Event()
        {
            //setup
            var expectedEvent = ListenInEventStoreFor<BlogDeleted>();

            BlogController controller = CreateAuthenticatedController(Config.UserName);

            //action
            IActionResult actionResult = await controller.Delete(ExistingBlogId);

            //assert
            expectedEvent.VerifySavedOnce(actualEvent =>
            {
                Assert.AreEqual(ExistingBlogId, actualEvent.BlogId, "BlogId");
                Assert.AreEqual(Config.UserName, actualEvent.UserName, "UserName");
            });
        }

    }
}
