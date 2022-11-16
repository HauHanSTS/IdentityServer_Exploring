using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MVC.Models;
using Newtonsoft.Json;
using System.Globalization;

namespace MVC.Controllers
{
    public class CallAPIController : BaseController
    {
        public async Task<IActionResult> GetApi01Resources()
        {
            string content = await CallApi("API_01.resource", "GET");
            ViewBag.ApiContent = content;
            return View();
        }

        public async Task<IActionResult> GetApi02Resources()
        {
            string content = await CallApi("API_02.resource", "GET");
            ViewBag.ApiContent = content;
            return View();
        }

        public async Task<IActionResult> GetApi01User()
        {
            var content = await CallApi("API_01.user", "GET");
            ViewBag.ApiContent = content;
            return View();
        }

        public async Task<IActionResult> GetApi02User()
        {
            var content = await CallApi("API_02.user", "GET");
            ViewBag.ApiContent = content;
            return View();
        }

        public async Task<IActionResult> WriteApi01Resources()
        {
            string content = await CallApi("API_01.resource", "POST");
            ViewBag.ApiContent = content;
            return View();
        }

        public async Task<IActionResult> WriteApi02Resources()
        {
            string content = await CallApi("API_02.resource", "POST");
            ViewBag.ApiContent = content;
            return View();
        }

        public async Task<IActionResult> WriteApi01User()
        {
            var content = await CallApi("API_01.user", "POST");
            ViewBag.ApiContent = content;
            return View();
        }

        public async Task<IActionResult> WriteApi02User()
        {
            var content = await CallApi("API_02.user", "POST");
            ViewBag.ApiContent = content;
            return View();
        }

        private async Task<string> CallApi(string domain, string method)
        {
            string result = string.Empty;
            if(!String.IsNullOrEmpty(domain))
            {
                try
                {
                    string apiUrl = "";
                    switch (domain)
                    {
                        case "API_01.resource":
                            {
                                apiUrl = "https://localhost:6001/api/resource";
                                break;
                            }
                        case "API_01.user":
                            {
                                apiUrl = "https://localhost:6001/api/user";
                                break;
                            }
                        case "API_02.resource":
                            {
                                apiUrl = "https://localhost:6002/api/resource";
                                break;
                            }
                        case "API_02.user":
                            {
                                apiUrl = "https://localhost:6002/api/user";
                                break;
                            }
                    }
                    if (String.IsNullOrEmpty(apiUrl))
                        throw new Exception("Null api url");

                    var apiResult = await HttpResponse(apiUrl, method);

                    //if (apiResult.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    //{
                    //    bool requestedNewToken = await RefreshTokens();
                    //    if (requestedNewToken)
                    //    {
                    //        apiResult = await HttpResponse(apiUrl, method);
                    //    }
                    //}

                    if (apiResult.IsSuccessStatusCode)
                    {
                        result = await apiResult.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        result = domain + " respone with error: " + apiResult.StatusCode;
                    }
                }
                catch(Exception ex)
                {
                   Console.WriteLine(ex.Message);
                }
            }
            return result;
        }
        private async Task<HttpResponseMessage> HttpResponse(string apiUrl, string method)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var client = new HttpClient();
            client.SetBearerToken(accessToken);
            var resp = method == "GET"
                ? await client.GetAsync(apiUrl)
                : await client.PostAsync(apiUrl, null);
            return resp;
        }
        private async Task<bool> RefreshTokens()
        {
            bool result = true;
            try
            {
                var client = new HttpClient();
                var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
                if (disco.IsError)
                {
                    throw new Exception(disco.Error);
                }
                var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
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
                var oldIdToken = await HttpContext.GetTokenAsync("id_token") ?? null;
                if(oldIdToken == null)
                {
                    throw new Exception("Invalid old Id_Token");
                }
                var tokens = new List<AuthenticationToken>
                {
                    new AuthenticationToken
                    {
                        Name = OpenIdConnectParameterNames.IdToken,
                        Value = oldIdToken
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
                var expiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
                tokens.Add(new AuthenticationToken
                {
                    Name = "expires_at",
                    Value = expiresAt.ToString("O")
                });
                // Sign-in the user with new tokens
                var authInfo = await HttpContext.AuthenticateAsync("Cookies");
                if(authInfo.Principal == null)
                {
                    throw new Exception("Invalid authentication information");
                }
                authInfo.Properties?.StoreTokens(tokens);
                await HttpContext.SignInAsync("Cookies", authInfo.Principal, authInfo.Properties);
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}
