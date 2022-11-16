namespace MVC.Middlewares
{
    public static class RefreshTokensMiddlewareExtension
    {
        public static IApplicationBuilder UseRefreshTokens(
        this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RefreshTokensMiddleware>();
        }
    }
}
