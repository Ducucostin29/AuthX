using System.ComponentModel.DataAnnotations;
using AuthX.Data;
using AuthX.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuthX.Pages;

public class ResetPasswordModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly PasswordService _passwordService;
    private readonly AuditService _audit;

    public ResetPasswordModel(AppDbContext db, PasswordService passwordService, AuditService audit)
    {
        _db = db;
        _passwordService = passwordService;
        _audit = audit;
    }

    [BindProperty]
    public string Token { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    
    public void OnGet(string? token)
    {
        if (!string.IsNullOrWhiteSpace(token))
        {
            Token = token.Trim();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var cleanToken = Token.Trim();

        var reset = await _db.PasswordResetTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == cleanToken);

        if (reset == null || reset.User == null || reset.Used || reset.ExpiresAtUtc < DateTime.UtcNow)
        {
            await _audit.LogAsync("PASSWORD_RESET_FAILED_INVALID_TOKEN", "auth", null, null);
            Message = "Invalid or expired token.";
            return Page();
        }

        reset.User.PasswordHash = _passwordService.HashPassword(reset.User, NewPassword);
        reset.Used = true;

        await _db.SaveChangesAsync();

        await _audit.LogAsync("PASSWORD_RESET_SUCCESS", "auth", null, reset.User.Id.ToString());

        Message = "Password reset successful.";
        return Page();
    }
}
