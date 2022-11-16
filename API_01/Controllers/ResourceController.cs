using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_01.Controllers
{
    [Route("api/resource")]
    [ApiController]
    [Authorize(Policy = "CanAccessResource")]
    public class ResourceController : ControllerBase
    {
        [HttpGet]
        public IActionResult ReadApiResource()
        {
            string result = "You can read API 01 resources";
            return Ok(result);
        }

        [HttpPost]
        public IActionResult WriteApiResource()
        {
            string result = "You can write API 01 resources";
            return Ok(result);
        }
    }
}
