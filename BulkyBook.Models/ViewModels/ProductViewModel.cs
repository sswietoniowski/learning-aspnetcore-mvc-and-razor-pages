using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyBook.Models.ViewModels;

public class ProductViewModel
{
    public Product Product { get; set; }
    public IEnumerable<SelectListItem> Categories { get; set; }
    public IEnumerable<SelectListItem> CoverTypes { get; set; }
}