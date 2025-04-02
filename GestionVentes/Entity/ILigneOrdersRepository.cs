using GestionVentes.Models;

namespace GestionVentes.Entity
{
    public interface ILigneOrdersRepository
    {
        Task<IEnumerable<LigneOrder>> GetAllLigneOrdersAsync();
        Task<LigneOrder> GetProductsByIdAsync(int idprod);
    }
}
