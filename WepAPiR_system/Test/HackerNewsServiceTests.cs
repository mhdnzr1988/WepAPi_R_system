using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using WepAPiR_system.Models;
using WepAPiR_system.Services;
using Xunit;

namespace WepAPiR_system.Test
{
    public class HackerNewsServiceTests
    {
        [Fact]
        public async Task GetNewestStoriesAsync_ReturnsFilteredStories()
        {
            // Arrange
            var storyIds = new List<int> { 1, 2 };
            var stories = new List<Story>
        {
            new Story { Id = 1, Title = "Hello HackerNews" },
            new Story { Id = 2, Title = "Another Post" }
        };

            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(m => m.RequestUri!.ToString().Contains("newstories")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(storyIds))
                });

            foreach (var story in stories)
            {
                handlerMock.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.Is<HttpRequestMessage>(m => m.RequestUri!.ToString().Contains($"{story.Id}.json")),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(JsonConvert.SerializeObject(story))
                    });
            }

            var httpClient = new HttpClient(handlerMock.Object);

            var cache = new MemoryCache(new MemoryCacheOptions());

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c.GetConnectionString("NewStoriesUrl"))
                      .Returns("https://hacker-news.firebaseio.com/v0/newstories.json");
            configMock.Setup(c => c.GetConnectionString("storyUrl"))
                      .Returns("https://hacker-news.firebaseio.com/v0/item");

            var service = new HackerNewsService(httpClient, cache, configMock.Object);

            // Act
            var result = await service.GetNewestStoriesAsync(1, 2, "Hacker");

            // Assert
            Assert.Single(result);
            Assert.Contains(result, s => s.Title.Contains("Hacker"));
        }
    }
}
