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

        // Сохраняем ID клиента в sessionStorage (состояние в сервисе не храним)
        public async Task SetClientAsync(Client client)
        {
            await _sessionStorage.SetAsync("LoggedInClientId", client.ClientId);
        }

        // Получаем клиента из sessionStorage и базы, возвращая его вызывающей стороне
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

        // Очистка sessionStorage при логауте
        public async Task LogoutAsync()
        {
            await _sessionStorage.DeleteAsync("LoggedInClientId");
        }
    }
}
