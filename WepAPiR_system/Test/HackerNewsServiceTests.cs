using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using WepAPiR_system.Models;
using WepAPiR_system.Repository;
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

            var repositoryMock = new Mock<IHackerNewsRepository>();

            repositoryMock.Setup(r => r.GetNewStoryIdsAsync())
                .ReturnsAsync(storyIds);

            repositoryMock.Setup(r => r.GetStoryByIdAsync(1))
                .ReturnsAsync(stories[0]);

            repositoryMock.Setup(r => r.GetStoryByIdAsync(2))
                .ReturnsAsync(stories[1]);

            var cache = new MemoryCache(new MemoryCacheOptions());

            var service = new HackerNewsService(repositoryMock.Object, cache);

            // Act
            var result = await service.GetNewestStoriesAsync(1, 2, "Hacker");

            // Assert
            Assert.Single(result);
            Assert.Contains(result, s => s.Title.Contains("Hacker"));
        }
    }

}
