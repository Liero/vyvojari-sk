using DevPortal.CommandStack.Events;
using DevPortal.QueryStack.Denormalizers;
using DevPortal.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rebus.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevPortal.Web
{
    [TestClass]
    public class Foo
    {
        [TestMethod]
        public void Test()
        {
            var denormalizer = SampleDataController.GetDenormalizers(new NewsItemPublished());

        }
    }
}
