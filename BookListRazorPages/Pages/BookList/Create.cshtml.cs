using BookListRazorPages.Data;
using BookListRazorPages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace BookListRazorPages.Pages.BookList;

public class CreateModel : PageModel
{
    private readonly BookListRazorDbContext _db;

    public CreateModel(BookListRazorDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public Book Book { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost()
    {
        if (ModelState.IsValid)
        {
            await _db.Books.AddAsync(Book);
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
        else
        {
            return Page();
        }
    }
}