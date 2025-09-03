using ECommerceApp.Models;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Services
{
    public class CartService : ICartService
    {
        private static readonly Dictionary<int, Cart> _carts = new Dictionary<int, Cart>();
        private readonly IBookService _bookService;

        public CartService(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<Cart> GetCartAsync(int userId)
        {
            if (!_carts.ContainsKey(userId))
            {
                _carts[userId] = new Cart { UserId = userId };
            }
            return await Task.FromResult(_carts[userId]);
        }

        public async Task<bool> AddToCartAsync(int userId, int bookId, int quantity = 1)
        {
            if (!await _bookService.IsBookAvailableAsync(bookId, quantity))
                return false;

            var book = await _bookService.GetBookByIdAsync(bookId);
            if (book == null) return false;

            var cart = await GetCartAsync(userId);
            cart.AddItem(book, quantity);

            return true;
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int bookId)
        {
            var cart = await GetCartAsync(userId);
            cart.RemoveItem(bookId);
            return true;
        }

        public async Task<bool> UpdateQuantityAsync(int userId, int bookId, int quantity)
        {
            if (quantity > 0 && !await _bookService.IsBookAvailableAsync(bookId, quantity))
                return false;

            var cart = await GetCartAsync(userId);
            cart.UpdateQuantity(bookId, quantity);
            return true;
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            var cart = await GetCartAsync(userId);
            cart.Clear();
            return true;
        }

        public async Task<decimal> GetCartTotalAsync(int userId)
        {
            var cart = await GetCartAsync(userId);
            return cart.TotalAmount;
        }

        public async Task<int> GetCartItemsCountAsync(int userId)
        {
            var cart = await GetCartAsync(userId);
            return cart.TotalItems;
        }

        public async Task<bool> ValidateCartAsync(int userId)
        {
            var cart = await GetCartAsync(userId);
            var itemsToRemove = new List<CartItem>();

            foreach (var item in cart.Items)
            {
                if (!await _bookService.IsBookAvailableAsync(item.BookId, item.Quantity))
                {
                    itemsToRemove.Add(item);
                }
            }

            foreach (var item in itemsToRemove)
            {
                cart.RemoveItem(item.BookId);
            }

            return itemsToRemove.Count == 0;
        }
    }
}
