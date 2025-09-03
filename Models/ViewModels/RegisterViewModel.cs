using System.ComponentModel.DataAnnotations;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
        [StringLength(50, ErrorMessage = "Le nom d'utilisateur ne peut pas dépasser 50 caractères")]
        [Display(Name = "Nom d'utilisateur")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Veuillez confirmer le mot de passe")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas")]
        [Display(Name = "Confirmer le mot de passe")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Prénom")]
        public string? FirstName { get; set; }

        [StringLength(100)]
        [Display(Name = "Nom")]
        public string? LastName { get; set; }
    }

}
