using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Repository;

public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
{
    public OrderDetailRepository(DbContext db) : base(db)
    {
    }

    public void Update(OrderDetail orderDetails)
    {
        db.Update(orderDetails);
    }
}