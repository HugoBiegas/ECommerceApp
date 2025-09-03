
using ECommerceApp.Models;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Services
{
    public interface ICartService
    {
        Task<Cart> GetCartAsync(int userId);
        Task<bool> AddToCartAsync(int userId, int bookId, int quantity = 1);
        Task<bool> RemoveFromCartAsync(int userId, int bookId);
        Task<bool> UpdateQuantityAsync(int userId, int bookId, int quantity);
        Task<bool> ClearCartAsync(int userId);
        Task<decimal> GetCartTotalAsync(int userId);
        Task<int> GetCartItemsCountAsync(int userId);
        Task<bool> ValidateCartAsync(int userId); // Vérifier stock et disponibilité
    }

}
