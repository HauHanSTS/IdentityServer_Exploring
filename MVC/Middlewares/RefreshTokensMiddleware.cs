using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace MVC.Middlewares
{
    public class RefreshTokensMiddleware
    {
        private readonly RequestDelegate _next;
        public RefreshTokensMiddleware(RequestDelegate next)
        {
            _next = next;   
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string logging = string.Empty;
            bool shouldBeRedirect = false;
            string controller = context.GetRouteValue("controller")?.ToString() ?? "";
            string action = context.GetRouteValue("action")?.ToString() ?? "";
            try
            {
                var expiresAtString = await context.GetTokenAsync("expires_at");
                if (expiresAtString != null)
                {
                    var expiresAt = DateTimeOffset.Parse(expiresAtString).UtcDateTime;
                    if (expiresAt <= DateTimeOffset.UtcNow)
                    {
                        var refreshToken = await context.GetTokenAsync("refresh_token");
                        var client = new HttpClient();
                        var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
                        if (disco.IsError)
                        {
                            throw new Exception(disco.Error);
                        }
                        var tokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
                        {
                            Address = disco.TokenEndpoint,
                            RefreshToken = refreshToken,
                            ClientId = "MVC",
                            ClientSecret = "secret",
                            GrantType = "refresh_token"
                        });
                        if (tokenResponse.IsError)
                        {
                            throw new Exception("Cannot request new access token by refresh token.");
                        }
                        var tokens = new List<AuthenticationToken>
                        {
                            new AuthenticationToken
                            {
                                Name = OpenIdConnectParameterNames.IdToken,
                                Value = tokenResponse.IdentityToken
                            },
                            new AuthenticationToken
                            {
                                Name= OpenIdConnectParameterNames.AccessToken,
                                Value = tokenResponse.AccessToken
                            },
                            new AuthenticationToken
                            {
                                Name= OpenIdConnectParameterNames.RefreshToken,
                                Value = tokenResponse.RefreshToken
                            }
                        };
                        var newExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
                        tokens.Add(new AuthenticationToken
                        {
                            Name = "expires_at",
                            Value = newExpiresAt.ToString("O")
                        });
                        // Sign-in the user with new tokens
                        var authInfo = await context.AuthenticateAsync("Cookies");
                        if (authInfo.Principal == null)
                        {
                            throw new Exception("Invalid authentication information");
                        }
                        authInfo.Properties?.StoreTokens(tokens);
                        await context.SignInAsync("Cookies", authInfo.Principal, authInfo.Properties);
                    }
                }
            }
            catch (Exception ex)
            {
                logging = ex.Message;
                shouldBeRedirect = true;
                //log error
            }
            if((!String.IsNullOrWhiteSpace(controller) && !String.IsNullOrWhiteSpace(action))
                && (controller != "Login" && action != "Logout")
                && shouldBeRedirect)
            {
                context.Response.Redirect("/Login/Logout");
            }
            else
            {
                await _next(context);
            }
        }
    }
}
