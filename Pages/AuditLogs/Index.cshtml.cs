using AuthX.Data;
using AuthX.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuthX.Pages.AuditLogs;

[Authorize]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public List<AuditLog> Logs { get; set; } = new();

    public async Task OnGetAsync()
    {
        Logs = await _db.AuditLogs
            .OrderByDescending(x => x.Timestamp)
            .Take(100)
            .ToListAsync();
    }
}