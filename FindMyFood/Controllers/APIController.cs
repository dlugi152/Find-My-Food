using System;
using System.Collections.Generic;
using System.Globalization;
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

            //todo uwzględnić daty
            var promotion = await (from promo in _context.Promotions
                join r in _context.Restaurant on promo.RestaurantId equals r.Id into joined
                from r in joined
                where IsInRadius(r.Longitude, r.Latitude, lng, lat, radius)
                select new GetPromotionResponse(promo, r)).ToListAsync();
            foreach (var response in promotion)
                try {
                    response.Rating = _context.Ratings.Where(rating => rating.RestaurantId == response.RestaurantId)
                        .Average(rating => rating.Rate);
                }
                catch (Exception) {
                    response.Rating = 0;
                }

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
        [HttpGet("SignIn/{email}&{password}")]
        public async Task<IActionResult> Login([FromRoute] string email, [FromRoute] string password) {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var loginViewModel = new LoginViewModel
            {
                Email = email,
                Password = password,
                RememberMe = true
            };
            await _signInManager.SignOutAsync();
            try {
                await AccountController.Login(loginViewModel, _signInManager);
                var user = await _context.Users.SingleOrDefaultAsync(applicationUser => applicationUser.Email == email);
                var client = await _context.Client.SingleOrDefaultAsync(m => m.Id == user.ClientId);
                return Ok(new StandardStatusResponse(true, client.Name));
            }
            catch (Exception ex) {
                return Ok(new StandardStatusResponse(false, ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpGet("ChangeUserPassword/{email}&{currentPassword}&{newPassword}")]
        public async Task<IActionResult> ChangeUserPassword([FromRoute] string email,
            [FromRoute] string currentPassword,
            [FromRoute] string newPassword) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try {
                ApplicationUser appUser = _context.Users.SingleOrDefault(user => user.Email == email);
                //var currentUser = await _userManager.GetUserAsync(User);
                var result = await _userManager.ChangePasswordAsync(appUser, currentPassword, newPassword);
                return Ok(result.Succeeded
                    ? new StandardStatusResponse(true, "Zmieniono")
                    : new StandardStatusResponse(false, "Hasła nie pasują"));
            }
            catch (Exception) {
                return Ok(new StandardStatusResponse(false, "Taki email nie istnieje"));
            }
        }

        [AllowAnonymous]
        [HttpGet("Rate/{email}&{password}&{restaurantId}&{rate}")]
        public async Task<IActionResult> Rate([FromRoute] string email, [FromRoute] string password,
            [FromRoute] int restaurantId, [FromRoute] int rate) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try {
                ApplicationUser appUser = _context.Users.SingleOrDefault(user => user.Email == email);
                if (!await _userManager.CheckPasswordAsync(appUser, password))
                    return Ok(new StandardStatusResponse(false, "Nieprawidłowe dane logowania"));
                Rating rating = new Rating();
                var id = _context.Client.SingleOrDefault(client => client.Id == appUser.ClientId)?.Id;
                if (id == null)
                    return Ok(new StandardStatusResponse(false,
                        "Stontaktuj się z administratorem, nieznany błąd z Twoim kontem"));
                rating.ClientId = (int) id;
                rating.Rate = rate;
                rating.RestaurantId = restaurantId;
                await _context.Ratings.AddAsync(rating);
                await _context.SaveChangesAsync();
                return Ok(new StandardStatusResponse(true, "Oceniono"));
            }
            catch (Exception) {
                return Ok(new StandardStatusResponse(false, "Taki email nie istnieje"));
            }
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

            var promotions =
                await (from promo in (_context.Promotions.Where(promotion => promotion.RestaurantId == restaurant.Id))
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
        [HttpGet("GetExtendedRestaurant")]
        public async Task<IActionResult> GetExtendedRestaurant() {
            Restaurant restaurant;
            ApplicationUser user;
            try {
                user = await _userManager.GetUserAsync(User);
                restaurant = await _context.Restaurant.SingleOrDefaultAsync(m => m.Id == user.RestaurantId);
            }
            catch (Exception) {
                return Ok(new StandardStatusResponse(false, "problem z Twoim kontem"));
            }

            var response = new ExtendedRestaurantResponse
            {
                Name = restaurant.Name,
                Address = restaurant.Address,
                Ceofirstname = restaurant.Ceofirstname,
                City = restaurant.City,
                Ceolastname = restaurant.Ceolastname,
                Country = restaurant.Country,
                Latitude = restaurant.Latitude,
                LongDescription = restaurant.LongDescription,
                Longitude = restaurant.Longitude,
                Motto = restaurant.Motto,
                PostalCode = restaurant.PostalCode,
                Website = restaurant.Website,
                Nopromotions = _context.Promotions.Count(promotion => promotion.RestaurantId == restaurant.Id),
                Norates = _context.Ratings.Count(rating => rating.RestaurantId == restaurant.Id),
                Rating = _context.Ratings.Any(rating => rating.RestaurantId == restaurant.Id)
                    ? _context.Ratings.Average(rating => rating.Rate)
                    : -1,
                LastRates = (from rate in _context.Ratings.Where(rating => rating.RestaurantId == restaurant.Id)
                    orderby rate.Id descending
                    select new SingleRate(rate.Client.Name, rate.Rate)).Take(3),
                Email = user.Email,
                County = restaurant.County,
                Province = restaurant.Province,
                Street = restaurant.Street,
                StreetNumber = restaurant.StreetNumber
            };
            return Ok(response);
        }

        [HttpPost]
        [Route("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] ExtendedRestaurantInfo info) {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            Restaurant restaurant;
            try {
                var user = await _userManager.GetUserAsync(User);
                restaurant = await _context.Restaurant.SingleOrDefaultAsync(m => m.Id == user.RestaurantId);
            }
            catch (Exception) {
                return Ok(new StandardStatusResponse(false, "Problem z Twoim kontem"));
            }

            restaurant.County = info.County;
            restaurant.PostalCode = info.PostalCode;
            restaurant.Province = info.Province;
            restaurant.Street = info.Street;
            restaurant.StreetNumber = info.StreetNumber;
            restaurant.Website = info.Website;
            restaurant.Address = info.Address;
            restaurant.Name = info.Name;
            restaurant.Ceofirstname = info.Ceofirstname;
            restaurant.Ceolastname = info.Ceolastname;
            restaurant.City = info.City;
            restaurant.Country = info.Country;
            restaurant.LongDescription = info.LongDescription;
            restaurant.Motto = info.Motto;
            _context.Update(restaurant);
            _context.SaveChanges();
            return Ok(new StandardStatusResponse(true, "zaktualizowano"));
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
            newPromotion.RepetitionMode = promo.RepetitionMode;
            try {
                string[] strings;
                switch (newPromotion.RepetitionMode) {
                    case Enums.PeriodEnum.NoLimit:
                        newPromotion.DateStart = null;
                        newPromotion.DateEnd = null;
                        break;
                    case Enums.PeriodEnum.Once:
                        if (string.IsNullOrEmpty(promo.StartTime) ||
                            string.IsNullOrEmpty(promo.DateRange) || string.IsNullOrEmpty(promo.EndTime))
                            return Ok(new StandardStatusResponse(false, "Nie podano daty lub godzin"));
                        var split = promo.DateRange.Split(' ');
                        newPromotion.DateStart = DateTime.Parse(split[0] + " " + promo.StartTime);
                        newPromotion.DateEnd = DateTime.Parse(split[3] + " " + promo.EndTime);
                        if (DateTime.Compare(newPromotion.DateStart.Value, newPromotion.DateEnd.Value) >= 0)
                            return Ok(new StandardStatusResponse(false,
                                "Promocja kończy się wcześniej niż się rozpoczyna lub trwa za krótko"));
                        break;
                    case Enums.PeriodEnum.Daily:
                        strings = promo.StartTime.Split(':');
                        newPromotion.DateStart = new DateTime(2000, 1, 1, int.Parse(strings[0]), int.Parse(strings[1]), 0);
                        strings = promo.EndTime.Split(':');
                        newPromotion.DateEnd = new DateTime(2000, 1, 1, int.Parse(strings[0]), int.Parse(strings[1]), 0);
                        if (DateTime.Compare(newPromotion.DateStart.Value, newPromotion.DateEnd.Value) >= 0)
                            return Ok(new StandardStatusResponse(false,
                                "Godzina rozpoczęcia jest wcześniejsza niż zakończenia"));
                        break;
                    case Enums.PeriodEnum.Weekly:
                        strings = promo.StartTime.Split(':');
                        newPromotion.DateStart = new DateTime(2000, 1, 1, int.Parse(strings[0]), int.Parse(strings[1]), 0);
                        strings = promo.EndTime.Split(':');
                        newPromotion.DateEnd = new DateTime(2000, 1, 1, int.Parse(strings[0]), int.Parse(strings[1]), 0);
                        if (DateTime.Compare(newPromotion.DateStart.Value, newPromotion.DateEnd.Value) >= 0)
                            return Ok(new StandardStatusResponse(false,
                                "Godzina rozpoczęcia jest wcześniejsza niż zakończenia"));
                        if (promo.DaysInWeek.Count != 1)
                            return Ok(new StandardStatusResponse(false, "Nieprawidłowy dzień powtarzania"));
                        newPromotion.AddDaysOfWeek(promo.DaysInWeek);
                        break;
                    case Enums.PeriodEnum.SingleDays:
                        newPromotion.AddDaysOfWeek(promo.DaysInWeek);
                        newPromotion.DateStart = newPromotion.DateEnd = null;
                        newPromotion.AddDaysOfWeek(promo.DaysInWeek);
                        break;
                    default:
                        return Ok(new StandardStatusResponse(false, "Wybrano zły sposób powtarzania"));
                }
            }
            catch (Exception ex) {
                return Ok(new StandardStatusResponse(false,
                    ex.Message));
            }

            _context.Promotions.Add(newPromotion);
            try {
                await _context.SaveChangesAsync();
                return Ok(new StandardStatusResponse(true, "Wszystko ok"));
            }
            catch (Exception ex) {
                return Ok(new StandardStatusResponse(false, ex.InnerException.Message));
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
        public double Rating;
        public string RestaurantName;
        public int RestaurantId;
        public string Tags;

        public GetPromotionResponse(Promotion promo, Restaurant rest) {
            RestaurantName = rest.Name;
            Longitude = rest.Longitude;
            Latitude = rest.Latitude;
            RestaurantId = rest.Id;
            Description = promo.Description;
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
        public Enums.PeriodEnum RepetitionMode { get; set; }
        public List<DayOfWeek> DaysInWeek { get; set; }
    }

    public class ExtendedRestaurantResponse : ExtendedRestaurantInfo
    {
        public int Nopromotions { get; set; }
        public double Rating { get; set; }
        public int Norates { get; set; }
        public IEnumerable<SingleRate> LastRates { get; set; }
    }

    public class ExtendedRestaurantInfo
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }
        public string Ceofirstname { get; set; }
        public string Ceolastname { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string LongDescription { get; set; }
        public string Motto { get; set; }
        public string Website { get; set; }
        public string County { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string Province { get; set; }
    }

    public class SingleRate
    {
        public SingleRate(string login, int rate) {
            Login = login;
            Rate = rate;
        }

        public string Login { get; set; }
        public int Rate { get; set; }
    }
}