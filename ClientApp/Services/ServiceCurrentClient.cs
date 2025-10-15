using Data.Contexts;
using Data.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace ClientApp.Services
{
    public class ServiceCurrentClient
    {
        private readonly ProtectedLocalStorage _localStorage;
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly BlazorBankContext _dbContext;

        public ServiceCurrentClient(
            ProtectedLocalStorage localStorage,
            ProtectedSessionStorage sessionStorage,
            BlazorBankContext dbContext)
        {
            _localStorage = localStorage;
            _sessionStorage = sessionStorage;
            _dbContext = dbContext;
        }

        // Сохраняем ID клиента
        public async Task SetClientAsync(Client client, bool rememberMe = true)
        {
            // Очищаем оба хранилища
            await _localStorage.DeleteAsync("LoggedInClientId");
            await _sessionStorage.DeleteAsync("LoggedInClientId");

            await Task.Delay(100);

            if (rememberMe)
            {
                // Долгосрочное хранение (LocalStorage)
                await _localStorage.SetAsync("LoggedInClientId", client.ClientId);
            }
            else
            {
                // Временная сессия (SessionStorage) - только на время сессии браузера
                await _sessionStorage.SetAsync("LoggedInClientId", client.ClientId);
            }
        }

        // Получаем клиента из любого хранилища
        public async Task<Client?> GetCurrentClientAsync()
        {
            try
            {
                // Сначала проверяем LocalStorage (Remember Me)
                var localResult = await _localStorage.GetAsync<int>("LoggedInClientId");
                if (localResult.Success)
                {
                    int clientId = localResult.Value;
                    return await _dbContext.Clients.FindAsync(clientId);
                }

                // Потом проверяем SessionStorage (без Remember Me)
                var sessionResult = await _sessionStorage.GetAsync<int>("LoggedInClientId");
                if (sessionResult.Success)
                {
                    int clientId = sessionResult.Value;
                    return await _dbContext.Clients.FindAsync(clientId);
                }
            }
            catch
            {
                // Если ошибка чтения - игнорируем
            }
            return null;
        }

        // Проверка, залогинен ли пользователь
        public async Task<bool> IsAuthenticatedAsync()
        {
            var client = await GetCurrentClientAsync();
            return client != null;
        }

        // Очистка обоих хранилищ при логауте
        public async Task LogoutAsync()
        {
            await _localStorage.DeleteAsync("LoggedInClientId");
            await _sessionStorage.DeleteAsync("LoggedInClientId");
        }
    }
}