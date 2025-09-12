using System.ComponentModel.DataAnnotations;

namespace Data.Validation
{
    public class AmountValidationAttribute : ValidationAttribute
    {
        private readonly double _min;
        private readonly double _max;

        public AmountValidationAttribute(double min, double max)
        {
            _min = min;
            _max = max;
            ErrorMessage = $"Amount must be between {_min} and {_max}, with maximum 2 decimal places.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult(ErrorMessage);

            if (decimal.TryParse(value.ToString(), out var amount))
            {
                if ((double)amount < _min || (double)amount > _max)
                    return new ValidationResult(ErrorMessage);

                if (decimal.Round(amount, 2) != amount)
                    return new ValidationResult(ErrorMessage);

                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}
