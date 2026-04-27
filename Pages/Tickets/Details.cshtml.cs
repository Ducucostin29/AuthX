using AuthX.Data;
using AuthX.Models;
using AuthX.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthX.Pages.Tickets
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly AuditService _audit;

        public DetailsModel(AppDbContext context, AuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public Ticket? Ticket { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == userId);

            if (Ticket == null)
            {
                await _audit.LogAsync(
                    action: "IDOR_ATTEMPT_OR_NOT_FOUND",
                    resource: "ticket",
                    resourceId: id.ToString(),
                    userId: userId
                );

                return NotFound();
            }

            await _audit.LogAsync(
                action: "VIEW_TICKET",
                resource: "ticket",
                resourceId: Ticket.Id.ToString(),
                userId: userId
            );

            return Page();
        }
    }
}