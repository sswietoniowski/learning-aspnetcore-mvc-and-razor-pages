using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Repository;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(DbContext db) : base(db)
    {
    }

    public void Update(Category category)
    {
        var categoryFromDb = dbSet.Find(category.Id);
        if (categoryFromDb is not null)
        {
            categoryFromDb.Name = category.Name;
        }
    }
}