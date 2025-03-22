using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionVentes.Models
{
    public class LigneOrder
    {
        [Key]
        public int LigneOrderID { get; set; }
        [Required]
        public int OrderQty { get; set; }
        public decimal LineItemTotal { get; set; }

        [ForeignKey("Order")]
        public int OrderID { get; set; }
        public Order Order { get; set; } // Relation : Une ligne de commande appartient à une commande

        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; } // Relation : Une ligne de commande concerne un produit
    }
}
