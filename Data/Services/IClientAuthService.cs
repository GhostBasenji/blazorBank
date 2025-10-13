using Data.Models;

namespace Data.Services;

public interface IClientAuthService
{
    Task<Client?> AuthenticateAsync(string loginOrEmail, string password);
    Task<(bool Success, string? ErrorMessage)> RegisterAsync(string username, string email, string password, string firstName, string lastName, string? phone = null);
    Task<bool> IsUsernameExistsAsync(string username);
    Task<bool> IsEmailExistsAsync(string email);
}