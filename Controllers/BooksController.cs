using Microsoft.AspNetCore.Mvc;
using ECommerceApp.Services;
using ECommerceApp.Models;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Controllers
{
    public class BooksController : BaseController
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;

        public BooksController(
            IBookService bookService,
            IAuthorService authorService,
            IAuthService authService,
            ICartService cartService)
            : base(authService, cartService)
        {
            _bookService = bookService;
            _authorService = authorService;
        }

        // GET: Books
        public async Task<IActionResult> Index(BookListViewModel model)
        {
            model.Books = await _bookService.SearchBooksAsync(model);
            model.TotalCount = model.Books.Count;

            // Pagination
            var startIndex = (model.Page - 1) * model.PageSize;
            model.Books = model.Books.Skip(startIndex).Take(model.PageSize).ToList();

            // Préparer les données pour les filtres
            ViewBag.Categories = Enum.GetValues<BookCategory>().Select(c => new { Value = (int)c, Text = c.ToString() });

            // Additional ViewBag for Books specific
            ViewBag.CanManageBooks = _authService.HasRole(UserRole.Librarian);

            return View(model);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            ViewBag.CanManageBooks = _authService.HasRole(UserRole.Librarian);

            return View(book);
        }

        // GET: Books/Create
        public async Task<IActionResult> Create()
        {
            if (!_authService.HasRole(UserRole.Librarian))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var model = new BookCreateEditViewModel
            {
                Authors = await _authorService.GetAllAuthorsAsync()
            };

            ViewBag.Categories = Enum.GetValues<BookCategory>().Select(c => new { Value = (int)c, Text = c.ToString() });
            return View(model);
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookCreateEditViewModel model)
        {
            if (!_authService.HasRole(UserRole.Librarian))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (!ModelState.IsValid)
            {
                model.Authors = await _authorService.GetAllAuthorsAsync();
                ViewBag.Categories = Enum.GetValues<BookCategory>().Select(c => new { Value = (int)c, Text = c.ToString() });
                return View(model);
            }

            var book = new Book
            {
                Title = model.Title,
                ISBN = model.ISBN,
                Category = model.Category,
                Price = model.Price,
                Stock = model.Stock,
                Description = model.Description,
                AuthorId = model.AuthorId,
                ImageUrl = model.ImageUrl ?? "/images/default-book.jpg"
            };

            var bookId = await _bookService.AddBookAsync(book);
            if (bookId > 0)
            {
                TempData["SuccessMessage"] = "Le livre a été ajouté avec succès.";
                return RedirectToAction("Details", new { id = bookId });
            }

            TempData["ErrorMessage"] = "Une erreur est survenue lors de l'ajout du livre.";
            model.Authors = await _authorService.GetAllAuthorsAsync();
            ViewBag.Categories = Enum.GetValues<BookCategory>().Select(c => new { Value = (int)c, Text = c.ToString() });
            return View(model);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!_authService.HasRole(UserRole.Librarian))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            var model = new BookCreateEditViewModel
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                Category = book.Category,
                Price = book.Price,
                Stock = book.Stock,
                Description = book.Description,
                AuthorId = book.AuthorId,
                ImageUrl = book.ImageUrl,
                Authors = await _authorService.GetAllAuthorsAsync()
            };

            ViewBag.Categories = Enum.GetValues<BookCategory>().Select(c => new { Value = (int)c, Text = c.ToString() });
            return View(model);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookCreateEditViewModel model)
        {
            if (!_authService.HasRole(UserRole.Librarian))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (!ModelState.IsValid)
            {
                model.Authors = await _authorService.GetAllAuthorsAsync();
                ViewBag.Categories = Enum.GetValues<BookCategory>().Select(c => new { Value = (int)c, Text = c.ToString() });
                return View(model);
            }

            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            book.Title = model.Title;
            book.ISBN = model.ISBN;
            book.Category = model.Category;
            book.Price = model.Price;
            book.Stock = model.Stock;
            book.Description = model.Description;
            book.AuthorId = model.AuthorId;
            book.ImageUrl = model.ImageUrl ?? book.ImageUrl;

            var success = await _bookService.UpdateBookAsync(book);
            if (success)
            {
                TempData["SuccessMessage"] = "Le livre a été modifié avec succès.";
                return RedirectToAction("Details", new { id = book.Id });
            }

            TempData["ErrorMessage"] = "Une erreur est survenue lors de la modification.";
            model.Authors = await _authorService.GetAllAuthorsAsync();
            ViewBag.Categories = Enum.GetValues<BookCategory>().Select(c => new { Value = (int)c, Text = c.ToString() });
            return View(model);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (!_authService.HasRole(UserRole.Librarian))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!_authService.HasRole(UserRole.Librarian))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var success = await _bookService.DeleteBookAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Le livre a été supprimé avec succès.";
            }
            else
            {
                TempData["ErrorMessage"] = "Une erreur est survenue lors de la suppression.";
            }

            return RedirectToAction("Index");
        }

        // AJAX: Add to cart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int bookId, int quantity = 1)
        {
            if (!_authService.IsLoggedIn())
            {
                return Json(new { success = false, message = "Vous devez être connecté pour ajouter des articles au panier." });
            }

            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Session expirée." });
            }

            var success = await _cartService.AddToCartAsync(userId.Value, bookId, quantity);
            if (success)
            {
                var itemsCount = await _cartService.GetCartItemsCountAsync(userId.Value);
                return Json(new { success = true, message = "Article ajouté au panier.", cartCount = itemsCount });
            }

            return Json(new { success = false, message = "Impossible d'ajouter cet article au panier. Vérifiez la disponibilité." });
        }
    }
}