using DevPortal.Web.AppCode.Config;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevPortal.Web.Services
{
    public interface IImageStore
    {
        Task<ImageData> SaveAsync(Stream fileStream);
    }

    public class ImgurImageStore : IImageStore
    {
        HttpClient _httpClient;

        public ImgurImageStore(IOptions<Imgur> imgurOptions)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Client-ID " + imgurOptions.Value.ClientId);
        }

        public async Task<ImageData> SaveAsync(Stream fileStream)
        {
            var form = new MultipartFormDataContent();
            form.Add(new StreamContent(fileStream), "image");

            var httpResponse = await _httpClient.PostAsync("https://api.imgur.com/3/image", form);
            httpResponse.EnsureSuccessStatusCode();

            var responseString = await httpResponse.Content.ReadAsStringAsync();
            var responseJson = Newtonsoft.Json.Linq.JObject.Parse(responseString);
            ImageData imageData = responseJson["data"].ToObject<ImageData>();
            imageData.Source = "imgur";

            var imageExtension = Path.GetExtension(imageData.Link);
            imageData.SmallSquare = $"http://i.imgur.com/{imageData.Id}s{imageExtension}";
            imageData.BigSquare = $"http://i.imgur.com/{imageData.Id}b{imageExtension}";
            return imageData;
        }
    }

    public class ImageData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        public string Source { get; set; }

        /// <summary>
        /// link to 90x90 cropped image
        /// </summary>
        public string SmallSquare { get; set; }
        /// <summary>
        /// link to 160x160 cropped image
        /// </summary>
        public string BigSquare { get; set; }
    }
}
