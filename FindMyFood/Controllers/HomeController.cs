using System.Diagnostics;
using System.Threading.Tasks;
using FindMyFood.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FindMyFood.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(UserManager<ApplicationUser> userManager) {
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index() {
            var user = await _userManager.GetUserAsync(User);
            if (user?.RestaurantId != null)
                return RedirectToAction("Index", "Restaurant");
            return View();
        }

        [AllowAnonymous]
        public IActionResult Error() {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}