using System.Threading.Tasks;
using Data.Models;
using Data.Contexts;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace ClientApp.Services
{
    public class ServiceCurrentClient
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly BlazorBankContext _dbContext;

        public ServiceCurrentClient(ProtectedSessionStorage sessionStorage, BlazorBankContext dbContext)
        {
            _sessionStorage = sessionStorage;
            _dbContext = dbContext;
        }

        public async Task SetClientAsync(Client client)
        {
            await _sessionStorage.SetAsync("LoggedInClientId", client.ClientId);
        }

        public async Task<Client?> GetCurrentClientAsync()
        {
            var result = await _sessionStorage.GetAsync<int>("LoggedInClientId");
            if (result.Success)
            {
                int clientId = result.Value;
                return await _dbContext.Clients.FindAsync(clientId);
            }
            return null;
        }

        public async Task LogoutAsync()
        {
            await _sessionStorage.DeleteAsync("LoggedInClientId");
        }
    }
}
