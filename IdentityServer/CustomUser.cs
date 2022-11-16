using IdentityModel;
using IdentityServer4;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;

namespace IdentityServer
{
    public class CustomUser
    {
        public string UserCode { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Claim> Claims { get; set; }
        public static List<CustomUser> Users
        {
            get 
            {
                var address = new
                {
                    street_address = "02 Nguyen The Loc",
                    locality = "Go Vap District",
                    postal_code = 69118,
                    country = "Vietnam"
                };

                return new List<CustomUser>
                {
                    new CustomUser
                    {
                        UserCode = "alice.admin",
                        Username = "alice",
                        Password = "alice",
                        Claims = new List<Claim>
                        {
                            new Claim(JwtClaimTypes.Role, "Admin"),
                            new Claim(JwtClaimTypes.Role, "HR"),
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                            new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json)
                        }
                    },
                    new CustomUser
                    {
                        UserCode = "bob.it",
                        Username = "bob",
                        Password = "bob",
                        Claims = new List<Claim>
                        {
                            new Claim(JwtClaimTypes.Role, "IT"),
                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Bob"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                            new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json)
                        }
                    }
                };
            }
        }
    }
}
