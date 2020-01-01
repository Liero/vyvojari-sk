using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevPortal.Web.NewEventSourcing
{
    [TestClass]
    public class DenormalizerInvokerTests
    {
        ServiceProvider _provider;

        public DenormalizerInvokerTests()
        {
            var services = new ServiceCollection();
            services.AddDbContext<DevPortalDbContext>(options => options.UseInMemoryDatabase("DevPortalDbContext"));
            _provider = services.BuildServiceProvider();
        }

        [TestMethod]
        public async Task DenormalizerInvoker_InvokesHandleMethod()
        {
            DenormalizerInvoker denormalizerInvoker = new DenormalizerInvoker(
                typeof(Denormalizer1),
                _provider);

            var evt = new Evt1();
            await denormalizerInvoker.Invoke(evt);

            Assert.AreEqual(evt, Denormalizer1.LastHandledEvent);
        }


        //fakes
        private class Evt1 : DomainEvent
        {

        }

        private class Denormalizer1
        {
            public static DomainEvent LastHandledEvent { get; private set; }

            public Task Handle(Evt1 evt)
            {
                LastHandledEvent = evt;
                return Task.CompletedTask;
            }
        }
    }
}
