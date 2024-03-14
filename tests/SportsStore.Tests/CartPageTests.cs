using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute;
using System.Text.Json;
using Microsoft.AspNetCore.Routing;

namespace SportsStore.Tests
{
    public class CartPageTests
    {
        [Fact]
        public void Can_Load_Cart()
        {
            // Arrange
            // - create a mock repository
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            var mockRepo = Substitute.For<IStoreRepository>();
            mockRepo.Products.Returns((new Product[] { p1, p2 }).AsQueryable());

            // - create a cart
            Cart testCart = new Cart();
            testCart.AddItem(p1, 2);
            testCart.AddItem(p2, 1);

            // Action
            CartModel cartModel = new CartModel(mockRepo, testCart);
            cartModel.OnGet("myUrl");

            //Assert
            Assert.Equal(2, cartModel.Cart?.Lines.Count());
            Assert.Equal("myUrl", cartModel.ReturnUrl);
        }

        [Fact]
        public void Can_Update_Cart()
        {
            // Arrange
            // - create a substitute repository
            var mockRepo = Substitute.For<IStoreRepository>();
            mockRepo.Products.Returns((new Product[] { new Product { ProductID = 1, Name = "P1" } }).AsQueryable());

            Cart? testCart = new Cart();

            // Action
            var cartModel = new CartModel(mockRepo, testCart);
            cartModel.OnPost(1, "myUrl");

            // Assert
            Assert.Single(testCart.Lines);
            Assert.Equal("P1", testCart.Lines.First().Product.Name);
            Assert.Equal(1, testCart.Lines.First().Quantity);
        }
    }
}