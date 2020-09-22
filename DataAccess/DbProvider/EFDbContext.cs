using System.Data.Common;
using System.Data.Entity;
using SportsStore.Domain.Entities;

namespace DataAccess.DbProvider
{
    public class EFDbContext: DbContext
    {
        public EFDbContext() : base()
        {
            //default constructor for production
        }

        public EFDbContext(DbConnection connection) : base(connection, false)
        {
            //additional constructor for injecting in unit-test
        }

        public DbSet<Product> Products { get; set; }
    }
}
