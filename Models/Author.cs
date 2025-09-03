using System.ComponentModel.DataAnnotations;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de l'auteur est requis")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Nationality { get; set; }

        [Display(Name = "Date de naissance")]
        public DateTime? BirthDate { get; set; }

        [StringLength(1000)]
        public string? Biography { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();

        public int Age => BirthDate.HasValue ? DateTime.Now.Year - BirthDate.Value.Year : 0;
    }
}
