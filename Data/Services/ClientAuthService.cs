using Data.Contexts;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Services;

public class ClientAuthService : IClientAuthService
{
    private readonly BlazorBankContext _ctx;

    public ClientAuthService(BlazorBankContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<Client?> AuthenticateAsync(string loginOrEmail, string password)
    {
        var hashedPassword = PasswordHasher.Hash(password);
        return await _ctx.Clients
            .FirstOrDefaultAsync(c =>
                (c.Username == loginOrEmail || c.Email == loginOrEmail)
                && c.PasswordHash == hashedPassword);
    }

    public async Task<Client?> RegisterAsync(string firstName, string lastName, string username, string email, string password)
    {
        var exists = await _ctx.Clients.AnyAsync(c => c.Username == username || c.Email == email);
        if (exists)
            throw new ArgumentException("Username or email already exists.");

        PasswordValidator.Validate(password);

        var client = new Client
        {
            FirstName = firstName,            
            LastName = lastName,              
            Username = username,
            Email = email,
            PasswordHash = PasswordHasher.Hash(password),
            CreatedAt = DateTime.UtcNow
        };

        _ctx.Clients.Add(client);
        await _ctx.SaveChangesAsync();
        return client;
    }
}
