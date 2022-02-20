using BulkyBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<CoverType> CoverTypes { get; set; }

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
    }
}
