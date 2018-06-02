using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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

        private static bool IsInRadius(double lng1, double lat1, double lng2, double lat2, double radius) {
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

            var user = await _userManager.GetUserAsync(User);
            var client = user != null ? await _context.Client.SingleOrDefaultAsync(m => m.Id == user.ClientId) : null;
            //todo uwzględnić daty
            var promotion = await (from promo in _context.Promotions
                join r in _context.Restaurant on promo.RestaurantId equals r.Id into joined
                from r in joined
                where IsInRadius(r.Longitude, r.Latitude, lng, lat, radius)
                select new GetPromotionResponse(promo, r, client)).ToListAsync();
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

        [HttpPost]
        [HttpGet("MyPromotions")]
        public async Task<IActionResult> PromotionsOfSignedIn() {
            Restaurant restaurant;
            try {
                var user = await _userManager.GetUserAsync(User);
                restaurant = await _context.Restaurant.SingleOrDefaultAsync(m => m.Id == user.RestaurantId);
            }
            catch (Exception) {
                return Ok(new StandardStatusResponse(false, "Problem z Twoim kontem"));
            }
            var promotions = await (from promo in (_context.Promotions.Where(promotion => promotion.RestaurantId == restaurant.Id))
                select new GetSimplePromotionResponse(promo)).ToListAsync();
            return Ok(promotions);
        }

        [HttpPost]
        [HttpGet("DeletePromotion/{id}")]
        public async Task<IActionResult> DeletePromotionOfSigned([FromRoute] int id) {
            Restaurant restaurant;
            try {
                var user = await _userManager.GetUserAsync(User);
                restaurant = await _context.Restaurant.SingleOrDefaultAsync(m => m.Id == user.RestaurantId);
            }
            catch (Exception) {
                return Ok(new StandardStatusResponse(false, "Problem z Twoim kontem"));
            }

            var promotion = await _context.Promotions.SingleOrDefaultAsync(m => m.Id == id);
            if (restaurant.Id != promotion.RestaurantId)
                return Ok(new StandardStatusResponse(false, "to nie jest promocja zalogowanego użytkownika"));
            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
            return Ok(new StandardStatusResponse(true, "usunięto"));
        }

        [HttpPost]
        [Route("AddPromotion")]
        public async Task<IActionResult> AddPromotion([FromBody] AddPromotionRequest promo) {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            Restaurant restaurant;
            try {
                var user = await _userManager.GetUserAsync(User);
                restaurant = await _context.Restaurant.SingleOrDefaultAsync(m => m.Id == user.RestaurantId);
            }
            catch (Exception) {
                return Ok(new StandardStatusResponse(false, "Problem z Twoim kontem"));
            }

            var newPromotion = new Promotion
            {
                Restaurant = restaurant,
                RestaurantId = restaurant.Id,
                Description = promo.Description
            };
            string[] ses;
            try {
                ses = promo.Tags.Split(',');
                for (var i = 0; i < ses.Length; i++)
                    ses[i] = ses[i].Trim();
                if (ses.Any(s => s == ""))
                    throw new Exception();
            }
            catch (Exception) {
                return Ok(new StandardStatusResponse(false, "Nieprawidlowe wartości w tagach"));
            }

            newPromotion.Tags = "";
            foreach (var s in ses)
                newPromotion.Tags += s + ",";
            newPromotion.Tags = newPromotion.Tags.Remove(newPromotion.Tags.Length - 1);

            if (!(string.IsNullOrEmpty(promo.StartTime + promo.DateRange + promo.EndTime) ||
                  (!string.IsNullOrEmpty(promo.StartTime) &&
                   !string.IsNullOrEmpty(promo.DateRange) && !string.IsNullOrEmpty(promo.EndTime))))
                return Ok(new StandardStatusResponse(false,
                    "Musisz podać jednocześnie zakres dat i godziny lub jednocześnie żadne z nich"));
            try {
                var split = promo.DateRange.Split(' ');
                newPromotion.DateStart = DateTime.Parse(split[0] + " " + promo.StartTime);
                newPromotion.DateEnd = DateTime.Parse(split[3] + " " + promo.EndTime);
                if (DateTime.Compare(newPromotion.DateStart.Value, newPromotion.DateEnd.Value) >= 0)
                    return Ok(new StandardStatusResponse(false,
                        "Promocja kończy się wcześniej niż się rozpoczyna lub trwa za krótko"));
            }
            catch (Exception) {
                newPromotion.DateEnd = newPromotion.DateStart = null;
            }

            _context.Promotions.Add(newPromotion);
            try {
                await _context.SaveChangesAsync();
                return Ok(new StandardStatusResponse(true, "Wszystko ok"));
            }
            catch (Exception) {
                return Ok(newPromotion);
            }
        }
    }

    public class GetSimplePromotionResponse
    {
        public string Description;
        public string Tags;
        public string DateStart;
        public string DateEnd;
        public int Id;

        public GetSimplePromotionResponse(Promotion promo) {
            Id = promo.Id;
            Description = promo.Description;
            Tags = promo.Tags;
            DateStart = promo.DateStart.HasValue ? promo.DateStart.Value.ToString(CultureInfo.CurrentCulture) : "---";
            DateEnd = promo.DateEnd.HasValue ? promo.DateEnd.Value.ToString(CultureInfo.CurrentCulture) : "---";
        }
    }

    public class GetPromotionResponse
    {
        public string Address;
        public string Description;
        public double Latitude;
        public double Longitude;
        public int Rating;
        public string RestaurantName;
        public string Tags;

        public GetPromotionResponse(Promotion promo, Restaurant rest, Client client) {
            RestaurantName = rest.Name;
            Longitude = rest.Longitude;
            Latitude = rest.Latitude;
            Description = promo.Description;
            if (client == null)
                Rating = -1;
            else {
                Rating rating = ApplicationDbContext.instance.Ratings.SingleOrDefault(r =>
                    r.ClientId == client.Id && r.RestaurantId == rest.Id);
                if (rating != null) Rating = rating.Rate;
                else
                    Rating = -1;
            }

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

    public class AddPromotionRequest
    {
        public string DateRange { get; set; }
        public string Description { get; set; }
        public string EndTime { get; set; }
        public string StartTime { get; set; }
        public string Tags { get; set; }
    }
}