using System.ComponentModel.DataAnnotations;

namespace Find_My_Food.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}