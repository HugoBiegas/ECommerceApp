
using ECommerceApp.Models;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> UpdateUserRoleAsync(int userId, UserRole role);
        Task<bool> UpdateUserCreditsAsync(int userId, decimal credits);
        Task<bool> AddCreditsAsync(int userId, decimal amount);
        Task<bool> DeductCreditsAsync(int userId, decimal amount);
        Task<decimal> GetUserCreditsAsync(int userId);
        Task<bool> DeactivateUserAsync(int userId);
        Task<bool> ActivateUserAsync(int userId);
        Task<List<User>> GetUsersByRoleAsync(UserRole role);
        Task<int> GetTotalUsersCountAsync();
        Task<List<User>> GetRecentUsersAsync(int count = 5);
    }
}
