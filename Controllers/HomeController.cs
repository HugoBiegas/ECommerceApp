using Microsoft.AspNetCore.Mvc;
using ECommerceApp.Models;
using ECommerceApp.Services;
using System.Diagnostics;

namespace ECommerceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookService _bookService;
        private readonly IAuthService _authService;
        private readonly ICartService _cartService;

        public HomeController(
            ILogger<HomeController> logger,
            IBookService bookService,
            IAuthService authService,
            ICartService cartService)
        {
            _logger = logger;
            _bookService = bookService;
            _authService = authService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            // Get featured books for homepage
            var featuredBooks = await _bookService.GetAvailableBooksAsync();

            // FIXED: Populate ViewBag with user information for layout consistency
            await SetViewBagUserInfoAsync();

            return View(featuredBooks);
        }

        public async Task<IActionResult> Privacy()
        {
            await SetViewBagUserInfoAsync();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // HELPER: Set ViewBag user information for layout
        private async Task SetViewBagUserInfoAsync()
        {
            ViewBag.IsLoggedIn = _authService.IsLoggedIn();

            if (_authService.IsLoggedIn())
            {
                var user = await _authService.GetCurrentUserAsync();
                if (user != null)
                {
                    ViewBag.UserCredits = user.Credits;
                    ViewBag.UserRole = user.Role;
                    ViewBag.UserEmail = user.Email;
                    ViewBag.CartItemsCount = await _cartService.GetCartItemsCountAsync(user.Id);
                }
            }
            else
            {
                ViewBag.UserCredits = 0;
                ViewBag.CartItemsCount = 0;
            }
        }
    }
}