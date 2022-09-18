using System.Collections.Generic;

namespace BulkyBook.Models.ViewModels;

public class CategoriesViewModel
{
    public IEnumerable<Category> Categories { get; set; }
    public PagingInfo PagingInfo { get; set; }
}