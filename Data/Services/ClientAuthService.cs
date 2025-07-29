//using Data.Contexts;
//using Data.Models;
//using Microsoft.EntityFrameworkCore;

//namespace Data.Services
//{
//    public class ClientAuthService
//    {
//        private readonly BlazorBankContext _db;

//        public ClientAuthService(BlazorBankContext db)
//        {
//            _db = db;
//        }

//        public async Task<Client?> AuthenticateAsync(string email, string password)
//        {
//            return await _db.Clients
//                .FirstOrDefaultAsync(c => c.Email == email && c.PasswordHash == password);
//        }
//    }
//}

