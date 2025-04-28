using WepAPiR_system.Models;

namespace WepAPiR_system.Repository
{
    public interface IHackerNewsRepository
    {
            Task<List<int>> GetNewStoryIdsAsync();
            Task<Story> GetStoryByIdAsync(int id);
    }
}
