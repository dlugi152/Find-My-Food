using System.Collections.Generic;

namespace FindMyFood.Models
{
    public class Client
    {
        public Client() { }

        public Client(string name) {
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
        public ApplicationUser ApplicationUser { get; set; }
    }
}