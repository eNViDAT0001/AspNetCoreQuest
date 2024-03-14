using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using SportsStore.Controllers;
using SportsStore.Models;
using SportsStore.Models.ViewModels;

namespace SportsStore.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void Can_Use_Repository()
        {
            // Arrange
            var mock = Substitute.For<IStoreRepository>();
            mock.Products.Returns((new Product[] {
                 new Product {ProductID = 1, Name = "P1"},
                 new Product {ProductID = 2, Name = "P2"}
                 }).AsQueryable());

            var controller = new HomeController(mock);

            // Act
            var result = ((ViewResult)controller.Index(null)).Model as ProductsListViewModel ?? new();

            // Assert
            Product[] prodArray = result.Products.ToArray();

            Assert.True(prodArray.Length == 2);
            Assert.Equal("P1", prodArray[0].Name);
            Assert.Equal("P2", prodArray[1].Name);
        }

        [Fact]
        public void Can_Paginate()
        {
            // Arrange
            var mock = Substitute.For<IStoreRepository>();
            mock.Products.Returns((new Product[] {
                                                 new Product {ProductID = 1, Name = "P1"},
                                                 new Product {ProductID = 2, Name = "P2"},
                                                 new Product {ProductID = 3, Name = "P3"},
                                                 new Product {ProductID = 4, Name = "P4"},
                                                 new Product {ProductID = 5, Name = "P5"}
                                                 }).AsQueryable());
            HomeController controller = new HomeController(mock);
            controller.PageSize = 3;

            // Act
            var result = ((ViewResult)controller.Index(null, 2)).Model as ProductsListViewModel ?? new();

            // Assert
            Product[] prodArray = result.Products.ToArray();

            Assert.True(prodArray.Length == 2);
            Assert.Equal("P4", prodArray[0].Name);
            Assert.Equal("P5", prodArray[1].Name);
        }

        [Fact]
        public void Can_Send_Pagination_View_Model()
        {
            // Arrange
            var mock = Substitute.For<IStoreRepository>();
            mock.Products.Returns((new Product[] {
                                                 new Product {ProductID = 1, Name = "P1"},
                                                 new Product {ProductID = 2, Name = "P2"},
                                                 new Product {ProductID = 3, Name = "P3"},
                                                 new Product {ProductID = 4, Name = "P4"},
                                                 new Product {ProductID = 5, Name = "P5"}
                                                 }).AsQueryable<Product>());
            // Arrange
            HomeController controller = new HomeController(mock) { PageSize = 3 };

            // Act
            ProductsListViewModel result = ((ViewResult)controller.Index(null, 2)).Model as ProductsListViewModel ?? new();

            // Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.Equal(2, pageInfo.CurrentPage);
            Assert.Equal(3, pageInfo.ItemsPerPage);
            Assert.Equal(5, pageInfo.TotalItems);
            Assert.Equal(2, pageInfo.TotalPages);
        }

        [Fact]
        public void Can_Filter_Products()
        {
            // Arrange
            // - create the mock repository
            var mock = Substitute.For<IStoreRepository>();
            mock.Products.Returns((new Product[] {
                                     new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                                     new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                                     new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                                     new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                                     new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
                                     }).AsQueryable());

            // Arrange - create a controller and make the page size 3 items
            HomeController controller = new HomeController(mock);
            controller.PageSize = 3;

            // Action
            Product[] result = (((ViewResult)controller.Index("Cat2", 1)).Model as ProductsListViewModel ?? new()).Products.ToArray();

            // Assert
            Assert.Equal(2, result.Length);
            Assert.True(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.True(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [Fact]
        public void Generate_Category_Specific_Product_Count()
        {
            // Arrange
            var mock = Substitute.For<IStoreRepository>();
            mock.Products.Returns((new Product[] {
                                     new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                                     new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                                     new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                                     new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                                     new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
                                     }).AsQueryable());

            var target = new HomeController(mock);
            target.PageSize = 3;

            Func<ViewResult, ProductsListViewModel?> GetModel = result => result?.ViewData?.Model as ProductsListViewModel;

            // Action
            int? res1 = GetModel((ViewResult)target.Index("Cat1"))?.PagingInfo.TotalItems;
            int? res2 = GetModel((ViewResult)target.Index("Cat2"))?.PagingInfo.TotalItems;
            int? res3 = GetModel((ViewResult)target.Index("Cat3"))?.PagingInfo.TotalItems;
            int? resAll = GetModel((ViewResult)target.Index(null))?.PagingInfo.TotalItems;

            // Assert
            Assert.Equal(2, res1);
            Assert.Equal(2, res2);
            Assert.Equal(1, res3);
            Assert.Equal(5, resAll);
        }
    }
}