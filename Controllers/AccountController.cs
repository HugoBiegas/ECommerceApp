using Microsoft.AspNetCore.Mvc;
using ECommerceApp.Services;
using ECommerceApp.Models.ViewModels;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly ILibrarianRequestService _librarianRequestService;

        public AccountController(IAuthService authService, IUserService userService, ILibrarianRequestService librarianRequestService)
        {
            _authService = authService;
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

            var success = await _authService.RegisterUserAsync(model);
            if (success)
            {
                // Connecter automatiquement l'utilisateur après inscription
                await _authService.LoginAsync(model.Email, model.Password);
                TempData["SuccessMessage"] = "Compte créé avec succès ! Vous avez reçu 100€ de crédits pour commencer vos achats.";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Une erreur est survenue lors de la création du compte.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
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
                    model.Role = currentUser.Role;
                    model.Credits = currentUser.Credits;
                    model.CreatedDate = currentUser.CreatedDate;
                    return View("Profile", model);
                }
            }

            // Vérifier que le nom d'utilisateur n'est pas déjà utilisé par un autre utilisateur
            if (!currentUser.Username.Equals(model.Username, StringComparison.OrdinalIgnoreCase))
            {
                if (await _authService.IsUsernameExistsAsync(model.Username))
                {
                    ModelState.AddModelError("Username", "Ce nom d'utilisateur est déjà pris.");
                    model.Role = currentUser.Role;
                    model.Credits = currentUser.Credits;
                    model.CreatedDate = currentUser.CreatedDate;
                    return View("Profile", model);
                }
            }

            // Mettre à jour les informations
            currentUser.Username = model.Username;
            currentUser.Email = model.Email;
            currentUser.FirstName = model.FirstName;
            currentUser.LastName = model.LastName;

            var success = await _userService.UpdateUserAsync(currentUser);
            if (success)
            {
                // Mettre à jour la session avec le nouvel email
                HttpContext.Session.SetString("UserEmail", currentUser.Email);
                TempData["SuccessMessage"] = "Profil mis à jour avec succès.";
            }
            else
            {
                TempData["ErrorMessage"] = "Une erreur est survenue lors de la mise à jour.";
            }

            return RedirectToAction("Profile");
        }

        [HttpGet]
        public async Task<IActionResult> RequestLibrarian()
        {
            if (!_authService.IsLoggedIn())
            {
                return RedirectToAction("Login");
            }

            var user = await _authService.GetCurrentUserAsync();
            if (user == null || user.Role != UserRole.User)
            {
                TempData["ErrorMessage"] = "Vous ne pouvez pas faire cette demande.";
                return RedirectToAction("Profile");
            }

            if (await _librarianRequestService.HasPendingRequestAsync(user.Id))
            {
                TempData["InfoMessage"] = "Vous avez déjà une demande en cours de traitement.";
                return RedirectToAction("Profile");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestLibrarian(LibrarianRequestViewModel model)
        {
            if (!_authService.IsLoggedIn())
            {
                return RedirectToAction("Login");
            }

            var user = await _authService.GetCurrentUserAsync();
            if (user == null || user.Role != UserRole.User)
            {
                TempData["ErrorMessage"] = "Vous ne pouvez pas faire cette demande.";
                return RedirectToAction("Profile");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var requestId = await _librarianRequestService.CreateRequestAsync(user.Id, model.Reason);
            if (requestId > 0)
            {
                TempData["SuccessMessage"] = "Votre demande a été envoyée aux administrateurs. Vous recevrez une réponse prochainement.";
                return RedirectToAction("Profile");
            }

            TempData["ErrorMessage"] = "Une erreur est survenue lors de l'envoi de votre demande.";
            return View(model);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}