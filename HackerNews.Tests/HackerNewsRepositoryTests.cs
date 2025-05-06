using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WepAPiR_system.Models;
using WepAPiR_system.Repository;
using WepAPiR_system.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
//using Xunit;

namespace HackerNews.Tests
{
    

    public class HackerNewsRepositoryTests
    {
        private const string _newStoriesUrl = "https://hacker-news.firebaseio.com/v0/newstories.json";
        private const string _storyUrl = "https://hacker-news.firebaseio.com/v0/item/";
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _configMock;
        private readonly HackerNewsRepository _repository;


        public HackerNewsRepositoryTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _configMock = new Mock<IConfiguration>();

            _configMock.Setup(c => c.GetConnectionString("NewStoriesUrl")).Returns(_newStoriesUrl);
            _configMock.Setup(c => c.GetConnectionString("storyUrl")).Returns(_storyUrl);

            _repository = new HackerNewsRepository(_httpClient, _configMock.Object);
        }

        [Fact]
        public async Task GetNewStoryIdsAsync_ReturnsListOfIds()
        {
            // Arrange
            var expectedIds = new List<int> { 1, 2, 3 };
            var json = JsonConvert.SerializeObject(expectedIds);

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.ToString().Contains("newstories")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            // Act
            var result = await _repository.GetNewStoryIdsAsync();

            // Assert
            Assert.Equal(expectedIds, result);
        }

        [Fact]
        public async Task GetStoryByIdAsync_ReturnsStoryObject()
        {
            // Arrange
            var story = new Story
            {
                Id = 123,
                Title = "Test Story",
                Url = "https://example.com/story"
            };
            var json = JsonConvert.SerializeObject(story);

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(m => m.RequestUri.ToString().Contains("123.json")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            // Act
            var result = await _repository.GetStoryByIdAsync(123);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(story.Id, result.Id);
            Assert.Equal(story.Title, result.Title);
            Assert.Equal(story.Url, result.Url);
        }


    }

}
