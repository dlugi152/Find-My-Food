using System;
using System.Threading.Tasks;
using FindMyFood.Data;
using FindMyFood.Models;
using FindMyFood.Models.AccountViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FindMyFood.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger) {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null) {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        public static async Task Login(LoginViewModel model, SignInManager<ApplicationUser> signInManager) {
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (!result.Succeeded)
                throw new Exception("Nieudana próba logowania");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null) {
            ViewData["ReturnUrl"] = returnUrl;
            try {
                if (ModelState.IsValid)
                    await Login(model, _signInManager);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception) {
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null) {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        public static async Task AddNewUser(RegisterViewModel model,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) {
            var user = new ApplicationUser {UserName = model.Email, Email = model.Email};
            var context = ApplicationDbContext.instance;
            switch (model.Role) {
                case Enums.RolesEnum.Restaurant:
                    user.Restaurant = new Restaurant(model.RestaurantName, model.RealAddress,
                        model.Longitude,
                        model.Latitude);
                    context.Restaurant.Add(user.Restaurant);
                    break;
                case Enums.RolesEnum.Client:
                    user.Client = new Client(model.ClientName);
                    context.Client.Add(user.Client);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) throw new Exception(result.Errors.ToString());
            context.SaveChanges();
            await signInManager.SignInAsync(user, false);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null) {
            ViewData["ReturnUrl"] = returnUrl;
            try {
                if (ModelState.IsValid) {
                    await AddNewUser(model, _userManager, _signInManager);
                    return RedirectToLocal(returnUrl);
                }
            }
            catch (Exception) {
                // ignored
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword() {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null) {
            if (code == null) throw new ApplicationException("A code must be supplied for password reset.");

            var model = new ResetPasswordViewModel {Code = code};
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model) {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return RedirectToAction(nameof(ResetPasswordConfirmation));

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded) return RedirectToAction(nameof(ResetPasswordConfirmation));

            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation() {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied() {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result) {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        }

        private IActionResult RedirectToLocal(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        #endregion
    }
}