using Data.Models;

namespace Data.Services;

public interface IClientAuthService
{
    Task<Client?> AuthenticateAsync(string loginOrEmail, string password);
    Task<Client?> RegisterAsync(string firstName, string lastName, string username, string email, string password);
}
