using ECommerceApp.Models;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Services
{
    public class OrderService : IOrderService
    {
        private static List<Order> _orders = new List<Order>();
        private readonly IUserService _userService;
        private readonly IBookService _bookService;
        private readonly ICartService _cartService;

        public OrderService(IUserService userService, IBookService bookService, ICartService cartService)
        {
            _userService = userService;
            _bookService = bookService;
            _cartService = cartService;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await Task.FromResult(_orders.OrderByDescending(o => o.OrderDate).ToList());
        }

        public async Task<List<Order>> GetUserOrdersAsync(int userId)
        {
            return await Task.FromResult(_orders.Where(o => o.UserId == userId).OrderByDescending(o => o.OrderDate).ToList());
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await Task.FromResult(_orders.FirstOrDefault(o => o.Id == id));
        }

        public async Task<int> CreateOrderAsync(CheckoutViewModel checkoutModel, int userId)
        {
            var cart = checkoutModel.Cart;
            if (cart.Items.Count == 0) return 0;

            // Vérifier les crédits
            var userCredits = await _userService.GetUserCreditsAsync(userId);
            if (userCredits < cart.TotalAmount) return 0;

            // Vérifier la disponibilité des livres
            foreach (var item in cart.Items)
            {
                if (!await _bookService.IsBookAvailableAsync(item.BookId, item.Quantity))
                    return 0;
            }

            // Créer la commande
            var order = new Order
            {
                Id = _orders.Count > 0 ? _orders.Max(o => o.Id) + 1 : 1,
                UserId = userId,
                CustomerName = checkoutModel.CustomerName,
                CustomerEmail = checkoutModel.CustomerEmail,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                TotalAmount = cart.TotalAmount,
                Notes = checkoutModel.Notes
            };

            // Ajouter les items
            foreach (var cartItem in cart.Items)
            {
                order.Items.Add(new OrderItem
                {
                    Id = order.Items.Count + 1,
                    OrderId = order.Id,
                    BookId = cartItem.BookId,
                    BookTitle = cartItem.BookTitle,
                    UnitPrice = cartItem.UnitPrice,
                    Quantity = cartItem.Quantity
                });

                // Réduire le stock
                var book = await _bookService.GetBookByIdAsync(cartItem.BookId);
                if (book != null)
                {
                    await _bookService.UpdateStockAsync(book.Id, book.Stock - cartItem.Quantity);
                }
            }

            _orders.Add(order);

            // Déduire les crédits
            await _userService.DeductCreditsAsync(userId, cart.TotalAmount);

            // Vider le panier
            await _cartService.ClearCartAsync(userId);

            return order.Id;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) return false;

            order.Status = status;
            return await Task.FromResult(true);
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) return false;

            order.Status = OrderStatus.Cancelled;

            // Rembourser les crédits
            await _userService.AddCreditsAsync(order.UserId, order.TotalAmount);

            // Restaurer le stock
            foreach (var item in order.Items)
            {
                var book = await _bookService.GetBookByIdAsync(item.BookId);
                if (book != null)
                {
                    await _bookService.UpdateStockAsync(book.Id, book.Stock + item.Quantity);
                }
            }

            return true;
        }

        public async Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await Task.FromResult(_orders.Where(o => o.Status == status).ToList());
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await Task.FromResult(_orders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.TotalAmount));
        }

        public async Task<decimal> GetTodayRevenueAsync()
        {
            var today = DateTime.Today;
            return await Task.FromResult(_orders.Where(o => o.OrderDate.Date == today && o.Status != OrderStatus.Cancelled).Sum(o => o.TotalAmount));
        }

        public async Task<int> GetTotalOrdersCountAsync()
        {
            return await Task.FromResult(_orders.Count);
        }

        public async Task<int> GetTodayOrdersCountAsync()
        {
            var today = DateTime.Today;
            return await Task.FromResult(_orders.Count(o => o.OrderDate.Date == today));
        }

        public async Task<List<Order>> GetRecentOrdersAsync(int count = 5)
        {
            return await Task.FromResult(_orders.OrderByDescending(o => o.OrderDate).Take(count).ToList());
        }
    }
}
