using ECommerceApp.Models;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Models.Enums;
using System.Security.Cryptography;
using System.Text;

namespace ECommerceApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null || !user.IsActive)
                return null;

            var hashedPassword = HashPassword(password);
            return user.Password == hashedPassword ? user : null;
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return null;

            return await _userService.GetUserByIdAsync(userId.Value);
        }

        public async Task<bool> RegisterUserAsync(RegisterViewModel model)
        {
            try
            {
                // Vérifier si l'email existe déjà
                if (await IsEmailExistsAsync(model.Email))
                    return false;

                // Vérifier si le nom d'utilisateur existe déjà
                if (await IsUsernameExistsAsync(model.Username))
                    return false;

                // FIXED: Get the users list properly
                var users = UserService.GetStaticUsers().ToList();

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = HashPassword(model.Password),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Role = UserRole.User,
                    Credits = 100.00m, // Crédits de départ
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                user.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;

                // Add to the static list through UserService
                return await AddUserToStaticList(user);
            }
            catch (Exception)
            {
                return false;
            }
        }

        // HELPER METHOD: Add user to static list
        private async Task<bool> AddUserToStaticList(User user)
        {
            try
            {
                // This is a workaround - in a real app we'd use a database
                // We'll use reflection to access the private static list
                var userServiceType = typeof(UserService);
                var usersField = userServiceType.GetField("_users",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Static);

                if (usersField?.GetValue(null) is List<User> usersList)
                {
                    usersList.Add(user);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = await ValidateUserAsync(email, password);
            if (user == null) return false;

            // Stocker les informations de session
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.SetInt32("UserId", user.Id);
                session.SetString("UserEmail", user.Email);
                session.SetString("UserRole", user.Role.ToString());
            }

            return true;
        }

        public async Task LogoutAsync()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            session?.Clear();
            await Task.CompletedTask;
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            return user != null;
        }

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            var users = UserService.GetStaticUsers();
            return users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsLoggedIn()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            return session?.GetInt32("UserId") != null;
        }

        public bool HasRole(UserRole role)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            var userRole = session?.GetString("UserRole");

            if (userRole == null) return false;

            if (Enum.TryParse<UserRole>(userRole, out var currentRole))
            {
                return (int)currentRole >= (int)role;
            }

            return false;
        }

        public int? GetCurrentUserId()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            return session?.GetInt32("UserId");
        }

        public string? GetCurrentUserEmail()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            return session?.GetString("UserEmail");
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "BookStoreSalt"));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}