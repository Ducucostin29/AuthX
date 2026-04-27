using AuthX.Data;
using AuthX.Models;

namespace AuthX.Services
{
    public class AuditService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string action, string resource, string? resourceId = null, string? userId = null)
        {
            var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                Action = action,
                Resource = resource,
                ResourceId = resourceId,
                IpAddress = ip,
                Timestamp = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }
    }
}