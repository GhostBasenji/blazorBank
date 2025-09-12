using Data.Validation;

namespace Data.DTOs
{
    public class AmountDto
    {
        [AmountValidation(0.01, 100_000)]
        public decimal Amount { get; set; }
    }
}
