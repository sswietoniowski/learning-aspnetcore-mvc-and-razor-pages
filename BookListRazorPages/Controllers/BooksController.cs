using BookListRazorPages.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BookListRazorPages.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController : Controller
{
    private readonly BookListRazorDbContext _db;

    public BooksController(BookListRazorDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Json(new { data = await _db.Books.ToListAsync() });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _db.Books.FindAsync(id);
        if (book is null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        _db.Books.Remove(book);
        await _db.SaveChangesAsync();

        return Json(new { success = true, message = "Delete successful" });
    }
}