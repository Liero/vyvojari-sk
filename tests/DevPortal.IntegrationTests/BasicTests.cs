using DevPortal.IntegrationTests.Helpers;
using DevPortal.Web.Models.NewsViewModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DevPortal.IntegrationTests
{
    [TestClass]
    public class BasicTests
    {
        private readonly TestWebApplicationFactory _factory;

        public BasicTests()
        {
            _factory = new TestWebApplicationFactory();
        }

        [DataTestMethod]
        [DataRow("/")]
        [DataRow("/News")]
        [DataRow("/Forum")]
        [DataRow("/Blog")]
        public async Task CanAccessBasicPagesWithoutLogin(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.AreEqual("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [TestMethod]
        public async Task CanCreateNewsItem()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(
                name: AuthenticationMiddewareMock.TestingHeaderPrefix + "Name",
                value: "IntegrationTestsUser");

            var response = await client.PostAsync("News/Create", new System.Net.Http.StringContent(
                JsonConvert.SerializeObject(new CreateNewsItemViewModel {
                    Title = "TestTitle",
                    Content = "TestContent",
                    Tags = "test,integration-test"
                })));

            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.Redirect);
            StringAssert.StartsWith(response.Headers.Location.PathAndQuery, "News/Detail/");
        }
    }
}
