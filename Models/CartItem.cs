using System.ComponentModel.DataAnnotations;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Models
{
    public class CartItem
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal TotalPrice => UnitPrice * Quantity;
    }

}
