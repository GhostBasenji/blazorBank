using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class TransactionType
{
    public int TransactionTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
