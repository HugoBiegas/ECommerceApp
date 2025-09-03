

using ECommerceApp.Models;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Services
{
    public interface ILibrarianRequestService
    {
        Task<int> CreateRequestAsync(int userId, string reason);
        Task<List<LibrarianRequest>> GetPendingRequestsAsync();
        Task<LibrarianRequest?> GetRequestByIdAsync(int id);
        Task<bool> ApproveRequestAsync(int requestId);
        Task<bool> RejectRequestAsync(int requestId);
        Task<bool> HasPendingRequestAsync(int userId);
        Task<int> GetPendingRequestsCountAsync();
    }

}
