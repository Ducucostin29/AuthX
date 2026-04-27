using System.ComponentModel.DataAnnotations;

namespace AuthX.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = "";

        public string? Description { get; set; }

        public string Severity { get; set; } = "LOW";

        public string Status { get; set; } = "OPEN";

        public string OwnerId { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}