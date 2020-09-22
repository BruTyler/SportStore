using System;
using System.Linq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace DataAccess.DbProvider
{
    public class EFProductRepository : EFBaseRepository, IProductRepository
    {
        public IQueryable<Product> Products
        {
            get { return _db.Products; }
        }

        public Product DeleteProduct(int productID)
        {
            var dbEntry = _db.Products.Find(productID);

            if (dbEntry != null)
            {
                _db.Products.Remove(dbEntry);
                _db.SaveChanges();
            }

            return dbEntry;
        }

        public void SaveProduct(Product product)
        {
            if (product == null)
                throw new ArgumentException("empty product");

            if (product.ProductID == 0)
                _db.Products.Add(product);
            else
            {
                var dbEntry = _db.Products.Find(product.ProductID);
                if (dbEntry != null)
                {
                    dbEntry.Category = product.Category;
                    dbEntry.Description = product.Description;
                    dbEntry.Name = product.Name;
                    dbEntry.Price = product.Price;
                    dbEntry.ImageData = product.ImageData;
                    dbEntry.ImageMimeType = product.ImageMimeType;
                }
            }

            _db.SaveChanges();
        }
    }
}
