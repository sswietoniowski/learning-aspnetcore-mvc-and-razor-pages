using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;

namespace BulkyBook.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;

    public ICategoryRepository Categories { get; }
    public ICoverTypeRepository CoverTypes { get; }
    public IProductRepository Products { get; }
    public ICompanyRepository Companies { get; }
    public IApplicationUserRepository ApplicationUsers { get; }

    public IStoredProcedureCall StoredProcedureCalls { get; }

    public IShoppingCartRepository ShoppingCarts { get; }

    public IOrderHeaderRepository OrderHeaders { get; }

    public IOrderDetailRepository OrderDetails { get; }

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        Categories = new CategoryRepository(_db);
        CoverTypes = new CoverTypeRepository(_db);
        Products = new ProductRepository(_db);
        Companies = new CompanyRepository(_db);
        ApplicationUsers = new ApplicationUserRepository(_db);
        ShoppingCarts = new ShoppingCartRepository(_db);
        OrderHeaders = new OrderHeaderRepository(_db);
        OrderDetails = new OrderDetailRepository(_db);

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