using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static IdentityModel.OidcConstants;

namespace API_01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalApiController : ControllerBase
    {
        [HttpGet]
        [Route("GetApi02ResourcesUsingSharedSecret")]
        [AllowAnonymous]
        public async Task<IActionResult> GetApi02ResourcesUsingSharedSecret()
        {
            string result = await ExternalApiHandler.ReadProtectedApiReousrceBySharedSecret();
            return Ok(result);
        }

        [HttpGet]
        [Route("GetApi02ResourcesUsingX509Certs")]
        [AllowAnonymous]
        public async Task<IActionResult> GetApi02ResourcesUsingX509Certs()
        {
            string result = await ExternalApiHandler.ReadProtectedApiReousrceByX509Certs();
            return Ok(result);
        }

        [HttpGet]
        [Route("GetApi02ResourcesUsingJWK")]
        [AllowAnonymous]
        public async Task<IActionResult> GetApi02ResourcesUsingJWK()
        {
            string result = await ExternalApiHandler.ReadProtectedApiReousrceByJWK();
            return Ok(result);
        }
    }
}
