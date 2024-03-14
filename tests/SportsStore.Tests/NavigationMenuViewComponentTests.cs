using SportsStore.Components;
using SportsStore.Models;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SportsStore.Tests
{
    public class NavigationMenuViewComponentTests
    {
        [Fact]
        public void Can_Select_Categories()
        {
            // Arrange
            var mock = Substitute.For<IStoreRepository>();
            mock.Products.Returns((new Product[] {
                                 new Product {ProductID = 1, Name = "P1",
                                 Category = "Apples"},
                                 new Product {ProductID = 2, Name = "P2",
                                 Category = "Apples"},
                                 new Product {ProductID = 3, Name = "P3",
                                 Category = "Plums"},
                                 new Product {ProductID = 4, Name = "P4",
                                 Category = "Oranges"},
                                 }).AsQueryable());

            var target = new NavigationMenuViewComponent(mock);

            // Act = get the set of categories
            string[] results = (((ViewViewComponentResult)target.Invoke())?.ViewData?.Model as IEnumerable<string> ?? Enumerable.Empty<string>()).ToArray();

            // Assert
            Assert.True(Enumerable.SequenceEqual(new string[] { "Apples", "Oranges", "Plums" }, results));
        }

        [Fact]
        public void Indicates_Selected_Category()
        {
            // Arrange
            string categoryToSelect = "Apples";
            var mock = Substitute.For<IStoreRepository>();
            mock.Products.Returns((new Product[] { 
                new Product {ProductID = 1, Name = "P1", Category = "Apples"}, 
                new Product {ProductID = 4, Name = "P2", Category = "Oranges"},
            }).AsQueryable());

            NavigationMenuViewComponent target = new NavigationMenuViewComponent(mock);
            target.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext
                {
                    RouteData = new Microsoft.AspNetCore.Routing.RouteData()
                }
            };
            target.RouteData.Values["category"] = categoryToSelect;

            // Action
            var result = ((ViewViewComponentResult)target.Invoke())?.ViewData?["SelectedCategory"] as string;

            // Assert
            Assert.Equal(categoryToSelect, result);

        }
    }
}
