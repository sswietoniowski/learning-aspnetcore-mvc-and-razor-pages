using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Repository;

public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
{
    public CoverTypeRepository(DbContext db) : base(db)
    {
    }

    public void Update(CoverType coverType)
    {
        var coverTypeFromDb = dbSet.Find(coverType.Id);
        if (coverTypeFromDb is not null)
        {
            coverTypeFromDb.Name = coverType.Name;
        }
    }
}