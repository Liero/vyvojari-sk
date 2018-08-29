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
    public class SampleDataControllerTests
    {
        [TestMethod]
        public void SampleDataController_GetDenormalizers()
        {
            var denormalizers = SampleDataController.GetDenormalizers(typeof(NewsItemPublished));
            CollectionAssert.Contains(denormalizers, typeof(NewsItemDenormalizer));
            CollectionAssert.Contains(denormalizers, typeof(ActivityDenormalizer));
        }
    }
}
