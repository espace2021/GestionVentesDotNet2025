using GestionVentes.Data;
using GestionVentes.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionVentes.Entity
{
    public class LigneOrdersRepository : ILigneOrdersRepository
    {
        private readonly ApplicationDbContext _context;
        public LigneOrdersRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<LigneOrder>> GetAllLigneOrdersAsync()
        {
            return await _context.LigneOrders
                                            .Include(l => l.Product)
                                            .ToListAsync();
        }

        public async Task<LigneOrder> GetProductsByIdAsync(int idprod)
        {
            return await _context.LigneOrders
                .Include(l => l.Product) 
                .FirstOrDefaultAsync(d => d.ProductID == idprod);

           }


    }
}
