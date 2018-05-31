using System.ComponentModel.DataAnnotations;

namespace FindMyFood.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}