using System.Collections.Generic;

namespace FindMyFood.Models
{
    public class Restaurant
    {
        public Restaurant() { }

        public Restaurant(string name, string address, string longitude, string latitude) {
            Name = name;
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ApplicationUser ApplicationUser { get; set; }
    }
}