using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        public OrderHeaderRepository(DbContext db) : base(db)
        {
        }

        public void Update(OrderHeader orderHeader)
        {
            db.Update(orderHeader);
        }
    }
}
