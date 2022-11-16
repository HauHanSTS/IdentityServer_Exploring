using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Threading.Tasks;

namespace IdentityServer.Services.Users
{
    public class ProfileServices : IProfileService
    {
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            //var testUser = TestUsers.Users.Find(u => u.SubjectId == subjectId);
            var testUser = CustomUser.Users.Find(u => u.UserCode == subjectId);
            if (testUser == null)
            {
                throw new ArgumentException("");
            }

            context.IssuedClaims.AddRange(testUser.Claims);
            await Task.CompletedTask;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            //var testUser = TestUsers.Users.Find(u => u.SubjectId == subjectId);
            var testUser = CustomUser.Users.Find(u => u.UserCode == subjectId);
            context.IsActive = testUser != null;
            await Task.CompletedTask;
        }
    }
}
