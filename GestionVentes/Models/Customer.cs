using System.ComponentModel.DataAnnotations;

namespace GestionVentes.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        public ICollection<Order> Orders { get; set; } // Relation : Un client a plusieurs commandes
    }
}
