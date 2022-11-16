using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_01.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize(Policy = "CanAccessUser")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public IActionResult ReadUsers()
        {
            string result = "You can read API 01 Users";
            return Ok(result);
        }

        [HttpPost]
        public IActionResult WriteUsers()
        {
            string result = "You can write API 01 Users";
            return Ok(result);
        }
    }
}
