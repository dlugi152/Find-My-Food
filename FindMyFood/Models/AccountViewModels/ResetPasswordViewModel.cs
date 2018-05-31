﻿using System.ComponentModel.DataAnnotations;

namespace FindMyFood.Models.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "{0} jest wymagany")]
        [EmailAddress(ErrorMessage = "To nie jest prawidłowy {0}")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Musisz podać {0}")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}