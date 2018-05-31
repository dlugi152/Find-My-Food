using System.ComponentModel.DataAnnotations;

namespace FindMyFood.Models
{
    public class Enums
    {
        public enum RolesEnum
        {
            [Display(Name = "Właściciel restauracji")]
            Restaurant,
            [Display(Name = "Klient")]
            Client
        }
    }
}