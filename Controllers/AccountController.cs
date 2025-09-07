using Microsoft.AspNetCore.Mvc;
using ECommerceApp.Services;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ILibrarianRequestService _librarianRequestService;

        public AccountController(
            IAuthService authService,
            IUserService userService,
            ILibrarianRequestService librarianRequestService,
            ICartService cartService)
            : base(authService, cartService)
        {
            _userService = userService;
            _librarianRequestService = librarianRequestService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (_authService.IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await _authService.LoginAsync(model.Email, model.Password);
            if (success)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (_authService.IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Vérifier si l'email existe déjà
            if (await _authService.IsEmailExistsAsync(model.Email))
            {
                ModelState.AddModelError("Email", "Cette adresse email est déjà utilisée.");
                return View(model);
            }

            // Vérifier si le nom d'utilisateur existe déjà
            if (await _authService.IsUsernameExistsAsync(model.Username))
            {
                ModelState.AddModelError("Username", "Ce nom d'utilisateur est déjà pris.");
                return View(model);
            }

            // Créer le nouvel utilisateur
            var success = await _authService.RegisterUserAsync(model);
            if (success)
            {
                // Connexion automatique après l'inscription
                await _authService.LoginAsync(model.Email, model.Password);
                TempData["SuccessMessage"] = "Compte créé avec succès ! Vous avez reçu 100€ de crédits pour commencer vos achats.";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Une erreur est survenue lors de la création du compte.");
            return View(model);
        }

        // GET: Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            TempData["InfoMessage"] = "Vous avez été déconnecté avec succès.";
            return RedirectToAction("Index", "Home");
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutPost()
        {
            await _authService.LogoutAsync();
            TempData["InfoMessage"] = "Vous avez été déconnecté avec succès.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!_authService.IsLoggedIn())
            {
                return RedirectToAction("Login");
            }

            var user = await _authService.GetCurrentUserAsync();
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new ProfileViewModel
            {
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Credits = user.Credits,
                CreatedDate = user.CreatedDate
            };

            // Vérifier s'il y a une demande de libraire en attente
            ViewBag.HasPendingLibrarianRequest = await _librarianRequestService.HasPendingRequestAsync(user.Id);
            ViewBag.CanRequestLibrarian = user.Role == UserRole.User && !ViewBag.HasPendingLibrarianRequest;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            if (!_authService.IsLoggedIn())
            {
                return RedirectToAction("Login");
            }

            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                return RedirectToAction("Login");
            }

            // Vérifier que l'email n'est pas déjà utilisé par un autre utilisateur
            if (!currentUser.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await _authService.IsEmailExistsAsync(model.Email))
                {
                    ModelState.AddModelError("Email", "Cette adresse email est déjà utilisée.");
                    return View("Profile", model);
                }
            }

            // Vérifier que le nom d'utilisateur n'est pas déjà utilisé par un autre utilisateur
            if (!currentUser.Username.Equals(model.Username, StringComparison.OrdinalIgnoreCase))
            {
                if (await _authService.IsUsernameExistsAsync(model.Username))
                {
                    ModelState.AddModelError("Username", "Ce nom d'utilisateur est déjà pris.");
                    return View("Profile", model);
                }
            }

            // Mettre à jour le profil
            currentUser.Username = model.Username;
            currentUser.Email = model.Email;
            currentUser.FirstName = model.FirstName;
            currentUser.LastName = model.LastName;

            var success = await _userService.UpdateUserAsync(currentUser);
            if (success)
            {
                TempData["SuccessMessage"] = "Profil mis à jour avec succès.";
                return RedirectToAction("Profile");
            }

            ModelState.AddModelError(string.Empty, "Une erreur est survenue lors de la mise à jour du profil.");
            return View("Profile", model);
        }

        // GET: Account/RequestLibrarian
        [HttpGet]
        public async Task<IActionResult> RequestLibrarian()
        {
            if (!_authService.IsLoggedIn())
            {
                return RedirectToAction("Login");
            }

            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                return RedirectToAction("Login");
            }

            // Vérifier que l'utilisateur est bien un User (pas déjà Librarian ou Admin)
            if (currentUser.Role != UserRole.User)
            {
                TempData["ErrorMessage"] = "Seuls les utilisateurs standard peuvent demander à devenir libraire.";
                return RedirectToAction("Profile");
            }

            // Vérifier s'il n'y a pas déjà une demande en attente
            var hasPendingRequest = await _librarianRequestService.HasPendingRequestAsync(userId.Value);
            if (hasPendingRequest)
            {
                TempData["ErrorMessage"] = "Vous avez déjà une demande de libraire en attente.";
                return RedirectToAction("Profile");
            }

            return View(new LibrarianRequestViewModel());
        }
        
        // POST: Account/RequestLibrarian
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestLibrarian(string reason)
        {
            if (!_authService.IsLoggedIn())
            {
                return RedirectToAction("Login");
            }

            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var success = await _librarianRequestService.CreateRequestAsync(userId.Value, reason);

            if (success > 0)
            {
                TempData["SuccessMessage"] = "Votre demande pour devenir libraire a été soumise et est en attente d'approbation.";
            }
            else
            {
                TempData["ErrorMessage"] = "Une erreur est survenue ou vous avez déjà une demande en attente.";
            }

            return RedirectToAction("Profile");
        }

        public IActionResult AccessDenied()
        {
            TempData["ErrorMessage"] = "Vous n'avez pas les droits nécessaires pour accéder à cette page.";
            return View();
        }
    }
}