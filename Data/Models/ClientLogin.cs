namespace Data.Models;

public partial class ClientLogin
{
    public int LoginId { get; set; }

    public int ClientId { get; set; }

    public DateTime? LoginTime { get; set; }

    public bool IsSuccessful { get; set; }

    public string? IpAddress { get; set; }

    public string? Description { get; set; }

    public virtual Client Client { get; set; } = null!;
}
