using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack;
using DevPortal.QueryStack.Model;
using DevPortal.Web.AppCode.EventSourcing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace DevPortal.Web.Controllers
{
    public class ControllerTestsBase<TController> where TController : Controller
    {
        private ServiceProvider _serviceProvider;

        public Mock<IEventStore> EventStoreMock { get; set; }
        public TController Controller { get; set; }
        protected ServiceCollection Services { get; set; }

        protected ServiceProvider ServiceProvider
        {
            get
            {
                return _serviceProvider ?? (_serviceProvider = Services.BuildServiceProvider());
            }
        }

        public T GetService<T>() => ServiceProvider.GetRequiredService<T>();

        [TestInitialize]
        public void TestInitialize() => Init();

        public virtual void Init()
        {
            EventStoreMock = new Mock<IEventStore>();
            Services = new ServiceCollection();
            Services.AddSingleton<IEventStore>(EventStoreMock.Object);
            Services.AddDbContext<DevPortalDbContext>(options =>
                options.UseInMemoryDatabase("DevPortalDbContext"), ServiceLifetime.Transient);

            Services.AddSingleton<IHandlerNotifications, Mocks.HandlerNotificationsStub>();
            EventStoreExtensions.HandlerNotificationAccessor = () => ServiceProvider.GetRequiredService<IHandlerNotifications>();
        }


        public virtual TController CreateController()
        {
            var controller = ActivatorUtilities.CreateInstance<TController>(ServiceProvider);
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

        protected EventStoreListener<T> ListenInEventStoreFor<T>() where T : DomainEvent
        {
            return new EventStoreListener<T>(EventStoreMock);
        }

        protected ICollection<Tag> CreateTags(Guid contentId, params string[] tags)
        {
            return tags
                .Select(t => new Tag
                {
                    ContentId = contentId,
                    Name = t
                }).ToList();
        }
    }
}
