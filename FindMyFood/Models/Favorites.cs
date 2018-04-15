namespace FindMyFood.Areas.Restaurant.Models
{
    public class Favorites
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int RestaurantId { get; set; }
        public Client Client { get; set; }
        public Restaurant Restaurant { get; set; }
    }
}