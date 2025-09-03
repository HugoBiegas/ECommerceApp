using System.ComponentModel.DataAnnotations;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Range(0.01, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public string? Notes { get; set; }

        public virtual User? User { get; set; }

        public string StatusDisplay => Status.ToString();
        public int TotalItems => Items.Sum(i => i.Quantity);
    }

}
