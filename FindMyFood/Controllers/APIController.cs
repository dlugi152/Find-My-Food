using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FindMyFood.Data;
using FindMyFood.Models;
using FindMyFood.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FindMyFood.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApiController(ApplicationDbContext context,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager) {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        // GET: api/API
        [HttpGet("Promotion")]
        public IEnumerable<Promotion> GetPromotions() {
            return _context.Promotions;
        }

        // GET: api/API/5

        [AllowAnonymous]
        [HttpGet("Promotion/{id}")]
        public async Task<IActionResult> GetPromotionById([FromRoute] int id) {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var promotion = await _context.Promotions.SingleOrDefaultAsync(m => m.Id == id);

            if (promotion == null) return NotFound();

            return Ok(promotion);
        }

        private bool IsInRadius(double lng1, double lat1, double lng2, double lat2, double radius) {
            var p = Math.PI / 180;
            var a = 0.5 - Math.Cos((lat2 - lat1) * p) / 2 + Math.Cos(lat1 * p) *
                    Math.Cos(lat2 * p) * (1 - Math.Cos((lng2 - lng1) * p)) / 2;

            return 2 * 6371 * Math.Asin(Math.Sqrt(a)) <= radius;
        }

        // GET: api/API/5
        [AllowAnonymous]
        [HttpGet("Promotion/{lng}&{lat}&{radius}")]
        public async Task<IActionResult> GetPromotionByLocation([FromRoute] double lng, [FromRoute] double lat,
            [FromRoute] double radius) {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            //todo uwzględnić daty
            double n = 5;
            var promotion = await (from promo in _context.Promotions
                join r in _context.Restaurant on promo.RestaurantId equals r.Id into joined
                from r in joined
                where IsInRadius(
                    double.TryParse(r.Longitude, out n)
                        ? double.Parse(r.Longitude)
                        : double.Parse(r.Longitude.Replace('.', ',')),
                    double.TryParse(r.Latitude, out n)
                        ? double.Parse(r.Latitude)
                        : double.Parse(r.Latitude.Replace('.', ',')),
                    lng, lat, radius)
                select new GetPromotionResponse(promo, r)).ToListAsync();
            return Ok(promotion);
        }

        // GET: api/API/5
        [AllowAnonymous]
        [HttpGet("Restaurant/{id}")]
        public async Task<IActionResult> GetRestaurantById([FromRoute] int id) {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var promotion = await _context.Restaurant.SingleOrDefaultAsync(m => m.Id == id);

            if (promotion == null) return NotFound();

            return Ok(promotion);
        }

        // GET: api/API/5
        [AllowAnonymous]
        [HttpGet("Registration/{login}&{email}&{password}")]
        public async Task<IActionResult> Register([FromRoute] string login, [FromRoute] string email,
            [FromRoute] string password) {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var registerViewModel = new RegisterViewModel
            {
                Role = Enums.RolesEnum.Client,
                Email = email,
                ClientName = login,
                Password = password
            };

            await _signInManager.SignOutAsync();
            try {
                await AccountController.AddNewUser(registerViewModel, _userManager, _signInManager);
                return Ok(new StandardStatusResponse(true, "woohoo, everything's fine"));
            }
            catch (Exception ex) {
                return Ok(new StandardStatusResponse(false, ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpGet("SignIn/{email}&{password}&{rememberMe}")]
        public async Task<IActionResult> Login([FromRoute] string email, [FromRoute] string password,
            [FromRoute] bool rememberMe) {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var loginViewModel = new LoginViewModel
            {
                Email = email,
                Password = password,
                RememberMe = rememberMe
            };
            await _signInManager.SignOutAsync();
            try {
                await AccountController.Login(loginViewModel, _signInManager);
                var user = await _userManager.GetUserAsync(User);
                var client = await _context.Client.SingleOrDefaultAsync(m => m.Id == user.ClientId);
                return Ok(new StandardStatusResponse(true, client.Name));
            }
            catch (Exception ex) {
                return Ok(new StandardStatusResponse(false, ex.Message));
            }
        }

        [HttpGet("ChangeUserPassword/{currentPassword}&{newPassword}")]
        public async Task<IActionResult> ChangeUserPassword([FromRoute] string currentPassword,
            [FromRoute] string newPassword) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUser = await _userManager.GetUserAsync(User);
            var result = await _userManager.ChangePasswordAsync(currentUser, currentPassword, newPassword);
            return Ok(result.Succeeded
                ? new StandardStatusResponse(true, "zmieniono")
                : new StandardStatusResponse(false, "hasła nie pasują"));
        }

        [HttpPost]
        [HttpGet("SignOut")]
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            return Ok("ok");
        }
    }

    public class GetPromotionResponse
    {
        public string Address;
        public string Description;
        public string Latitude;
        public string Longitude;
        public string Rating;
        public string RestaurantName;
        public string Tags;

        public GetPromotionResponse(Promotion promo, Restaurant rest) {
            RestaurantName = rest.Name;
            Longitude = rest.Longitude;
            Latitude = rest.Latitude;
            Description = promo.Description;
            Rating = "1";
            Tags = promo.Tags;
            Address = rest.Address;
        }
    }

    public class StandardStatusResponse
    {
        public string Message;
        public bool Response;

        public StandardStatusResponse(bool response, string message) {
            Response = response;
            Message = message;
        }
    }
}