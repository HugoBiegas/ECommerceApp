using ECommerceApp.Models;
using ECommerceApp.Models.Enums;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    public class OrdersController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrdersController(
            IOrderService orderService,
            IAuthService authService,
            ICartService cartService)
            : base(authService, cartService)
        {
            _orderService = orderService;
        }

        // GET: Orders
        public async Task<IActionResult> Index(OrderStatus? status = null)
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

            List<Order> orders;
            var isLibrarianOrAdmin = _authService.HasRole(UserRole.Librarian);

            if (isLibrarianOrAdmin)
            {
                // Librarians and Admins can see all orders
                if (status.HasValue)
                {
                    orders = await _orderService.GetOrdersByStatusAsync(status.Value);
                }
                else
                {
                    orders = await _orderService.GetAllOrdersAsync();
                }
            }
            else
            {
                // Regular users see only their orders
                orders = await _orderService.GetUserOrdersAsync(userId.Value);
                if (status.HasValue)
                {
                    orders = orders.Where(o => o.Status == status.Value).ToList();
                }
            }

            ViewBag.IsLibrarianOrAdmin = isLibrarianOrAdmin;
            ViewBag.OrderStatuses = Enum.GetValues<OrderStatus>().Select(s => new { Value = (int)s, Text = s.ToString() });

            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (!_authService.IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var userId = _authService.GetCurrentUserId();
            var isLibrarianOrAdmin = _authService.HasRole(UserRole.Librarian);

            // Check if user can access this order
            if (!isLibrarianOrAdmin && order.UserId != userId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewBag.IsLibrarianOrAdmin = isLibrarianOrAdmin;
            ViewBag.CanManageOrders = isLibrarianOrAdmin; // AJOUT : ViewBag manquant
            ViewBag.OrderStatuses = Enum.GetValues<OrderStatus>().Select(s => new { Value = (int)s, Text = s.ToString() });

            return View(order);
        }

        // POST: Orders/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus status)
        {
            if (!_authService.HasRole(UserRole.Librarian))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            bool success;

            // Si le statut est Cancelled, utiliser la méthode CancelOrderAsync pour la logique complète
            if (status == OrderStatus.Cancelled)
            {
                success = await _orderService.CancelOrderAsync(orderId);
                if (success)
                {
                    TempData["SuccessMessage"] = "Commande annulée avec succès. Les crédits ont été remboursés.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Erreur lors de l'annulation de la commande.";
                }
            }
            else
            {
                success = await _orderService.UpdateOrderStatusAsync(orderId, status);
                if (success)
                {
                    TempData["SuccessMessage"] = "Statut de la commande mis à jour avec succès.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Erreur lors de la mise à jour du statut.";
                }
            }

            return RedirectToAction("Details", new { id = orderId });
        }

        // POST: Orders/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id) // CORRECTION : id au lieu de orderId
        {
            if (!_authService.IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var userId = _authService.GetCurrentUserId();
            var isLibrarianOrAdmin = _authService.HasRole(UserRole.Librarian);

            // Vérifier les droits : propriétaire de la commande OU Librarian/Admin
            if (!isLibrarianOrAdmin && order.UserId != userId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var success = await _orderService.CancelOrderAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Commande annulée avec succès. Les crédits ont été remboursés.";
            }
            else
            {
                TempData["ErrorMessage"] = "Impossible d'annuler cette commande.";
            }

            return RedirectToAction("Details", new { id });
        }
    }
}