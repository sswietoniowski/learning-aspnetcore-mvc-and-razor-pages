using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;

namespace BulkyBook.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public ICategoryRepository Categories { get; }

        public IStoredProcedureCall StoredProcedureCalls { get; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Categories = new CategoryRepository(_db);
            StoredProcedureCalls = new StoredProcedureCall(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db?.Dispose();
            StoredProcedureCalls?.Dispose();
        }
    }
}
