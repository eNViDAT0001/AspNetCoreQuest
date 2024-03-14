using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.ViewModels;

namespace SportsStore.Controllers
{
    public class HomeController : Controller
    {
        public int PageSize = 4;

        private readonly IStoreRepository storeRepository;

        public HomeController(IStoreRepository storeRepository)
        {
            this.storeRepository = storeRepository;
        }

        public IActionResult Index(string? category, int productPage = 1)
        {
            return View(new ProductsListViewModel
            {
                Products = storeRepository.Products
                .Where(p => category == null || p.Category == category)
                .OrderBy(p => p.ProductID)
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = productPage,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ? storeRepository.Products.Count() : storeRepository.Products.Where(e => e.Category == category).Count()
                },
                CurrentCategory = category
            });
        }
    }
}