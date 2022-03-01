using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        public ShoppingCartRepository(DbContext db) : base(db)
        {
        }

        public void Update(ShoppingCart shoppingCart)
        {
            db.Update(shoppingCart);
        }
    }
}
