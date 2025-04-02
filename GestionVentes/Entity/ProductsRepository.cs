using GestionVentes.Data;
using GestionVentes.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionVentes.Entity
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductsRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }
        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(d => d.ProductID == id);
        }

    }
}
