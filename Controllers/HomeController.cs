using System.Diagnostics;
using ECommerceApp.Models;
using ECommerceApp.Services;
using ECommerceApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookService _bookService;
        private readonly IAuthService _authService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, IBookService bookService, IAuthService authService, ICartService cartService)
        {
            _logger = logger;
            _bookService = bookService;
            _authService = authService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            // Récupérer les livres disponibles pour la page d'accueil
            var availableBooks = await _bookService.GetAvailableBooksAsync();

            // Prendre les 8 premiers pour l'affichage
            var featuredBooks = availableBooks.Take(8).ToList();

            ViewBag.IsLoggedIn = _authService.IsLoggedIn();
            ViewBag.UserEmail = _authService.GetCurrentUserEmail();

            if (_authService.IsLoggedIn())
            {
                var userId = _authService.GetCurrentUserId();
                if (userId.HasValue)
                {
                    ViewBag.CartItemsCount = await _cartService.GetCartItemsCountAsync(userId.Value);
                    var user = await _authService.GetCurrentUserAsync();
                    ViewBag.UserCredits = user?.Credits ?? 0;
                    ViewBag.UserRole = user?.Role.ToString();
                }
            }

            return View(featuredBooks);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}