using System.Collections.Generic;

namespace FindMyFood.Models
{
    public class Restaurant
    {
        public Restaurant() { }

        public Restaurant(string name, string address, double longitude, double latitude, string city, string country,
            string postalCode, string county, string province, string street, string streetNumber) {
            Country = country;
            Province = province;
            Street = street;
            StreetNumber = streetNumber;
            Name = name;
            Address = address;
            Latitude = latitude;
            City = city;
            Country = country;
            PostalCode = postalCode;
            Longitude = longitude;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

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

        public ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ApplicationUser ApplicationUser { get; set; }
    }
}