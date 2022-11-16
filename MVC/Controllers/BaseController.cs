using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;

namespace MVC.Controllers
{
    public class BaseController : Controller
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string logging = string.Empty;
            ViewBag.TokenExpires = null;
            try
            {
                var expiresAtString = await context.HttpContext.GetTokenAsync("expires_at");
                if (expiresAtString != null)
                {
                    var utcTime = DateTimeOffset.Parse(expiresAtString).UtcDateTime;
                    ViewBag.TokenExpires = utcTime.ToString("o", CultureInfo.InvariantCulture);
                }
            }
            catch (Exception ex)
            {
                logging = ex.Message;
                // Log
            }
            await next();
        }
    }
}
