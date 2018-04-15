using FindMyFood.Areas.Restaurant.Models;
using Microsoft.AspNetCore.Identity;

namespace Find_My_Food.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<int>
    {
        public int? ClientId { get; set; }
        public int? RestaurantId { get; set; }
        public virtual Client Client { get; set; }
        public virtual Restaurant Restaurant { get; set; }
    }
}