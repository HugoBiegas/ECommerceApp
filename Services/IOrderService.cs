
using ECommerceApp.Models;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetUserOrdersAsync(int userId);
        Task<Order?> GetOrderByIdAsync(int id);
        Task<int> CreateOrderAsync(CheckoutViewModel checkoutModel, int userId);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<bool> CancelOrderAsync(int orderId);
        Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetTodayRevenueAsync();
        Task<int> GetTotalOrdersCountAsync();
        Task<int> GetTodayOrdersCountAsync();
        Task<List<Order>> GetRecentOrdersAsync(int count = 5);
    }

}
