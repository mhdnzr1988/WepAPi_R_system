using Microsoft.AspNetCore.Mvc;
using WepAPiR_system.Models;
using WepAPiR_system.Services;

namespace WepAPiR_system.BusinessLogic
{
    public class BAL_HackerNews
    {
        private readonly IHackerNewsService _service;
        public BAL_HackerNews(IHackerNewsService service) {
            _service = service;
        }

        public async Task<IEnumerable<Story>> GetNewestStoriesAsync(int page, int pageSize, string query = null)
        {
            return await _service.GetNewestStoriesAsync(page, pageSize, query);           
        }
    }
}
