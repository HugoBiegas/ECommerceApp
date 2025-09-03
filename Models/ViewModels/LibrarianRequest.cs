using System.ComponentModel.DataAnnotations;
using ECommerceApp.Models.Enums;


namespace ECommerceApp.Models.ViewModels
{
    public class LibrarianRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public bool IsProcessed { get; set; } = false;
    }

}
