using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class LoginController : BaseController
    {
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}
