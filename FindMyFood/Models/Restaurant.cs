using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Find_My_Food.Models;

namespace FindMyFood.Areas.Restaurant.Models
{
    public class Restaurant
    {
        public Restaurant(string Name, string Address, double Longitude, double Latitude) {
            //Id = 1;
            this.Name = Name;
            this.Address = Address;
            this.Latitude = Latitude;
            this.Longitude = Longitude;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        [CheckLongitude]
        public double Longitude { get; set; }

        [CheckLatitude]
        public double Latitude { get; set; }

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