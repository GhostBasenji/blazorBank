namespace Data.Models;

public partial class EmployeeAction
{
    public int ActionId { get; set; }

    public int EmployeeId { get; set; }

    public string ActionType { get; set; } = null!;

    public DateTime? ActionDate { get; set; }

    public string? Description { get; set; }

    public int? RelatedEntityId { get; set; }

    public string? RelatedEntityType { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
