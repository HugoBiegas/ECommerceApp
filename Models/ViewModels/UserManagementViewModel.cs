using System.ComponentModel.DataAnnotations;
using ECommerceApp.Models.Enums;


namespace ECommerceApp.Models.ViewModels
{
    public class UserManagementViewModel
    {
        public List<User> Users { get; set; } = new List<User>();
        public List<LibrarianRequest> PendingRequests { get; set; } = new List<LibrarianRequest>();
    }

}
