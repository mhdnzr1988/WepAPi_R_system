using Microsoft.Extensions.Caching.Memory;
using Moq;
using WepAPiR_system.Models;
using WepAPiR_system.Repository;
using WepAPiR_system.Services;

namespace HackerNews.Tests
{
    public class HackerNewsServiceTests
    {
        private readonly Mock<IHackerNewsRepository> _repositoryMock;
        private readonly IMemoryCache _memoryCache;
        private readonly HackerNewsService _service;

        public HackerNewsServiceTests()
        {
            _repositoryMock = new Mock<IHackerNewsRepository>();

            // Create a real memory cache instance for testing
            var memoryCacheOptions = new MemoryCacheOptions();
            _memoryCache = new MemoryCache(memoryCacheOptions);

            _service = new HackerNewsService(_repositoryMock.Object, _memoryCache);
        }
        [Fact]
        public async Task GetNewestStoriesAsync_ReturnsFilteredStories_WhenNotCached()
        {
            // Arrange
            var storyIds = new List<int> { 1, 2, 3 };
            _repositoryMock.Setup(r => r.GetNewStoryIdsAsync())
                .ReturnsAsync(storyIds);

            _repositoryMock.Setup(r => r.GetStoryByIdAsync(1))
                .ReturnsAsync(new Story { Id = 1, Title = "C# is awesome", Url = "http://example.com" });

            _repositoryMock.Setup(r => r.GetStoryByIdAsync(2))
                .ReturnsAsync(new Story { Id = 2, Title = "Learn Java", Url = "http://example.com" });

            _repositoryMock.Setup(r => r.GetStoryByIdAsync(3))
                .ReturnsAsync(new Story { Id = 3, Title = null, Url = "http://example.com" }); // Should be filtered out

            // Act
            var result = await _service.GetNewestStoriesAsync(page: 1,query: "C#");

            // Assert
            var storyList = result.ToList();
            Assert.Single(storyList);
            Assert.Equal("C# is awesome", storyList[0].Title);
        }

        [Fact]
        public async Task GetNewestStoriesAsync_ReturnsFromCache_IfCached()
        {
            // Arrange
            var cacheKey = "newStories_page1_query_all";
            var cachedStories = new List<Story>
            {
                new Story { Id = 99, Title = "Cached Story", Url = "http://example.com" }
            };
            

            // Pre-populate cache
            _memoryCache.Set(cacheKey, cachedStories, TimeSpan.FromMinutes(5));

            // Act
            var result = await _service.GetNewestStoriesAsync(page: 1,  query: null);


            // Assert
            var storyList = result.ToList();
            Assert.Single(storyList);
            Assert.Equal("Cached Story", storyList[0].Title);

            // Ensure repository was never called
            
            _repositoryMock.Verify(r => r.GetNewStoryIdsAsync(), Times.Never);
        }
    }
}
    