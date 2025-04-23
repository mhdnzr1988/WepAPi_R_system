using Microsoft.Extensions.Caching.Memory;
using Moq;
using WepAPiR_system.Services;
using Xunit;

namespace WepAPiR_system.Test
{
    public class HackerNewsServiceTests
    {
        [Fact]
        public async Task GetNewestStoriesAsync_ReturnsStories()
        {
            var mockHttp = new Mock<HttpClient>();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var service = new HackerNewsService(new HttpClient(), memoryCache);

            var stories = await service.GetNewestStoriesAsync(1, 10);
            Assert.NotNull(stories);
        }
    }
}
