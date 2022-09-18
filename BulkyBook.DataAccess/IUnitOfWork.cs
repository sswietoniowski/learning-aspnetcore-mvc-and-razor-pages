using BulkyBook.DataAccess.Repository.IRepository;
using System;

namespace BulkyBook.DataAccess;

public interface IUnitOfWork : IDisposable
{
    public ICategoryRepository Categories { get; }
    public ICoverTypeRepository CoverTypes { get; }
    public IProductRepository Products { get; }
    public ICompanyRepository Companies { get; }
    public IApplicationUserRepository ApplicationUsers { get; }
    public IShoppingCartRepository ShoppingCarts { get; }
    public IOrderHeaderRepository OrderHeaders { get; }
    public IOrderDetailRepository OrderDetails { get; }

    public IStoredProcedureCall StoredProcedureCalls { get; }

    public void Save();
}