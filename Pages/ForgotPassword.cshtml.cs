using AuthX.Data;
using AuthX.Models;
using AuthX.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuthX.Pages;

public class ForgotPasswordModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly AuditService _audit;

    public ForgotPasswordModel(AppDbContext db, AuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
    public string GeneratedToken { get; set; } = string.Empty;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == Email);

        Message = "If the account exists, a reset link was generated.";

        if (user == null)
        {
            await _audit.LogAsync("PASSWORD_RESET_REQUEST_UNKNOWN_EMAIL", "auth", null, null);
            return Page();
        }

        var token = PasswordService.GenerateResetToken();

        var reset = new PasswordResetToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15),
            Used = false
        };

        _db.PasswordResetTokens.Add(reset);
        await _db.SaveChangesAsync();

        await _audit.LogAsync("PASSWORD_RESET_REQUEST", "auth", null, user.Id.ToString());


        GeneratedToken = Url.Page(
            "/ResetPassword",
            null,
            new { token = token },
            Request.Scheme
        ) ?? token;

        return Page();
    }
}