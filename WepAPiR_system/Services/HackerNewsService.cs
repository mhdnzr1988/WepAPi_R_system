using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using WepAPiR_system.Models;
using Microsoft.Extensions.Options;
using WepAPiR_system.Repository;

namespace WepAPiR_system.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly IHackerNewsRepository _repository;
        private readonly IMemoryCache _cache;

        public HackerNewsService(IHackerNewsRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<IEnumerable<Story>> GetNewestStoriesAsync(int page, int pageSize, string query = null)
        {
            var cacheKey = $"newStories_{page}_{pageSize}_{query}";
            if (_cache.TryGetValue(cacheKey, out List<Story> cachedStories))
            {
                return cachedStories;
            }

            var storyIds = await _repository.GetNewStoryIdsAsync();
            
            var pagedIds = storyIds
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var stories = new List<Story>();
            foreach (var id in pagedIds)
            {
                var story = await _repository.GetStoryByIdAsync(id);
                if (story != null && story.Title != null && story.Url != null &&
                    (string.IsNullOrEmpty(query) || (story.Title?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)))
                {
                    stories.Add(story);
                }
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            _cache.Set(cacheKey, stories, cacheEntryOptions);

            return stories;
        }
    }

}
