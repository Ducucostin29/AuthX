using AuthX.Data;
using AuthX.Models;
using AuthX.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AuthX.Pages.Tickets
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly AuditService _audit;

        public CreateModel(AppDbContext context, AuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        [BindProperty]
        public Ticket Ticket { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Ticket.OwnerId = userId!;
            Ticket.Status = "OPEN";
            Ticket.CreatedAt = DateTime.UtcNow;
            Ticket.UpdatedAt = DateTime.UtcNow;

            _context.Tickets.Add(Ticket);
            await _context.SaveChangesAsync();

            await _audit.LogAsync(
                action: "CREATE_TICKET",
                resource: "ticket",
                resourceId: Ticket.Id.ToString(),
                userId: userId
            );

            return RedirectToPage("/Tickets/Index");
        }
    }
}