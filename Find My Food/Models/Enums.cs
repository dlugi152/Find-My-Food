using System.ComponentModel.DataAnnotations;

namespace Find_My_Food.Models
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