using System;

namespace FindMyFood.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }
    }
}