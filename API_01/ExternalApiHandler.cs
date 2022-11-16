using IdentityModel;
using IdentityModel.Client;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using static IdentityModel.OidcConstants;

namespace API_01
{
    public class ExternalApiHandler
    {
        private static SigningCredentials CreateSigningCredentialsByX509Certs()
        {
            string certPath = "cert.pfx";
            var certFilePath = Path.Combine(Directory.GetCurrentDirectory(), certPath);
            var certs = new X509Certificate2(certFilePath, "123", X509KeyStorageFlags.MachineKeySet |
                                                                X509KeyStorageFlags.PersistKeySet |
                                                                X509KeyStorageFlags.Exportable);
            return new SigningCredentials(new X509SecurityKey(certs), SecurityAlgorithms.RsaSha256);
        }
        private static SigningCredentials CreateSigningCredentialsByJWK()
        {
            var jwkPath = "key.json";
            var jwtFilePath = Path.Combine(Directory.GetCurrentDirectory(), jwkPath);
            var jwkContent = File.ReadAllText(jwtFilePath);

            JsonWebKey jwk = new(jwkContent);

            var keyPath = "private-key.txt";
            var keyFilePath = Path.Combine(Directory.GetCurrentDirectory(), keyPath);
            var keyContent = File.ReadAllText(keyFilePath);
            keyContent = keyContent.Replace("-----BEGIN RSA PRIVATE KEY-----", "");
            keyContent = keyContent.Replace("-----END RSA PRIVATE KEY-----", "");
            var keyBytes = Convert.FromBase64String(keyContent);

            var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(keyBytes, out _);

            var rsaSecurityKey = new RsaSecurityKey(rsa)
            {
                KeyId = jwk.KeyId
            };

            return new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };
        }
        public static async Task<string> ReadProtectedApiReousrceBySharedSecret()
        {
            string result;
            try
            {
                var client = new HttpClient();
                var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
                if (disco.IsError)
                {
                    throw new Exception(disco.Error);
                }

                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "API_01",
                    ClientSecret = "api_secret",
                    Scope = "API_02.resource.read API_02.resource.write API_02.user.read"
                });
                if (tokenResponse.IsError)
                {
                    throw new Exception(tokenResponse.Error);
                }

                var apiClient = new HttpClient();
                apiClient.SetBearerToken(tokenResponse.AccessToken);
                var resp = await apiClient.GetAsync("https://localhost:6002/api/resource");
                if (!resp.IsSuccessStatusCode)
                {
                    throw new Exception(resp.StatusCode.ToString());
                }

                result = await resp.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public static async Task<string> ReadProtectedApiReousrceByX509Certs()
        {
            string result;
            try
            {
                var now = DateTime.UtcNow;
                var client = new HttpClient();
                var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
                if (disco.IsError)
                {
                    throw new Exception(disco.Error);
                }

                var tokenEndpoint = disco.TokenEndpoint;
                var clientId = "API_01";
                var signingCredentials = CreateSigningCredentialsByX509Certs();

                // create client_assertion JWT token
                var token = new JwtSecurityToken(
                    clientId,
                    tokenEndpoint,
                    new List<Claim>
                    {
                        new Claim("jti", Guid.NewGuid().ToString()),
                        new Claim(JwtClaimTypes.Subject, clientId),
                        new Claim(JwtClaimTypes.IssuedAt, now.ToEpochTime().ToString(), ClaimValueTypes.Integer64)
                    },
                    now,
                    now.AddMinutes(5),
                    signingCredentials
                );
                var tokenHandler = new JwtSecurityTokenHandler();
                var assertionToken = tokenHandler.WriteToken(token);

                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = tokenEndpoint,
                    ClientId = "API_01",
                    ClientAssertion = new ClientAssertion
                    {
                        Type = ClientAssertionTypes.JwtBearer,
                        Value = assertionToken
                    },
                    ClientCredentialStyle = ClientCredentialStyle.PostBody,
                    Scope = "API_02.resource.read API_02.resource.write API_02.user.read"
                });

                if (tokenResponse.IsError)
                {
                    throw new Exception(tokenResponse.Error);
                }

                var apiClient = new HttpClient();
                apiClient.SetBearerToken(tokenResponse?.AccessToken);
                var resp = await apiClient.GetAsync("https://localhost:6002/api/resource");
                if (!resp.IsSuccessStatusCode)
                {
                    throw new Exception(resp.StatusCode.ToString());
                }

                result = await resp.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public static async Task<string> ReadProtectedApiReousrceByJWK()
        {
            string result;
            try
            {
                var now = DateTime.UtcNow;
                var client = new HttpClient();
                var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
                if (disco.IsError)
                {
                    throw new Exception(disco.Error);
                }

                var tokenEndpoint = disco.TokenEndpoint;
                var clientId = "API_01";
                var signingCredentials = CreateSigningCredentialsByJWK();
                // create client_assertion JWT token
                var token = new JwtSecurityToken(
                    "API_01",
                    tokenEndpoint,
                    new List<Claim>
                    {
                        new Claim("jti", Guid.NewGuid().ToString()),
                        new Claim(JwtClaimTypes.Subject, clientId),
                        new Claim(JwtClaimTypes.IssuedAt, now.ToEpochTime().ToString(), ClaimValueTypes.Integer64)
                    },
                    now,
                    now.AddMinutes(5),
                    signingCredentials
                );
                var tokenHandler = new JwtSecurityTokenHandler();
                var assertionToken = tokenHandler.WriteToken(token);

                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = tokenEndpoint,
                    ClientId = "API_01",
                    ClientAssertion = new ClientAssertion
                    {
                        Type = ClientAssertionTypes.JwtBearer,
                        Value = assertionToken
                        //Value = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImhhbmR1eWhhdSIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJlOGU2MzAyZS01NmVlLTRmYWItOGJjNy0xYWQzMzMyOTVjM2UiLCJzdWIiOiJBUElfMDEiLCJpYXQiOjE2Njc3MjU1MTIsIm5iZiI6MTY2NzcyNTUxMiwiZXhwIjoxNjY3NzI1ODEyLCJpc3MiOiJBUElfMDEiLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo1MDAxL2Nvbm5lY3QvdG9rZW4ifQ.nwSs3P5lomk_Nk7g5iPWkkCCRvLLzzwGHaClHlbumnr7e-d8Vh75L4hZAP0RKk1dM4FdOUnxGGrV209WBFsvbRtnih1xzBwVS9T3pb2Y7coDKDJPsZQKI8WTSSLRARgaj4bal2lgd0NT6F-GK4cJd9huRpjqUJyE8JBSziB69rM1Ft-wb8H55iSxaOdWTGXgEVA0otHS78D2picAy0UU2rlbGelRVinRJKV8kEAbhYsValZElmezjopHx2mfT_4_2jSxqUSrrvWlcMEk9js0KE5Tb7dOoj7XC9-5zTaM6fapR4PTsE8DeqIDhSWl3omNoPsuOTueYVyZ6BDiMFOYSA"
                    },
                    ClientCredentialStyle = ClientCredentialStyle.PostBody,
                    Scope = "API_02.resource.read API_02.resource.write API_02.user.read"
                });

                if (tokenResponse.IsError)
                {
                    throw new Exception(tokenResponse.Error);
                }

                var apiClient = new HttpClient();
                apiClient.SetBearerToken(tokenResponse?.AccessToken);
                var resp = await apiClient.GetAsync("https://localhost:6002/api/resource");
                if (!resp.IsSuccessStatusCode)
                {
                    throw new Exception(resp.StatusCode.ToString());
                }

                result = await resp.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
    }
}
