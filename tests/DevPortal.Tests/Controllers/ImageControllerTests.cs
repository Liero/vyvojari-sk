using DevPortal.CommandStack.Events;
using DevPortal.CommandStack.Infrastructure;
using DevPortal.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DevPortal.Web.Controllers
{
    [TestClass]
    public class ImageControllerTests : ControllerTestsBase<ImageController>
    {
        public Mock<IImageStore> ImageStoreMock { get; set; }
        private ImageData _fakeImageData;
        private Mock<IFormFile> _formFileMock;

        public override void Init()
        {
            base.Init();
            _fakeImageData = new ImageData
            {
                Type = "image/jpg",
                Link = "someimage.jpg",
                BigSquare = "someimage_big.jpg",
                SmallSquare = "someimage_big.jpg",
                Source = "ImageStoreMock",
            };

            ImageStoreMock = new Mock<IImageStore>();
            ImageStoreMock
                .Setup(store => store.SaveAsync(It.IsAny<Stream>()))
                .Returns(() => Task.FromResult(_fakeImageData));

            Services.AddSingleton(ImageStoreMock.Object);

            _formFileMock = new Mock<IFormFile>();
            _formFileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
        }

        [TestMethod]
        public async Task ImageController_Upload_StoresImageUploaded_Event()
        {
            ImageUploaded actualEvent = null;

            //setup
            EventStoreMock
                .Setup(es => es.Save(It.IsAny<ImageUploaded>()))
                .Callback<DomainEvent>(evt => actualEvent = (ImageUploaded)evt);

            ImageController controller = CreateAuthenticatedController("TestUser");

            //action
            var uploadResult = await controller.Upload(_formFileMock.Object);

            //assert
            Assert.AreEqual(uploadResult, _fakeImageData.Link);

            Assert.IsNotNull(actualEvent, "ImageUploaded was not stored");
            Assert.AreEqual(actualEvent.UserName, "TestUser");
            Assert.AreEqual(actualEvent.Link, uploadResult);
            Assert.AreEqual(actualEvent.System, _fakeImageData.Source);
        }

        [TestMethod]
        public async Task ImageController_Upload_WhenThumbnailSpecifiend_ReturnsThumbnail()
        {
            //setup
            ImageController controller = CreateAuthenticatedController("TestUser");

            //action
            var smallSquareResult = await controller.Upload(_formFileMock.Object, thumbnail: "smallsquare");
            var bigSquareResult = await controller.Upload(_formFileMock.Object, thumbnail: "bigsquare");
            
            //assert
            Assert.AreEqual(smallSquareResult, _fakeImageData.SmallSquare);
            Assert.AreEqual(bigSquareResult, _fakeImageData.BigSquare);
        }
    }
}
