using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionVentes.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }
        [MaxLength(50)]
        public string Category { get; set; }
        public decimal ListPrice { get; set; }

        public ICollection<LigneOrder> LigneOrders { get; set; } // Relation : Un produit peut être dans plusieurs lignes de commande
    }
}
