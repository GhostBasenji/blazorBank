using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
    public class AccountInfoDto
    {
        public string AccountNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal? Balance { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
