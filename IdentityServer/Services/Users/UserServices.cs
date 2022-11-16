using System.Linq;

namespace IdentityServer.Services.Users
{
    public class UserServices : IUserServices
    {
        public CustomUser FindByUsername(string username)
        {
            return CustomUser.Users.Find(u => u.Username == username);
        }

        public bool ValidateCredentials(string username, string password)
        {
            return CustomUser.Users.FirstOrDefault(u => u.Username == username && u.Password == password) != null;
        }
    }
}
