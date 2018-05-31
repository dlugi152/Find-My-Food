using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace FindMyFood.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "{0} jest wymagany")]
        [EmailAddress(ErrorMessage = "To nie jest prawidłowy {0}")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Musisz podać {0}")]
        [StringLength(100, ErrorMessage = "{0} musi mieć długość od {2} do maks {1} znaków", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("Password", ErrorMessage = "Hasła nie pasują")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "{0} jest wymagana")]
        [EnumDataType(typeof(Enums.RolesEnum))]
        [Display(Name = "Rola")]
        public Enums.RolesEnum Role { get; set; }

        [RegistrationValidator]
        [StringLength(255, ErrorMessage = "{0} musi mieć długość od {2} do maks {1} znaków", MinimumLength = 3)]
        [Display(Name = "Nazwa Restauracji")]
        public string RestaurantName { get; set; }

        [RegistrationValidator]
        //[RegularExpression()]
        [StringLength(255, ErrorMessage = "{0} musi mieć długość od {2} do maks {1} znaków", MinimumLength = 3)]
        [Display(Name = "Wyszukiwarka adresu")]
        public string Address { get; set; }

        [RegistrationValidator]
        [StringLength(255, ErrorMessage = "{0} musi mieć długość od {2} do maks {1} znaków", MinimumLength = 5)]
        [Display(Name = "Login")]
        public string ClientName { get; set; }

        public string RealAddress { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}