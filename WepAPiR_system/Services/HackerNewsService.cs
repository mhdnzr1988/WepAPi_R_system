using Microsoft.Extensions.Caching.Memory;
using WepAPiR_system.Models;
using WepAPiR_system.Repository;

namespace WepAPiR_system.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly IHackerNewsRepository _repository;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly int Count200Stories;

        public HackerNewsService(IHackerNewsRepository repository, IMemoryCache cache, IConfiguration configuration)
        {
            _repository = repository;
            _cache = cache;
            _configuration= configuration;
            Count200Stories = Convert.ToInt16(_configuration.GetConnectionString("Top200Stories"));
        }
       

        public async Task<IEnumerable<Story>> GetNewestStoriesAsync(int page, string query = null)
        {

            var normalizedQuery = string.IsNullOrWhiteSpace(query)? "all" : query.Trim().ToLowerInvariant();//If the query is empty or whitespace, set it to "all"; otherwise, normalize it (trim and lowercase) for consistent cache key formatting

            var cacheKey = $"newStories_page{page}_query_{normalizedQuery}"; //unique cache key based on the page, page size, and search query so results can be reused 


            //Check if the results for this key are already cached.
            if (_cache.TryGetValue(cacheKey, out List<Story> cachedStories))
            {
                return cachedStories; //return the cached list immediately
            }

            var storyIds = await _repository.GetNewStoryIdsAsync() ?? new List<int>(); ; //Fetch a list of IDs for the newest stories from the repository.
            var tasks = storyIds.Select(id => _repository.GetStoryByIdAsync(id));
            var allStories = await Task.WhenAll(tasks);

            // Apply filters and take up to Count200Stories
            var stories = new List<Story>();
             stories = allStories
                .Where(story => story != null &&
                                !string.IsNullOrEmpty(story.Title) &&
                                !string.IsNullOrEmpty(story.Url) &&
                                (string.IsNullOrEmpty(query) || story.Title.Contains(query, StringComparison.OrdinalIgnoreCase))) //Filter data as title and url is not null
                .Take(Count200Stories) // Fetch only top 200 records.
                .ToList();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)); //Cache the filtered list of stories for 5 minutes using the earlier-generated cache key
            _cache.Set(cacheKey, stories, cacheEntryOptions);

            return stories;//Return the list of stories.
        }
    }

}
