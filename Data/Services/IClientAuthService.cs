using Data.Models;

namespace Data.Services;

public interface IClientAuthService
{
    Task<Client?> AuthenticateAsync(string loginOrEmail, string password);
}
