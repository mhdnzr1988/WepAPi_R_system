using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using WepAPiR_system.Models;
using Microsoft.Extensions.Options;
using WepAPiR_system.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

            var normalizedQuery = string.IsNullOrWhiteSpace(query)? "all" : query.Trim().ToLowerInvariant();//If the query is empty or whitespace, set it to "all"; otherwise, normalize it (trim and lowercase) for consistent cache key formatting

            var cacheKey = $"newStories_page{page}_size{pageSize}_query_{normalizedQuery}"; //unique cache key based on the page, page size, and search query so results can be reused 


            //Check if the results for this key are already cached.
            if (_cache.TryGetValue(cacheKey, out List<Story> cachedStories))
            {
                return cachedStories; //return the cached list immediately
            }

            var storyIds = await _repository.GetNewStoryIdsAsync(); //Fetch a list of IDs for the newest stories from the repository.

            //var pagedIds = storyIds .Skip((page - 1) * pageSize) .Take(pageSize); // remove pagination from api

            var stories = new List<Story>();
            //fetch the full story object asynchronously
            foreach (var id in storyIds)
            {
                var story = await _repository.GetStoryByIdAsync(id);
                if (story != null && story.Title != null && story.Url != null &&
                    (string.IsNullOrEmpty(query) || (story.Title?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false))) //Filter data as title and url is not null
                {
                    stories.Add(story); // insert all story in stories list
                }
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)); //Cache the filtered list of stories for 5 minutes using the earlier-generated cache key
            _cache.Set(cacheKey, stories, cacheEntryOptions);
            return stories;//Return the list of stories.
        }
    }

}
