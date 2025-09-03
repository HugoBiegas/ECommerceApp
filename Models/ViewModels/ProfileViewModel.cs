using System.ComponentModel.DataAnnotations;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Models.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "Nom d'utilisateur")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Prénom")]
        public string? FirstName { get; set; }

        [StringLength(100)]
        [Display(Name = "Nom")]
        public string? LastName { get; set; }

        [Display(Name = "Rôle")]
        public UserRole Role { get; set; }

        [Display(Name = "Crédits")]
        public decimal Credits { get; set; }

        [Display(Name = "Membre depuis")]
        public DateTime CreatedDate { get; set; }
    }

}
