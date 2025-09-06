using Microsoft.AspNetCore.Mvc;
using ECommerceApp.Models;
using ECommerceApp.Services;
using System.Diagnostics;

namespace ECommerceApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookService _bookService;

        public HomeController(
            ILogger<HomeController> logger,
            IBookService bookService,
            IAuthService authService,
            ICartService cartService)
            : base(authService, cartService)
        {
            _logger = logger;
            _bookService = bookService;
        }

        public async Task<IActionResult> Index()
        {
            // Get featured books for homepage
            var featuredBooks = await _bookService.GetAvailableBooksAsync();

            // ViewBag is automatically set by BaseController
            return View(featuredBooks);
        }

        public IActionResult Privacy()
        {
            // ViewBag is automatically set by BaseController
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}