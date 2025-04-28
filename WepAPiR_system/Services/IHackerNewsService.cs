using Castle.Components.DictionaryAdapter.Xml;
using WepAPiR_system.Models;

namespace WepAPiR_system.Services
{
    public interface IHackerNewsService
    {
        Task<IEnumerable<Story>> GetNewestStoriesAsync(int page, int pageSize, string query = null);

    }

}
