using BookListRazorPages.Models;
using Microsoft.EntityFrameworkCore;

namespace BookListRazorPages.Data;

public class BookListRazorDbContext : DbContext
{
    public DbSet<Book> Books { get; set; }

    public BookListRazorDbContext(DbContextOptions<BookListRazorDbContext> options) : base(options)
    {
    }


}