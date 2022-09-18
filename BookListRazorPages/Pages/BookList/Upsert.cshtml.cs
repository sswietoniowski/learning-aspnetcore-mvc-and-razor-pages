using BookListRazorPages.Data;
using BookListRazorPages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace BookListRazorPages.Pages.BookList;

public class UpsertModel : PageModel
{
    private readonly BookListRazorDbContext _db;

    public UpsertModel(BookListRazorDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public Book Book { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        Book = new Book();

        if (id is null)
        {
            // insert
            return Page();
        }

        Book = await _db.Books.FindAsync(id);

        if (Book is null)
        {
            return NotFound();
        }

        // update
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            if (Book.Id == 0) // new record (insert)
            {
                await _db.Books.AddAsync(Book);
            }
            else // existing record (update)
            {
                _db.Books.Update(Book);
            }

            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
        else
        {
            return RedirectToPage();
        }
    }
}