using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Find_My_Food.Models.AccountViewModels
{
    internal class RegistrationValidatorAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string _comparisonProperty;

        public RegistrationValidatorAttribute() {
            _comparisonProperty = nameof(RegisterViewModel.Role);
        }

        public void AddValidation(ClientModelValidationContext context) {
            string error = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-error", error);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            ErrorMessage = ErrorMessageString;
            string currentValue = (string)value;

            PropertyInfo property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null) {
                return new ValidationResult("Nieprawidłowa pole, nie mogło być sprawdzone");
            }

            Enums.RolesEnum comparisonValue = (Enums.RolesEnum)property.GetValue(validationContext.ObjectInstance);
            switch (comparisonValue) {
                case Enums.RolesEnum.Restaurant:
                    if (validationContext.MemberName == nameof(RegisterViewModel.RestaurantName) &&
                        string.IsNullOrEmpty(currentValue)) {
                        return new ValidationResult(validationContext.DisplayName + " nie może być pusta");
                    }

                    if (validationContext.MemberName == nameof(RegisterViewModel.Address) &&
                        string.IsNullOrEmpty(currentValue)) {
                        return new ValidationResult(validationContext.DisplayName + " nie może być pusty");
                    }

                    return ValidationResult.Success;
                case Enums.RolesEnum.Client:
                    if (validationContext.MemberName == nameof(RegisterViewModel.ClientName) &&
                        string.IsNullOrEmpty(currentValue)) {
                        return new ValidationResult(validationContext.DisplayName + " nie może być pusty");
                    }

                    return ValidationResult.Success;
                default:
                    return new ValidationResult("Rola jeszcze nie wspierana, skontaktuj się z administratorem");
            }
        }
    }
}