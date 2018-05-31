using System.ComponentModel.DataAnnotations;

namespace FindMyFood.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}