using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionVentes.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }

        // Relation avec Customer
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }
        public ICollection<LigneOrder> LigneOrders { get; set; } = new List<LigneOrder>(); // Relation : Une commande a plusieurs lignes de commande
    }
}
