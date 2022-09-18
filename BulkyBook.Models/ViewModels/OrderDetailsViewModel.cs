using System.Collections.Generic;

namespace BulkyBook.Models.ViewModels;

public class OrderDetailsViewModel
{
    public OrderHeader OrderHeader { get; set; }
    public IEnumerable<OrderDetail> OrderDetails { get; set; }
}