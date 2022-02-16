using BookListRazor.Models;
using Microsoft.EntityFrameworkCore;

namespace BookListRazor.Data
{
    public class BookListRazorDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }

        public BookListRazorDbContext(DbContextOptions<BookListRazorDbContext> options) : base(options)
        {
        }


    }
}
