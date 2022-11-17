using System.Threading.Tasks;
using BookListRazorPagesCRUD.Data;
using BookListRazorPagesCRUD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookListRazorPagesCRUD.Pages.BookList;

public class EditModel : PageModel
{
    private readonly BookListRazorDbContext _db;

    public EditModel(BookListRazorDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public Book Book { get; set; }

    public async Task OnGetAsync(int id)
    {
        Book = await _db.Books.FindAsync(id);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var book = await _db.Books.FindAsync(Book.Id);
            book.Name = Book.Name;
            book.Author = Book.Author;
            book.ISBN = Book.ISBN;

            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
        else
        {
            return RedirectToPage();
        }
    }
}