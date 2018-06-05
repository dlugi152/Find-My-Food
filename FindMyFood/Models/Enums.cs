using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;

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

        public enum PeriodEnum
        {
            NoLimit,
            Once,
            Daily,
            Weekly,
            SingleDays
        }
    }
}