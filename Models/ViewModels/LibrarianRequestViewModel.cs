using System.ComponentModel.DataAnnotations;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Models.ViewModels
{
    public class LibrarianRequestViewModel
    {
        [Required(ErrorMessage = "Veuillez expliquer pourquoi vous souhaitez devenir libraire")]
        [StringLength(500, ErrorMessage = "La raison ne peut pas dépasser 500 caractères")]
        [Display(Name = "Pourquoi souhaitez-vous devenir libraire ?")]
        public string Reason { get; set; } = string.Empty;
    }

}
