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
        // На этом этапе пароль не хэшируется — просто для тестов
        return await _context.Clients
            .FirstOrDefaultAsync(c =>
                (c.Username == loginOrEmail || c.Email == loginOrEmail)
                && c.PasswordHash == password);
    }
}