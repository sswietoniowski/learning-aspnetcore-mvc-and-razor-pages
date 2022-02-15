using BookListRazor.Data;
using BookListRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookListRazor.Pages.BookList
{
    public class IndexModel : PageModel
    {
        private readonly BookListRazorDbContext _db;

        public IndexModel(BookListRazorDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Book> Books { get; set; }

        public async Task OnGetAsync()
        {
            Books = await _db.Books.ToListAsync();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var book = await _db.Books.FindAsync(id);

            if (book is null)
            {
                return NotFound();
            }

            _db.Books.Remove(book);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
