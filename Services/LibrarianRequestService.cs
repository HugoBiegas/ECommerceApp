
using ECommerceApp.Models;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Services
{
    public class LibrarianRequestService : ILibrarianRequestService
    {
        private static List<LibrarianRequest> _requests = new List<LibrarianRequest>();
        private readonly IUserService _userService;

        public LibrarianRequestService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<int> CreateRequestAsync(int userId, string reason)
        {
            // Vérifier s'il y a déjà une demande en attente
            if (await HasPendingRequestAsync(userId))
                return 0;

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return 0;

            var request = new LibrarianRequest
            {
                Id = _requests.Count > 0 ? _requests.Max(r => r.Id) + 1 : 1,
                UserId = userId,
                Username = user.Username,
                Email = user.Email,
                Reason = reason,
                RequestDate = DateTime.Now,
                IsProcessed = false
            };

            _requests.Add(request);
            return await Task.FromResult(request.Id);
        }

        public async Task<List<LibrarianRequest>> GetPendingRequestsAsync()
        {
            return await Task.FromResult(_requests.Where(r => !r.IsProcessed).OrderBy(r => r.RequestDate).ToList());
        }

        public async Task<LibrarianRequest?> GetRequestByIdAsync(int id)
        {
            return await Task.FromResult(_requests.FirstOrDefault(r => r.Id == id));
        }

        public async Task<bool> ApproveRequestAsync(int requestId)
        {
            var request = _requests.FirstOrDefault(r => r.Id == requestId);
            if (request == null || request.IsProcessed) return false;

            // Promouvoir l'utilisateur
            var success = await _userService.UpdateUserRoleAsync(request.UserId, UserRole.Librarian);
            if (success)
            {
                request.IsProcessed = true;
                return true;
            }

            return false;
        }

        public async Task<bool> RejectRequestAsync(int requestId)
        {
            var request = _requests.FirstOrDefault(r => r.Id == requestId);
            if (request == null || request.IsProcessed) return false;

            request.IsProcessed = true;
            return await Task.FromResult(true);
        }

        public async Task<bool> HasPendingRequestAsync(int userId)
        {
            return await Task.FromResult(_requests.Any(r => r.UserId == userId && !r.IsProcessed));
        }

        public async Task<int> GetPendingRequestsCountAsync()
        {
            return await Task.FromResult(_requests.Count(r => !r.IsProcessed));
        }
    }
}
