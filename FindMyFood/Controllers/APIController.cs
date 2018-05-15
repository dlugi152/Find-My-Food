using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FindMyFood.Areas.Restaurant.Models;
using Find_My_Food.Data;
using Microsoft.AspNetCore.Razor.Language;

namespace FindMyFood.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class APIController : Controller
    {
        private readonly ApplicationDbContext _context;

        public APIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/API
        [HttpGet("Promotion")]
        public IEnumerable<Promotion> GetPromotions()
        {
            return _context.Promotions;
        }

        // GET: api/API/5
        [HttpGet("Promotion/{id}")]
        public async Task<IActionResult> GetPromotionById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var promotion = await _context.Promotions.SingleOrDefaultAsync(m => m.Id == id);

            if (promotion == null)
            {
                return NotFound();
            }

            return Ok(promotion);
        }

        private bool IsInRadius(double lng1, double lat1, double lng2, double lat2, double radius)
        {
            double p = Math.PI / 180;
            double a = 0.5 - Math.Cos((lat2 - lat1) * p) / 2 + Math.Cos(lat1 * p) *
                       Math.Cos(lat2 * p) * (1 - Math.Cos((lng2 - lng1) * p)) / 2;

            return 2 * 6371 * Math.Asin(Math.Sqrt(a)) <= radius;
        }

        // GET: api/API/5
        [HttpGet("Promotion/{lng}&{lat}&{radius}")]
        public IEnumerable<PromotionAPI> GetPromotionByLocation([FromRoute] double lng, [FromRoute] double lat, [FromRoute] double radius)
        {/*
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }*/
            //double d = double.Parse(_context.Restaurant.Single(m2 => m2.Id == 1).Longitude.Replace('.',','));
            //todo uwzględnić daty
            List <Promotion> promotion = _context.Promotions.Where(m =>
                IsInRadius(double.Parse(_context.Restaurant.Single(m2 => m2.Id == 1).Longitude.Replace('.', ',')),
                    double.Parse(_context.Restaurant.Single(m2 => m2.Id == 1).Latitude.Replace('.', ',')),
                    lng, lat, radius)).ToList();
            List<PromotionAPI> promoApi = new List<PromotionAPI>();
            foreach (var promotion1 in promotion)
            {
                Restaurant rest =
                    _context.Restaurant.SingleOrDefault(restaurant => restaurant.Id == promotion1.RestaurantId);
                promoApi.Add(new PromotionAPI(promotion1, rest));
            }
            /*
        if (promotion == null)
        {
            return NotFound();
        }*/

            return promoApi;
        }

        // GET: api/API/5
        [HttpGet("Restaurant/{id}")]
        public async Task<IActionResult> GetRestaurantById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var promotion = await _context.Restaurant.SingleOrDefaultAsync(m => m.Id == id);

            if (promotion == null)
            {
                return NotFound();
            }

            return Ok(promotion);
        }

        // PUT: api/API/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPromotion([FromRoute] int id, [FromBody] Promotion promotion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != promotion.Id)
            {
                return BadRequest();
            }

            _context.Entry(promotion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PromotionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/API
        [HttpPost]
        public async Task<IActionResult> PostPromotion([FromBody] Promotion promotion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPromotionById", new { id = promotion.Id }, promotion);
        }

        // DELETE: api/API/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromotion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var promotion = await _context.Promotions.SingleOrDefaultAsync(m => m.Id == id);
            if (promotion == null)
            {
                return NotFound();
            }

            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();

            return Ok(promotion);
        }

        private bool PromotionExists(int id)
        {
            return _context.Promotions.Any(e => e.Id == id);
        }
    }

    public class PromotionAPI
    {
        public string restaurantName;
        public string longitude;
        public string latitude;
        public string description;
        public string rating;
        public string address;
        public string tags;
        public PromotionAPI(Promotion promo,Restaurant rest)
        {
            restaurantName = rest.Name;
            longitude = rest.Longitude;
            latitude = rest.Latitude;
            description = promo.Description;
            rating = "1";
            tags = promo.Tags;
            address = rest.Address;
        }
    }
}