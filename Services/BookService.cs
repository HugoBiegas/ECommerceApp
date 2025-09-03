using ECommerceApp.Models;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Services
{
    public class BookService : IBookService
    {
        private static List<Book> _books = new List<Book>();
        private readonly IAuthorService _authorService;

        public BookService(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            var books = _books.ToList();

            // Associer les auteurs aux livres
            foreach (var book in books)
            {
                book.Author = authors.FirstOrDefault(a => a.Id == book.AuthorId);
            }

            return books;
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                var authors = await _authorService.GetAllAuthorsAsync();
                book.Author = authors.FirstOrDefault(a => a.Id == book.AuthorId);
            }
            return book;
        }

        public async Task<List<Book>> SearchBooksAsync(BookListViewModel searchModel)
        {
            var books = await GetAllBooksAsync();
            var query = books.AsEnumerable();

            // Filtrer par terme de recherche
            if (!string.IsNullOrWhiteSpace(searchModel.SearchTerm))
            {
                var searchTerm = searchModel.SearchTerm.ToLower();
                query = query.Where(b =>
                    b.Title.ToLower().Contains(searchTerm) ||
                    b.Author?.Name.ToLower().Contains(searchTerm) == true ||
                    b.Description?.ToLower().Contains(searchTerm) == true);
            }

            // Filtrer par catégorie
            if (searchModel.CategoryFilter.HasValue)
            {
                query = query.Where(b => b.Category == searchModel.CategoryFilter.Value);
            }

            // Filtrer par prix
            if (searchModel.MinPrice.HasValue)
            {
                query = query.Where(b => b.Price >= searchModel.MinPrice.Value);
            }

            if (searchModel.MaxPrice.HasValue)
            {
                query = query.Where(b => b.Price <= searchModel.MaxPrice.Value);
            }

            // Filtrer par disponibilité
            if (searchModel.AvailableOnly == true)
            {
                query = query.Where(b => b.InStock);
            }

            // Trier
            query = searchModel.SortBy.ToLower() switch
            {
                "price_asc" => query.OrderBy(b => b.Price),
                "price_desc" => query.OrderByDescending(b => b.Price),
                "date_asc" => query.OrderBy(b => b.PublicationDate ?? DateTime.MinValue),
                "date_desc" => query.OrderByDescending(b => b.PublicationDate ?? DateTime.MinValue),
                "author" => query.OrderBy(b => b.Author?.Name ?? ""),
                _ => query.OrderBy(b => b.Title)
            };

            return query.ToList();
        }

        public async Task<int> AddBookAsync(Book book)
        {
            book.Id = _books.Count > 0 ? _books.Max(b => b.Id) + 1 : 1;
            book.CreatedDate = DateTime.Now;
            _books.Add(book);
            return await Task.FromResult(book.Id);
        }

        public async Task<bool> UpdateBookAsync(Book book)
        {
            var existingBook = _books.FirstOrDefault(b => b.Id == book.Id);
            if (existingBook == null) return false;

            existingBook.Title = book.Title;
            existingBook.AuthorId = book.AuthorId;
            existingBook.Category = book.Category;
            existingBook.Price = book.Price;
            existingBook.PublicationDate = book.PublicationDate;
            existingBook.IsAvailable = book.IsAvailable;
            existingBook.ISBN = book.ISBN;
            existingBook.Description = book.Description;
            existingBook.Stock = book.Stock;
            existingBook.ImageUrl = book.ImageUrl;

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null) return false;

            _books.Remove(book);
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateStockAsync(int bookId, int newStock)
        {
            var book = _books.FirstOrDefault(b => b.Id == bookId);
            if (book == null) return false;

            book.Stock = newStock;
            return await Task.FromResult(true);
        }

        public async Task<bool> IsBookAvailableAsync(int bookId, int quantity)
        {
            var book = _books.FirstOrDefault(b => b.Id == bookId);
            return await Task.FromResult(book != null && book.IsAvailable && book.Stock >= quantity);
        }

        public async Task<List<Book>> GetBooksByCategoryAsync(BookCategory category)
        {
            var books = await GetAllBooksAsync();
            return books.Where(b => b.Category == category && b.InStock).ToList();
        }

        public async Task<List<Book>> GetBooksByAuthorAsync(int authorId)
        {
            var books = await GetAllBooksAsync();
            return books.Where(b => b.AuthorId == authorId).ToList();
        }

        public async Task<List<Book>> GetAvailableBooksAsync()
        {
            var books = await GetAllBooksAsync();
            return books.Where(b => b.InStock).ToList();
        }

        public async Task<int> GetTotalBooksCountAsync()
        {
            return await Task.FromResult(_books.Count);
        }

        public async Task<List<Book>> GetTopSellingBooksAsync(int count = 5)
        {
            // Simuler les livres les plus vendus (dans un vrai projet, on baserait cela sur les statistiques de ventes)
            var books = await GetAvailableBooksAsync();
            return books.Take(count).ToList();
        }

        public async Task<List<Book>> GetRecentBooksAsync(int count = 5)
        {
            var books = await GetAllBooksAsync();
            return books.OrderByDescending(b => b.CreatedDate).Take(count).ToList();
        }

        public async Task<bool> BookExistsAsync(int id)
        {
            return await Task.FromResult(_books.Any(b => b.Id == id));
        }

        // Méthode pour initialiser les données de test
        public static void InitializeTestBooks()
        {
            if (_books.Count == 0)
            {
                _books.AddRange(new List<Book>
                {
                    new Book { Id = 1, Title = "Le Petit Prince", AuthorId = 1, Category = BookCategory.Fiction, Price = 12.99m, PublicationDate = new DateTime(1943, 4, 6), IsAvailable = true, Stock = 10, ImageUrl="https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=300&h=400&fit=crop", Description = "Un conte poétique et philosophique sous l'apparence d'un conte pour enfants." },
                    new Book { Id = 2, Title = "1984", AuthorId = 2, Category = BookCategory.Fiction, Price = 15.99m, PublicationDate = new DateTime(1949, 6, 8), IsAvailable = true, Stock = 8, ImageUrl="https://drive.google.com/file/d/1ESL-CkL3__RnoQ3Ewl_vD-AmnuGPEMFY/view?usp=sharing", Description = "Un roman dystopique qui dépeint une société totalitaire." },
                    new Book { Id = 3, Title = "L'Étranger", AuthorId = 3, Category = BookCategory.Fiction, Price = 14.50m, PublicationDate = new DateTime(1942, 5, 19), IsAvailable = true, Stock = 5, ImageUrl="https://drive.google.com/file/d/1ze1SszkodoIqJBJ-8DxFMZ2LMhR9oFaY/view?usp=sharing", Description = "L'histoire de Meursault, un homme indifférent au monde qui l'entoure." },
                    new Book { Id = 4, Title = "Une brève histoire du temps", AuthorId = 4, Category = BookCategory.Science, Price = 22.00m, PublicationDate = new DateTime(1988, 4, 1), IsAvailable = true, Stock = 12, ImageUrl="https://drive.google.com/file/d/1ZjlP7mGYBSI9YqVM1CDhaga5mkFiq86Q/view?usp=sharing", Description = "Les mystères de l'univers expliqués par Stephen Hawking." },
                    new Book { Id = 5, Title = "Sapiens", AuthorId = 5, Category = BookCategory.History, Price = 24.90m, PublicationDate = new DateTime(2011, 9, 4), IsAvailable = true, Stock = 15, ImageUrl="https://drive.google.com/file/d/1h9A-X3IFDxFGltqAtI0MNw8UKoe_W9uD/view?usp=sharing", Description = "Une brève histoire de l'humanité." },
                    new Book { Id = 6, Title = "Clean Code", AuthorId = 6, Category = BookCategory.Technology, Price = 45.00m, PublicationDate = new DateTime(2008, 8, 1), IsAvailable = true, Stock = 7, ImageUrl="https://drive.google.com/file/d/1YWm4SFcrGdKRsEkj-9oC5cZrqc9QApR1/view?usp=sharing", Description = "Manuel de développement agile pour créer un code propre." },
                    new Book { Id = 7, Title = "Dune", AuthorId = 7, Category = BookCategory.Fantasy, Price = 19.99m, PublicationDate = new DateTime(1965, 8, 1), IsAvailable = true, Stock = 9, ImageUrl="https://drive.google.com/file/d/14rz5N6fBgg86WJuSHBIBQwfMA4TbltkD/view?usp=sharing", Description = "Un chef-d'œuvre de science-fiction épique." },
                    new Book { Id = 8, Title = "Les Misérables", AuthorId = 8, Category = BookCategory.Fiction, Price = 29.99m, PublicationDate = new DateTime(1862, 3, 30), IsAvailable = true, Stock = 6, ImageUrl="https://drive.google.com/file/d/1qeITvHeNbskBgKDXiAhLS5D22hYDH-68/view?usp=sharing", Description = "Le roman social de Victor Hugo." },
                });
            }
        }
    }
}
