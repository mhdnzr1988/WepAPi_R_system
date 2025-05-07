using Microsoft.AspNetCore.Mvc;
using WepAPiR_system.Services;

namespace WepAPiR_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoriesController : ControllerBase
    {

        private readonly IHackerNewsService _service;
        public StoriesController(IHackerNewsService service) // inject DI
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int page, string query = null)        
        {
            var stories = await _service.GetNewestStoriesAsync(page, query); // Call the service method 
            return Ok(stories);
        }
    }

}
