using System.ComponentModel.DataAnnotations;

namespace Data.DTOs
{
    public class CreateAccountDto
    {
        [Required(ErrorMessage = "Please select a currency")]
        public int CurrencyId { get; set; }
    }
}