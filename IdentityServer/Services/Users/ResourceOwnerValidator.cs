using IdentityServer4.Validation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Services.Users
{
    public class ResourceOwnerValidator : IResourceOwnerPasswordValidator
    {
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = TestUsers.Users.FirstOrDefault(u => u.Username == context.UserName);
            if(user == null)
            {
                context.Result = new GrantValidationResult(
                    IdentityServer4.Models.TokenRequestErrors.InvalidGrant, "User does not exists.");
                return;
            }
            var resourceOwnerPassword = user.Password;
            if(string.IsNullOrEmpty(resourceOwnerPassword) || (resourceOwnerPassword != context.Password))
            {
                context.Result = new GrantValidationResult(
                    IdentityServer4.Models.TokenRequestErrors.InvalidGrant, "Incorrect password.");
                return;
            }
            context.Result = new GrantValidationResult(
                            subject: user.SubjectId,
                            authenticationMethod: "custom",
                            claims: user.Claims);
            await Task.CompletedTask;
            return;
        }
    }
}
