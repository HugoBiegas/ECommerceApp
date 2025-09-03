using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ECommerceApp.Services;

namespace ECommerceApp.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly IAuthService _authService;
        protected readonly ICartService _cartService;

        public BaseController(IAuthService authService, ICartService cartService)
        {
            _authService = authService;
            _cartService = cartService;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Set common ViewBag properties for all actions
            await SetCommonViewBagAsync();

            await base.OnActionExecutionAsync(context, next);
        }

        protected virtual async Task SetCommonViewBagAsync()
        {
            ViewBag.IsLoggedIn = _authService.IsLoggedIn();

            if (_authService.IsLoggedIn())
            {
                var user = await _authService.GetCurrentUserAsync();
                if (user != null)
                {
                    ViewBag.UserEmail = user.Email;
                    ViewBag.UserRole = user.Role;
                    ViewBag.UserCredits = user.Credits;

                    // Get cart count
                    ViewBag.CartItemsCount = await _cartService.GetCartItemsCountAsync(user.Id);
                }
            }
        }
    }
}