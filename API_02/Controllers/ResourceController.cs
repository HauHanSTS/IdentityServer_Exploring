using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_02.Controllers
{
    [Route("api/resource")]
    [ApiController]
    [Authorize(Policy = "CanAccessResource")]
    public class ResourceController : ControllerBase
    {
        [HttpGet]
        public IActionResult ReadApiResource()
        {
            string result = "You can read API 02 resources";
            return Ok(result);
        }

        [HttpPost]
        public IActionResult WriteApiResource()
        {
            string result = "You can write API 02 resources";
            return Ok(result);
        }
    }
}
