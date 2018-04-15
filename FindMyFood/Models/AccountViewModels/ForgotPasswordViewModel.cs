using System.ComponentModel.DataAnnotations;

namespace Find_My_Food.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}