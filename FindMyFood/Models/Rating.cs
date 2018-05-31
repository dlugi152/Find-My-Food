namespace FindMyFood.Models
{
    public class Rating
    {
        public Rating() { }

        public int Id { get; set; }
        public int Rate { get; set; }
        public int ClientId { get; set; }
        public int RestaurantId { get; set; }
        public Client Client { get; set; }
        public Restaurant Restaurant { get; set; }
    }
}