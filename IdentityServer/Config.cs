// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("API_01.resource.read", "resource reading"),
                new ApiScope("API_01.resource.write", "resource writing"),
                new ApiScope("API_01.user.write", "user writing"),
                new ApiScope("API_01.user.read", "user reading"),

                new ApiScope("API_02.resource.read", "resource reading"),
                new ApiScope("API_02.resource.write", "resource writing"),
                new ApiScope("API_02.user.write", "user writing"),
                new ApiScope("API_02.user.read", "user reading"),
            };

        public static IEnumerable<ApiResource> ApiResources => 
            new ApiResource[] 
            {
                new ApiResource("API_01", "API 01 resources")
                {
                    Scopes = { "API_01.resource.read", "API_01.resource.write", "API_01.user.read", "API_01.user.write" }
                },
                new ApiResource("API_02", "API 02 resources")
                {
                    Scopes = { "API_02.resource.read", "API_02.resource.write", "API_02.user.read" }
                },
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "MVC",

                    ClientName = "MVC Client",

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    AccessTokenLifetime = 300,

                    AbsoluteRefreshTokenLifetime = 2592000,

                    //SlidingRefreshTokenLifetime = 20,

                    RefreshTokenExpiration = TokenExpiration.Absolute,

                    AllowedGrantTypes = GrantTypes.Code,

                    RequirePkce = true,

                    AllowOfflineAccess = true,

                    RefreshTokenUsage = TokenUsage.ReUse,

                    AlwaysIncludeUserClaimsInIdToken = true,

                    // where to redirect to after login
                    RedirectUris = { "https://localhost:5002/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                    // scopes that client has access to
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "API_01.resource.read", "API_01.resource.write",
                        "API_01.user.read", "API_01.user.write",
                        "API_02.resource.read", "API_02.resource.write",
                        "API_02.user.read",
                    }
                },
                new Client
                {
                    ClientId = "API_01",

                    ClientName = "API 01",

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("api_secret".Sha256()),
                        new Secret
                        {
                            Type = IdentityServerConstants.SecretTypes.X509CertificateBase64,
                            Value = "MIIDHDCCAgSgAwIBAgIUa6Y1MvAbcLyfe9FyKhufzywc7/0wDQYJKoZIhvcNAQELBQAwDTELMAkGA1UEBhMCVVMwHhcNMjIxMTA1MDgwNjIxWhcNMjMxMTA1MDgwNjIxWjANMQswCQYDVQQGEwJVUzCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAMVKBlSBleXaRJL4JPPB3tMpybqEtMloBD9KXUAVmHlv9Ehxpj8JHq+zgOLZNC0QYAa9E3FEddpdpVGVL+ndUyDSJCzIgzQSEdBdI/vQPV4oconCXrQ2VAsrFG7TlaKsIPa8Iexs302ErY0h4hWrsiejIPIGbANV1u7DzBGk/jmTxyGJf/3Cr18zJBbKv+9GxTz4IO91eIOhDqCNfe5PUVQ+99Fbr80yY/nrmUodxIMU1aGHfxLudGKAxVx7xQ9s1PJkWvTAnWL0qVo+PyrQTohtL+aOTlJ7T3h8ujfLIM1N7+EqBmZIUZwATP0YMo/PCsJslceT77zt7J6gSdXckX0CAwEAAaN0MHIwHQYDVR0OBBYEFPjvQMSrb8KYA5djsFsMhXPi7IxWMB8GA1UdIwQYMBaAFPjvQMSrb8KYA5djsFsMhXPi7IxWMA4GA1UdDwEB/wQEAwIFoDAgBgNVHSUBAf8EFjAUBggrBgEFBQcDAQYIKwYBBQUHAwIwDQYJKoZIhvcNAQELBQADggEBAGvEMX9PtxamZUgtU4bhjz/IYbLtoFmvLqWelYDaefL6e49zwI31RUv0FYGyqsmLtAycH1nglAlNw25bSG/jxQdx9E2ASl1OBswjEieVaM10ZSl8bfgVqNCOrODyXWia6FAJXfgpyQyon15U3KRcQNG5D7aUvmR32FogSt5qy3xpZBeUi8PyGRo4lAQZUqIMlT5BDF2CiYCKOa5jMwa6lJ3dpOaO0MPRxPW+1BwppF+AkWenCQ1pp14SRtY/YntkjjXd0+ZsmH7TfG5vXPC1tFhCJ9pm/MBByxYDhDft3ksENLJM8ew1EvYOtJGIQ2OdM1OrkAf4HEuHfmMWKDlyWR8="
                        },
                        new Secret
                        {
                            Type = IdentityServerConstants.SecretTypes.JsonWebKey,
                            Value = "{'kty': 'RSA','e': 'AQAB','kid': 'handuyhau','n': 'q6O1C1c_dTJ7OjwnUszZKVyV6J1YtpuiBEvViP-yYDuXp7G7ff_urAgoUVe2DzlcIFLGBEORVlVLKE6H_fflEDFxDzSHpKtj2mwopfz2JenSD4u0VG_g-6GPAguJjnb9EujjbgOTjlkm1GkUIql7D0J8uhFkANclBZj7QiIfWGCF68T3GmLQ-Uuo5aTfMmshQkNsLGMimer62rTAWut89GyczOk5-mDk_KdGsFbI3yYqFa8VLdgzzp7V19RXsOdNi2rUQlfQogyrvj9QFTJHnhP6XG4xOdbDD0aGTlnX45__KCOAp3BcHobRFPpP-zrFO2SbChJugADwocBywJsXjw'}"
                        }
                    },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    AllowOfflineAccess = true,

                    // scopes that client has access to
                    AllowedScopes = new List<string>
                    {
                        "API_02.resource.read", "API_02.resource.write",
                        "API_02.user.read",
                    }
                }
            };
    }
}