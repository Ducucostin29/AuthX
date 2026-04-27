public class AuditLog
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public string Action { get; set; } = "";

    public string Resource { get; set; } = "";

    public string? ResourceId { get; set; }

    public string? IpAddress { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}