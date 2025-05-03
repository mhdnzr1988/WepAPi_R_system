using Newtonsoft.Json;
using WepAPiR_system.Models;

namespace WepAPiR_system.Repository
{
    public class HackerNewsRepository : IHackerNewsRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _newStoriesUrl;
        private readonly string _storyUrl;

        public HackerNewsRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _newStoriesUrl = configuration.GetConnectionString("NewStoriesUrl");
            _storyUrl = configuration.GetConnectionString("storyUrl");
        }
        //To retrieve the IDs of new stories from URL
        public async Task<List<int>> GetNewStoryIdsAsync()
        {
            var response = await _httpClient.GetStringAsync(_newStoriesUrl);
            return JsonConvert.DeserializeObject<List<int>>(response); //Returns the list of story IDs.
        }
        //eturns a Story object.
        public async Task<Story> GetStoryByIdAsync(int id)
        {
            var url = $"{_storyUrl}{id}.json";
            var response = await _httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<Story>(response);//Returns the resulting Story
        }
    }

}
