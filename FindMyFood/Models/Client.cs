using System.Collections.Generic;
using Find_My_Food.Models;

namespace FindMyFood.Areas.Restaurant.Models
{
    public class Client
    {
        public Client() { }

        public Client(string name) {
            Name = name;
            Favorites = new HashSet<Favorites>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Favorites> Favorites { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}