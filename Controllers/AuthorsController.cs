using Microsoft.AspNetCore.Mvc;
using ECommerceApp.Services;
using ECommerceApp.Models;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorService _authorService;
        private readonly IBookService _bookService;
        private readonly IAuthService _authService;

        public AuthorsController(IAuthorService authorService, IBookService bookService, IAuthService authService)
        {
            _authorService = authorService;
            _bookService = bookService;
            _authService = authService;
        }

        // GET: Authors
        public async Task<IActionResult> Index(string searchTerm = "")
        {
            List<Author> authors;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                authors = await _authorService.SearchAuthorsAsync(searchTerm);
                ViewBag.SearchTerm = searchTerm;
            }
            else
            {
                authors = await _authorService.GetAllAuthorsAsync();
            }

            ViewBag.CanManageAuthors = _authService.HasRole(UserRole.Librarian);

            return View(authors);
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            // Récupérer les livres de cet auteur
            var books = await _bookService.GetBooksByAuthorAsync(id);
            ViewBag.AuthorBooks = books;
            ViewBag.CanManageAuthors = _authService.HasRole(UserRole.Librarian);

            return View(author);
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            if (!_authService.HasRole(UserRole.Librarian))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View();
        }

        // POST: Authors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author author)
        {
            if (!_authService.HasRole(UserRole.Librarian))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(author);
            }

            var authorId = await _authorService.AddAuthorAsync(author);
            if (authorId > 0)
            {
                TempData["SuccessMessage"] = "L'auteur a été ajouté avec succès.";
                return RedirectToAction("Details", new { id = authorId });
            }

            TempData["ErrorMessage"] = "Une erreur est survenue lors de l'ajout de l'auteur.";
            return View(author);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!_authService.HasRole(UserRole.Librarian))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Authors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Author author)
        {
            if (!_authService.HasRole(UserRole.Librarian))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (id != author.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(author);
            }

            var success = await _authorService.UpdateAuthorAsync(author);
            if (success)
            {
                TempData["SuccessMessage"] = "L'auteur a été modifié avec succès.";
                return RedirectToAction("Details", new { id = author.Id });
            }

            TempData["ErrorMessage"] = "Une erreur est survenue lors de la modification.";
            return View(author);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (!_authService.HasRole(UserRole.Librarian))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            // Vérifier s'il y a des livres associés
            var books = await _bookService.GetBooksByAuthorAsync(id);
            ViewBag.AssociatedBooks = books;
            ViewBag.CanDelete = books.Count == 0;

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!_authService.HasRole(UserRole.Librarian))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Vérifier encore une fois qu'on peut supprimer
            if (!await _authorService.CanDeleteAuthorAsync(id))
            {
                TempData["ErrorMessage"] = "Impossible de supprimer cet auteur car il a des livres associés.";
                return RedirectToAction("Delete", new { id });
            }

            var success = await _authorService.DeleteAuthorAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "L'auteur a été supprimé avec succès.";
            }
            else
            {
                TempData["ErrorMessage"] = "Une erreur est survenue lors de la suppression.";
            }

            return RedirectToAction("Index");
        }
    }
}