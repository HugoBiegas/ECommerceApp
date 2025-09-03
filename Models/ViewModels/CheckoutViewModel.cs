using System.ComponentModel.DataAnnotations;
using ECommerceApp.Models.Enums;


namespace ECommerceApp.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public Cart Cart { get; set; } = new Cart();

        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(100)]
        [Display(Name = "Nom complet")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress]
        [Display(Name = "Email de confirmation")]
        public string CustomerEmail { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Notes (optionnel)")]
        public string? Notes { get; set; }

        public decimal UserCredits { get; set; }
        public bool HasSufficientCredits => UserCredits >= Cart.TotalAmount;
    }

}
