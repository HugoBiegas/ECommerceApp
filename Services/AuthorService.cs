
using ECommerceApp.Models;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Services
{
    public class AuthorService : IAuthorService
    {
        private static List<Author> _authors = new List<Author>();

        public async Task<List<Author>> GetAllAuthorsAsync()
        {
            return await Task.FromResult(_authors.OrderBy(a => a.Name).ToList());
        }

        public async Task<Author?> GetAuthorByIdAsync(int id)
        {
            return await Task.FromResult(_authors.FirstOrDefault(a => a.Id == id));
        }

        public async Task<int> AddAuthorAsync(Author author)
        {
            author.Id = _authors.Count > 0 ? _authors.Max(a => a.Id) + 1 : 1;
            author.CreatedDate = DateTime.Now;
            _authors.Add(author);
            return await Task.FromResult(author.Id);
        }

        public async Task<bool> UpdateAuthorAsync(Author author)
        {
            var existingAuthor = _authors.FirstOrDefault(a => a.Id == author.Id);
            if (existingAuthor == null) return false;

            existingAuthor.Name = author.Name;
            existingAuthor.Nationality = author.Nationality;
            existingAuthor.BirthDate = author.BirthDate;
            existingAuthor.Biography = author.Biography;

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            if (!await CanDeleteAuthorAsync(id)) return false;

            var author = _authors.FirstOrDefault(a => a.Id == id);
            if (author == null) return false;

            _authors.Remove(author);
            return await Task.FromResult(true);
        }

        public async Task<bool> AuthorExistsAsync(int id)
        {
            return await Task.FromResult(_authors.Any(a => a.Id == id));
        }

        public async Task<bool> CanDeleteAuthorAsync(int id)
        {
            // Vérifier s'il y a des livres associés à cet auteur
            // Dans un vrai projet, on ferait une requête vers BookService
            // Ici on simule en retournant true si c'est un auteur sans "livres" (logique simplifiée)
            var booksCount = await GetBooksCountByAuthorAsync(id);
            return booksCount == 0;
        }

        public async Task<List<Author>> SearchAuthorsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAuthorsAsync();

            var term = searchTerm.ToLower();
            return await Task.FromResult(_authors
                .Where(a => a.Name.ToLower().Contains(term) ||
                           a.Nationality?.ToLower().Contains(term) == true)
                .OrderBy(a => a.Name)
                .ToList());
        }

        public async Task<int> GetBooksCountByAuthorAsync(int authorId)
        {
            // Simulation du nombre de livres par auteur
            // Dans un vrai projet, on ferait appel au BookService
            return await Task.FromResult(0); // Temporaire pour permettre la suppression
        }

        // Méthode pour initialiser les données de test
        public static void InitializeTestAuthors()
        {
            if (_authors.Count == 0)
            {
                _authors.AddRange(new List<Author>
                {
                    new Author { Id = 1, Name = "Antoine de Saint-Exupéry", Nationality = "Française", BirthDate = new DateTime(1900, 6, 29), Biography = "Écrivain, poète, aviateur et reporter français." },
                    new Author { Id = 2, Name = "George Orwell", Nationality = "Britannique", BirthDate = new DateTime(1903, 6, 25), Biography = "Écrivain et journaliste britannique, auteur de romans dystopiques." },
                    new Author { Id = 3, Name = "Albert Camus", Nationality = "Française", BirthDate = new DateTime(1913, 11, 7), Biography = "Écrivain, philosophe, romancier, dramaturge et essayiste français." },
                    new Author { Id = 4, Name = "Stephen Hawking", Nationality = "Britannique", BirthDate = new DateTime(1942, 1, 8), Biography = "Physicien théoricien et cosmologiste britannique." },
                    new Author { Id = 5, Name = "Yuval Noah Harari", Nationality = "Israélienne", BirthDate = new DateTime(1976, 2, 24), Biography = "Historien israélien, professeur à l'Université hébraïque de Jérusalem." },
                    new Author { Id = 6, Name = "Robert C. Martin", Nationality = "Américaine", BirthDate = new DateTime(1952, 12, 5), Biography = "Développeur de logiciels américain et auteur." },
                    new Author { Id = 7, Name = "Frank Herbert", Nationality = "Américaine", BirthDate = new DateTime(1920, 10, 8), Biography = "Écrivain américain de science-fiction." },
                    new Author { Id = 8, Name = "Victor Hugo", Nationality = "Française", BirthDate = new DateTime(1802, 2, 26), Biography = "Écrivain français considéré comme l'un des plus importants écrivains romantiques." },
                });
            }
        }
    }
}
