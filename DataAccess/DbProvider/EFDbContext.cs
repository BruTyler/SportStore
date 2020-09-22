using System.Data.Entity;
using SportsStore.Domain.Entities;

namespace DataAccess.DbProvider
{
    public class EFDbContext: DbContext
    {
        public DbSet<Product> Products { get; set; }
    }
}
