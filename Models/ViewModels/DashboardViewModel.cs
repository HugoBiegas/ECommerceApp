using System.ComponentModel.DataAnnotations;
using ECommerceApp.Models.Enums;


namespace ECommerceApp.Models.ViewModels
{
    public class DashboardViewModel
    {
        // Statistiques générales
        public int TotalUsers { get; set; }
        public int TotalBooks { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        // Statistiques du jour
        public int TodayOrders { get; set; }
        public decimal TodayRevenue { get; set; }

        // Demandes en attente
        public int PendingLibrarianRequests { get; set; }

        // Livres les plus vendus
        public List<Book> TopSellingBooks { get; set; } = new List<Book>();

        // Commandes récentes
        public List<Order> RecentOrders { get; set; } = new List<Order>();

        // Utilisateurs récents
        public List<User> RecentUsers { get; set; } = new List<User>();
    }

}
