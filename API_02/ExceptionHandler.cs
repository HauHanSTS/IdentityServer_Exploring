using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API_02
{
    public class ExceptionHandler : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.Result = new ObjectResult(context.Exception.Message + "; " + context.Exception.InnerException?.Message) {
                StatusCode = 200
            };
            context.ExceptionHandled = true;
        }
    }
}
