using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Microsoft.Extensions.Options;
using DevPortal.Web.AppCode.Config;
using Newtonsoft.Json;
using DevPortal.Web.Services;
using DevPortal.CommandStack.Infrastructure;
using DevPortal.CommandStack.Events;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DevPortal.Web.Controllers
{
    [Route("api/image")]
    public class ImageController : Controller
    {
        private readonly IImageStore _imageStore;
        private readonly IEventStore _eventStore;

        public ImageController(IImageStore imageStore, IEventStore eventStore)
        {
            _imageStore = imageStore;
            _eventStore = eventStore;
        }

        [HttpPost("upload")]
        public async Task<string> Upload(IFormFile image, string thumbnail = null)
        {
            thumbnail = thumbnail?.ToLower();
            try
            {
                using (var inputStream = image.OpenReadStream())
                {
                    var imageData = await _imageStore.SaveAsync(inputStream);

                    _eventStore.Save(new ImageUploaded
                    {
                        ImageId = imageData.Id,
                        Link = imageData.Link,
                        System = imageData.Source,
                        UserName = User.Identity.Name,
                    });

                    if (thumbnail == "smallsquare") return imageData.SmallSquare;
                    if (thumbnail == "bigsquare") return imageData.BigSquare;
                    return imageData.Link;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Upload to imgur failed", ex);
            }
        }
    }
}
