using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            Mock<IProductRepository> ProductRepoMock = new Mock<IProductRepository>();
            ProductRepoMock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
            }.AsQueryable);

            ProductRepoMock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
            }.AsQueryable);

            ProductController productController = new ProductController(ProductRepoMock.Object);
            productController.PageSize = 4;
            var currentProd = (ProductsListViewModel)productController.List(null, 2).Model;

            var result = currentProd.Products.ToArray();
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Count() == 1);
            Assert.AreEqual(5, result[0].ProductID);
            Assert.AreEqual("P5", result[0].Name);

        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            HtmlHelper myHelper = null;
            PagingInfo pagingInfo = new PagingInfo()
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            Func<int, string> pageUrlDelegate = i => "Page" + i;

            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            Assert.AreEqual(result.ToString(), @"<a href=""Page1"">1</a>"
                + @"<a class=""selected"" href=""Page2"">2</a>"
                + @"<a href=""Page3"">3</a>");
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            // Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
                }.AsQueryable());

            // Arrange
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;
            // Act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;
            // Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }


        [TestMethod]
        public void Can_Filter_Products()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(x => x.Products).Returns(new Product[]
            {
                new Product() { Category="Cat1", Name="P1", ProductID=1},
                new Product() { Category="Cat1", Name="P2", ProductID=2},
                new Product() { Category="Cat2", Name="P3", ProductID=3},
                new Product() { Category="Cat2", Name="P4", ProductID=4},
                new Product() { Category="Cat3", Name="P5", ProductID=5}
            }.AsQueryable());

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 10;

            var resultView = (ProductsListViewModel) controller.List("Cat1").Model;
            var selectedProducts = resultView.Products.ToArray();

            Assert.AreEqual(2, resultView.Products.Count());
            Assert.AreEqual("Cat1", resultView.CurrentCategory);

            Assert.IsTrue(selectedProducts[0].Name == "P1");
            Assert.IsTrue(selectedProducts[1].Name == "P2" && selectedProducts[0].Category == "Cat1");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            // Arrange
            // - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                new Product {ProductID = 2, Name = "P2", Category = "Apples"},
                new Product {ProductID = 3, Name = "P3", Category = "Plums"},
                new Product {ProductID = 4, Name = "P4", Category = "Oranges"},
                }.AsQueryable());

            var navController = new NavController(mock.Object);

            var resultPartialView = (IEnumerable<string>) navController.Menu().Model;
            var menuArray = resultPartialView.ToArray();

            Assert.AreEqual(3, menuArray.Count());
            Assert.AreEqual("Apples", menuArray[0]);
        }

        [TestMethod]
        public void Can_Determine_Category()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                new Product {ProductID = 2, Name = "P2", Category = "Apples"},
                new Product {ProductID = 3, Name = "P3", Category = "Plums"},
                new Product {ProductID = 4, Name = "P4", Category = "Oranges"},
            }.AsQueryable());

            var navController = new NavController(mock.Object);

            var resultPartialView = navController.Menu("Apples");
            var viewBag = resultPartialView.ViewBag;

            Assert.AreEqual("Apples", viewBag.SelectedCategory);
        }


        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                    new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                    new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                    new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                    new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                    new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
                }.AsQueryable());

            // Arrange - create a controller and make the page size 3 items
            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;

            var prodAllArray = ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;
            var prod1Array = ((ProductsListViewModel)target.List("Cat1").Model).PagingInfo.TotalItems;
            var prod2Array = ((ProductsListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;
            var prod3Array = ((ProductsListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;

            Assert.AreEqual(5, prodAllArray);
            Assert.AreEqual(2, prod1Array);
            Assert.AreEqual(2, prod2Array);
            Assert.AreEqual(1, prod3Array);
        }

        [TestMethod]
        public void CRUD_Cart()
        {
            var cart = new Cart();
            cart.AddItem(new Product { ProductID = 1, Name = "P1", Category = "Cat1", Price = 2 }, 1);
            cart.AddItem(new Product { ProductID = 2, Name = "P2", Category = "Cat2", Price = 5 }, 2);
            cart.AddItem(new Product { ProductID = 1, Name = "P1", Category = "Cat1", Price = 2 }, 1);

            var sumInCart = cart.ComputeTotalValue();

            Assert.AreEqual(4, cart.Lines.Sum(x => x.Quantity));
            Assert.AreEqual(14, sumInCart);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            // Arrange - create some test products
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            // Arrange - create a new cart
            Cart target = new Cart();
            // Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductID).ToArray();
            // Assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }



    }
    }
