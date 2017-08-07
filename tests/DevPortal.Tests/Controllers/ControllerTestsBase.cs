using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace DevPortal.Web.Controllers
{
    public class ControllerTestsBase<TController> where TController : Controller
    {
        public Mock<IEventStore> EventStoreMock { get; set; }
        public TController Controller { get; set; }
        protected ServiceCollection Services { get; set; }

        [TestInitialize]
        public virtual void Init()
        {
            EventStoreMock = new Mock<IEventStore>();
            Services = new ServiceCollection();
            Services.AddTransient<TController, TController>();
            Services.AddSingleton<IEventStore>(EventStoreMock.Object);
            Services.AddTransient<DevPortalDbContext>();
        }

        public virtual TController CreateController()
        {
            var controller = Services.BuildServiceProvider().GetRequiredService<TController>();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            return controller;
        }
        public virtual TController CreateAuthenticatedController(string userName)
        {
            var controller = CreateController();
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, userName)
            }, "FakeAuthentication"));
            return controller;
        }


    }
}
