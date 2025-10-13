using Data.Contexts;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Services;

public class ClientAuthService : IClientAuthService
{
    private readonly BlazorBankContext _context;

    public ClientAuthService(BlazorBankContext context)
    {
        _context = context;
    }

    public async Task<Client?> AuthenticateAsync(string loginOrEmail, string password)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Username == loginOrEmail || c.Email == loginOrEmail);

        if (client == null)
            return null;

        // Проверяем пароль с хэшем
        if (PasswordHasher.VerifyPassword(password, client.PasswordHash))
            return client;

        return null;
    }

    public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(
        string username,
        string email,
        string password,
        string firstName,
        string lastName,
        string? phone = null)
    {
        // Проверка уникальности username
        if (await IsUsernameExistsAsync(username))
            return (false, "Username already exists.");

        // Проверка уникальности email
        if (await IsEmailExistsAsync(email))
            return (false, "Email already exists.");

        // Хэшируем пароль
        string hashedPassword = PasswordHasher.HashPassword(password);

        // Создание нового клиента
        var newClient = new Client
        {
            Username = username,
            Email = email,
            PasswordHash = hashedPassword,
            FirstName = firstName,
            LastName = lastName,
            Phone = phone,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            _context.Clients.Add(newClient);
            await _context.SaveChangesAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Registration failed: {ex.Message}");
        }
    }

    public async Task<bool> IsUsernameExistsAsync(string username)
    {
        return await _context.Clients
            .AnyAsync(c => c.Username == username);
    }

    public async Task<bool> IsEmailExistsAsync(string email)
    {
        return await _context.Clients
            .AnyAsync(c => c.Email == email);
    }
}