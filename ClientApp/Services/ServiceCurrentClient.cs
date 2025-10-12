using Data.Contexts;
using Data.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace ClientApp.Services
{
    public class ServiceCurrentClient
    {
        private readonly ProtectedLocalStorage _localStorage;
        private readonly BlazorBankContext _dbContext;

        public ServiceCurrentClient(ProtectedLocalStorage localStorage, BlazorBankContext dbContext)
        {
            _localStorage = localStorage;
            _dbContext = dbContext;
        }

        public async Task SetClientAsync(Client client, bool rememberMe = true)
        {
            if (rememberMe)
            {
                await _localStorage.SetAsync("LoggedInClientId", client.ClientId);
            }
        }

        public async Task<Client?> GetCurrentClientAsync()
        {
            try
            {
                var result = await _localStorage.GetAsync<int>("LoggedInClientId");
                if (result.Success)
                {
                    int clientId = result.Value;
                    return await _dbContext.Clients.FindAsync(clientId);
                }
            }
            catch
            {
                
            }
            return null;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var client = await GetCurrentClientAsync();
            return client != null;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.DeleteAsync("LoggedInClientId");
        }
    }
}