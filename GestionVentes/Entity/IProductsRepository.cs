using GestionVentes.Models;

namespace GestionVentes.Entity
{
    public interface IProductsRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetByIdAsync(int id);
    }
}
