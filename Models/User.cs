using System.ComponentModel.DataAnnotations;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
        [StringLength(50, ErrorMessage = "Le nom d'utilisateur ne peut pas dépasser 50 caractères")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
        public string Password { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.User;

        [Range(0, double.MaxValue, ErrorMessage = "Les crédits ne peuvent pas être négatifs")]
        public decimal Credits { get; set; } = 100.00m;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
