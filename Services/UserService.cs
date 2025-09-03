using System.Text;
using ECommerceApp.Models;
using ECommerceApp.Models.Enums;
using Microsoft.VisualBasic;

namespace ECommerceApp.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static List<User> _users = new List<User>();
        private static readonly object _lock = new object();

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await Task.FromResult(_users.Where(u => u.IsActive).ToList());
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await Task.FromResult(_users.FirstOrDefault(u => u.Id == id && u.IsActive));
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await Task.FromResult(_users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && u.IsActive));
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser == null) return false;

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            // Ne pas mettre à jour le mot de passe ici pour la sécurité

            return await Task.FromResult(true);
        }
       

        public async Task<bool> UpdateUserRoleAsync(int userId, UserRole role)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return false;

            user.Role = role;
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateUserCreditsAsync(int userId, decimal credits)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return false;

            user.Credits = credits;
            return await Task.FromResult(true);
        }

        public async Task<bool> AddCreditsAsync(int userId, decimal amount)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null || amount < 0) return false;

            user.Credits += amount;
            return await Task.FromResult(true);
        }

        public async Task<bool> DeductCreditsAsync(int userId, decimal amount)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null || amount < 0 || user.Credits < amount) return false;

            user.Credits -= amount;
            return await Task.FromResult(true);
        }

        public async Task<decimal> GetUserCreditsAsync(int userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            return await Task.FromResult(user?.Credits ?? 0);
        }

        public async Task<bool> DeactivateUserAsync(int userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return false;

            user.IsActive = false;
            return await Task.FromResult(true);
        }

        public async Task<bool> ActivateUserAsync(int userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return false;

            user.IsActive = true;
            return await Task.FromResult(true);
        }

        public async Task<List<User>> GetUsersByRoleAsync(UserRole role)
        {
            return await Task.FromResult(_users.Where(u => u.Role == role && u.IsActive).ToList());
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            return await Task.FromResult(_users.Count(u => u.IsActive));
        }

        public async Task<List<User>> GetRecentUsersAsync(int count = 5)
        {
            return await Task.FromResult(_users
                .Where(u => u.IsActive)
                .OrderByDescending(u => u.CreatedDate)
                .Take(count)
                .ToList());
        }

        public static IReadOnlyList<User> GetStaticUsers()
        {
            lock (_lock)
            {
                if (_users.Count == 0)
                {
                    InitializeTestUsers();
                }

                return _users.AsReadOnly();
            }
        }

        // Méthode pour initialiser les données de test
        public static void InitializeTestUsers()
        {
            if (_users.Count == 0)
            {
                _users.AddRange(new List<User>
                {
                    new User
                    {
                        Id = 1,
                        Username = "admin",
                        Email = "admin@bookstore.com",
                        Password = HashPassword("password"),
                        FirstName = "Admin",
                        LastName = "System",
                        Role = UserRole.Admin,
                        Credits = 1000.00m,
                        CreatedDate = DateTime.Now.AddDays(-30),
                        IsActive = true
                    },
                    new User
                    {
                        Id = 2,
                        Username = "librarian",
                        Email = "librarian@bookstore.com",
                        Password = HashPassword("password"),
                        FirstName = "Marie",
                        LastName = "Libraire",
                        Role = UserRole.Librarian,
                        Credits = 500.00m,
                        CreatedDate = DateTime.Now.AddDays(-15),
                        IsActive = true
                    },
                    new User
                    {
                        Id = 3,
                        Username = "user",
                        Email = "user@bookstore.com",
                        Password = HashPassword("password"),
                        FirstName = "Jean",
                        LastName = "Lecteur",
                        Role = UserRole.User,
                        Credits = 100.00m,
                        CreatedDate = DateTime.Now.AddDays(-5),
                        IsActive = true
                    }
                });
            }
        }

        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "BookStoreSalt"));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
