using System.Threading.Tasks;
using Data.Models;
using Data.Contexts;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Client.Services
{
    public class CurrentClientService
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly BlazorBankContext _dbContext;

        public Data.Models.Client? LoggedInClient { get; private set; }

        public CurrentClientService(ProtectedSessionStorage sessionStorage, BlazorBankContext dbContext)
        {
            _sessionStorage = sessionStorage;
            _dbContext = dbContext;
        }

        // Сохраняем клиента в памяти и sessionStorage
        public async Task SetClientAsync(Data.Models.Client client)
        {
            LoggedInClient = client;
            await _sessionStorage.SetAsync("LoggedInClientId", client.ClientId);
        }

        // Загружаем клиента из sessionStorage и базы
        public async Task LoadClientAsync()
        {
            var result = await _sessionStorage.GetAsync<int>("LoggedInClientId");
            if (result.Success)
            {
                int clientId = result.Value;
                LoggedInClient = await _dbContext.Clients.FindAsync(clientId);
            }
            else
            {
                LoggedInClient = null;
            }
        }

        // Очистка клиента и sessionStorage при логауте
        public async Task LogoutAsync()
        {
            LoggedInClient = null;
            await _sessionStorage.DeleteAsync("LoggedInClientId");
        }
    }
}
