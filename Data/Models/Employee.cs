using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Position { get; set; } = null!;

    public DateTime? HireDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<EmployeeAction> EmployeeActions { get; set; } = new List<EmployeeAction>();
}
