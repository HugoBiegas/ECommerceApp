using ECommerceApp.Models;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    public class CartController : BaseController
    {
        private readonly IBookService _bookService;

        public CartController(
            ICartService cartService,
            IBookService bookService,
            IAuthService authService)
            : base(authService, cartService)
        {
            _bookService = bookService;
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            if (!_authService.IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _authService.GetCurrentUserAsync();
            var cart = await _cartService.GetCartAsync(userId.Value);

            // Get complete book details for cart items
            var books = new List<Book>();
            foreach (var item in cart.Items)
            {
                var book = await _bookService.GetBookByIdAsync(item.BookId);
                if (book != null)
                {
                    books.Add(book);
                }
            }

            var model = new CartViewModel
            {
                Cart = cart,
                UserCredits = user?.Credits ?? 0,
                Books = books
            };

            return View(model);
        }

        // POST: Cart/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int bookId, int quantity)
        {
            if (!_authService.IsLoggedIn())
            {
                return Json(new { success = false, message = "Vous devez être connecté." });
            }

            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Session expirée." });
            }

            var success = await _cartService.UpdateQuantityAsync(userId.Value, bookId, quantity);
            if (success)
            {
                var newTotal = await _cartService.GetCartTotalAsync(userId.Value);
                var newItemsCount = await _cartService.GetCartItemsCountAsync(userId.Value);

                return Json(new
                {
                    success = true,
                    message = "Quantité mise à jour.",
                    newTotal = newTotal,
                    newItemsCount = newItemsCount
                });
            }

            return Json(new { success = false, message = "Erreur lors de la mise à jour." });
        }

        // POST: Cart/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int bookId)
        {
            if (!_authService.IsLoggedIn())
            {
                return Json(new { success = false, message = "Vous devez être connecté." });
            }

            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Session expirée." });
            }

            var success = await _cartService.RemoveFromCartAsync(userId.Value, bookId);
            if (success)
            {
                var newTotal = await _cartService.GetCartTotalAsync(userId.Value);
                var newItemsCount = await _cartService.GetCartItemsCountAsync(userId.Value);

                return Json(new
                {
                    success = true,
                    message = "Article retiré du panier.",
                    newTotal = newTotal,
                    newItemsCount = newItemsCount
                });
            }

            return Json(new { success = false, message = "Erreur lors de la suppression." });
        }

        // GET: Cart/Checkout
        public async Task<IActionResult> Checkout()
        {
            if (!_authService.IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _authService.GetCurrentUserAsync();
            var cart = await _cartService.GetCartAsync(userId.Value);

            if (cart.Items.Count == 0)
            {
                TempData["InfoMessage"] = "Votre panier est vide.";
                return RedirectToAction("Index", "Books");
            }

            // Validate cart before checkout
            await _cartService.ValidateCartAsync(userId.Value);

            var model = new CheckoutViewModel
            {
                Cart = cart,
                CustomerName = user?.FullName ?? "",
                CustomerEmail = user?.Email ?? "",
                UserCredits = user?.Credits ?? 0
            };

            return View(model);
        }

        // POST: Cart/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!_authService.IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            // Re-populate cart data
            model.Cart = await _cartService.GetCartAsync(userId.Value);
            var user = await _authService.GetCurrentUserAsync();
            model.UserCredits = user?.Credits ?? 0;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Cart.Items.Count == 0)
            {
                TempData["ErrorMessage"] = "Votre panier est vide.";
                return RedirectToAction("Index", "Books");
            }

            if (!model.HasSufficientCredits)
            {
                TempData["ErrorMessage"] = "Crédits insuffisants pour passer cette commande.";
                return View(model);
            }

            // Create order through OrderService
            var orderService = HttpContext.RequestServices.GetRequiredService<IOrderService>();
            var orderId = await orderService.CreateOrderAsync(model, userId.Value);

            if (orderId > 0)
            {
                TempData["SuccessMessage"] = $"Commande #{orderId} créée avec succès !";
                return RedirectToAction("Details", "Orders", new { id = orderId });
            }

            TempData["ErrorMessage"] = "Erreur lors de la création de la commande.";
            return View(model);
        }
    }
}