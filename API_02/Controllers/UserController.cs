using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_02.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize(Policy = "CanAccessUser")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        //[Authorize(Roles = "HR, IT")]
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "HR")]
        public IActionResult ReadUsers()
        {
            string result = "You can read API 02 Users";
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "CanWriteUser")]
        public IActionResult WriteUsers()
        {
            string result = "You can write API 02 Users";
            return Ok(result);
        }
    }
}
