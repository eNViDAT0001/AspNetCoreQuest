using Microsoft.AspNetCore.Mvc;
using SportsStore.Controllers;
using SportsStore.Models;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Tests
{
    public class OrderControllerTests
    {
        [Fact]
        public void Cannot_Checkout_Empty_Cart()
        {
            // Arrange - create a mock repository
            var mock = Substitute.For<IOrderRepository>();
            // Arrange - create an empty cart
            Cart cart = new Cart();
            // Arrange - create the order
            Order order = new Order();
            // Arrange - create an instance of the controller
            OrderController target = new OrderController(mock, cart);

            // Act
            ViewResult? result = target.Checkout(order) as ViewResult;

            // Assert - check that the order hasn't been stored
            mock.Received(0).SaveOrder(Arg.Any<Order>());
            // Assert - check that the method is returning the default view
            Assert.True(string.IsNullOrEmpty(result?.ViewName));
            // Assert - check that I am passing an invalid model to the view
            Assert.False(result?.ViewData.ModelState.IsValid);
        }

        [Fact]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            // Arrange - create a mock order repository
            var mock = Substitute.For<IOrderRepository>();
            // Arrange - create a cart with one item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            // Arrange - create an instance of the controller
            OrderController target = new OrderController(mock, cart);
            // Arrange - add an error to the model
            target.ModelState.AddModelError("error", "error");
            // Act - try to checkout
            ViewResult? result = target.Checkout(new Order()) as ViewResult;
            // Assert - check that the order hasn't been passed stored
            mock.Received(0).SaveOrder(Arg.Any<Order>());
            // Assert - check that the method is returning the default view
            Assert.True(string.IsNullOrEmpty(result?.ViewName));
            // Assert - check that I am passing an invalid model to the view
            Assert.False(result?.ViewData.ModelState.IsValid);
        }

        [Fact]
        public void Can_Checkout_And_Submit_Order()
        {
            // Arrange - create a mock order repository
            var mock = Substitute.For<IOrderRepository>();
            // Arrange - create a cart with one item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            // Arrange - create an instance of the controller
            OrderController target = new OrderController(mock, cart);
            // Act - try to checkout
            RedirectToPageResult? result =
            target.Checkout(new Order()) as RedirectToPageResult;
            // Assert - check that the order has been stored
            mock.Received(0).SaveOrder(Arg.Any<Order>());
            // Assert - check that the method is redirecting to the Completed action
            Assert.Equal("/Completed", result?.PageName);
        }
    }
}