using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FindMyFood.Models;
using FindMyFood.Models.ManageViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FindMyFood.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private readonly ILogger _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UrlEncoder _urlEncoder;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManageController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ManageController> logger,
            UrlEncoder urlEncoder) {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _urlEncoder = urlEncoder;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var model = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model) {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var email = user.Email;
            if (model.Email != email) {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                    throw new
                        ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber) {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                    throw new
                        ApplicationException(
                            $"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
            }

            StatusMessage = "Your profile has been updated";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword) return RedirectToAction(nameof(SetPassword));

            var model = new ChangePasswordViewModel {StatusMessage = StatusMessage};
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model) {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var changePasswordResult =
                await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded) {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, false);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToAction(nameof(ChangePassword));
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword) return RedirectToAction(nameof(ChangePassword));

            var model = new SetPasswordViewModel {StatusMessage = StatusMessage};
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model) {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded) {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, false);
            StatusMessage = "Your password has been set.";

            return RedirectToAction(nameof(SetPassword));
        }

        #region Helpers

        private void AddErrors(IdentityResult result) {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        }

        private string FormatKey(string unformattedKey) {
            var result = new StringBuilder();
            var currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length) {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }

            if (currentPosition < unformattedKey.Length) result.Append(unformattedKey.Substring(currentPosition));

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey) {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("Find_My_Food"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user, EnableAuthenticatorViewModel model) {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey)) {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            model.SharedKey = FormatKey(unformattedKey);
            model.AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
        }

        #endregion
    }
}