using BulkyBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<CoverType> CoverTypes { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Company> Companies { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // added only to solve problem with controllers scaffolding
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=BulkyBookMVC;Trusted_Connection=True;MultipleActiveResultSets=true");
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // 1st two prices were handled using annotations, the last two will be done with builder
            // remember that's only for the demo purposes - normaly we would use only one method
            builder.Entity<Product>()
                .Property(p => p.Price50)
                .HasColumnType("decimal(18,2)");
            builder.Entity<Product>()
                .Property(p => p.Price100)
                .HasColumnType("decimal(18,2)");

            base.OnModelCreating(builder);
        }
    }
}
