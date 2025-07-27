using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class AccountStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
