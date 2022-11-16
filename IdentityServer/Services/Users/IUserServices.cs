namespace IdentityServer.Services.Users
{
    public interface IUserServices
    {
        bool ValidateCredentials(string username, string password);
        CustomUser FindByUsername(string username);
    }
}
