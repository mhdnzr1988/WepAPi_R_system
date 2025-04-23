using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using WepAPiR_system.Models;

namespace WepAPiR_system.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private const string NewStoriesUrl = "https://hacker-news.firebaseio.com/v0/newstories.json";

        public HackerNewsService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<IEnumerable<Story>> GetNewestStoriesAsync(int page, int pageSize, string query = null)
        {
            var cacheKey = "newStories";
            if (!_cache.TryGetValue(cacheKey, out List<int> storyIds))
            {
                var response = await _httpClient.GetStringAsync(NewStoriesUrl);
                storyIds = JsonConvert.DeserializeObject<List<int>>(response);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                _cache.Set(cacheKey, storyIds, cacheEntryOptions);
            }

            var pagedIds = storyIds.Skip((page - 1) * pageSize).Take(pageSize);
            var stories = new List<Story>();

            foreach (var id in pagedIds)
            {
                var storyUrl = $"https://hacker-news.firebaseio.com/v0/item/{id}.json";
                var storyJson = await _httpClient.GetStringAsync(storyUrl);
                var story = JsonConvert.DeserializeObject<Story>(storyJson);

                if (string.IsNullOrEmpty(query) || (story?.Title != null && story.Title.Contains(query, StringComparison.OrdinalIgnoreCase)))
                {
                    stories.Add(story);
                }
            }

            return stories;
        }
    }

}
