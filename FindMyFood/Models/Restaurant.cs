using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Find_My_Food.Models;

namespace FindMyFood.Areas.Restaurant.Models
{
    public class Restaurant
    {
        public Restaurant(string name, string address, string longitude, string latitude) {
            //Id = 1;
            Name = name;
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        //[CheckLongitude]
        public string Longitude { get; set; }

        //[CheckLatitude]
        public string Latitude { get; set; }

        public ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
        public ICollection<Favorites> Favorites { get; set; } = new List<Favorites>();
        public ApplicationUser ApplicationUser { get; set; }
    }

    public class CheckLatitudeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value) {
            if (int.Parse(value.ToString()) < -90 || int.Parse(value.ToString()) > 90) {
                return false;
            }

            return base.IsValid(value);
        }
    }

    public class CheckLongitudeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value) {
            if (int.Parse(value.ToString()) < -180 || int.Parse(value.ToString()) > 180) {
                return false;
            }

            return base.IsValid(value);
        }
    }
}
