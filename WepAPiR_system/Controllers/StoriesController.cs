using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WepAPiR_system.BusinessLogic;
using WepAPiR_system.Services;

namespace WepAPiR_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoriesController : ControllerBase
    {

        private readonly IHackerNewsService _service;
        public StoriesController(IHackerNewsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int page, int pageSize, string query = null)        
        {
            BAL_HackerNews obj_Hacker = new BAL_HackerNews(_service);
            var stories = await obj_Hacker.GetNewestStoriesAsync(page, pageSize, query);
            return Ok(stories);
        }
    }

}
