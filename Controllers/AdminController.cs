using Microsoft.AspNetCore.Mvc;
using ECommerceApp.Services;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IBookService _bookService;
        private readonly IOrderService _orderService;
        private readonly ILibrarianRequestService _librarianRequestService;

        public AdminController(
            IAuthService authService,
            IUserService userService,
            IBookService bookService,
            IOrderService orderService,
            ILibrarianRequestService librarianRequestService)
        {
            _authService = authService;
            _userService = userService;
            _bookService = bookService;
            _orderService = orderService;
            _librarianRequestService = librarianRequestService;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            if (!_authService.HasRole(UserRole.Admin))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var model = new DashboardViewModel
            {
                TotalUsers = await _userService.GetTotalUsersCountAsync(),
                TotalBooks = await _bookService.GetTotalBooksCountAsync(),
                TotalOrders = await _orderService.GetTotalOrdersCountAsync(),
                TotalRevenue = await _orderService.GetTotalRevenueAsync(),
                TodayOrders = await _orderService.GetTodayOrdersCountAsync(),
                TodayRevenue = await _orderService.GetTodayRevenueAsync(),
                PendingLibrarianRequests = await _librarianRequestService.GetPendingRequestsCountAsync(),
                TopSellingBooks = await _bookService.GetTopSellingBooksAsync(5),
                RecentOrders = await _orderService.GetRecentOrdersAsync(5),
                RecentUsers = await _userService.GetRecentUsersAsync(5)
            };

            return View(model);
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            if (!_authService.HasRole(UserRole.Admin))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var users = await _userService.GetAllUsersAsync();
            var pendingRequests = await _librarianRequestService.GetPendingRequestsAsync();

            var model = new UserManagementViewModel
            {
                Users = users,
                PendingRequests = pendingRequests
            };

            ViewBag.UserRoles = Enum.GetValues<UserRole>().Select(r => new { Value = (int)r, Text = r.ToString() });

            return View(model);
        }

        // POST: Admin/UpdateUserRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserRole(int userId, UserRole role)
        {
            if (!_authService.HasRole(UserRole.Admin))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser != null && currentUser.Id == userId && role != UserRole.Admin)
            {
                TempData["ErrorMessage"] = "Vous ne pouvez pas modifier votre propre rôle d'administrateur.";
                return RedirectToAction("Users");
            }

            var success = await _userService.UpdateUserRoleAsync(userId, role);
            if (success)
            {
                TempData["SuccessMessage"] = "Rôle utilisateur mis à jour avec succès.";
            }
            else
            {
                TempData["ErrorMessage"] = "Erreur lors de la mise à jour du rôle.";
            }

            return RedirectToAction("Users");
        }

        // POST: Admin/UpdateUserCredits
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserCredits(int userId, decimal credits)
        {
            if (!_authService.HasRole(UserRole.Admin))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (credits < 0)
            {
                TempData["ErrorMessage"] = "Les crédits ne peuvent pas être négatifs.";
                return RedirectToAction("Users");
            }

            var success = await _userService.UpdateUserCreditsAsync(userId, credits);
            if (success)
            {
                TempData["SuccessMessage"] = "Crédits utilisateur mis à jour avec succès.";
            }
            else
            {
                TempData["ErrorMessage"] = "Erreur lors de la mise à jour des crédits.";
            }

            return RedirectToAction("Users");
        }

        // POST: Admin/DeactivateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateUser(int userId)
        {
            if (!_authService.HasRole(UserRole.Admin))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser != null && currentUser.Id == userId)
            {
                TempData["ErrorMessage"] = "Vous ne pouvez pas désactiver votre propre compte.";
                return RedirectToAction("Users");
            }

            var success = await _userService.DeactivateUserAsync(userId);
            if (success)
            {
                TempData["SuccessMessage"] = "Utilisateur désactivé avec succès.";
            }
            else
            {
                TempData["ErrorMessage"] = "Erreur lors de la désactivation.";
            }

            return RedirectToAction("Users");
        }

        // POST: Admin/ActivateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateUser(int userId)
        {
            if (!_authService.HasRole(UserRole.Admin))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var success = await _userService.ActivateUserAsync(userId);
            if (success)
            {
                TempData["SuccessMessage"] = "Utilisateur activé avec succès.";
            }
            else
            {
                TempData["ErrorMessage"] = "Erreur lors de l'activation.";
            }

            return RedirectToAction("Users");
        }

        // POST: Admin/ApproveLibrarianRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveLibrarianRequest(int requestId)
        {
            if (!_authService.HasRole(UserRole.Admin))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var success = await _librarianRequestService.ApproveRequestAsync(requestId);
            if (success)
            {
                TempData["SuccessMessage"] = "Demande de libraire approuvée. L'utilisateur a été promu.";
            }
            else
            {
                TempData["ErrorMessage"] = "Erreur lors de l'approbation de la demande.";
            }

            return RedirectToAction("Users");
        }

        // POST: Admin/RejectLibrarianRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectLibrarianRequest(int requestId)
        {
            if (!_authService.HasRole(UserRole.Admin))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var success = await _librarianRequestService.RejectRequestAsync(requestId);
            if (success)
            {
                TempData["InfoMessage"] = "Demande de libraire rejetée.";
            }
            else
            {
                TempData["ErrorMessage"] = "Erreur lors du rejet de la demande.";
            }

            return RedirectToAction("Users");
        }

        // GET: Admin/Statistics
        public async Task<IActionResult> Statistics()
        {
            if (!_authService.HasRole(UserRole.Admin))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var model = new DashboardViewModel
            {
                TotalUsers = await _userService.GetTotalUsersCountAsync(),
                TotalBooks = await _bookService.GetTotalBooksCountAsync(),
                TotalOrders = await _orderService.GetTotalOrdersCountAsync(),
                TotalRevenue = await _orderService.GetTotalRevenueAsync(),
                TodayOrders = await _orderService.GetTodayOrdersCountAsync(),
                TodayRevenue = await _orderService.GetTodayRevenueAsync(),
                TopSellingBooks = await _bookService.GetTopSellingBooksAsync(10)
            };

            // Statistiques par rôle
            var usersByRole = new Dictionary<UserRole, int>();
            foreach (UserRole role in Enum.GetValues<UserRole>())
            {
                var users = await _userService.GetUsersByRoleAsync(role);
                usersByRole[role] = users.Count;
            }
            ViewBag.UsersByRole = usersByRole;

            // Statistiques par statut de commande
            var ordersByStatus = new Dictionary<OrderStatus, int>();
            foreach (OrderStatus status in Enum.GetValues<OrderStatus>())
            {
                var orders = await _orderService.GetOrdersByStatusAsync(status);
                ordersByStatus[status] = orders.Count;
            }
            ViewBag.OrdersByStatus = ordersByStatus;

            return View(model);
        }
    }
}