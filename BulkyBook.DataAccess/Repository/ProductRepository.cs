using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(DbContext db) : base(db)
        {
        }

        public void Update(Product product)
        {
            var productFromDb = dbSet.Find(product.Id);
            if (productFromDb is not null)
            {
                // could be simplified greately with AutoMapper :-), but I left it here as it is (remember, this is only demo)
                productFromDb.Title = product.Title;
                productFromDb.Description = product.Description;
                productFromDb.Author = product.Author;
                productFromDb.ListPrice = product.ListPrice;
                productFromDb.Price = product.Price;
                productFromDb.Price50 = product.Price50;
                productFromDb.Price100 = product.Price100;
                if (product.ImageUrl is not null)
                {
                    productFromDb.ImageUrl = product.ImageUrl;
                }
                productFromDb.CategoryId = product.CategoryId;
                productFromDb.CoverTypeId = product.CoverTypeId;
            }
        }
    }
}
