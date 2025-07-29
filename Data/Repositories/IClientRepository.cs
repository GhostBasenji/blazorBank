//using Data.Contexts;
//using Data.Models;
//using Microsoft.EntityFrameworkCore;

//namespace Data.Repositories
//{
//    public interface IClientRepository
//    {
//        Task<IEnumerable<Client>> GetAllAsync();
//        Task<Client?> GetByIdAsync(int id);
//    }

//    public class ClientRepository : IClientRepository
//    {
//        private readonly BlazorBankContext _context;

//        public ClientRepository(BlazorBankContext context)
//        {
//            _context = context;
//        }

//        public async Task<IEnumerable<Client>> GetAllAsync()
//        {
//            return await _context.Clients.ToListAsync();
//        }

//        public async Task<Client?> GetByIdAsync(int id)
//        {
//            return await _context.Clients.FindAsync(id);
//        }
//    }
//}
