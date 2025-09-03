using ECommerceApp.Models.Enums;
using ECommerceApp.Models;
using ECommerceApp.Models.ViewModels;

namespace ECommerceApp.Services
{
    public interface IAuthService
    {
        Task<User?> ValidateUserAsync(string email, string password);
        Task<User?> GetCurrentUserAsync();
        Task<bool> RegisterUserAsync(RegisterViewModel model);
        Task<bool> LoginAsync(string email, string password);
        Task LogoutAsync();
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsUsernameExistsAsync(string username);
        bool IsLoggedIn();
        bool HasRole(UserRole role);
        int? GetCurrentUserId();
        string? GetCurrentUserEmail();
    }
}
