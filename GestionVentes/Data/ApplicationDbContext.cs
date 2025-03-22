using GestionVentes.Models;
using Microsoft.EntityFrameworkCore;


namespace GestionVentes.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<LigneOrder> LigneOrders { get; set; }
    }
}


