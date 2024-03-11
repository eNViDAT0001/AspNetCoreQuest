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
            var result = ((ViewResult)controller.Index()).Model as ProductsListViewModel ?? new();

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
            var result = ((ViewResult)controller.Index(2)).Model as ProductsListViewModel ?? new();

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
            ProductsListViewModel result = ((ViewResult)controller.Index(2)).Model as ProductsListViewModel ?? new();

            // Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.Equal(2, pageInfo.CurrentPage);
            Assert.Equal(3, pageInfo.ItemsPerPage);
            Assert.Equal(5, pageInfo.TotalItems);
            Assert.Equal(2, pageInfo.TotalPages);
        }
    }
}