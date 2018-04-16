using System.ComponentModel.DataAnnotations;

namespace Find_My_Food.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "{0} jest wymagany")]
        [EmailAddress(ErrorMessage = "To nie jest prawidłowy {0}")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Musisz podać {0}")]
        [Display(Name = "Hasło")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Zapamiętaj")]
        public bool RememberMe { get; set; }
    }
}