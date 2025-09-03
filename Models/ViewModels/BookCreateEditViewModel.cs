using System.ComponentModel.DataAnnotations;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Models.ViewModels
{
    public class BookCreateEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le titre est requis")]
        [StringLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'auteur est requis")]
        [Display(Name = "Auteur")]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "La catégorie est requise")]
        [Display(Name = "Catégorie")]
        public BookCategory Category { get; set; }

        [Required(ErrorMessage = "Le prix est requis")]
        [Range(0.01, 999.99, ErrorMessage = "Le prix doit être entre 0.01€ et 999.99€")]
        [Display(Name = "Prix (€)")]
        public decimal Price { get; set; }

        [Display(Name = "Date de publication")]
        [DataType(DataType.Date)]
        public DateTime? PublicationDate { get; set; }

        [Display(Name = "Disponible")]
        public bool IsAvailable { get; set; } = true;

        [StringLength(13)]
        public string? ISBN { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(0, 1000, ErrorMessage = "Le stock doit être entre 0 et 1000")]
        public int Stock { get; set; } = 1;

        [StringLength(500)]
        [Display(Name = "URL de l'image")]
        public string? ImageUrl { get; set; }

        // Pour la liste déroulante des auteurs
        public List<Author>? Authors { get; set; }
    }

}
