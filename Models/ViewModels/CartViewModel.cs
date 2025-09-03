using System.ComponentModel.DataAnnotations;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Models.ViewModels
{
    public class CartViewModel
    {
        public Cart Cart { get; set; } = new Cart();
        public decimal UserCredits { get; set; }
        public bool HasSufficientCredits => UserCredits >= Cart.TotalAmount;
        public List<Book> Books { get; set; } = new List<Book>(); // Pour récupérer les détails complets des livres
    }

}
