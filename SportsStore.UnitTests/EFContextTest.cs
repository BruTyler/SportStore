using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess.DbProvider;
using SportsStore.Domain.Entities;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class EFContextTest
    {
        private EFDbContext _context;

        [TestInitialize]
        public void SetupTest()
        {
            //init Ef database-in-memory
            DbConnection connection = Effort.DbConnectionFactory.CreateTransient();
            _context = new EFDbContext(connection);
        }

        [TestMethod]
        public void AddProduct()
        {
            _context.Products.Add(new Product()
            {
                Category = "category",
                Name = "test name",
                Description = "descr",
                Price = 1,
            });
            
            var succeedStoredCounter  = _context.SaveChanges();

            Assert.AreEqual(1, succeedStoredCounter);
        }

        [TestMethod]
        public void AddAndReadProduct()
        {
            _context.Products.Add(new Product()
            {
                Category = "category",
                Name = "test name",
                Description = "descr",
                Price = 1,
            });

            var succeedStoredCounter = _context.SaveChanges();
            var storedProd = _context.Products.Find(1);

            Assert.AreEqual(1, succeedStoredCounter);
            Assert.AreEqual("descr", storedProd.Description);
        }
    }
}
