using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using WepAPiR_system.Models;
using Microsoft.Extensions.Options;

namespace WepAPiR_system.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly string NewStoriesUrl = "";
        private readonly string storyUrl = "";

        public HackerNewsService(HttpClient httpClient, IMemoryCache cache, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _cache = cache;
            NewStoriesUrl = configuration.GetConnectionString("NewStoriesUrl");
            //NewStoriesUrl = _appSettings.NewStoriesUrl;
            storyUrl = configuration.GetConnectionString("storyUrl");
        }
        public async Task<IEnumerable<Story>> GetNewestStoriesAsync(int page, int pageSize, string query = null)
        {
            var cacheKey = $"newStories_{page}_{pageSize}_{query}";
            if (_cache.TryGetValue(cacheKey, out List<Story> cachedStories))
            {
                return cachedStories;
            }

            var response = await _httpClient.GetStringAsync(NewStoriesUrl);
            var storyIds = JsonConvert.DeserializeObject<List<int>>(response);

            var pagedIds = storyIds
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var stories = new List<Story>();
            foreach (var id in pagedIds)
            {
                var storyUrl1 = storyUrl + id + ".json";
                var storyJson = await _httpClient.GetStringAsync(storyUrl1);
                var story = JsonConvert.DeserializeObject<Story>(storyJson);

                if (story != null && story.Title != null && story.Url != null &&(string.IsNullOrEmpty(query)  || (story.Title?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)))
                {
                    stories.Add(story);
                }
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            _cache.Set(cacheKey, stories, cacheEntryOptions);

            return stories;
        }

        public async Task<IEnumerable<Story>> GetNewestStoriesAsync2(int page, int pageSize, string query = null)
        {
            var cacheKey = "newStories";
            var storyIds = new List<int>();
            if (!_cache.TryGetValue(cacheKey, out List<Story> stories))
            {
                return stories;
            }
            var response = await _httpClient.GetStringAsync(NewStoriesUrl);
            storyIds = JsonConvert.DeserializeObject<List<int>>(response);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));


            var pagedIds1 = storyIds.Skip((page - 1) * pageSize).Take(pageSize);

            var pagedIds = storyIds.Skip((page - 1));


            foreach (var id in pagedIds)
            {
                var storyUrl1 = storyUrl + id + ".json";//  https://hacker-news.firebaseio.com/v0/item/43793595.json
                var storyJson = await _httpClient.GetStringAsync(storyUrl1);
                var story = JsonConvert.DeserializeObject<Story>(storyJson);

                if (string.IsNullOrEmpty(query) || (story?.Title != null && story.Title.Contains(query, StringComparison.OrdinalIgnoreCase)))
                {
                    stories.Add(story);
                }
            }
            _cache.Set(cacheKey, stories, cacheEntryOptions);


            return stories;
        }

        public async Task<IEnumerable<Story>> GetNewestStoriesAsync1(int page, int pageSize, string query = null)
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
                var storyUrl1 = storyUrl + id + ".json";//  https://hacker-news.firebaseio.com/v0/item/43793595.json
                var storyJson = await _httpClient.GetStringAsync(storyUrl1);
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
